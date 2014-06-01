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
    public class Button : UIElement
    {
        public override bool Activateable
        {
            get { return true; }
        }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        public Action OnClicked { get; set; }

        public Button(IUIElementContainer parent, int x, int y, Action onClicked, string text = "OK")
            : this(parent, x, y, onClicked, Color.White, text) { }

        public Button(IUIElementContainer parent, int x, int y, Action onClicked, Color textColor, string text = "OK")
            : this(parent, x, y, text.Length + 4, onClicked, textColor, text) { }

        public Button(IUIElementContainer parent, int x, int y, int width, Action onClicked, string text = "OK")
            : this(parent, x, y, width, onClicked, Color.White, text) { }

        public Button(IUIElementContainer parent, int x, int y, int width, Action onClicked, Color textColor, string text = "OK")
            : base(parent, x, y, width, 1)
        {
            Text = text;
            TextColor = textColor;
            OnClicked = onClicked;
        }

        public override void Draw()
        {
            var bracketColor = ActiveElement ? Color.DarkGray : Color.Gray;
            if (MouseOver)
            {
                bracketColor = Color.LightGray;
            }

            Game.TileManager.DrawTile(
                UITiles.IndicateLeft,
                X, Y, Parent);
            Game.TileManager.DrawTile(
                UITiles.IndicateRight,
                X + Width - 1, Y, Parent);

            if (Width >= Text.Length + 2)
            {
                var textPosX = X + Width / 2 - Text.Length / 2;
                Game.TileManager.DrawStringAt(textPosX, Y, Text, TextColor, Parent);
            }
            else
            {
                var textToDraw = Text.Take(Text.Length - (Width - 2));
                Game.TileManager.DrawStringAt(X + 1, Y, textToDraw, TextColor, Parent);
            }
        }

        public override Task<bool> ManageInput()
        {
            if (Game.InputManager.KeyPressed(Keys.Enter))
            {
                if (OnClicked != null)
                    OnClicked();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public override void ManageMouseInput()
        {
            if (Game.InputManager.MouseLeftClicked)
            {
                OnClicked();
            }
        }
    }
}

