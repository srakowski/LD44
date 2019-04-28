using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoReaper
{
    public class InputState
    {
        public InputState() { }

        public static KeyboardState PrevKBState { get; private set; }
        public static KeyboardState CurrKBState { get; private set; }

        public static MouseState PrevMouseState { get; private set; }
        public static MouseState CurrMouseState { get; private set; }
        public Vector2 MousePosition => CurrMouseState.Position.ToVector2();

        public void Update(GameTime gameTime)
        {
            PrevKBState = CurrKBState;
            CurrKBState = Keyboard.GetState();

            PrevMouseState = CurrMouseState;
            CurrMouseState = Mouse.GetState();
        }

        public bool KeyDown(Keys key) => CurrKBState.IsKeyDown(key);

        public bool KeyPress(params Keys[] keys) => keys.Any((key) => CurrKBState.IsKeyDown(key) && PrevKBState.IsKeyUp(key));
    }
}