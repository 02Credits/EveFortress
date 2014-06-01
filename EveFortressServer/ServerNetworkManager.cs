using EveFortressModel;
using Lidgren.Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class ServerNetworkManager : IUpdateNeeded, IDisposeNeeded
    {
        public NetServer Clients { get; set; }

        public ServerNetworkManager()
        {
            var config = new NetPeerConfiguration("EveFortress");
            config.Port = 19952;
            Clients = new NetServer(config);
            Clients.Start();
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
        }

        public void Update()
        {
            NetIncomingMessage msg;
            while ((msg = Clients.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        Program.MessageParser.ParseMessage(msg);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus)msg.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Disconnected:
                                Program.PlayerManager.DisconnectPlayer(msg.SenderConnection);
                                if (Program.PlayerManager.ConnectionNames.ContainsKey(msg.SenderConnection))
                                {
                                    Console.WriteLine(Program.PlayerManager.ConnectionNames[msg.SenderConnection] + " has disconnected.");
                                }
                                else
                                {
                                    Console.WriteLine("A not yet logged in player disconnected");
                                }
                                break;
                            default:
                                var text = msg.ReadString();
                                Console.WriteLine(Enum.GetName(status.GetType(), status) + ":" + text);
                                break;
                        }
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
                Clients.Recycle(msg);
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
            Clients.Shutdown("Server shutdown");
        }
    }
}
