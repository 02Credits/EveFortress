using EveFortressModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EveFortressClient
{
    public class TileManager
    {

        private Dictionary<string, TileSheet> TileSheets = new Dictionary<string, TileSheet>();

        public Color DefaultColor { get; set; }

        public int TileSize { get; set; }

        public TileManager()
        {
            TileSheets["Entities"] = new TileSheet(Game.ContentManager.Load<Texture2D>("EntityTiles.png"), 16, 0);
            TileSheets["Items"] = new TileSheet(Game.ContentManager.Load<Texture2D>("ItemTiles.png"), 16, 1);
            TileSheets["UI"] = new TileSheet(Game.ContentManager.Load<Texture2D>("UITiles.png"), 16, 2);

            DefaultColor = Color.White;
            TileSize = 16;
        }

        public void DrawTile(List<TileDisplayInformation> tilesToDraw, int TilePositionX, int TilePositionY, IUIElementContainer parent = null)
        {
            foreach (var tile in tilesToDraw)
            {
                DrawTile(tile, TilePositionX, TilePositionY, parent);
            }
        }

        public void DrawTile(TileDisplayInformation tileToDraw, int TilePositionX, int TilePositionY, IUIElementContainer parent = null)
        {
            if (parent != null)
            {
                if (TilePositionX < 0 || TilePositionX >= parent.X + parent.Width ||
                    TilePositionY < 0 || TilePositionY >= parent.Y + parent.Height)
                {
                    return;
                }
            }

            var currentParent = parent;
            while (currentParent != null)
            {
                TilePositionX += currentParent.X;
                TilePositionY += currentParent.Y;
                currentParent = currentParent.Parent;
            }

            Color color;
            if (tileToDraw.IncludeColor)
            {
                color = new Color(
                    tileToDraw.R,
                    tileToDraw.G,
                    tileToDraw.B,
                    tileToDraw.A);
            }
            else
            {
                color = DefaultColor;
            }

            var destinationRectangle = new Rectangle(TilePositionX * TileSize, TilePositionY * TileSize, TileSize, TileSize);

            DrawTileFromSheet(tileToDraw.TileNumber, TileSheets[tileToDraw.SheetID], destinationRectangle, color);
        }

        public void DrawTileFromSheet(int index, TileSheet tileSheet, Rectangle destination, Color color)
        {
            if (index != 0)
            {
                index -= 1;
                var tileX = index % (tileSheet.Texture.Width / tileSheet.TileSize);
                var tileY = (index - tileX) / (tileSheet.Texture.Width / tileSheet.TileSize);
                var sourceRect = new Rectangle(tileX * tileSheet.TileSize, tileY * tileSheet.TileSize, tileSheet.TileSize, tileSheet.TileSize);
                Game.SpriteManager.AddSprite(tileSheet.Texture, destination, tileSheet.Z, sourceRect, color);
            }
        }

        public TileDisplayInformation GetTileFromChar(char c)
        {
            switch (c)
            {
                case '0': return UITiles.Zero;
                case '1': return UITiles.One;
                case '2': return UITiles.Two;
                case '3': return UITiles.Three;
                case '4': return UITiles.Four;
                case '5': return UITiles.Five;
                case '6': return UITiles.Six;
                case '7': return UITiles.Seven;
                case '8': return UITiles.Eight;
                case '9': return UITiles.Nine;
                case '-': return UITiles.Dash;
                case '_': return UITiles.Underscore;
                case '(': return UITiles.LeftParen;
                case ')': return UITiles.RightParen;
                case '!': return UITiles.ExclamationPoint;
                case '/': return UITiles.ForwardSlash;
                case '\\': return UITiles.BackSlash;
                case ':': return UITiles.Collen;
                case ';': return UITiles.Semicollen;
                case ' ': return UITiles.None;
                case '?': return UITiles.QuestionMark;
                case '{': return UITiles.LeftCurleyBrace;
                case '}': return UITiles.RightCurleyBrace;
                case '|': return UITiles.Pipe;
                case '.': return UITiles.Period;
                case '*': return UITiles.Asterix;
                default:
                    UITiles tile;
                    var success = Enum.TryParse<UITiles>(c.ToString(), out tile);
                    if (success)
                        return tile;
                    else
                        return UITiles.Asterix;
            }
        }

        public List<TileDisplayInformation> GetTilesFromString(IEnumerable<char> s)
        {
            var returnList = new List<TileDisplayInformation>();
            foreach (var c in s)
            {
                returnList.Add(GetTileFromChar(c));
            }
            return returnList;
        }

        public void DrawStringAt(int x, int y, IEnumerable<char> s, IUIElementContainer parent = null)
        {
            DrawStringAt(x, y, s, DefaultColor, parent);
        }

        public void DrawStringAt(int x, int y, IEnumerable<char> s, Color textColor, IUIElementContainer parent = null)
        {
            var tiles = GetTilesFromString(s);
            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                tile.R = textColor.R;
                tile.G = textColor.G;
                tile.B = textColor.B;
                tile.A = textColor.A;
                tile.IncludeColor = true;
                var currentX = x + i;
                DrawTile(tile, currentX, y, parent);
            }
        }

        public void DrawBlock(int left, int top, int width, int height, TileDisplayInformation tileToDraw, IUIElementContainer parent = null)
        {
            for (int y = top; y < top + height; y++)
            {
                for (int x = left; x < left + width; x++)
                {
                    DrawTile(tileToDraw, x, y, parent);
                }
            }
        }
    }
}