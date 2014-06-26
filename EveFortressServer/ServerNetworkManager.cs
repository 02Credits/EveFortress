using EveFortressModel;
using Lidgren.Network;
using NetworkLibrary;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class ServerNetworkManager : NetworkManager
    {

        public ServerNetworkManager()
        {
            Program.Updateables.Add(this);
            Program.Disposables.Add(this);
            var config = new NetPeerConfiguration("EveFortress");
            config.Port = 19952;
            config.AcceptIncomingConnections = true;
            LidgrenPeer = new NetPeer(config);
            LidgrenPeer.Start();
        }

        public override void ConnectionDisconnected(NetConnection connection)
        {
            Program.PlayerManager.DisconnectPlayer(connection);
            if (Program.PlayerManager.ConnectionNames.ContainsKey(connection))
            {
                Console.WriteLine(Program.PlayerManager.ConnectionNames[connection] + " has disconnected.");
            }
            else
            {
                Console.WriteLine("A not yet logged in player disconnected");
            }
        }

        public override byte[] ParseMessage(string commandName, NetIncomingMessage message)
        {
            return Program.MessageParser.ParseMessage(commandName, message);
        }
    }
}
