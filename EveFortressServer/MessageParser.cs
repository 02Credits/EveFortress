 
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
                Func<LoginInformation, LoginInformation> method = (T0) => Program.ServerMethods.Login(T0, msg.SenderConnection);
                return Program.ServerNetworkManager.ExecuteMethodFromMessage<LoginInformation, LoginInformation>(msg, method);
            };
            Parsers["RegisterUser"] = (msg) =>
            {
                Func<LoginInformation, LoginInformation> method = (T0) => Program.ServerMethods.RegisterUser(T0, msg.SenderConnection);
                return Program.ServerNetworkManager.ExecuteMethodFromMessage<LoginInformation, LoginInformation>(msg, method);
            };
            Parsers["Chat"] = (msg) =>
            {
                Action<string> method = (T0) => Program.ServerMethods.Chat(T0, msg.SenderConnection);
                return Program.ServerNetworkManager.ExecuteMethodFromMessage<string>(msg, method);
            };
            Parsers["SubscribeToChunk"] = (msg) =>
            {
                Func<long, long, long, Chunk> method = (T0, T1, T2) => Program.ServerMethods.SubscribeToChunk(T0, T1, T2, msg.SenderConnection);
                return Program.ServerNetworkManager.ExecuteMethodFromMessage<long, long, long, Chunk>(msg, method);
            };
            Parsers["UnsubscribeToChunk"] = (msg) =>
            {
                Action<long, long, long> method = (T0, T1, T2) => Program.ServerMethods.UnsubscribeToChunk(T0, T1, T2, msg.SenderConnection);
                return Program.ServerNetworkManager.ExecuteMethodFromMessage<long, long, long>(msg, method);
            };
        }
    }
}

