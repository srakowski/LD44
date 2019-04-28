using CryptoReaper.Simulation.CryptFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CryptoReaper.Simulation
{
    /// <summary>
    /// A building where the mining machines run.
    /// </summary>
    class Crypt
    {
        public abstract class Feature
        {
            protected string _textureKey;
            protected Texture2D _texture;

            protected Feature(string textureKey)
            {
                _textureKey = textureKey;
            }

            public virtual bool CanPlaceToken(GameToken gameToken) => false;

            public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
            {
                if (_textureKey == null)
                    return;

                if (_texture == null)
                    _texture = ContentStore.Get<Texture2D>(_textureKey);

                spriteBatch.Draw(_texture, position, Color.White);
            }
        }

        private readonly Dictionary<string, Feature> _cryptFeatures = new Dictionary<string, Feature>();

        public struct Coords
        {
            private string _key;

            public Coords(int row, int col)
            {
                Row = row;
                Col = col;
                _key = $"{row},{col}";
                WorldCoord = new Vector2(Col * Constants.TileUnit, Row * Constants.TileUnit);
            }

            public int Row { get; }

            public int Col { get; }

            public Vector2 WorldCoord { get; }

            public bool HasFeature(Dictionary<string, Feature> features) => features.TryGetValue(_key, out var _);

            public Feature GetFeature(Dictionary<string, Feature> features)
            {
                if (!this.HasFeature(features)) return new Undefined();
                return features[_key];
            }

            public void SetFeature(Dictionary<string, Feature> cryptFeatures, Feature value)
            {
                cryptFeatures[_key] = value;
            }
        }

        public Feature this[int row, int col]
        {
            get => this[new Coords(row, col)];
            set => this[new Coords(row, col)] = value;
        }

        public Feature this[Coords coords]
        {
            get => coords.GetFeature(_cryptFeatures);
            set => coords.SetFeature(_cryptFeatures, value);
        }

        public PlaceGameTokenResult PlaceGameToken(Coords coords, GameToken gameToken)
        {
            if (!coords.HasFeature(_cryptFeatures))
                return PlaceGameTokenResult.InvalidFeature;

            var buildingFeature = coords.GetFeature(_cryptFeatures);

            if (buildingFeature is GameTokenContainer dc)
            {
                return dc.SetGameToken(gameToken).Match(
                    success: () => PlaceGameTokenResult.Success,
                    occupied: () => PlaceGameTokenResult.AlreadyHasGameToken,
                    unsupported: () => PlaceGameTokenResult.CryptFeatureDoesNotSupportGameTokenType
                );
            }

            return PlaceGameTokenResult.CryptFeatureDoesNotSupportGameTokens;
        }

        public enum PlaceGameTokenResult
        {
            Success,
            InvalidFeature,
            CryptFeatureDoesNotSupportGameTokens,
            CryptFeatureDoesNotSupportGameTokenType,
            AlreadyHasGameToken,
        }

        public RemoveGameTokenResult RemoveGameToken(Coords coords)
        {
            if (!coords.HasFeature(_cryptFeatures))
                return RemoveGameTokenResult.InvalidFeature;

            var buildingFeature = coords.GetFeature(_cryptFeatures);

            if (buildingFeature is OpenSpace os)
            {
                return os.ClearGameToken()
                    .Match(success: () => RemoveGameTokenResult.Success);
            }

            return RemoveGameTokenResult.BuildingFixtureDoesNotSupportGameTokens;
        }

        public enum RemoveGameTokenResult
        {
            Success,
            InvalidFeature,
            BuildingFixtureDoesNotSupportGameTokens,
        }
    }

    static partial class Extension
    {
        public static T Match<T>(
            this Crypt.PlaceGameTokenResult self,
            Func<T> success,
            Func<T> invalidFeature,
            Func<T> cryptFeatureDoesNotSupportGameTokens,
            Func<T> cryptFeatureDoesNotSupportGameTokenType,
            Func<T> alreadyHasDevice
            )
        {
            if (self == Crypt.PlaceGameTokenResult.Success) return success();
            else if (self == Crypt.PlaceGameTokenResult.InvalidFeature) return invalidFeature();
            else if (self == Crypt.PlaceGameTokenResult.CryptFeatureDoesNotSupportGameTokens) return cryptFeatureDoesNotSupportGameTokens();
            else if (self == Crypt.PlaceGameTokenResult.CryptFeatureDoesNotSupportGameTokenType) return cryptFeatureDoesNotSupportGameTokenType();
            else if (self == Crypt.PlaceGameTokenResult.AlreadyHasGameToken) return alreadyHasDevice();
            throw new Exception("value not mapped");
        }
    }
}
