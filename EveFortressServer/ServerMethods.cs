using EveFortressModel;
using Lidgren.Network;
using System.Linq;

namespace EveFortressServer
{
    public class ServerMethods
    {
        public LoginInformation Login(LoginInformation info, NetConnection connection)
        {
            var loginInformation = Program.GetSystem<PlayerManager>().LoginAttempt(info, connection);
            return loginInformation;
        }

        public LoginInformation RegisterUser(LoginInformation info, NetConnection connection)
        {
            return Program.GetSystem<PlayerManager>().RegisterUser(info, connection);
        }

        public void Chat(string text, NetConnection connection)
        {
            var message = Program.GetSystem<PlayerManager>().ConnectionNames[connection] + ": " + text;

            foreach (var c in Program.GetSystem<PlayerManager>().Connections.Values)
            {
                Program.GetSystem<ClientMethods>().ChatMessage(message, c);
            }
        }

        public Chunk SubscribeToChunk(long x, long y, NetConnection connection)
        {
            var player = Program.GetSystem<PlayerManager>().Players[
                            Program.GetSystem<PlayerManager>().ConnectionNames[connection]];
            player.SubscribedChunks.Add(new Point<long>(x, y));
            var chunk = Program.GetSystem<ChunkManager>().GetChunk(x, y);
            return chunk;
        }

        public void UnsubscribeToChunk(long x, long y, NetConnection connection)
        {
            var player = Program.GetSystem<PlayerManager>().Players[
                            Program.GetSystem<PlayerManager>().ConnectionNames[connection]];
            player.SubscribedChunks.Remove(new Point<long>(x, y));
        }
    }
}