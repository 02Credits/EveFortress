 
// GENERATED CODE! ALL CHANGES WILL BE OVERWRITTEN!!!
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
    public class MessageParser
    {
        public Dictionary<string, Action<NetIncomingMessage, long>> Parsers { get; set; }
        public MessageParser()
        {
            Parsers = new Dictionary<string, Action<NetIncomingMessage, long>>();
            PopulateParsers();
        }

        public void ParseMessage(NetIncomingMessage msg)
        {
            var commandName = msg.ReadString();
            var id = msg.ReadInt64();
            if (commandName == "response")
            {
				Console.WriteLine("Recieved Response for ConversationID: " + id);
                Program.ClientMethods.TaskCompletionSources[id](msg);
            }
			else if (commandName == "callback")
			{
				Console.WriteLine("Recieved Callback for CallbackID: " + id);
				Program.ClientMethods.CallbackActions[id](msg);
			}
            else
            {
				Console.WriteLine("Recieved " + commandName + "Message for ConversationID: " + id);
                Parsers[commandName](msg, id);
            }
        }

        public void SendResponse(long conversationID, NetConnection connection)
        {
            SendResponse(conversationID, connection, new byte[]{});
        }

        public void SendResponse(long conversationID, NetConnection connection, byte[] responseData)
        {
            var message = Program.ServerNetworkManager.Clients.CreateMessage();
            message.Write("response");
            message.Write(conversationID);
            if (responseData.Length != 0)
            {
                message.Write(responseData.Length);
                message.Write(responseData);
            }
            Program.ServerNetworkManager.Clients.SendMessage(message, connection, NetDeliveryMethod.ReliableUnordered);
        }

        private void PopulateParsers()
        {
            Parsers["Login"] = (msg, id) =>
            {
                var infoByteCount = msg.ReadInt32();
                var infoBytes = msg.ReadBytes(infoByteCount);
                var info = Program.ServerNetworkManager.Deserialize<LoginInformation>(infoBytes);
                var result = Program.ServerMethods.Login(info, msg.SenderConnection);
                byte[] resultData = Program.ServerNetworkManager.Serialize(result);
                SendResponse(id, msg.SenderConnection, resultData);
            };
            Parsers["RegisterUser"] = (msg, id) =>
            {
                var infoByteCount = msg.ReadInt32();
                var infoBytes = msg.ReadBytes(infoByteCount);
                var info = Program.ServerNetworkManager.Deserialize<LoginInformation>(infoBytes);
                var result = Program.ServerMethods.RegisterUser(info, msg.SenderConnection);
                byte[] resultData = Program.ServerNetworkManager.Serialize(result);
                SendResponse(id, msg.SenderConnection, resultData);
            };
            Parsers["Chat"] = (msg, id) =>
            {
                var textByteCount = msg.ReadInt32();
                var textBytes = msg.ReadBytes(textByteCount);
                var text = Program.ServerNetworkManager.Deserialize<string>(textBytes);
				Program.ServerMethods.Chat(text, msg.SenderConnection);
				SendResponse(id, msg.SenderConnection, new byte[]{});
            };
            Parsers["SubscribeToChatEvents"] = (msg, id) =>
            {
				var callbackID = msg.ReadInt64();
				var sender = msg.SenderConnection;
				Action<string> callback = (obj) => 
				{
					var callbackMessage = Program.ServerNetworkManager.Clients.CreateMessage();
					byte[] callbackParamData = Program.ServerNetworkManager.Serialize(obj);
					callbackMessage.Write("callback");
					callbackMessage.Write(callbackID);
					callbackMessage.Write(callbackParamData.Length);
					callbackMessage.Write(callbackParamData);
					Program.ServerNetworkManager.Clients.SendMessage(callbackMessage, sender, NetDeliveryMethod.ReliableUnordered);
				};
				var returnCallbackID = Program.ClientMethods.GetNextCallbackID();
				var returnAction = Program.ServerMethods.SubscribeToChatEvents(callback);
				Program.ClientMethods.CallbackActions[returnCallbackID] = (returnMsg) =>
				{
					returnAction();
				};
				var message = Program.ServerNetworkManager.Clients.CreateMessage();
				message.Write("response");
				message.Write(id);
				message.Write(returnCallbackID);
				Program.ServerNetworkManager.Clients.SendMessage(message, sender, NetDeliveryMethod.ReliableUnordered);
            };
        }
    }
}

