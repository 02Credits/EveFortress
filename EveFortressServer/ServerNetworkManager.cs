using Lidgren.Network;
using NetworkLibrary;
using System;

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

        public override void ConnectionConnected(NetConnection connection)
        {
            Console.WriteLine("Player connected");
        }

        public override void ConnectionDisconnected(NetConnection connection)
        {
            if (Program.PlayerManager.ConnectionNames.ContainsKey(connection))
            {
                Console.WriteLine(Program.PlayerManager.ConnectionNames[connection] + " has disconnected.");
            }
            else
            {
                Console.WriteLine("A not yet logged in player disconnected");
            }
            Program.PlayerManager.DisconnectPlayer(connection);
        }

        public override byte[] ParseMessage(string commandName, NetIncomingMessage message)
        {
            return Program.MessageParser.ParseMessage(commandName, message);
        }
    }
}