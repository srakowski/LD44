using Microsoft.Xna.Framework.Graphics;
using System;

namespace CryptoReaper
{
    abstract class GameState
    {
        public virtual void Initialize() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
