using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFLOW_LVA
{
    public partial class Sample : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        // Thread for edge interop
        private Thread thread1;

        private int width = 640;
        private int length = 480;

        public Sample()
        {
            try
            {
                InitializeComponent();
                InitializeLists();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void InitExternalProgram(string program)
        {
            Process proc = Process.Start(program);
            proc.WaitForInputIdle();

            while (proc.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                proc.Refresh();
            }
        }

        private void InitializeLists()
        {
            listView2.View = View.Details;
            listView2.Columns.Add("Date");
            listView2.Columns.Add("Message");
            listView2.GridLines = true;
            listView2.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView2.Columns[0].Width = 100;
            listView2.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.None);
            listView2.Columns[1].Width = 550;
        }


        public void RefreshEdgeLogs(string message)
        {
            try
            {
                if (this.listView2 != null && this.listView2.InvokeRequired)
                {
                    this.listView2.Invoke((MethodInvoker)delegate ()
                    {
                        this.listView2.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString("HH:mm:ss"), message }));
                        this.listView2.EnsureVisible(this.listView2.Items.Count - 1);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void RefreshEdgeBBOX(double l, double t, double w, double h, string type, double score)
        {
            try
            {
                if (this.newLabel != null && this.newLabel.InvokeRequired)
                {
                    this.newLabel.Invoke((MethodInvoker)delegate ()
                    {
                        this.newLabel.color = Color.Red;
                        this.newLabel.Location = new System.Drawing.Point((int)(l * this.width)-5, (int)(t * this.length)-5);
                        this.newLabel.Size = new System.Drawing.Size((int)(w * this.width)+10, (int)(h * this.length+10));
                        this.newLabel.BackColor = Color.Transparent;
                        this.newLabel.Text = "\n Type: " + type + "\n\n Score: " + score;
                        this.newLabel.BringToFront();
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        /// <summary>
        /// When the Vlc control needs to find the location of the libvlc.dll.
        /// You could have set the VlcLibDirectory in the designer, but for this sample, we are in AnyCPU mode, and we don't know the process bitness.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcControl_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(!String.IsNullOrEmpty(this.textBox1.Text) && !String.IsNullOrEmpty(this.textBox2.Text) &&
                    !String.IsNullOrEmpty(this.textBox3.Text) && !String.IsNullOrEmpty(this.textBox4.Text))
                {
                    button1.Enabled = false;
                    button2.Enabled = true;
                    vlcControl1.Play(new Uri("rtsp://user:pass@" + this.textBox3.Text));

                    EdgeInterop.iotHubConnectionString = this.textBox1.Text;
                    EdgeInterop.gatewayHost = this.textBox2.Text;
                    EdgeInterop.certificatePath = this.textBox4.Text;
                    thread1 = new Thread(EdgeInterop.SubscribeToEdgeMessages);
                    thread1.Start();
                    this.label5.Text = "Connection succeeded";
                    this.label5.ForeColor = Color.DarkGreen;
                }
                else
                {
                    button1.Enabled = true;
                    button2.Enabled = false;
                    this.label5.Text = "Paraemters cannot be empty";
                    this.label5.ForeColor = Color.Red;
                    this.newLabel.Text = "";
                    this.newLabel.color = Color.Black;
                    this.newLabel.BackColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                button2.Enabled = false;
                Debug.WriteLine(ex.Message);
                EdgeInterop.Unsubscribe();
                this.label5.Text = "Error - Please restart the app";
                this.label5.ForeColor = Color.Red;
                this.newLabel.SendToBack();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = true;
                button2.Enabled = false;
                vlcControl1.Stop();
                this.label5.Text = "Waiting for connection";
                this.label5.ForeColor = Color.Blue;
                this.newLabel.Text = "";
                this.newLabel.color = Color.Black;
                this.newLabel.BackColor = Color.Black;
                EdgeInterop.Unsubscribe();
                thread1.Abort();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
