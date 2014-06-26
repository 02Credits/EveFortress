 
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
                return Game.ClientNetworkManager.ExecuteMethodFromMessage<string>(msg, Game.ClientMethods.ChatMessage);
            };
            Parsers["UpdateChunk"] = (msg) =>
            {
                return Game.ClientNetworkManager.ExecuteMethodFromMessage<long, long, List<Voxel>>(msg, Game.ClientMethods.UpdateChunk);
            };
        }
    }
}

