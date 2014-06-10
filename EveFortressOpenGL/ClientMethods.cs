using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressClient
{
    public class ClientMethods
    {
        public void ChatMessage(string message)
        {
            Game.ChatManager.AddMessage(message);
        }

        public void UpdateChunk(long x, long y, List<Voxel> patch)
        {
            var chunk = Game.ChunkManager.GetChunk(x, y);
            if (chunk != null)
            {
                chunk.ApplyPatch(patch);
            }
        }
    }
}