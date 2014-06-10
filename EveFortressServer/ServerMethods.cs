using EveFortressModel;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressServer
{
    public class ServerMethods
    {
        public LoginInformation Login(LoginInformation info, NetConnection connection)
        {
            return Program.PlayerManager.LoginAttempt(info, connection);
        }

        public LoginInformation RegisterUser(LoginInformation info, NetConnection connection)
        {
            return Program.PlayerManager.RegisterUser(info, connection);
        }

        public void Chat(string text, NetConnection connection)
        {
            var message = Program.PlayerManager.ConnectionNames[connection] + ": " + text;

            foreach (var c in Program.PlayerManager.Connections.Values)
            {
                Program.ClientMethods.ChatMessage(message, c);
            }
        }

        public Chunk SubscribeToChunk(long x, long y, NetConnection connection)
        {
            var player = Program.PlayerManager.Players[
                            Program.PlayerManager.ConnectionNames[connection]];
            player.SubscribedChunks.Add(Tuple.Create(x, y));
            return Program.WorldManager.GetChunk(x, y);
        }

        public void UnsubscribeToChunk(long x, long y, NetConnection connection)
        {
            var player = Program.PlayerManager.Players[
                            Program.PlayerManager.ConnectionNames[connection]];
            player.SubscribedChunks.Remove(Tuple.Create(x, y));
        }
    }
}
