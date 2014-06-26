 
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
            return Game.ClientNetworkManager.SendCommand<LoginInformation, LoginInformation>("Login", info);
        }
        public Task<LoginInformation> RegisterUser(LoginInformation info)
        {
            return Game.ClientNetworkManager.SendCommand<LoginInformation, LoginInformation>("RegisterUser", info);
        }
        public Task<object> Chat(string text)
        {
            return Game.ClientNetworkManager.SendCommand<object, string>("Chat", text);
        }
        public Task<Chunk> SubscribeToChunk(long x, long y)
        {
            return Game.ClientNetworkManager.SendCommand<Chunk, long, long>("SubscribeToChunk", x, y);
        }
        public Task<object> UnsubscribeToChunk(long x, long y)
        {
            return Game.ClientNetworkManager.SendCommand<object, long, long>("UnsubscribeToChunk", x, y);
        }
        public Task<object> SetVoxel(long x, long y, byte z, Voxel v)
        {
            return Game.ClientNetworkManager.SendCommand<object, long, long, byte, Voxel>("SetVoxel", x, y, z, v);
        }
    }
}