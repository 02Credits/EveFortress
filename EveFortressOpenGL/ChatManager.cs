using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ChatManager : IResetNeeded
    {
        public List<string> Messages { get; set; }

        public ChatManager()
        {
            Messages = new List<string>();
            Game.Resetables.Add(this);
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void Reset()
        {
            Messages.Clear();
        }
    }
}
