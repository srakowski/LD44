using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CryptoReaper.GameStates
{
    class MainMenu : GameState
    {
        private Texture2D _title;

        public MainMenu(CryptoReaperGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            this._title = ContentStore.Get<Texture2D>("Sprites/title");
        }

        public override void Update(GameTime gameTime, InputState input)
        {
            if (input.KeyDown(Keys.Enter))
            {
                Game.StartNewGame();
                Game.SetActiveState(new Playing(Game));
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            Game.GraphicsDevice.Clear(Color.White);
            sb.Begin();
            sb.Draw(_title, Vector2.Zero, Color.White);
            sb.End();
        }
    }
}
