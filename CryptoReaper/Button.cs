using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CryptoReaper
{
    public class Button
    {
        public event EventHandler<EventArgs> Clicked;

        public Button(Rectangle bounds, Texture2D texture, Color color)
        {
            Bounds = bounds;
            Texture = texture;
            Color = color;
        }

        public Rectangle Bounds { get; }

        public Texture2D Texture { get; }        

        public Color Color { get; }

        public void Update(InputState input)
        {
            if (input.MouseClicked && Bounds.Contains(input.MousePosition))
                Clicked?.Invoke(this, new EventArgs());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color);
        }
    }
}
