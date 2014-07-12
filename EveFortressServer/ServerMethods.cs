using EveFortressModel;
using Lidgren.Network;

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

        public Chunk SubscribeToChunk(long x, long y, long z, NetConnection connection)
        {
            var player = Program.PlayerManager.Players[
                            Program.PlayerManager.ConnectionNames[connection]];
            player.SubscribedChunks.Add(new Point<long>(x, y, z));
            var chunk = Program.WorldManager.GetChunk(x, y, z);
            chunk.Blocks.PackUp();
            return chunk;
        }

        public void UnsubscribeToChunk(long x, long y, long z, NetConnection connection)
        {
            var player = Program.PlayerManager.Players[
                            Program.PlayerManager.ConnectionNames[connection]];
            player.SubscribedChunks.Remove(new Point<long>(x, y, z));
        }
    }
}
