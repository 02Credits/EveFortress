using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressModel.Components
{
    public class Mobile : Component
    {
        public Point<long> TargetLocation { get; set; }
        public int Speed { get; set; }
        public int CurrentMoveWait { get; set; }

        public Mobile() { }

        public Mobile(int speed)
        {
            Speed = speed;
        }
    }
}
