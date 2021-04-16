# EFLOW_Demo

1. Downloaded Windows 10 IoT
2. Install [Live Video Analytics](https://docs.microsoft.com/en-us/azure/media-services/live-video-analytics-edge/get-started-detect-motion-emit-events-quickstart)
3. Install EFLOW following the [Install Azure IoT Edge for Linux on Windows | Microsoft Docs](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge-on-windows?view=iotedge-2018-06&tabs=windowsadmincenter) guide.
4. Use Visual Studio Code to deploy the modules using the [deployment.eflow_demo.template.json](./deployment.eflow_demo.template.json)
5. Run the EFLOW+LVA code on the Windows Host
6. Ssh into the EFLOW VM 
7. Run `sudo iptables -A INPUT -p udp --dport 554 -j ACCEPT`
