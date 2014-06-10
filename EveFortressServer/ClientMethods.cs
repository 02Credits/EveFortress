 
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

        public Task<object> ChatMessage(string message, NetConnection connection)
        {
            var _completionSource = new TaskCompletionSource<object>();
            var _conversationID = GetNextConversationID();

            var _message = Program.ServerNetworkManager.Clients.CreateMessage();
            _message.Write("ChatMessage");
            _message.Write(_conversationID);

            byte[] _messageData = SerializationUtils.Serialize(message);
            _message.Write(_messageData.Length);
            _message.Write(_messageData);

            TaskCompletionSources[_conversationID] = (msg) =>
            {
				_completionSource.SetResult(null);
            };

            Program.ServerNetworkManager.Clients.SendMessage(_message, connection, NetDeliveryMethod.ReliableUnordered);
            return _completionSource.Task;
        }

        public Task<object> UpdateChunk(long x, long y, List<Voxel> patch, NetConnection connection)
        {
            var _completionSource = new TaskCompletionSource<object>();
            var _conversationID = GetNextConversationID();

            var _message = Program.ServerNetworkManager.Clients.CreateMessage();
            _message.Write("UpdateChunk");
            _message.Write(_conversationID);

            byte[] _xData = SerializationUtils.Serialize(x);
            _message.Write(_xData.Length);
            _message.Write(_xData);
            byte[] _yData = SerializationUtils.Serialize(y);
            _message.Write(_yData.Length);
            _message.Write(_yData);
            byte[] _patchData = SerializationUtils.Serialize(patch);
            _message.Write(_patchData.Length);
            _message.Write(_patchData);

            TaskCompletionSources[_conversationID] = (msg) =>
            {
				_completionSource.SetResult(null);
            };

            Program.ServerNetworkManager.Clients.SendMessage(_message, connection, NetDeliveryMethod.ReliableUnordered);
            return _completionSource.Task;
        }
    }
}