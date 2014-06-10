using EveFortressModel;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class MapTab : UITab
    {
        public override int MinimumWidth
        {
            get { return 5; }
        }

        public override int MinimumHeight
        {
            get { return 5; }
        }

        public override string Title
        {
            get { return "Map Test"; }
        }

        long x = 0;
        long y = 0;
        byte z = (byte)(Chunk.DEPTH / 2);

        public override void Render()
        {
            var left = x - Width / 2;
            var right = left + Width;
            var top = y - Height / 2;
            var bottom = top + Height;
            var tiles = Game.ChunkManager.GetTiles(left, right, top, bottom, z);
            var dx = 0;
            var dy = 0;
            foreach (var tile in tiles)
            {
                Game.TileManager.DrawTile(tile, dx, dy, this);
                dx++;
                if (dx > Width)
                {
                    dy++;
                    dx = 0;
                }
            }

            var zLevelTiles = Game.TileManager.GetTilesFromString("Z" + z);
            for (int i = 0; i < zLevelTiles.Count; i++)
            {
                var dz = i + 2;
                Game.TileManager.DrawTile(zLevelTiles[i], Width, dz, this);
            }
        }

        public override Task<bool> ManageInput()
        {
            if (Game.InputManager.KeyTyped(Keys.A))
            {
                x -= 1;
                return Task.FromResult(true);
            }
            if (Game.InputManager.KeyTyped(Keys.D))
            {
                x += 1;
                return Task.FromResult(true);
            }
            if (Game.InputManager.KeyTyped(Keys.W))
            {
                y -= 1;
                return Task.FromResult(true);
            }
            if (Game.InputManager.KeyTyped(Keys.S))
            {
                y += 1;
                return Task.FromResult(true);
            }
            if (Game.InputManager.KeyTyped(Keys.E))
            {
                z += 1;
            }
            if (Game.InputManager.KeyTyped(Keys.C))
            {
                z -= 1;
            }
            return Task.FromResult(false);
        }
    }
}
