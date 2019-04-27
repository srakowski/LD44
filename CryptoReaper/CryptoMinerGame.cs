using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CryptoReaper
{
    class CryptoMinerGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ContentStore _content;
        private GameState _activeState;
        private SpriteBatch _spriteBatch;

        public CryptoMinerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();

            _content = new ContentStore(this);

            _activeState = new GameStates.MainMenu();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _activeState.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _content.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _activeState.Draw(_spriteBatch);
        }

        [STAThread]
        static void Main()
        {
            using (var g = new CryptoMinerGame())
                g.Run();
        }
    }
}
