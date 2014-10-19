 
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
using Utils;

namespace EveFortressClient
{
    public class MessageParser
    {
        public Dictionary<string, Func<NetIncomingMessage, byte[]>> Parsers { get; set; }
        public MessageParser()
        {
            Parsers = new Dictionary<string, Func<NetIncomingMessage, byte[]>>();
            PopulateParsers();
        }

        public byte[] ParseMessage(string command, NetIncomingMessage msg)
        {
            return Parsers[command](msg);
        }

        private void PopulateParsers()
        {
            Parsers["ChatMessage"] = (msg) =>
            {
                return Game.GetSystem<ClientNetworkManager>().ExecuteMethodFromMessage<string>(msg, Game.GetSystem<ClientMethods>().ChatMessage);
            };
            Parsers["UpdateChunk"] = (msg) =>
            {
                return Game.GetSystem<ClientNetworkManager>().ExecuteMethodFromMessage<Point<long>, List<Patch>>(msg, Game.GetSystem<ClientMethods>().UpdateChunk);
            };
            Parsers["SendNewEntity"] = (msg) =>
            {
                return Game.GetSystem<ClientNetworkManager>().ExecuteMethodFromMessage<Entity>(msg, Game.GetSystem<ClientMethods>().SendNewEntity);
            };
        }
    }
}

