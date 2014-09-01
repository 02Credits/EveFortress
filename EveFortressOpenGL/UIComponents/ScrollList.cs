using EveFortressModel;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace EveFortressClient
{
    public class ScrollList : UIElement, IUIElementContainer
    {
        public override bool Activateable { get { return false; } }

        public new UIElement ActiveElement { get; set; }

        private List<UIElement> elements = new List<UIElement>();

        public List<UIElement> Elements
        {
            get { return elements; }
        }

        public float ScrollPercentage { get; set; }

        public int InnerHeight
        {
            get
            {
                return Elements.Aggregate(0, (acc, e) => acc + e.Height);
            }
        }

        public int HeightDelta
        {
            get
            {
                var returnVal = InnerHeight - Height;
                if (returnVal < 0)
                {
                    return 1;
                }
                return returnVal;
            }
        }

        public ScrollList(IUIElementContainer parent, CVal<int> x, CVal<int> y, CVal<int> width, CVal<int> height)
            : base(parent, x, y, width, height)
        {
            ScrollPercentage = 1;
        }

        public override void Draw()
        {
            var currentYPos = Height + 1;
            if (HeightDelta > 1)
            {
                var scrollBarHeight = (int)(Height * ((float)Height / InnerHeight));
                var scrollBarPositionY = (int)((Height - scrollBarHeight) * ScrollPercentage);

                for (int y = scrollBarPositionY; y < scrollBarPositionY + scrollBarHeight; y++)
                {
                    Game.TileManager.DrawTile(UITiles.BorderVertical, Width, y, this);
                }

                var scrollChange = HeightDelta * (1 - ScrollPercentage);
                currentYPos += (int)scrollChange;
            }
            var reversedElements = Elements.ToList();
            reversedElements.Reverse();
            foreach (var element in reversedElements)
            {
                element.Width = Width - element.X;
                element.Y = currentYPos - element.Height;
                currentYPos = element.Y;
                element.Draw();
            }
        }
    }
}