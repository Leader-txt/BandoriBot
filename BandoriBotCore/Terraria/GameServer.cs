
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace BandoriBot.Terraria
{
    public sealed class GameServer : IDisposable
    {
        private enum MsgType
        {
            SetServerName,
            WriteMessage,
            Heartbeat
        }
        private TcpClient client;
        private NetworkStream stream;
        private BinaryReader br;
        private BinaryWriter bw;
        private Thread recvthread, tickthread;

        public string Name { get; set; }
        public bool Valid { get; private set; }

        public event Action<string, uint> OnMessage;

        private void Listener()
        {
            var buf = new byte[4];
            while (!disposed)
            {
                try
                {
                    Thread.Sleep(0);
                    stream.Read(buf, 0, 4);
                    switch ((MsgType)BitConverter.ToInt32(buf, 0))
                    {
                        case MsgType.SetServerName:
                            Name = br.ReadString();
                            break;
                        case MsgType.WriteMessage:
                            var msg = br.ReadString();
                            var clr = br.ReadUInt32();
                            OnMessage?.Invoke(msg, clr);
                            break;
                    }
                }
                catch
                {

                }
            }
        }

        private void ConnectTillSuc()
        {
            while (true)
            {
                try
                {
                    Connect();
                    return;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }
        private void Heartbeat()
        {
            while (!disposed)
            {
                Thread.Sleep(1000);
                try
                {
                    lock (bw)
                        bw.Write((int)MsgType.Heartbeat);
                }
                catch
                {
                    ConnectTillSuc();
                }
            }
        }

        private string host;
        private ushort port;
        private bool disposed = false;

        public void Connect()
        {
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();
            br = new BinaryReader(stream);
            bw = new BinaryWriter(stream);
            recvthread = new Thread(Listener);
            tickthread = new Thread(Heartbeat);
            recvthread.Start();
            tickthread.Start();
            SetName();
            Valid = true;
        }

        public GameServer(string host, ushort port, string name)
        {
            this.host = host;
            this.port = port;
            this.Name = name;
            Connect();
        }

        public void SetName()
        {
            lock (bw)
            {
                bw.Write((int)MsgType.SetServerName);
                bw.Write(Name);
            }
        }

        public void SendMsg(string message, uint color = 0xffffff)
        {
            if (string.IsNullOrEmpty(message)) return;
            lock (bw)
            {
                bw.Write((int)MsgType.WriteMessage);
                bw.Write(message);
                bw.Write(color);
            }
        }

        public void Dispose()
        {
            try
            {
                disposed = true;
                client.Close();
                br.Dispose();
                bw.Dispose();
            }
            catch
            {

            }
            Valid = false;
        }
    }

}