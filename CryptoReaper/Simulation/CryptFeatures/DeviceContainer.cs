using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryptoReaper.Simulation.CryptFeatures
{
    abstract class GameTokenContainer : Crypt.Feature
    {
        private readonly IEnumerable<Type> _supportedGameTokens;

        protected GameTokenContainer(string textureKey, IEnumerable<Type> supportedGameTokenTypes)
            : base(textureKey)
        {
            _supportedGameTokens = supportedGameTokenTypes;
        }

        public GameToken GameToken { get; private set; }

        public override bool CanPlaceToken(GameToken gameToken) =>
            GameToken == null &&
            _supportedGameTokens.Contains(gameToken.GetType());

        public SetGameTokenResult SetGameToken(GameToken gameToken)
        {
            if (GameToken != null)
                return SetGameTokenResult.Occupied;

            if (!_supportedGameTokens.Contains(gameToken.GetType()))
                return SetGameTokenResult.Unsupported;

            GameToken = gameToken;

            return SetGameTokenResult.Success;
        }

        public enum SetGameTokenResult
        {
            Success,
            Occupied,
            Unsupported
        }

        public ClearGameTokenResult ClearGameToken()
        {
            GameToken = null;
            return ClearGameTokenResult.Success;
        }

        public enum ClearGameTokenResult
        {
            Success,
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            base.Draw(spriteBatch, position);
            if (GameToken != null)
            {
                GameToken.Draw(spriteBatch, position);
            }
        }
    }

    static partial class Extension
    {
        public static T Match<T>(
            this GameTokenContainer.SetGameTokenResult self,
            Func<T> success,
            Func<T> occupied,
            Func<T> unsupported
            )
        {
            if (self == GameTokenContainer.SetGameTokenResult.Success) return success();
            else if (self == GameTokenContainer.SetGameTokenResult.Occupied) return occupied();
            else if (self == GameTokenContainer.SetGameTokenResult.Unsupported) return unsupported();
            throw new Exception("value not mapped");
        }

        public static T Match<T>(this OpenSpace.ClearGameTokenResult self, Func<T> success) =>
            self == GameTokenContainer.ClearGameTokenResult.Success ? success() :
            throw new Exception("value not mapped");
    }
}
