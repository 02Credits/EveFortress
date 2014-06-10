using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EveFortressModel;

namespace EveFortressClient
{
    public class InputManager : IUpdateNeeded
    {
        public KeyboardState PreviousKeyboardState { get; set; }
        public MouseState PreviousMouseState { get; set; }
        public KeyboardState CurrentKeyboardState { get; set; }
        public MouseState CurrentMouseState { get; set; }

        public List<IInputNeeded> InputSubscriptions = new List<IInputNeeded>();

        int framesHeld;
        Keys heldKey;

        public Point MousePixelPosition
        {
            get
            {
                return PreviousMouseState.Position;
            }
        }

        public Point MouseTilePosition
        {
            get
            {
                return new Point(
                    MousePixelPosition.X / Game.TileManager.TileSize,
                    MousePixelPosition.Y / Game.TileManager.TileSize);
            }
        }

        public bool MouseLeftDown
        {
            get
            {
                return PreviousMouseState.LeftButton == ButtonState.Pressed;
            }
        }

        public bool MouseRightDown
        {
            get
            {
                return PreviousMouseState.RightButton == ButtonState.Pressed;
            }
        }

        public bool MouseRightClicked
        {
            get
            {
                return PreviousMouseState.RightButton == ButtonState.Released &&
                       CurrentMouseState.RightButton == ButtonState.Pressed;
            }
        }

        public bool MouseLeftClicked
        {
            get
            {
                return PreviousMouseState.LeftButton == ButtonState.Released &&
                       CurrentMouseState.LeftButton == ButtonState.Pressed;
            }
        }

        public int MouseScrollDelta
        {
            get
            {
                return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
            }
        }

        public bool MouseScrolledUp
        {
            get
            {
                return MouseScrollDelta > 0;
            }
        }

        public bool MouseScrolledDown
        {
            get
            {
                return MouseScrollDelta < 0;
            }
        }

        public bool KeyDown(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key);
        }

        public bool KeyPressed(Keys key)
        {
            return PreviousKeyboardState.IsKeyUp(key) &&
                   CurrentKeyboardState.IsKeyDown(key);
        }

        public bool KeyReleased(Keys key)
        {
            return PreviousKeyboardState.IsKeyDown(key) &&
                   CurrentKeyboardState.IsKeyDown(key);
        }

        public bool KeyHeld(Keys key)
        {
            if (key == heldKey && framesHeld >= 30)
                return true;
            return false;
        }

        public bool KeyTyped(Keys key)
        {
            return KeyPressed(key) || KeyHeld(key);
        }

        public string GetInputString()
        {
            var stringRep = "";
            var keys = CurrentKeyboardState.GetPressedKeys();

            bool shift = false;
            if (keys.Contains(Keys.LeftShift) || keys.Contains(Keys.RightShift))
            {
                shift = true;
            }

            foreach (var key in keys)
            {
                if (KeyTyped(key))
                {
                    var name = Enum.GetName(key.GetType(), key);
                    if (name.Length == 1)
                    {
                        if (shift)
                        {
                            stringRep += name.ToUpper();
                        }
                        else
                        {
                            stringRep += name.ToLower();
                        }
                    }
                    else
                    {
                        if (shift)
                        {
                            switch (key)
                            {
                                case Keys.D0: stringRep += ")"; break;
                                case Keys.D1: stringRep += "!"; break;
                                case Keys.D2: stringRep += "@"; break;
                                case Keys.D3: stringRep += "#"; break;
                                case Keys.D4: stringRep += "$"; break;
                                case Keys.D5: stringRep += "%"; break;
                                case Keys.D6: stringRep += "^"; break;
                                case Keys.D7: stringRep += "&"; break;
                                case Keys.D8: stringRep += "*"; break;
                                case Keys.D9: stringRep += "("; break;
                                case Keys.OemQuestion: stringRep += "?"; break;
                                case Keys.OemMinus: stringRep += "_"; break;
                                case Keys.OemPipe: stringRep += "|"; break;
                                case Keys.OemQuotes: stringRep += "\""; break;
                                case Keys.OemSemicolon: stringRep += ":"; break;
                            }
                        }
                        else
                        {
                            switch (key)
                            {
                                case Keys.Space: stringRep += " "; break;
                                case Keys.D0: stringRep += "0"; break;
                                case Keys.D1: stringRep += "1"; break;
                                case Keys.D2: stringRep += "2"; break;
                                case Keys.D3: stringRep += "3"; break;
                                case Keys.D4: stringRep += "4"; break;
                                case Keys.D5: stringRep += "5"; break;
                                case Keys.D6: stringRep += "6"; break;
                                case Keys.D7: stringRep += "7"; break;
                                case Keys.D8: stringRep += "8"; break;
                                case Keys.D9: stringRep += "9"; break;
                                case Keys.OemPipe: stringRep += "\\"; break;
                                case Keys.OemPeriod: stringRep += "."; break;
                                case Keys.OemMinus: stringRep += "-"; break;
                                case Keys.OemQuotes: stringRep += "'"; break;
                                case Keys.OemQuestion: stringRep += "/"; break;
                                case Keys.OemSemicolon: stringRep += ";"; break;
                                case Keys.Back: return "";
                            }
                        }
                    }
                }
            }
            return stringRep;
        }

        public InputManager()
        {
            Game.Updateables.Add(this);
            InputSubscriptions.Add(Game.WindowManager);
            InputSubscriptions.Add(Game.TabManager);
        }

        public async void Update()
        {
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();

            var keys = CurrentKeyboardState.GetPressedKeys();
            if (keys.Length > 0)
            {
                var lastKey = keys[keys.Length - 1];
                if (lastKey == heldKey)
                {
                    framesHeld += 1;
                }
                else
                {
                    heldKey = lastKey;
                    framesHeld = 0;
                }
            }
            else
            {
                framesHeld = 0;
            }

            foreach (var subscription in InputSubscriptions)
            {
                if (await subscription.ManageInput())
                {
                    break;
                }
            }

            PreviousKeyboardState = CurrentKeyboardState;
            PreviousMouseState = CurrentMouseState;
        }
    }
}
