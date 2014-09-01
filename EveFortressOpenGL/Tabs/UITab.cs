using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public abstract class UITab : Tab, IUIElementContainer
    {
        public IUIElementContainer Parent { get { return null; } }

        public override int MinimumWidth
        {
            get
            {
                var minWidth = Title.Length;
                foreach (var element in Elements)
                {
                    if (!element.Collapsed)
                    {
                        if (element.X + element.Width > minWidth)
                        {
                            minWidth = element.X + element.Width;
                        }
                    }
                }
                return minWidth;
            }
        }

        public override int MinimumHeight
        {
            get
            {
                var minHeight = 5;
                foreach (var element in Elements)
                {
                    if (!element.Collapsed)
                    {
                        if (element.Y + element.Height > minHeight)
                        {
                            minHeight = element.Y + element.Height;
                        }
                    }
                }
                return minHeight;
            }
        }

        private List<UIElement> elements = new List<UIElement>();

        public List<UIElement> Elements
        {
            get
            {
                return elements;
            }
        }

        private UIElement activeElement;

        public UIElement ActiveElement
        {
            get
            {
                return activeElement;
            }
            set
            {
                if (activeElement != null)
                    activeElement.OnFocusLossed();
                activeElement = value;
                if (activeElement != null)
                    activeElement.OnFocusGained();
            }
        }

        public override void Render()
        {
            foreach (var element in Elements)
            {
                if (!element.Collapsed)
                {
                    element.Draw();
                }
            }
        }

        public override void Update()
        {
            foreach (var element in Elements)
            {
                element.Update();
            }
        }

        public async override Task<bool> ManageInput()
        {
            if (ActiveElement != null)
            {
                if (await ActiveElement.ManageInput())
                {
                    return true;
                }
            }

            if (Game.InputManager.KeyPressed(Keys.Tab))
            {
                return TabSelect();
            }
            return false;
        }

        public bool TabSelect()
        {
            var index = Elements.Count;
            if (ActiveElement != null)
                index = Elements.IndexOf(ActiveElement);

            var dir = Game.InputManager.KeyDown(Keys.LeftShift) || Game.InputManager.KeyDown(Keys.RightShift) ? -1 : 1;

            if (Elements.Any(e => e.Activateable && !e.Collapsed))
            {
                do
                {
                    index += dir;
                    if (index >= Elements.Count)
                        index = 0;
                    if (index < 0)
                        index = Elements.Count - 1;
                } while (Elements[index].Activateable != true || Elements[index].Collapsed);

                Elements[index].Focus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ManageMouseInput()
        {
            foreach (var element in Elements)
            {
                if (element.MouseOver && element.Activateable && !element.Collapsed)
                {
                    element.ManageMouseInput();
                    if (Game.InputManager.MouseLeftClicked)
                    {
                        if (element != ActiveElement)
                        {
                            element.Focus();
                        }
                    }
                }
            }
        }
    }
}