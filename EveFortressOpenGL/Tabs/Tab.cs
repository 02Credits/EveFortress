using System.Threading.Tasks;
using Utils;

namespace EveFortressClient
{
    public abstract class Tab
    {
        public CVal<int> Width { get; set; }

        public CVal<int> Height { get; set; }

        public abstract int MinimumWidth { get; }

        public abstract int MinimumHeight { get; }

        public CVal<int> X { get; set; }

        public CVal<int> Y { get; set; }

        public abstract string Title { get; }

        public TabSection ParentSection { get; set; }

        public virtual void Update()
        {
        }

        public abstract void Render();

        public virtual void HandleClosing()
        {
        }

        public virtual Task<bool> ManageInput()
        {
            return Task.FromResult(false);
        }

        public virtual void ManageMouseInput()
        {
        }
    }
}