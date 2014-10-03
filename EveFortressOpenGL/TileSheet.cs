using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class TileSheet
    {
        public Texture2D Texture { get; set;}

        public int TileSize { get; set; }

        public double Z { get; set; }

        public int TileWidth
        {
            get
            {
                return (Texture.Width - 1) / (TileSize + 1);
            }
        }

        public int TileHeight
        {
            get
            {
                return (Texture.Height - 1) / (TileSize + 1);
            }
        }

        public int Count
        {
            get
            {
                return TileWidth * TileHeight;
            }
        }

        public TileSheet(Texture2D texture, int tileSize, double z)
        {
            Texture = texture;
            TileSize = tileSize;
            Z = z;
        }
    }
}
