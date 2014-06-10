using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        ScrollList list;

        public ChatTab()
        {
            list = new ScrollList(this, 1, 1, new CVal<int>(() => Width - 2), new CVal<int>(() => Height - 4));
            new TextInput(this, 1,
                new CVal<int>(() => Height - 1),
                new CVal<int>(() => Width - 1),
                (s, i) =>
                {
                    i.Text = "";
                    Game.ServerMethods.Chat(s);
                }).Focus();
        }

        public override void Update()
        {
            var messages = Game.ChatManager.Messages;
            for (int i = list.Elements.Count; i < messages.Count; i++)
            {
                new TextBlock(list, 0, 0, messages[i]);
                list.ScrollPercentage = 1;
            }
        }

        public override async Task<bool> ManageInput()
        {
            if (Game.InputManager.KeyTyped(Keys.Up) || Game.InputManager.MouseScrolledUp)
            {
                list.ScrollPercentage -= 1f / list.HeightDelta;
            }
            if (Game.InputManager.KeyTyped(Keys.Down) || Game.InputManager.MouseScrolledDown)
            {
                list.ScrollPercentage += 1f / list.HeightDelta;
            }

            if (list.ScrollPercentage < 0) list.ScrollPercentage = 0;
            if (list.ScrollPercentage > 1) list.ScrollPercentage = 1;
            return await base.ManageInput();
        }
    }
}
