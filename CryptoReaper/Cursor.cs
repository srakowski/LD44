using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static CryptoReaper.Functions;

namespace CryptoReaper
{
    class Cursor
    {
        private CryptoReaperGame _game;
        private Crypt _crypt;
        private Vector2 _pos;

        public Cursor(CryptoReaperGame game, Crypt crypt)
        {
            _game = game;
            _crypt = crypt;
        }

        public CryptDevice PlaceDevice { get; set; }

        public Func<CryptDevice> Next { get; set; }

        internal void Update(InputState inputState)
        {
            _pos = inputState.MousePosition;

            if (inputState.MouseClicked)
            {
                if (PlaceDevice == null) return;
                var cryptPos = (_pos / _game.TileScalar).ToPoint();
                var canPlace = _crypt.CanPlaceCryptDevice(PlaceDevice, cryptPos);
                if (!canPlace) return;
                _crypt.PlaceCryptDevice(PlaceDevice, cryptPos);
                PlaceDevice = Next();
                return;
            }

            if (inputState.MouseRightClicked && PlaceDevice != null)
            {
                PlaceDevice = null;
                return;
            }

            if (inputState.MouseRightClicked)
            {
                var cryptPos = (_pos / _game.TileScalar).ToPoint();
                _crypt.RemoveDeviceAt(cryptPos);
            }
        }

        internal void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures)
        {
            if (PlaceDevice == null) return;
            var cryptPos = (_pos / _game.TileScalar).ToPoint();
            var canPlace = _crypt.CanPlaceCryptDevice(PlaceDevice, cryptPos);
            var color = canPlace ? Color.Green : Color.Red;
            var texture = textures[GetDeviceTexture(PlaceDevice)];
            spriteBatch.Draw(texture, cryptPos.ToVector2() * _game.TileScalar, color);
        }
    }
}
