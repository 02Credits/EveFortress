 
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

namespace EveFortressClient
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
                Game.ServerMethods.TaskCompletionSources[id](msg);
            }
			else if (commandName == "callback")
			{
				Console.WriteLine("Recieved Callback for CallbackID: " + id);
				Game.ServerMethods.CallbackActions[id](msg);
			}
            else
            {
                Parsers[commandName](msg, id);
            }
        }

        public void SendResponse(long conversationID)
        {
            SendResponse(conversationID, new byte[]{});
        }

        public void SendResponse(long conversationID, byte[] responseData)
        {
            var message = Game.ClientNetworkManager.Connection.CreateMessage();
            message.Write("response");
            message.Write(conversationID);
            if (responseData.Length != 0)
            {
                message.Write(responseData.Length);
                message.Write(responseData);
            }
            Game.ClientNetworkManager.Connection.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
        }

        private void PopulateParsers()
        {
            Parsers["ConnectionEstablished"] = (msg, id) =>
            {
				Game.ClientMethods.ConnectionEstablished();
				SendResponse(id, new byte[]{});
            };
        }
    }
}

