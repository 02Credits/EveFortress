using EveFortressModel;
using Microsoft.Xna.Framework.Input;
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

        private long x = 0;
        private long y = 0;
        private long z = Chunk.TERRAIN_HEIGHT;

        private long left
        {
            get
            {
                return x - Width / 2;
            }
        }

        private long right
        {
            get
            {
                return left + Width;
            }
        }

        private long top
        {
            get
            {
                return y - Height / 2;
            }
        }

        private long bottom
        {
            get
            {
                return top + Height;
            }
        }

        public override void Render()
        {
            var dx = 0;
            var dy = 0;
            while (top + dy <= bottom)
            {
                var displayInfo = Game.ChunkManager.PerspectiveRayCast(x, y, (byte)(z + 5), left + dx, top + dy, z);
                Game.TileManager.DrawTile(displayInfo, dx, dy, this);
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
                var tileZ = i + 2;
                Game.TileManager.DrawTile(zLevelTiles[i], Width, tileZ, this);
            }

            var coordsTiles = Game.TileManager.GetTilesFromString("X" + x + "Y" + y);
            var startPos = Width - coordsTiles.Count;
            for (int i = 0; i < coordsTiles.Count; i++)
            {
                var tileX = i + startPos;
                Game.TileManager.DrawTile(coordsTiles[i], tileX, 0, this);
            }
        }

        public override Task<bool> ManageInput()
        {
            var inputManaged = false;
            if (Game.InputManager.KeyDown(Keys.A))
            {
                x -= 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.D))
            {
                x += 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.W))
            {
                y -= 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyDown(Keys.S))
            {
                y += 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyTyped(Keys.C))
            {
                z += 1;
                inputManaged = true;
            }
            if (Game.InputManager.KeyTyped(Keys.E))
            {
                z -= 1;
                if (z < 0)
                {
                    z = 0;
                }
                inputManaged = true;
            }
            return Task.FromResult(inputManaged);
        }
    }
}