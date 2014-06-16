using EveFortressModel;
using Lidgren.Network;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ClientNetworkManager : IUpdateNeeded, IDisposeNeeded
    {
        const bool DEBUG = true;
        public NetClient Connection { get; set; }

        public bool Connected
        {
            get
            {
                return Connection.ServerConnection != null && 
                      (Connection.ServerConnection.Status == NetConnectionStatus.Connected ||
                       Connection.ServerConnection.Status == NetConnectionStatus.None);
            }
        }

        ConcurrentQueue<NetIncomingMessage> Messages = new ConcurrentQueue<NetIncomingMessage>();

        public ClientNetworkManager()
        {
            var config = new NetPeerConfiguration("EveFortress");
            Connection = new NetClient(config);
            Connection.Start();
            Game.Updateables.Add(this);
            Game.Disposables.Add(this);
            Task.Run(() =>
            {
                while (true)
                {
                    if (Connected)
                    {
                        NetIncomingMessage msg = Connection.ReadMessage();
                        if (msg != null)
                        {
                            Messages.Enqueue(msg);
                            continue;
                        }
                    }
                    Thread.Sleep(20);
                }
            });
        }

        bool connecting;
        private async void Connect()
        {
            if (!connecting)
            {
                connecting = true;
                await Task.Run(() =>
                {
                    if (DEBUG)
                    {
                        Connection.Connect("localhost", 19952);
                    }
                    else
                    {
                        Connection.Connect("the-simmons.dnsalias.net", 19952);
                    }
                });
                connecting = false;
            }
        }

        public void Update()
        {
            if (Connected)
            {
                NetIncomingMessage msg;
                while (Messages.TryDequeue(out msg))
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            Game.MessageParser.ParseMessage(msg);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)msg.ReadByte();
                            var text = msg.ReadString();
                            Console.WriteLine(Enum.GetName(status.GetType(), status) + ":" + text);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(msg.ReadString());
                            break;
                        default:
                            Console.WriteLine("Unhandled type: " + msg.MessageType);
                            break;
                    }
                    Connection.Recycle(msg);
                }
            }
            else
            {
                if (Connection.ServerConnection == null ||
                    Connection.ServerConnection.Status != NetConnectionStatus.InitiatedConnect ||
                    Connection.ServerConnection.Status != NetConnectionStatus.RespondedConnect)
                {
                    Game.QueueReset();
                    Game.TabManager.MainSection.ReplaceTab(new ConnectingTab());
                    Connect();
                }
            }
        }

        public byte[] Serialize<T>(T itemToSerialize)
        {
            byte[] returnArray;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, itemToSerialize);
                returnArray = stream.ToArray();
            }
            return returnArray;
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public void Dispose()
        {
            Connection.Disconnect("Closed by the user");
        }
    }
}