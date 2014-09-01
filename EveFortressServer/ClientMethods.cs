 
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
        public Task<object> UpdateChunk(Point<long> loc, List<Tuple<Point<byte>, BlockTypes>> patch, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, Point<long>, List<Tuple<Point<byte>, BlockTypes>>>(connection, "UpdateChunk", loc, patch);
        }
        public Task<object> SendEntities(IEnumerable<EntityPatch> entityPatches, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, IEnumerable<EntityPatch>>(connection, "SendEntities", entityPatches);
        }
        public Task<object> PatchEntity(EntityPatch patch, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, EntityPatch>(connection, "PatchEntity", patch);
        }
    }
}