﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CryptoReaper
{
    class CryptoReaperGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly InputState _inputState;
        private SpriteBatch _spriteBatch;

        public CryptoReaperGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _inputState.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
        }

        [STAThread]
        static void Main()
        {
            using (var g = new CryptoReaperGame())
                g.Run();
        }
    }
}
