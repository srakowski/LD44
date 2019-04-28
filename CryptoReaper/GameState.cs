using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryptoReaper
{
    abstract class GameState
    {
        protected GameState(CryptoReaperGame game)
        {
            Game = game;
        }

        protected CryptoReaperGame Game { get; }

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime, InputState input) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
