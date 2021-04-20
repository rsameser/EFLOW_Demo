# EFLOW_Demo

1. Download Windows 10 IoT
2. Login to the Windows VM and install Visual Studio Code
3. Install Widnows Net Core 3.1 https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-3.1.14-windows-x64-installer
4. Install [Live Video Analytics](https://docs.microsoft.com/en-us/azure/media-services/live-video-analytics-edge/get-started-detect-motion-emit-events-quickstart)
5. Make sure Windows 10 Hyper-V is enabled: https://docs.microsoft.com/en-us/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v
6. Install EFLOW following the [Install Azure IoT Edge for Linux on Windows | Microsoft Docs](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge-on-windows?view=iotedge-2018-06&tabs=windowsadmincenter) guide.
7. Download the Windows app - https://microsoft-my.sharepoint-df.com/:u:/p/fcabrera/EUvyooP-wZxMn4L1Hzjy8K4By7bDvrsLiV6EF-LZGGsOfw?e=SXZ3MD
8. Download certiciates https://microsoft-my.sharepoint-df.com/:u:/p/fcabrera/EdBZVhNFfpJLro5h7hJ8pjgBLl5qaXZ7fE8gn8GwsBbtTw?e=GEns2F  and move them to c:\certificates
9. Use Visual Studio Code to deploy the modules using the [deployment.eflow_demo.amd64.json](./deployment.eflow_demo.amd64.json)
10. Run the EFLOW+LVA code on the Windows Host
11. Ssh into the EFLOW VM  `Ssh-EflowVm`
12. Run: `sudo iptables -A INPUT -p udp --dport 554 -j ACCEPT`
13. Run: `sudo iptables -A INPUT -p tcp --dport 443 -j ACCEPT` 
14. Run: `sudo iptables -A INPUT -p tcp --dport 5671 -j ACCEPT` 
15. Run: `sudo iptables-save | sudo tee /etc/systemd/scripts/ip4save > /dev/null`
16. Run: `mkdir ~/certs/`
17. Check the EFLOW VM IP `sudo ifconfig`
18. wget https://raw.githubusercontent.com/Azure/live-video-analytics/master/edge/setup/prep_device.sh > prep_device.sh
19. sudo sh prep_device.sh
20. Copy Certificates to EFLOW VM environment

   * Use PowerShell on the Windows host to get the EFLOW VM IP address.  

       ```powershell
       Get-EflowVmAddr
       ``` 

  * Use SCP to copy the certificates created in [Step 5](./Create%20Certificates%20for%20Authentication.MD) to the `~/certs` folder of your EFLOW VM environment.  
      ```powershell
      scp -i 'C:\Program Files\Azure IoT Edge\id_rsa'  .\certs\* iotedge-user@<eflowvm-ip>:~/certs/​
      ```
19. **Read the certificates**  
    Run the following command to allow Azure IoT Edge to read the certificates.
    ```bash
    sudo chown -R iotedge: ~/certs
    ```
20. **Provision the Azure IoT Edge for Linux configuration**  
    To edit config.yaml run the following command:
    ```bash
    sudo nano /etc/iotedge/config.yaml
    ```    
   * Set the location of the certificates that were copied ot the device earlier.
        ```yaml
        certificates:
          device_ca_cert: "/home/iotedge-user/certs/new-edge-device-full-chain.cert.pem"
          device_ca_pk: "/home/iotedge-user/certs/new-edge-device.key.pem"
          trusted_ca_certs: "/home/iotedge-user/certs/azure-iot-test-only.root.ca.cert.pem"
        ```
        > **Note:** <Make sure there are no whitespaces before certificates paths and two spaces indenting each sub part. 

   * If you are on a network without dynamic DNS, you will need to assign the VM a static IP address and replace the line
    `hostname: "…"` with `hostname: "<Linux VM Hostname>"`.
 
   * To save the file and exit nano, press <kbd>CTRL</kbd>+<kbd>x</kbd>, confirm save and exit with <kbd>Y</kbd> and press <kbd>Enter</kbd>. This concludes the provisioning and configuration.
   
21. **Restart IoT Edge**    
    Restart IoT Edge by running the following command.
    ```base
    sudo systemctl restart iotedge
    ```
