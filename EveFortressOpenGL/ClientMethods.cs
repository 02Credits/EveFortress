using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressClient
{
    public class ClientMethods
    {
        public bool Connected { get; set; }

        public void ConnectionEstablished()
        {
            Connected = true;
        }
    }
}
