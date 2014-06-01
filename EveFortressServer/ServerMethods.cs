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

        List<Action<string>> chatSubscriptions = new List<Action<string>>();
        List<string> chatMessages = new List<string>();
        public void Chat(string text, NetConnection connection)
        {
            var message = Program.PlayerManager.ConnectionNames[connection] + ": " + text;
            chatMessages.Add(message);
            foreach (var chatSub in chatSubscriptions)
            {
                chatSub(message);
            } 

            if (chatMessages.Count > 50)
            {
                chatMessages = chatMessages.Skip(chatMessages.Count - 50).ToList();
            }
        }

        public Action SubscribeToChatEvents(Action<string> callback)
        {
            chatSubscriptions.Add(callback);
            foreach (var chatMessage in chatMessages)
            {
                callback(chatMessage);
            }
            return () =>
            {
                chatSubscriptions.Remove(callback);
            };
        }

        public List<TileDisplayInformation> GetDisplayTiles(int left, int top, int right, int bottom, int z)
        {
            throw new NotImplementedException();
        }
    }
}
