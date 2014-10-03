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
            var chunk = Game.ChunkManager.GetChunk(Chunk.GetChunkCoords(patch.PreviousPosition));
            if (chunk != null)
            {
                if (chunk.Entities.ContainsKey(patch.ID))
                {
                    var entity = chunk.Entities[patch.ID];
                    entity.ApplyPatch(patch);
                    if (!chunk.ContainsLoc(patch.Position))
                    {
                        chunk.Entities.Remove(entity.ID);
                        Game.ChunkManager.GetChunk(Chunk.GetChunkCoords(entity.Position)).Entities[entity.ID] = entity;
                    }
                }
            }
        }

        public void SendNewEntity(Entity entity)
        {
            var chunkLoc = Chunk.GetChunkCoords(entity.Position);
            var chunk = Game.ChunkManager.GetChunk(chunkLoc);
            chunk.Entities[entity.ID] = entity;
        }
    }
}