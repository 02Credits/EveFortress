using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveFortressModel;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class WindowManager : IUpdateNeeded, IDrawNeeded, IInputNeeded
    {
        public GameWindow Window { get; set; }
        public bool IgnoreSizeChanges { get; set; }
        public int TargetWidth { get; private set; }
        public int TargetHeight { get; private set; }

        int frameRate = 0;
        int frameCounter = 0;
        long lastFrameTime = 0;
        long elapsedTime = 0;

        DateTime windowOpened;

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

        public WindowManager(GameWindow window)
        {
            Window = window;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowSizeChanged;
            WindowSizeChanged(null, null);
            windowOpened = DateTime.Now;
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
            Game.Time = (long)(DateTime.Now - windowOpened).TotalMilliseconds;
            var frameTime = Game.Time - lastFrameTime;
            lastFrameTime = Game.Time;
            elapsedTime += frameTime;

            if (elapsedTime > 1000)
            {
                elapsedTime -= 1000;
                frameRate = frameCounter;
                frameCounter = 0;
            }

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
            frameCounter++;
            Window.Title = "EveFortress FPS:" + frameRate;
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
