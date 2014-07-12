using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class WindowManager : IUpdateNeeded, IDrawNeeded, IInputNeeded
    {
        public GameWindow Window { get; set; }
        public bool IgnoreSizeChanges { get; set; }
        public int TargetWidth { get; private set; }
        public int TargetHeight { get; private set; }

        public int TileWidth
        {
            get { return Window.ClientBounds.Width / Game.TileManager.TileSize; }
            set { TargetWidth = value * Game.TileManager.TileSize; }
        }
        public int TileHeight
        {
            get { return Window.ClientBounds.Height / Game.TileManager.TileSize; }
            set { TargetHeight = value * Game.TileManager.TileSize; }
        }

        public WindowManager()
        {
            Window = Game.GameWindow;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;
            WindowSizeChanged(null, null);
            Game.Updateables.Add(this);
            Game.Drawables.Add(this);
        }

        public void WindowSizeChanged(object sender, EventArgs e)
        {
            if (!IgnoreSizeChanges)
            {
                TargetWidth = Window.ClientBounds.Width;
                TargetHeight = Window.ClientBounds.Height;
            }
            else
            {
                IgnoreSizeChanges = false;
            }
        }

        public void ChangeTileSize(int tileSize)
        {
            if (tileSize < 8)
                tileSize = 8;
            if (tileSize > 48)
                tileSize = 48;

            var tooBig = false;
            if (tileSize * Game.TabManager.MinimumWidth > TargetWidth)
                tooBig = true;
            if (tileSize * Game.TabManager.MinimumHeight > TargetHeight)
                tooBig = true;

            if (tileSize != Game.TileManager.TileSize && !tooBig)
            {
                IgnoreSizeChanges = true;
                Game.TileManager.TileSize = tileSize;
            }
        }

        public void Update()
        {
            var tileWidth = (int)Math.Round((double)TargetWidth / Game.TileManager.TileSize);
            if (tileWidth < Game.TabManager.MinimumWidth)
            {
                tileWidth = Game.TabManager.MinimumWidth;
                TargetWidth = tileWidth * Game.TileManager.TileSize;
            }
            var tileHeight = (int)Math.Round((double)TargetHeight / Game.TileManager.TileSize);
            if (tileHeight < Game.TabManager.MinimumHeight)
            {
                tileHeight = Game.TabManager.MinimumHeight;
                TargetHeight = tileHeight * Game.TileManager.TileSize;
            }
            Game.Graphics.PreferredBackBufferWidth =
                tileWidth *
                Game.TileManager.TileSize;
            Game.Graphics.PreferredBackBufferHeight =
                tileHeight *
                Game.TileManager.TileSize;
            Game.Graphics.ApplyChanges();
            Game.TabManager.Resize();
        }

        public void Draw()
        {
            Window.Title = "EveFortress FPS:" + Game.TimeManager.FrameRate;
        }

        public Task<bool> ManageInput()
        {
            if (Game.InputManager.KeyTyped(Keys.OemPlus))
            {
                ChangeTileSize(Game.TileManager.TileSize + 1);
            }
            else if (Game.InputManager.KeyTyped(Keys.OemMinus))
            {
                ChangeTileSize(Game.TileManager.TileSize - 1);
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
