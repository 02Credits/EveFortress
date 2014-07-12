using EveFortressModel;
using System;
using System.Collections.Generic;

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
    }
}