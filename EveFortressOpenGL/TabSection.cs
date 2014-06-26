using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class TabSection
    {
        public TabSection Parent { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public TabSection First { get; set; }
        public TabSection Second { get; set; }
        public Tab Tab { get; set; }

        public bool Populated { get; set; }
        public float SplitPercentage { get; set; }
        public bool Vertical { get; set; }

        int _width;
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value < MinimumWidth)
                    _width = MinimumWidth;
                else
                    _width = value;
            }
        }
        int _height;
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < MinimumHeight)
                    _height = MinimumHeight;
                else
                    _height = value;
            }
        }
        public int MinimumWidth
        {
            get
            {
                if (Populated)
                {
                    if (Vertical)
                    {
                        return First.MinimumWidth + Second.MinimumWidth + 1;
                    }
                    else
                    {
                        if (First.MinimumWidth > Second.MinimumWidth)
                        {
                            return First.MinimumWidth;
                        }
                        else
                        {
                            return Second.MinimumWidth;
                        }
                    }
                }
                else
                {
                    return Tab.MinimumWidth;
                }
            }
        }

        public int MinimumHeight
        {
            get
            {
                if (Populated)
                {
                    if (!Vertical)
                    {
                        return First.MinimumHeight + Second.MinimumHeight + 1;
                    }
                    else
                    {
                        if (First.MinimumHeight > Second.MinimumHeight)
                        {
                            return First.MinimumHeight;
                        }
                        else
                        {
                            return Second.MinimumHeight;
                        }
                    }
                }
                else
                {
                    return Tab.MinimumHeight;
                }
            }
        }

        public TabSection(TabSection parent, Tab tab)
        {
            Parent = parent;
            Tab = tab;
            tab.ParentSection = this;
        }

        public void ReplaceTab(Tab newTab)
        {
            if (Populated)
            {
                First.Close();
                Second.Close();
            }
            else
            {
                Tab.ParentSection = null;
                Tab.HandleClosing();
            }
            Tab = newTab;
            Tab.ParentSection = this;
            Resize();
        }

        public void Update()
        {
            if (Populated)
            {
                First.Update();
                Second.Update();
                if (!Game.InputManager.MouseLeftDown)
                    draggingDivider = false;
            }
            else
            {
                Tab.Update();
            }
        }

        public void Render()
        {
            if (Populated)
            {
                if (Vertical)
                {
                    var x = X + First.Width;
                    Game.TileManager.DrawTile(UITiles.BorderCapTop, x, Y - 1);
                    for (int y = Y; y < Y + Height; y++)
                    {
                        Game.TileManager.DrawTile(UITiles.BorderVertical, x, y);
                    }
                    Game.TileManager.DrawTile(UITiles.BorderCapBottom, x, Y + Height);
                }
                else
                {
                    var y = Y + First.Height;
                    Game.TileManager.DrawTile(UITiles.BorderCapLeft, X - 1, y);
                    for (int x = X; x < X + Width; x++)
                    {
                        Game.TileManager.DrawTile(UITiles.BorderHorizontal, x, y);
                    }
                    Game.TileManager.DrawTile(UITiles.BorderCapRight, X + Width, y);
                }
                First.Render();
                Second.Render();
            }
            else
            {
                Tab.Render();
                if (Game.TabManager.ActiveSection != this)
                {
                    Game.TileManager.DefaultColor = Color.DarkGray;
                }
                var stringMaxLength = Width - 4;
                var stringLength = stringMaxLength < Tab.Title.Length ? stringMaxLength : Tab.Title.Length;
                Game.TileManager.DrawStringAt(X + 1, Y - 1, Tab.Title.Take(stringLength));
                Game.TileManager.DefaultColor = Color.White;
            }
        }

        public bool Split(bool vertical, bool first, Tab newTab)
        {
            if (!Populated)
            {
                if (vertical)
                {
                    if (Tab.MinimumWidth + newTab.MinimumWidth >= Width)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Tab.MinimumHeight + newTab.MinimumHeight >= Height)
                    {
                        return false;
                    }
                }
                Populated = true;
                Vertical = vertical;
                SplitPercentage = 0.5f;
                if (first)
                {
                    First = new TabSection(this, newTab);
                    Second = new TabSection(this, Tab);
                }
                else
                {
                    First = new TabSection(this, Tab);
                    Second = new TabSection(this, newTab);
                }
                Tab = null;
                Game.TabManager.ActiveSection = first ? First : Second;
                Resize();
            }
            return true;
        }

        public void CloseChild(TabSection section)
        {
            section.Close();
            var other = section == First ? Second : First;
            if (Parent != null)
            {
                other.Parent = Parent;
                if (Parent.First == this)
                {
                    Parent.First = other;
                }
                else
                {
                    Parent.Second = other;
                }
            }
            else
            {
                other.Parent = null;
                Game.TabManager.MainSection = other;
                other.X = 1;
                other.Y = 1;
            }
            Game.TabManager.ActiveSection = other;
            Game.TabManager.MainSection.Resize();
        }

        public void Close()
        {
            if (Populated)
            {
                Tab.HandleClosing();
            }
            else
            {
                if (First != null)
                    First.Close();
                if (Second != null)
                    Second.Close();
            }
        }

        public void Resize()
        {
            if (Populated)
            {
                First.X = X;
                First.Y = Y;
                if (Vertical)
                {
                    First.Width = (int)(Width * SplitPercentage) - 1;
                    First.Height = Height;
                    First.Resize();
                    if (First.Width < First.MinimumWidth)
                        First.Width = First.MinimumWidth;
                    Second.X = X + First.Width + 1;
                    Second.Y = Y;
                    Second.Width = Width - First.Width - 1;
                    Second.Height = Height;
                    Second.Resize();
                    if (Second.Width == Second.MinimumWidth)
                    {
                        First.Width = Width - Second.Width - 1;
                        Second.X = X + First.Width + 1;
                    }
                }
                else
                {
                    First.Width = Width;
                    First.Height = (int)(Height * SplitPercentage) - 1;
                    First.Resize();
                    if (First.Height < First.MinimumHeight)
                        First.Height = First.MinimumHeight;
                    Second.X = X;
                    Second.Y = Y + First.Height + 1;
                    Second.Width = Width;
                    Second.Height = Height - First.Height - 1;
                    Second.Resize();
                    if (Second.Height == Second.MinimumHeight)
                    {
                        First.Height = Height - Second.Height - 1;
                        Second.Y = Y + First.Height + 1;
                    }
                }
                First.Resize();
                Second.Resize();
            }
            else
            {
                Tab.X = X;
                Tab.Y = Y;
                Tab.Width = Width - 1;
                Tab.Height = Height - 1;
            }
        }

        public TabSection GetSectionMouseIsIn()
        {
            int x = Game.InputManager.MouseTilePosition.X;
            int y = Game.InputManager.MouseTilePosition.Y;
            if (Populated)
            {
                if (Vertical)
                {
                    if (First.X <= x && First.X + First.Width > x)
                    {
                        return First;
                    }
                    else if (Second.X <= x && Second.X + Second.Width > x)
                    {
                        return Second;
                    }
                }
                else
                {
                    if (First.Y <= y && First.Y + First.Height > y)
                    {
                        return First;
                    }
                    else if (Second.Y <= y && Second.Y + Second.Height > y)
                    {
                        return Second;
                    }
                }
            }
            return null;
        }

        public async Task<bool> ManageInput()
        {
            if (!Populated)
            {
                if ((Game.InputManager.KeyDown(Keys.LeftControl) || Game.InputManager.KeyDown(Keys.RightControl)) &&
                     Game.ClientNetworkManager.Connected && !(Tab is LoginTab))
                {
                    if (Game.InputManager.KeyPressed(Keys.Left))
                    {
                        Split(true, true, new NewTab());
                        return true;
                    }
                    else if (Game.InputManager.KeyPressed(Keys.Right))
                    {
                        Split(true, false, new NewTab());
                        return true;
                    }
                    else if (Game.InputManager.KeyPressed(Keys.Down))
                    {
                        Split(false, true, new NewTab());
                        return true;
                    }
                    else if (Game.InputManager.KeyPressed(Keys.Up))
                    {
                        Split(false, false, new NewTab());
                        return true;
                    }
                }
                else if (Game.InputManager.KeyPressed(Keys.Escape))
                {
                    if (Parent != null)
                    {
                        Parent.CloseChild(this);
                        return true;
                    }
                    else if (Game.ClientNetworkManager.Connected && !(Tab is LoginTab))
                    {
                        ReplaceTab(new NewTab());
                    }
                }
                else if (Game.InputManager.KeyPressed(Keys.Home) && Game.ClientNetworkManager.Connected)
                {
                    ReplaceTab(new NewTab());
                }
                else
                {
                    return await Tab.ManageInput();
                }
            }
            return false;
        }

        bool draggingDivider = false;
        public void ManageMouseInput()
        {
            if (!Populated)
            {
                if (Game.InputManager.MouseLeftDown)
                {
                    Game.TabManager.ActiveSection = this;
                }
                Tab.ManageMouseInput();
            }
            else
            {
                var mousedOverSection = GetSectionMouseIsIn();
                if (Game.InputManager.MouseLeftDown)
                {
                    if (draggingDivider)
                    {
                        int dimension;
                        int pixelPos;
                        int mousePos;
                        float newPercentage;
                        if (Vertical)
                        {
                            dimension = Width * Game.TileManager.TileSize;
                            pixelPos = (X - 1) * Game.TileManager.TileSize;
                            mousePos = Game.InputManager.MousePixelPosition.X;
                        }
                        else
                        {
                            dimension = Height * Game.TileManager.TileSize;
                            pixelPos = (Y - 1) * Game.TileManager.TileSize;
                            mousePos = Game.InputManager.MousePixelPosition.Y;
                        }
                        newPercentage = ((float)mousePos - pixelPos) / dimension;
                        if (newPercentage > 0 && newPercentage < 1)
                        {
                            SplitPercentage = newPercentage;
                        }
                    }
                    else
                    {
                        if (mousedOverSection != null)
                        {
                            mousedOverSection.ManageMouseInput();
                        }
                        else
                        {
                            var tilePos = Game.InputManager.MouseTilePosition;
                            if (tilePos.X > 0 && tilePos.X < Game.WindowManager.TileWidth - 1 &&
                                tilePos.Y > 0 && tilePos.Y < Game.WindowManager.TileHeight - 1)
                            {
                                draggingDivider = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
