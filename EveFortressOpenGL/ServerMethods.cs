 
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
using Utils;

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

            byte[] infoData = SerializationUtils.Serialize(info);
            message.Write(infoData.Length);
            message.Write(infoData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                var bytes = msg.ReadBytes(dataLength);
				completionSource.SetResult(SerializationUtils.Deserialize<LoginInformation>(bytes));
                TaskCompletionSources.Remove(conversationID);
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

            byte[] infoData = SerializationUtils.Serialize(info);
            message.Write(infoData.Length);
            message.Write(infoData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                var bytes = msg.ReadBytes(dataLength);
				completionSource.SetResult(SerializationUtils.Deserialize<LoginInformation>(bytes));
                TaskCompletionSources.Remove(conversationID);
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

            byte[] textData = SerializationUtils.Serialize(text);
            message.Write(textData.Length);
            message.Write(textData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
				completionSource.SetResult(null);
                TaskCompletionSources.Remove(conversationID);
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }

        public Task<Chunk> SubscribeToChunk(long x, long y)
        {
            var completionSource = new TaskCompletionSource<Chunk>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("SubscribeToChunk");
            message.Write(conversationID);

            byte[] xData = SerializationUtils.Serialize(x);
            message.Write(xData.Length);
            message.Write(xData);
            byte[] yData = SerializationUtils.Serialize(y);
            message.Write(yData.Length);
            message.Write(yData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                var bytes = msg.ReadBytes(dataLength);
				completionSource.SetResult(SerializationUtils.Deserialize<Chunk>(bytes));
                TaskCompletionSources.Remove(conversationID);
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }

        public Task<object> UnsubscribeToChunk(long x, long y)
        {
            var completionSource = new TaskCompletionSource<object>();
            var conversationID = GetNextConversationID();
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("UnsubscribeToChunk");
            message.Write(conversationID);

            byte[] xData = SerializationUtils.Serialize(x);
            message.Write(xData.Length);
            message.Write(xData);
            byte[] yData = SerializationUtils.Serialize(y);
            message.Write(yData.Length);
            message.Write(yData);

            TaskCompletionSources[conversationID] = (msg) =>
            {
				completionSource.SetResult(null);
                TaskCompletionSources.Remove(conversationID);
            };

            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }
    }
}