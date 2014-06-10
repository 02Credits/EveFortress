using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel
{
    public interface IInputNeeded
    {
        Task<bool> ManageInput();
    }
}
