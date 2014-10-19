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
            get { return Window.ClientBounds.Width / Game.GetSystem<TileManager>().TileSize; }
            set { TargetWidth = value * Game.GetSystem<TileManager>().TileSize; }
        }

        public int TileHeight
        {
            get { return Window.ClientBounds.Height / Game.GetSystem<TileManager>().TileSize; }
            set { TargetHeight = value * Game.GetSystem<TileManager>().TileSize; }
        }

        public WindowManager()
        {
            Window = Game.GameWindow;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;
            WindowSizeChanged(null, null);
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
            if (tileSize * Game.GetSystem<TabManager>().MinimumWidth > TargetWidth)
                tooBig = true;
            if (tileSize * Game.GetSystem<TabManager>().MinimumHeight > TargetHeight)
                tooBig = true;

            if (tileSize != Game.GetSystem<TileManager>().TileSize && !tooBig)
            {
                IgnoreSizeChanges = true;
                Game.GetSystem<TileManager>().TileSize = tileSize;
            }
        }

        public void Update()
        {
            if (Game.WindowActive)
            {
                var tileWidth = (int)Math.Round((double)TargetWidth / Game.GetSystem<TileManager>().TileSize);
                if (tileWidth < Game.GetSystem<TabManager>().MinimumWidth)
                {
                    tileWidth = Game.GetSystem<TabManager>().MinimumWidth;
                    TargetWidth = tileWidth * Game.GetSystem<TileManager>().TileSize;
                }
                var tileHeight = (int)Math.Round((double)TargetHeight / Game.GetSystem<TileManager>().TileSize);
                if (tileHeight < Game.GetSystem<TabManager>().MinimumHeight)
                {
                    tileHeight = Game.GetSystem<TabManager>().MinimumHeight;
                    TargetHeight = tileHeight * Game.GetSystem<TileManager>().TileSize;
                }
                Game.Graphics.PreferredBackBufferWidth =
                    tileWidth *
                    Game.GetSystem<TileManager>().TileSize;
                Game.Graphics.PreferredBackBufferHeight =
                    tileHeight *
                    Game.GetSystem<TileManager>().TileSize;
                Game.Graphics.ApplyChanges();
                Game.GetSystem<TabManager>().Resize();
            }
        }

        public void Draw()
        {
            Window.Title = "EveFortress FPS:" + Game.GetSystem<TimeManager>().FrameRate;
        }

        public Task<bool> ManageInput()
        {
            if (Game.GetSystem<InputManager>().KeyTyped(Keys.OemPlus))
            {
                ChangeTileSize(Game.GetSystem<TileManager>().TileSize + 1);
            }
            else if (Game.GetSystem<InputManager>().KeyTyped(Keys.OemMinus))
            {
                ChangeTileSize(Game.GetSystem<TileManager>().TileSize - 1);
            }
            else
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}