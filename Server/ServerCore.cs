using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RemoteSignTool.Server {
    internal class ServerCore : IDisposable {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder lParam);

        [DllImport("User32.Dll")]
        private static extern Int32 PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private NetworkConnection udpServer = null;

        private NetworkConnection tcpServer = null;

        public void Dispose() {
            udpServer?.Dispose();
            tcpServer?.Dispose();
        }

        public void Listen() {
            var udpServerEp = new IPEndPoint(IPAddress.Any, 8090);
            udpServer = new NetworkConnection(udpServerEp, NetworkConnectionType.UdpServer);
            udpServer.Received += UdpReceived;
            udpServer.ReceiveError += Server_ReceiveError;

            var tcpServerEp = new IPEndPoint(IPAddress.Any, 8090);
            tcpServer = new NetworkConnection(tcpServerEp, NetworkConnectionType.TcpServer);
            tcpServer.Received += TcpReceived;
            tcpServer.ReceiveError += Server_ReceiveError;
        }

        private void UdpReceived(object sender, NetworkConnectionReceivedEventArgs e) {
            var cmd = e.Data.GetString();
            if (cmd == "signtool?") {
                var local = (e.Socket.LocalEndPoint as IPEndPoint).Address.GetAddressBytes();
                e.Send(local);
            }
        }

        private void TcpReceived(object sender, NetworkConnectionReceivedEventArgs e) {
            try {
                if (e.Data.Length < 12) {
                    throw new IndexOutOfRangeException("Not enough data");
                }

                var index = 0;
                var argsLength = e.Data.GetInt32(ref index);
                var nameLength = e.Data.GetInt32(ref index);
                var dataLength = e.Data.GetInt32(ref index);
                if (e.Data.Length < index + argsLength + nameLength + dataLength) {
                    throw new IndexOutOfRangeException("Not enough data");
                }

                var args = e.Data.GetString(ref index, argsLength);
                var name = e.Data.GetString(ref index, nameLength);

                var tempFile = Path.GetTempFileName();
                var file = new FileStream(tempFile, FileMode.Create);
                file.Write(e.Data, index, dataLength);
                file.Close();

                args += " \"" + tempFile + "\"";

                var signed = SignFile(Program.MainForm.signtoolPath.Text, args, Program.MainForm.pin.Text, out Int32 exitCode, out String message);

                var rawList = new List<Byte>();
                var rawMessage = message.GetBytes();
                var rawName = name.GetBytes();
                var rawData = new Byte[0];

                if (signed) {
                    Logger.Info("File '" + tempFile + "' signed");
                    rawData = File.ReadAllBytes(tempFile);
                }

                rawList.AddRange(exitCode.GetBytes());
                rawList.AddRange(rawMessage.Length.GetBytes());
                rawList.AddRange(rawName.Length.GetBytes());                
                rawList.AddRange(rawData.Length.GetBytes());

                rawList.AddRange(rawMessage);
                rawList.AddRange(rawName);
                rawList.AddRange(rawData);

                Logger.Info("Sending data to client");
                e.Send(rawList.ToArray());
                Logger.Info("Data has been sent");
            } catch (Exception ex) {
                Logger.Error("An error occurred");
                Logger.Exception(ex);
            }
        }

        private void Server_ReceiveError(object sender, ErrorEventArgs e) {
            Logger.Error("Receive error");
            Logger.Exception(e.GetException());
        }

        public Boolean SignFile(String signtool, String args, String pin, out Int32 exitCode, out String message) {
            Logger.Info("Exec \"" + signtool + "\" " + args);
            message = "";
            try {
                var info = new ProcessStartInfo {
                    FileName = signtool,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                };
                var process = Process.Start(info);

                var thread = new Thread(() => {
                    while (true) {
                        Thread.Sleep(500);
                        var edit = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, "Edit", null);
                        if (edit != null && edit != IntPtr.Zero) {
                            SendMessage(edit, 0x000C, 0, new StringBuilder(pin));
                            Thread.Sleep(300);
                            PostMessage(edit, 0x0100, 0x0D, 0);
                            Thread.Sleep(100);
                            PostMessage(edit, 0x0101, 0x0D, 0);
                            break;
                        }
                    }
                }) { IsBackground = true };
                thread.Start();

                message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                exitCode = process.ExitCode;

                if (thread.ThreadState == System.Threading.ThreadState.Running) {
                    thread.Abort();
                }

                if (exitCode != 0) {
                    Logger.Error("signtool exit code is " + exitCode + "\r\n" + message);
                } else {
                    Logger.Info("signtool exit code is " + exitCode + "\r\n" + message);
                }
            } catch (Exception ex) {
                exitCode = 254;
                Logger.Error(message = "Unable to start signtool");
                Logger.Exception(ex);
            }
            return exitCode == 0;
        }
    }
}
