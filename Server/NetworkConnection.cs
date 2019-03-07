using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace RemoteSignTool {
    public class NetworkConnection : IDisposable {
        public event ErrorEventHandler ReceiveError;

        public event EventHandler<NetworkConnectionReceivedEventArgs> Received;

        private NetworkConnectionType type;

        private Socket socket = null;

        private IPEndPoint endPoint = null;

        private Thread thread = null;

        public NetworkConnection(IPEndPoint ip, NetworkConnectionType type) {
            this.type = type;
            endPoint = ip;
            if (type == NetworkConnectionType.TcpClient) {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip);
                socket.BeginReceiveFrom(ReceiveCallback);
            } else if (type == NetworkConnectionType.TcpServer) {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ip);
                socket.Listen(100);
                thread = new Thread(() => {
                    var allDone = new ManualResetEvent(false);
                    while (true) {
                        allDone.Reset();
                        socket.BeginAccept((ar) => {
                            try {
                                allDone.Set();
                                var listener = ar.AsyncState as Socket;
                                var socket = listener.EndAccept(ar);
                                socket.BeginReceiveFrom(ReceiveCallback);
                            } catch { }
                        }, socket);
                        allDone.WaitOne();
                    }
                }) { IsBackground = true };
                thread.Start();
            } else if (type == NetworkConnectionType.UdpClient || type == NetworkConnectionType.UdpServer) {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { EnableBroadcast = true };
                socket.Bind(ip);
                socket.BeginReceiveFrom(ReceiveCallback);
            } else {
                throw new NotSupportedException("The parameter 'type' must be one of the 'NetworkConnectionType'");
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                var state = ar.AsyncState as NetworkConnectionState;
                var socket = state.socket;
                var length = 0;

                try {
                    length = socket.EndReceiveFrom(ar, ref state.remoteEndPoint);
                } catch (ObjectDisposedException ex) {
                    return;
                } catch (Exception ex) {
                    ReceiveError?.Invoke(this, new ErrorEventArgs(ex));
                }

                if (length > 0) {
                    state.data.AddRange(state.buffer.Take(length));
                }

                if (state.data.Count > 4) {
                    var raw = state.data.ToArray();
                    var dataLength = BitConverter.ToInt32(raw, 0);
                    if (state.data.Count >= dataLength + 4) {
                        var result = raw.Skip(4).Take(dataLength).ToArray();
                        state.data = raw.Skip(dataLength + 4).ToList();
                        if (socket.ProtocolType == ProtocolType.Tcp) {
                            state.remoteEndPoint = socket.RemoteEndPoint;
                        }

                        Received?.Invoke(this, new NetworkConnectionReceivedEventArgs(result, socket, state.remoteEndPoint as IPEndPoint));
                    }

                    try {
                        socket.BeginReceiveFrom(ReceiveCallback, state);
                    } catch (Exception ex) {
                        ReceiveError?.Invoke(this, new ErrorEventArgs(ex));
                    }
                }
            } catch (Exception ex) {
                ReceiveError?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        public void Send(Byte[] data) {
            if (socket.ProtocolType == ProtocolType.Udp) {
                throw new NotSupportedException("UDP protocol is not support to send data without endpoint");
            }

            if (type == NetworkConnectionType.TcpServer || type == NetworkConnectionType.UdpServer) {
                throw new NotSupportedException("Server cannot actively send data");
            }

            socket.SendTo(data, NetworkConnectionState.BUFFER_SIZE, endPoint);
        }

        public void SendTo(Byte[] data, IPEndPoint end) {
            if (socket.ProtocolType == ProtocolType.Tcp) {
                throw new NotSupportedException("TCP protocol is not support to send data to a specified endpoint before connected");
            }

            if (type == NetworkConnectionType.TcpServer || type == NetworkConnectionType.UdpServer) {
                throw new NotSupportedException("Server cannot actively send data");
            }

            socket.SendTo(data, NetworkConnectionState.BUFFER_SIZE, end);
        }

        public void Dispose() {
            thread?.Abort();

            if (socket?.Connected == true) {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket?.Close();
        }
    }

    public class NetworkConnectionReceivedEventArgs : EventArgs {
        public IPEndPoint EndPoint { get; }

        public Byte[] Data { get; }

        public Socket Socket { get; }

        public Int32 BufferSize { get { return NetworkConnectionState.BUFFER_SIZE; } }

        internal NetworkConnectionReceivedEventArgs(Byte[] data, Socket socket, IPEndPoint end) {
            Data = data;
            EndPoint = end;
            Socket = socket;
        }

        public void Send(Byte[] data) {
            Socket.SendTo(data, BufferSize, EndPoint);
        }
    }

    internal class NetworkConnectionState {
        public const Int32 BUFFER_SIZE = 1048576;

        public Socket socket = null;

        public EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public Byte[] buffer = new Byte[BUFFER_SIZE];

        public List<Byte> data = new List<Byte>(BUFFER_SIZE);
    }

    public enum NetworkConnectionType {
        TcpClient, UdpClient, TcpServer, UdpServer
    }

    public static partial class Extensions {
        public static void SendTo(this Socket s, Byte[] buffer, Int32 blockSize, EndPoint remoteEP) {
            var length = buffer.Length + 4;
            var tempRaw = new List<Byte>(length);
            tempRaw.AddRange(BitConverter.GetBytes(buffer.Length));
            tempRaw.AddRange(buffer);
            if (s.ProtocolType == ProtocolType.Tcp && s.Connected) {
                using (var networkStream = new BufferedStream(new NetworkStream(s)))
                using (var dataStream = new MemoryStream(tempRaw.ToArray())) {
                    dataStream.CopyTo(networkStream);
                    networkStream.Flush();
                }
            } else if (s.ProtocolType == ProtocolType.Udp) {
                var raw = tempRaw as IEnumerable<Byte>;
                while (raw.Count() > 0) {
                    s.SendTo(raw.Take(blockSize).ToArray(), remoteEP);
                    raw = raw.Skip(blockSize);
                }
            }
        }

        public static void BeginReceiveFrom(this Socket s, AsyncCallback callback) {
            s.BeginReceiveFrom(callback, new NetworkConnectionState() { socket = s });
        }

        public static void BeginReceiveFrom(this Socket s, AsyncCallback callback, Object state) {
            if (!(state is NetworkConnectionState)) {
                throw new NotSupportedException("The parameter 'state' must be an instance that inherits the 'NetworkConnectionState' class");
            }

            var state2 = state as NetworkConnectionState;
            s.BeginReceiveFrom(state2.buffer, 0, state2.buffer.Length, SocketFlags.None, ref state2.remoteEndPoint, callback, state);
        }

        public static Int32 GetInt32(this Byte[] buffer, ref Int32 index) {
            var value = BitConverter.ToInt32(buffer, index);
            index += 4;
            return value;
        }

        public static String GetString(this Byte[] buffer, ref Int32 index, Int32 length) {
            var value = Encoding.UTF8.GetString(buffer, index, length);
            index += length;
            return value;
        }

        public static String GetString(this Byte[] buffer) {
            var index = 0;
            return buffer.GetString(ref index, buffer.Length);
        }

        public static Byte[] GetBytes(this Int32 value) {
            return BitConverter.GetBytes(value);
        }

        public static Byte[] GetBytes(this String str) {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
