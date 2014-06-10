using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EveFortressClient
{
    public class NewTab : UITab
    {
        public override string Title
        {
            get { return "Tab Selection"; }
        }

        List<Button> newTabOptions = new List<Button>();
        string currentSearchText;
        int framesSinceLastKeyPressed;

        public NewTab()
        {
            AddOption("Chat", () => { ParentSection.ReplaceTab(new ChatTab()); });
            AddOption("Map", () => { ParentSection.ReplaceTab(new MapTab()); });
        }

        public void AddOption(string title, Action action)
        {
            newTabOptions.Add(new Button(this, 1, newTabOptions.Count + 1, action, title));
        }

        public override Task<bool> ManageInput()
        {
            var text = Game.InputManager.GetInputString();
            if (text.Length > 0)
            {
                newTabOptions.ForEach((b) => b.TextColor = Color.White);
                currentSearchText += text;
                framesSinceLastKeyPressed = 0;
                var possibleButtons = newTabOptions.Where(b => b.Text.ToLower().StartsWith(currentSearchText.ToLower()));
                if (possibleButtons.Count() == 1)
                {
                    var selectedButton = possibleButtons.First();
                    selectedButton.TextColor = Color.GreenYellow;
                    selectedButton.Focus();
                }
                else if (possibleButtons.Count() > 1)
                {
                    possibleButtons.ToList().ForEach((b) => b.TextColor = Color.Yellow);
                    possibleButtons.First().Focus();
                }
                else
                {
                    currentSearchText = "";
                }
                return Task.FromResult(true);
            }
            return base.ManageInput();
        }

        public override void Update()
        {
            framesSinceLastKeyPressed += 1;
            if (framesSinceLastKeyPressed > 90)
            {
                currentSearchText = "";
                framesSinceLastKeyPressed = 0;
                newTabOptions.ForEach((b) => b.TextColor = Color.White);
            }
        }
    }
}
