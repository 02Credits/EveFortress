using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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
            var message = new Label(this,
                new CVal<int>(() => Width / 2),
                new CVal<int>(() => Height / 2),
                "Connecting...");
            message.X = new CVal<int>(() => Width / 2 - message.Text.Length / 2);
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
