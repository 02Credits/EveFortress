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

        public void UpdateChunk(long x, long y, long z, List<Tuple<byte, byte, byte, BlockTypes>> patch)
        {
            var chunk = Game.ChunkManager.GetChunk(x, y, z);
            if (chunk != null)
            {
                chunk.ApplyPatch(patch);
            }
        }
    }
}