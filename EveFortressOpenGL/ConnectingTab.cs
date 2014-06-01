using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ConnectingTab : UITab
    {
        public override string Title
        {
            get { return "Connecting"; }
        }

        public ConnectingTab()
        {
            var message = new TextBlock(this,
                new CVal<int>(() => Width / 2),
                new CVal<int>(() => Height / 2),
                "Disconnected and attempting reconnect...");
            message.Width = new CVal<int>(() => Width - 8);
            message.X = new CVal<int>(() => Width / 2 - message.Width / 2);
        }

        public override void Update()
        {
            if (Game.ClientNetworkManager.Connected)
            {
                Game.TabManager.ActiveSection.ReplaceTab(new LoginTab());
            }
        }
    }
}
