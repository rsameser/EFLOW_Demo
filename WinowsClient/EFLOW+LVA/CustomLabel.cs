using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EFLOW_LVA
{
    public class CustomLabel : Label
    {
        public Color color = Color.Red;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                                         this.color, 3, ButtonBorderStyle.Solid,
                                         this.color, 3, ButtonBorderStyle.Solid,
                                         this.color, 3, ButtonBorderStyle.Solid,
                                         this.color, 3, ButtonBorderStyle.Solid);
        }
    }
}



