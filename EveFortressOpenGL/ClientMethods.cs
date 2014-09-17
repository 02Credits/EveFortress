using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EveFortressClient
{
    public class ClientMethods
    {
        public void ChatMessage(string message)
        {
            Game.ChatManager.AddMessage(message);
        }

        public void UpdateChunk(Point<long> chunkPosition, List<Patch> patches)
        {
            var chunk = Game.ChunkManager.GetChunk(chunkPosition);

            foreach (var patch in patches)
            {
                chunk.ApplyPatch(patch);
            }
        }

        public void PatchEntity(EntityPatch patch)
        {
            Game.EntityManager.PatchEntity(patch);
        }

        public void SendNewEntity(Entity entity)
        {
            var chunkLoc = Chunk.GetChunkCoords(entity.Position);
            var chunk = Game.ChunkManager.GetChunk(chunkLoc);
            chunk.Entities[entity.ID] = entity;
        }
    }
}