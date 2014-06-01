using EveFortressModel;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ClientNetworkManager : IUpdateNeeded, IDisposeNeeded
    {
        const bool DEBUG = false;
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

        public ClientNetworkManager()
        {
            var config = new NetPeerConfiguration("EveFortress");
            Connection = new NetClient(config);
            Connection.Start();
            Game.Updateables.Add(this);
            Game.Disposables.Add(this);
        }

        private void Connect()
        {
            if (DEBUG)
            {
                Connection.Connect("localhost", 19952);
            }
            else
            {
                Connection.Connect("the-simmons.dnsalias.net", 19952);
            }
        }

        public void Update()
        {
            if (Connected)
            {
                NetIncomingMessage msg;
                while ((msg = Connection.ReadMessage()) != null)
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
                    Game.TabManager.ActiveSection.ReplaceTab(new ConnectingTab());
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