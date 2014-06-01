using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressClient
{
    public class TextBlock : UIElement
    {
        public override bool Activateable
        {
            get { return false; }
        }

        public override CVal<int> Width { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        public TextBlock(IUIElementContainer parent, CVal<int> x, CVal<int> y, string text)
            : this(parent, x, y, text, Color.White) { }

        public TextBlock(IUIElementContainer parent, CVal<int> x, CVal<int> y, string text, Color textColor)
            : base(parent, x, y, text.Length, 1)
        {
            Width = new CVal<int>(() => Text.Length);
            Text = text;
            TextColor = textColor;
        }

        public override void Draw()
        {
            var lines = BreakIntoLines(Text, Width);
            Height = lines.Count;
            for (int y = 0; y < lines.Count; y++)
            {
                Game.TileManager.DrawStringAt(X, Y + y, lines[y], TextColor, Parent);
            }
        }

        public List<string> BreakIntoLines(string text, int lineLength)
        {
            var returnList = new List<string>();
            var nextLine = "";
            for (int i = 0; i < text.Length; i++)
            {
                nextLine += text[i];
                if (nextLine.Length > lineLength)
                {
                    var lastChars = nextLine.Substring(nextLine.Length - 3);
                    nextLine = nextLine.Substring(0, nextLine.Length - 3) + "-";
                    returnList.Add(nextLine);
                    nextLine = lastChars;
                }
                else if (nextLine.Length > lineLength - 8)
                {
                    if (text[i] == ' ')
                    {
                        returnList.Add(nextLine);
                        nextLine = "";
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(nextLine))
                returnList.Add(nextLine);
            return returnList;
        }
    }
}