using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ChatTab : UITab
    {
        public override string Title
        {
            get { return "Chat"; }
        }

        public override int MinimumHeight
        {
            get
            {
                return 6;
            }
        }

        public override int MinimumWidth
        {
            get
            {
                return 10;
            }
        }

        Task<Action> unsubscribeFromEvents;

        public ChatTab()
        {
            var list = new ScrollList(this, 1, 0, new CVal<int>(() => Width - 1), new CVal<int>(() => Height - 2));
            new TextInput(this, 1,
                new CVal<int>(() => Height - 1),
                new CVal<int>(() => Width - 1),
                (s, i) =>
                {
                    i.Text = "";
                    Game.ServerMethods.Chat(s);
                }).Focus();
            unsubscribeFromEvents = Game.ServerMethods.SubscribeToChatEvents((s) => new TextBlock(list, 0, 0, s));
        }

        public async override void HandleClosing()
        {
            (await unsubscribeFromEvents)();
        }
    }
}
