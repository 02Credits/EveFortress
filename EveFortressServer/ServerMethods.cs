using EveFortressModel;
using Lidgren.Network;
using System.Linq;

namespace EveFortressServer
{
    public class ServerMethods
    {
        public LoginInformation Login(LoginInformation info, NetConnection connection)
        {
            var loginInformation = Program.PlayerManager.LoginAttempt(info, connection);
            return loginInformation;
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
            player.SubscribedChunks.Add(new Point<long>(x, y));
            var chunk = Program.WorldManager.GetChunk(x, y);
            return chunk;
        }

        public void UnsubscribeToChunk(long x, long y, NetConnection connection)
        {
            var player = Program.PlayerManager.Players[
                            Program.PlayerManager.ConnectionNames[connection]];
            player.SubscribedChunks.Remove(new Point<long>(x, y));
        }
    }
}