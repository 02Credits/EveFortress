 
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
        public Task<object> ChatMessage(string message, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, string>(connection, "ChatMessage", message);
        }
        public Task<object> UpdateChunk(long x, long y, List<Voxel> patch, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, long, long, List<Voxel>>(connection, "UpdateChunk", x, y, patch);
        }
    }
}