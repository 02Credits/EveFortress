using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public interface IUIElementContainer
    {
        IUIElementContainer Parent { get; }

        CVal<int> Width { get; set; }
        CVal<int> Height { get; set; }

        CVal<int> X { get; set; }
        CVal<int> Y { get; set; }

        List<UIElement> Elements { get; }

        UIElement ActiveElement { get; set; }
    }
}
