﻿using EveFortressModel;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class TabManager : IUpdateNeeded, IDrawNeeded, IInputNeeded
    {
        public TabSection MainSection { get; set; }

        public TabSection ActiveSection { get; set; }

        public int MinimumWidth
        {
            get
            {
                if (MainSection == null)
                    return 0;
                else
                    return MainSection.MinimumWidth + 2;
            }
        }

        public int MinimumHeight
        {
            get
            {
                if (MainSection == null)
                    return 0;
                else
                    return MainSection.MinimumHeight + 2;
            }
        }

        public TabManager()
        {
            MainSection = new TabSection(null, new LoginTab());
            MainSection.X = 1;
            MainSection.Y = 1;
            MainSection.Width = Game.GetSystem<WindowManager>().TileWidth - 2;
            MainSection.Height = Game.GetSystem<WindowManager>().TileHeight - 2;
            ActiveSection = MainSection;
            MainSection.Resize();
        }

        public void Resize()
        {
            MainSection.Width = Game.GetSystem<WindowManager>().TileWidth - 2;
            MainSection.Height = Game.GetSystem<WindowManager>().TileHeight - 2;
            MainSection.Resize();
        }

        public void Update()
        {
            MainSection.Update();
        }

        public void Draw()
        {
            Game.GetSystem<TileManager>().DrawTile(UITiles.BorderTopLeft, 0, 0);
            Game.GetSystem<TileManager>().DrawTile(UITiles.BorderTopRight, Game.GetSystem<WindowManager>().TileWidth - 1, 0);
            Game.GetSystem<TileManager>().DrawTile(UITiles.BorderBottomRight, Game.GetSystem<WindowManager>().TileWidth - 1, Game.GetSystem<WindowManager>().TileHeight - 1);
            Game.GetSystem<TileManager>().DrawTile(UITiles.BorderBottomLeft, 0, Game.GetSystem<WindowManager>().TileHeight - 1);
            for (int x = 1; x < Game.GetSystem<WindowManager>().TileWidth - 1; x++)
            {
                Game.GetSystem<TileManager>().DrawTile(UITiles.BorderHorizontal, x, 0);
                Game.GetSystem<TileManager>().DrawTile(UITiles.BorderHorizontal, x, Game.GetSystem<WindowManager>().TileHeight - 1);
            }
            for (int y = 1; y < Game.GetSystem<WindowManager>().TileHeight - 1; y++)
            {
                Game.GetSystem<TileManager>().DrawTile(UITiles.BorderVertical, 0, y);
                Game.GetSystem<TileManager>().DrawTile(UITiles.BorderVertical, Game.GetSystem<WindowManager>().TileWidth - 1, y);
            }
            MainSection.Render();
        }

        public async Task<bool> ManageInput()
        {
            MainSection.ManageMouseInput();
            return await ActiveSection.ManageInput();
        }
    }
}