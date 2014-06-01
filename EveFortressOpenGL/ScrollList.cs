using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ScrollList : UIElement, IUIElementContainer
    {
        public override bool Activateable { get { return false; } }
        public new UIElement ActiveElement { get; set; }

        List<UIElement> elements = new List<UIElement>();
        public List<UIElement> Elements
        {
            get { return elements; }
        }

        public ScrollList(IUIElementContainer parent, CVal<int> x, CVal<int> y, CVal<int> width, CVal<int> height)
            : base(parent, x, y, width, height) { }

        public override void Draw()
        {
            var currentYPos = Height;
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
