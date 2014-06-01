using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public abstract class UIElement : IDrawNeeded, IUpdateNeeded, IInputNeeded
    {
        public abstract bool Activateable { get; }
        public bool ActiveElement { get { return this == Parent.ActiveElement; } }
        public bool MouseOver 
        { 
            get 
            {
                return Game.InputManager.MouseTilePosition.X >= Parent.X + X &&
                       Game.InputManager.MouseTilePosition.X < Parent.X + X + Width &&
                       Game.InputManager.MouseTilePosition.Y >= Parent.Y + Y &&
                       Game.InputManager.MouseTilePosition.Y < Parent.Y + Y + Height;
            }
        }

        public virtual CVal<int> X { get; set; }
        public virtual CVal<int> Y { get; set; }

        public virtual CVal<int> Width { get; set; }
        public virtual CVal<int> Height { get; set; }

        public IUIElementContainer Parent { get; set; }

        public bool Collapsed { get; set; }

        public UIElement(IUIElementContainer parent, CVal<int> x, CVal<int> y,
                                       CVal<int> width, CVal<int> height)
        {
            Parent = parent;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Parent.Elements.Add(this);
        }

        public virtual Task<bool> ManageInput()
        {
            return Task.FromResult(false);
        }

        public virtual void ManageMouseInput() { }

        public virtual void Update() { }

        public abstract void Draw();

        public virtual void OnFocusGained() { }

        public virtual void OnFocusLossed() { }

        public virtual void Focus()
        {
            if (Activateable)
            {
                Parent.ActiveElement = this;
            }
        }
    }
}
