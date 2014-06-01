using EveFortressModel;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class TextInput : UIElement
    {
        public override bool Activateable
        {
            get { return true; }
        }

        public Action<string, TextInput> ReturnAction { get; set; }

        public string Text { get; set; }

        public bool Password { get; set; }

        public TextInput(IUIElementContainer parent, CVal<int> x, CVal<int> y, CVal<int> maxWidth, Action<string, TextInput> returnAction = null, string text = "", bool password = false)
            : base(parent, x, y, maxWidth, 1)
        {
            if (text == "")
                firstTime = false;
            ReturnAction = returnAction;
            Text = text;
            Password = password;
        }

        public TextInput(IUIElementContainer parent, CVal<int> x, CVal<int> y, CVal<int> maxWidth, Action<string> returnAction = null, string text = "", bool password = false)
            : this(parent, x, y, maxWidth, (s, i) => returnAction(s), text, password) { }

        int cursorCounter = 0;
        bool drawCursor = true;
        public override void Draw()
        {
            cursorCounter += 1;
            var textToDraw = Text;

            if (Password)
            {
                var acc = "";
                for (int i = 0; i < textToDraw.Length; i++)
                {
                    acc += "*";
                }
                textToDraw = acc;
            }
            
            if (textToDraw.Length > Width - 2)
            {
                textToDraw = textToDraw.Skip(textToDraw.Length - (Width - 2))
                                       .Aggregate("", (acc, ch) => acc + ch);
            }

            if (cursorCounter >= 60)
            {
                cursorCounter = 0;
                drawCursor = !drawCursor;
            }


            if (ActiveElement)
            {
                if (drawCursor)
                {
                    textToDraw += "|";
                }
            }
            else
            {
                drawCursor = true;
                cursorCounter = 0;
            }

            Game.TileManager.DrawTile(UITiles.Right, X, Y, Parent);
            Game.TileManager.DrawStringAt(X + 1, Y, textToDraw, Parent);
        }

        public bool firstTime = true;
        public override void OnFocusGained()
        {
            if (firstTime)
            {
                firstTime = false;
                Text = "";
            }
        }

        public override Task<bool> ManageInput()
        {
            var text = Game.InputManager.GetInputString();
            if (text.Length > 0)
            {
                Text += text;
                drawCursor = true;
                cursorCounter = 0;
                return Task.FromResult(true);
            }
            else
            {
                if (Game.InputManager.KeyTyped(Keys.Back))
                {
                    if (Text.Length > 0)
                    {
                        Text = Text.Take(Text.Length - 1)
                                   .Aggregate("", (acc, ch) => acc + ch);
                        drawCursor = true;
                        cursorCounter = 0;
                    }
                    return Task.FromResult(true);
                }
            }

            if (Game.InputManager.KeyPressed(Keys.Enter))
            {
                if (ReturnAction != null)
                {
                    ReturnAction(Text, this);
                }
            }
            return Task.FromResult(false);
        }
    }
}
