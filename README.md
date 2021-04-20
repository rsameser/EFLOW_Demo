# EFLOW_Demo

1. Download Windows 10 IoT
2. Login to the Windows VM and install Visual Studio Code
3. Install [Live Video Analytics](https://docs.microsoft.com/en-us/azure/media-services/live-video-analytics-edge/get-started-detect-motion-emit-events-quickstart)
4. Make sure Windows 10 Hyper-V is enabled: https://docs.microsoft.com/en-us/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v
5. Install EFLOW following the [Install Azure IoT Edge for Linux on Windows | Microsoft Docs](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge-on-windows?view=iotedge-2018-06&tabs=windowsadmincenter) guide.
6. Use Visual Studio Code to deploy the modules using the [deployment.eflow_demo.template.json](./deployment.eflow_demo.template.json)
7. Run the EFLOW+LVA code on the Windows Host
8. Ssh into the EFLOW VM 
9. Run `sudo iptables -A INPUT -p udp --dport 554 -j ACCEPT`
10. Check the EFLOW VM IP `sudo ifconfig`
