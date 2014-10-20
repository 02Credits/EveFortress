 
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

namespace EveFortressServer
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
            Parsers["Login"] = (msg) =>
            {
                Func<LoginInformation, LoginInformation> method = (T0) => Program.GetSystem<ServerMethods>().Login(T0, msg.SenderConnection);
                return Program.GetSystem<ServerNetworkManager>().ExecuteMethodFromMessage<LoginInformation, LoginInformation>(msg, method);
            };
            Parsers["RegisterUser"] = (msg) =>
            {
                Func<LoginInformation, LoginInformation> method = (T0) => Program.GetSystem<ServerMethods>().RegisterUser(T0, msg.SenderConnection);
                return Program.GetSystem<ServerNetworkManager>().ExecuteMethodFromMessage<LoginInformation, LoginInformation>(msg, method);
            };
            Parsers["Chat"] = (msg) =>
            {
                Action<string> method = (T0) => Program.GetSystem<ServerMethods>().Chat(T0, msg.SenderConnection);
                return Program.GetSystem<ServerNetworkManager>().ExecuteMethodFromMessage<string>(msg, method);
            };
        }
    }
}

