using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class IDManager
    {
        public long CurrentID;
        public long GetNextID()
        {
            var id = CurrentID;
            CurrentID++;
            return id;
        }
    }
}
