 
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
        public Task<LoginInformation> Login(LoginInformation info)
        {
            return Game.GetSystem<ClientNetworkManager>().SendCommand<LoginInformation, LoginInformation>("Login", info);
        }
        public Task<LoginInformation> RegisterUser(LoginInformation info)
        {
            return Game.GetSystem<ClientNetworkManager>().SendCommand<LoginInformation, LoginInformation>("RegisterUser", info);
        }
        public Task<object> Chat(string text)
        {
            return Game.GetSystem<ClientNetworkManager>().SendCommand<object, string>("Chat", text);
        }
    }
}