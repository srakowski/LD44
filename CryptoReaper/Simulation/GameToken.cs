using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryptoReaper.Simulation
{
    abstract class GameToken
    {
        protected string _textureKey;
        protected Texture2D _texture;

        protected GameToken(string textureKey)
        {
            _textureKey = textureKey;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (_textureKey == null)
                return;

            if (_texture == null)
                _texture = ContentStore.Get<Texture2D>(_textureKey);

            spriteBatch.Draw(_texture, position, Color.White);
        }
    }

    abstract class Device : GameToken
    {
        protected Device(string textureKey) : base(textureKey)
        {
        }
    }
}
