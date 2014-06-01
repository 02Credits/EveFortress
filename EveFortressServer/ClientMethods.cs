 
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

namespace EveFortressServer
{
    public class ClientMethods
    {
        public long CurrentConversationID { get; set; }
		public long CurrentCallbackID { get; set; }
        public Dictionary<long, Action<NetIncomingMessage>> TaskCompletionSources { get; set; }
		public Dictionary<long, Action<NetIncomingMessage>> CallbackActions { get; set; }

        public ClientMethods()
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

        public Task<object> ConnectionEstablished(NetConnection connection)
        {
            var completionSource = new TaskCompletionSource<object>();
            var conversationID = GetNextConversationID();

            var message = Program.ServerNetworkManager.Clients.CreateMessage();
            message.Write("ConnectionEstablished");
            message.Write(conversationID);


            TaskCompletionSources[conversationID] = (msg) =>
            {
				completionSource.SetResult(null);
            };

            Program.ServerNetworkManager.Clients.SendMessage(message, connection, NetDeliveryMethod.ReliableUnordered);
            return completionSource.Task;
        }
    }
}