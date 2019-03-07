using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace RemoteSignTool.Client {
    internal class ClientCore {
        private NetworkConnection tcpClient = null;

        private ManualResetEvent serverFinder = new ManualResetEvent(false);

        private ManualResetEvent signer = new ManualResetEvent(false);

        private Boolean hasError = false;

        public ClientCore() {
            serverFinder.Reset();

            var serverEp = new IPEndPoint(IPAddress.Broadcast, 8090);
            var clientEp = new IPEndPoint(IPAddress.Any, 0);
            var udpClient = new NetworkConnection(clientEp, NetworkConnectionType.UdpClient);
            udpClient.Received += UdpReceived;
            udpClient.ReceiveError += ReceiveError;
            udpClient.SendTo(Encoding.UTF8.GetBytes("signtool?"), serverEp);

            new Thread(() => {
                Thread.Sleep(3000);
                serverFinder.Set();
                if (tcpClient == null) {
                    hasError = true;
                }
            }) { IsBackground = true }.Start();

            serverFinder.WaitOne();

            if (hasError) {
                udpClient?.Dispose();
                tcpClient?.Dispose();
                throw new InvalidOperationException("Cannot connect to server");
            }
        }

        private void UdpReceived(object sender, NetworkConnectionReceivedEventArgs e) {
            if (tcpClient == null) {
                try {
                    var serverIp = e.EndPoint.Address;
                    Logger.Info("Find server at " + serverIp);
                    tcpClient = new NetworkConnection(new IPEndPoint(serverIp, 8090), NetworkConnectionType.TcpClient);
                    tcpClient.Received += TcpReceived;
                    tcpClient.ReceiveError += ReceiveError;
                } catch (Exception ex) {
                    hasError = true;
                    Logger.Exception(ex);
                }
                serverFinder.Set();
            }
        }

        private void TcpReceived(object sender, NetworkConnectionReceivedEventArgs e) {
            try {
                if (e.Data.Length < 16) {
                    throw new IndexOutOfRangeException("Not enough data");
                }

                var index = 0;
                var exitCode = e.Data.GetInt32(ref index);
                var messageLength = e.Data.GetInt32(ref index);
                var nameLength = e.Data.GetInt32(ref index);
                var dataLength = e.Data.GetInt32(ref index);
                if (e.Data.Length < index + messageLength + nameLength + dataLength) {
                    throw new IndexOutOfRangeException("Not enough data");
                }

                var message = e.Data.GetString(ref index, messageLength);
                var name = e.Data.GetString(ref index, nameLength);

                if (exitCode == 0) {
                    var file = new FileStream(name, FileMode.Create);
                    file.Write(e.Data, index, dataLength);
                    file.Close();
                }

                signer.Set();
            } catch (Exception ex) {
                hasError = true;
                Logger.Error("An error occurred");
                Logger.Exception(ex);
            }
        }

        private void ReceiveError(object sender, ErrorEventArgs e) {
            hasError = true;
            Logger.Error("Receive error");
            Logger.Exception(e.GetException());
            serverFinder.Set();
            signer.Set();
        }

        public Boolean SignFile(String fileName, String args) {
            signer.Reset();
            hasError = false;
            var rawList = new List<Byte>();
            var rawArgs = args.GetBytes();
            var rawName = fileName.GetBytes();
            var rawData = File.ReadAllBytes(fileName);

            rawList.AddRange(rawArgs.Length.GetBytes());
            rawList.AddRange(rawName.Length.GetBytes());
            rawList.AddRange(rawData.Length.GetBytes());

            rawList.AddRange(rawArgs);
            rawList.AddRange(rawName);
            rawList.AddRange(rawData);

            try {
                tcpClient.Send(rawList.ToArray());
                signer.WaitOne();
            } catch (Exception ex) {
                Logger.Error("Cannot send data to server");
                Logger.Exception(ex);
                hasError = true;
            }

            tcpClient.Dispose();

            if (hasError) {
                return false;
            } else {
                return true;
            }
        }
    }
}
