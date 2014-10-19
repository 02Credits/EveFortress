using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace EveFortressClient
{
    public class Label : UIElement
    {
        public override bool Activateable
        {
            get { return false; }
        }

        public override CVal<int> Width { get; set; }

        public string Text { get; set; }

        public Color TextColor { get; set; }

        public Label(IUIElementContainer parent, CVal<int> x, CVal<int> y, string text)
            : this(parent, x, y, text, Color.White) { }

        public Label(IUIElementContainer parent, CVal<int> x, CVal<int> y, string text, Color textColor)
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
                Game.GetSystem<TileManager>().DrawStringAt(X, Y + y, lines[y], TextColor, Parent);
            }
        }

        public List<string> BreakIntoLines(string text, int lineLength)
        {
            var remainingChars = text;
            var returnList = new List<string>();
            var nextLine = "";
            while (remainingChars.Length > 0)
            {
                var nextChar = remainingChars[0];
                remainingChars = remainingChars.Substring(1);
                if (nextChar == ' ')
                {
                    if (nextLine.Length > lineLength - 8)
                    {
                        if (!remainingChars.Contains(' ') || remainingChars.IndexOf(' ') >= lineLength - nextLine.Length)
                        {
                            returnList.Add(nextLine);
                            nextLine = "";
                            continue;
                        }
                    }
                }
                else
                {
                    if (nextLine.Length == lineLength - 1 && remainingChars.Length != 0)
                    {
                        returnList.Add(nextLine + "-");
                        nextLine = "";
                    }
                }
                nextLine += nextChar;
            }
            if (!string.IsNullOrWhiteSpace(nextLine))
                returnList.Add(nextLine);
            return returnList;
        }
    }
}