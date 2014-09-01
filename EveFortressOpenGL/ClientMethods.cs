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

        public void UpdateChunk(Point<long> loc, List<Tuple<Point<byte>, BlockTypes>> patch)
        {
            var chunk = Game.ChunkManager.GetChunk(loc);
            if (chunk != null)
            {
                chunk.ApplyPatch(patch);
            }
        }

        public void SendEntities(IEnumerable<EntityPatch> entityPatches)
        {
            Game.EntityManager.AddEntities(entityPatches.Select(p => p.CreateEntity()));
        }

        public void PatchEntity(EntityPatch patch)
        {
            Game.EntityManager.PatchEntity(patch);
        }
    }
}