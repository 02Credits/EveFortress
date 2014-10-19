using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;
using Utils;

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

        private ScrollList list;

        public ChatTab()
        {
            list = new ScrollList(this, 1, 1, new CVal<int>(() => Width - 2), new CVal<int>(() => Height - 4));
            new TextInput(this, 1,
                new CVal<int>(() => Height - 1),
                new CVal<int>(() => Width - 1),
                (s, i) =>
                {
                    i.Text = "";
                    Game.GetSystem<ServerMethods>().Chat(s);
                }).Focus();
        }

        public override void Update()
        {
            var messages = Game.GetSystem<ChatManager>().Messages;
            for (int i = list.Elements.Count; i < messages.Count; i++)
            {
                new Label(list, 0, 0, messages[i]);
                list.ScrollPercentage = 1;
            }
        }

        public override async Task<bool> ManageInput()
        {
            if (Game.GetSystem<InputManager>().KeyTyped(Keys.Up) || Game.GetSystem<InputManager>().MouseScrolledUp)
            {
                list.ScrollPercentage -= 1f / list.HeightDelta;
            }
            if (Game.GetSystem<InputManager>().KeyTyped(Keys.Down) || Game.GetSystem<InputManager>().MouseScrolledDown)
            {
                list.ScrollPercentage += 1f / list.HeightDelta;
            }

            if (list.ScrollPercentage < 0) list.ScrollPercentage = 0;
            if (list.ScrollPercentage > 1) list.ScrollPercentage = 1;
            return await base.ManageInput();
        }
    }
}