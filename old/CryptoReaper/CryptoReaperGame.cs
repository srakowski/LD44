using CryptoReaper.Simulation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CryptoReaper
{
    class CryptoReaperGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ContentStore _content;
        private readonly InputState _inputState;
        private GameState _activeState;
        private SpriteBatch _spriteBatch;
        private Gameplay _gameplay;

        public CryptoReaperGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();

            _content = new ContentStore(this);
            _inputState = new InputState();

            this.IsMouseVisible = true;

            _activeState = new GameStates.MainMenu(this);

        }

        public Gameplay Gameplay => _gameplay;

        public void StartNewGame()
        {
            _gameplay = Gameplay.Create();
        }

        public void SetActiveState(GameState gameState)
        {
            _activeState = gameState;
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
            _inputState.Update(gameTime);
            _activeState.Update(gameTime, _inputState);
        }

        protected override void Draw(GameTime gameTime)
        {
            _activeState.Draw(_spriteBatch);
        }

        [STAThread]
        static void Main()
        {
            using (var g = new CryptoReaperGame())
                g.Run();
        }
    }
}
