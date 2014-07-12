using EveFortressModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    // Simply holds a list of the known chat messages
    public class ChatManager : IResetNeeded
    {
        public List<string> Messages { get; set; }

        // Sets up the resetable requirement
        public ChatManager()
        {
            Messages = new List<string>();
            Game.Resetables.Add(this);
        }

        // This is called by the client methods when the server sent a chat message
        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        // Resets the message list
        public void Reset()
        {
            Messages.Clear();
        }
    }
}
