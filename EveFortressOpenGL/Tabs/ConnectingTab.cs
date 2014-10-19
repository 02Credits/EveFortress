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
            if (Game.GetSystem<ClientNetworkManager>().Connected)
            {
                Game.GetSystem<TabManager>().ActiveSection.ReplaceTab(new LoginTab());
            }
        }
    }
}