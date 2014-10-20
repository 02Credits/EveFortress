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
    }
}