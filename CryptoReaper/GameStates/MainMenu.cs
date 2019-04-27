using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryptoReaper.GameStates
{
    class MainMenu : GameState
    {
        private Texture2D _title;

        public override void Initialize()
        {
            this._title = ContentStore.Get<Texture2D>("Sprites/title");
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            sb.Draw(_title, Vector2.Zero, Color.White);
            sb.End();
        }
    }
}
