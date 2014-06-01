 
// GENERATED FILE! CHANGES WILL BE OVERWRITTEN

using EveFortressModel;
using Lidgren.Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ServerMethods
    {
        public long CurrentConversationID { get; set; }
		public long CurrentCallbackID { get; set; }
        public Dictionary<long, Action<NetIncomingMessage>> TaskCompletionSources { get; set; }
		public Dictionary<long, Action<NetIncomingMessage>> CallbackActions { get; set; }

        public ServerMethods()
        {
            TaskCompletionSources = new Dictionary<long, Action<NetIncomingMessage>>();
			CallbackActions = new Dictionary<long, Action<NetIncomingMessage>>();
        }

        public long GetNextConversationID()
        {
            var id = CurrentConversationID;
            CurrentConversationID++;
            return id;
        }

		public long GetNextCallbackID()
		{
			var id = CurrentCallbackID;
			CurrentCallbackID++;
			return id;
		}

        public Task<LoginInformation> Login(LoginInformation info)
        {
            var completionSource = new TaskCompletionSource<LoginInformation>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("Login");
            message.Write(conversationID);

            byte[] infoData = Game.ClientNetworkManager.Serialize(info);
            message.Write(infoData.Length);
            message.Write(infoData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                var bytes = msg.ReadBytes(dataLength);
				completionSource.SetResult(Game.ClientNetworkManager.Deserialize<LoginInformation>(bytes));
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }

        public Task<LoginInformation> RegisterUser(LoginInformation info)
        {
            var completionSource = new TaskCompletionSource<LoginInformation>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("RegisterUser");
            message.Write(conversationID);

            byte[] infoData = Game.ClientNetworkManager.Serialize(info);
            message.Write(infoData.Length);
            message.Write(infoData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                var bytes = msg.ReadBytes(dataLength);
				completionSource.SetResult(Game.ClientNetworkManager.Deserialize<LoginInformation>(bytes));
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }

        public Task<object> Chat(string text)
        {
            var completionSource = new TaskCompletionSource<object>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("Chat");
            message.Write(conversationID);

            byte[] textData = Game.ClientNetworkManager.Serialize(text);
            message.Write(textData.Length);
            message.Write(textData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
				completionSource.SetResult(null);
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }

        public Task<Action> SubscribeToChatEvents(Action<string> callback)
        {
            var completionSource = new TaskCompletionSource<Action>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("SubscribeToChatEvents");
            message.Write(conversationID);

			var callbackID = GetNextCallbackID();
			CallbackActions[callbackID] = (msg) =>
			{
				var byteLength = msg.ReadInt32();
				var bytes = msg.ReadBytes(byteLength);
				var param = Game.ClientNetworkManager.Deserialize<string>(bytes);
				callback(param);
			};
			message.Write(callbackID);

            TaskCompletionSources[conversationID] = (msg) =>
            {
				var returnCallbackID = msg.ReadInt64();
				Action returnParam = () =>
				{
					var callbackMessage = Game.ClientNetworkManager.Connection.CreateMessage();
					callbackMessage.Write("callback");
					callbackMessage.Write(returnCallbackID);
					Game.ClientNetworkManager.Connection.SendMessage(callbackMessage, NetDeliveryMethod.ReliableUnordered);
				};
				completionSource.SetResult(returnParam);
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }
    }
}