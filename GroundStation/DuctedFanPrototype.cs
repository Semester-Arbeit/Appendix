using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GroundStation
{
    public class DuctedFanPrototype
    {
        private string flightComputerIp;
        private int udpPort;
        private UdpClient udp;

        private bool stopListening = false;
        private bool inFlight = false;

        public event Action<TelemetryData> TelemetryUpdate;

        private DataLogger FlightData = new DataLogger("FlightLog", DataLogger.Type.csv);


        public DuctedFanPrototype(string flightComputerIp = "192.168.3.1", int udpPort = 2390)
        {
            this.flightComputerIp = flightComputerIp;
            this.udpPort = udpPort;
        }

        public void sendData(string data)
        {
            Console.Write("SendUDP:");
            Console.WriteLine(data);
            this.udp = new UdpClient(udpPort);
            udp.Connect(this.flightComputerIp, this.udpPort);
            var sendBytes = Encoding.ASCII.GetBytes(data);
            try
            {
                Console.WriteLine(udp.Send(sendBytes, sendBytes.Length));
            }
            catch (Exception r)
            {
                Console.WriteLine(r.ToString());
            }
            udp.Close();
        }

        public void launch()
        {
            if (!inFlight)
            {
                Console.WriteLine("Lanching");
                sendData("L");
                this.stopListening = false;
                this.inFlight = true;
                startListener();
            }
        }

        public async void abort()
        {
            this.stopListening = true;
            this.inFlight = false;
            await Task.Delay(20);
            sendData("A");
            
        }

        public void getRocketStatus()
        {
            sendData("S");
            startListener(0,TelemetryData.statusUpdateSender.preFilght, false);
        }

        public void getParameters()
        {
            sendData("G");
            startListener(0, TelemetryData.statusUpdateSender.standby, false);
        }

        public void sendControlParametersData(string data)
        {
            sendData("P");
            sendData(data);
        }

        public void calibrate()
        {
            sendData("C");
        }


        public void getSensorData()
        {
            sendData("I");
            startListener(0, TelemetryData.statusUpdateSender.inFlight, false);
        }

        public void stopReceivingData()
        {
            this.stopListening = true;
        }

        public async void startListener(int peridticTelemetryUpdate = 4, TelemetryData.statusUpdateSender sUpdate = TelemetryData.statusUpdateSender.inFlight, bool dataLogging = true)
        {
            UdpClient receivingUdpClient = new UdpClient(2390);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            string returnData = "";
            try
            {
                int i = 0;
                while (i <15000 && receivingUdpClient.Available < 1)
                {
                    await Task.Delay(1);
                    i++;
                }
                if(i == 15000)
                {
                    stopReceivingData();
                    return;
                }
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                returnData = Encoding.ASCII.GetString(receiveBytes);
                List<double> TelemteryDataList = new List<double>();

                i = 0;
                while (returnData != "E" && !stopListening)
                {
                    if(sUpdate == TelemetryData.statusUpdateSender.inFlight)
                    {
                        TelemteryDataList.Clear();
                        string csvLine = "";
                        for (int m = 0; m < 22; m++)
                        {
                            double oneValue = BitConverter.ToDouble(receiveBytes, m * 8);
                            csvLine += oneValue.ToString() + ",";
                            TelemteryDataList.Add(oneValue);
                            
                        }
                        if(dataLogging)
                        {
                            FlightData.AppendLine(csvLine.Substring(0, csvLine.Length - 1));
                        }
                    }
                    
                    if (i >= peridticTelemetryUpdate)
                    {
                        TelemetryUpdate(new TelemetryData()
                        {
                            rawData = returnData,
                            parsedData = TelemteryDataList,
                            statusUpdate = sUpdate

                        });
                        await Task.Delay(3);
                        i = 0;
                    }
                    i++;

                    receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    returnData = Encoding.ASCII.GetString(receiveBytes);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            receivingUdpClient.Close();
            if(dataLogging)
            {
                FlightData.WriteFile();
            }
        }
    }
}
