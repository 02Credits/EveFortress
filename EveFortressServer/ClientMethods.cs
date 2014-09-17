 
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
        public Task<object> UpdateChunk(Point<long> chunkPosition, List<Patch> patches, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, Point<long>, List<Patch>>(connection, "UpdateChunk", chunkPosition, patches);
        }
        public Task<object> PatchEntity(EntityPatch patch, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, EntityPatch>(connection, "PatchEntity", patch);
        }
        public Task<object> SendNewEntity(Entity entity, NetConnection connection)
        {
            return Program.ServerNetworkManager.SendCommand<object, Entity>(connection, "SendNewEntity", entity);
        }
    }
}