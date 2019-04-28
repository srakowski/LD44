using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using static CryptoReaper.Constants;

namespace CryptoReaper.GameStates
{
    class Playing : GameState
    {
        private Texture2D _selectTexture;
        private Vector2 _selectPosition;

        public Playing(CryptoReaperGame game) : base(game)
        {
            Camera = new Camera(game);
            Camera.SetPosition(WorldOffset);
            Camera.Offset = new Vector2(TileUnit / 2, TileUnit / 2);
        }

        private Camera Camera { get; }

        public override void Update(GameTime gameTime, InputState input)
        {
            Game.Gameplay.Update(gameTime, input);
            Camera.Update(gameTime, input);
            var mousePos = Camera.ToWorldCoords(input.MousePosition);
            _selectPosition =
                new Vector2(
                    mousePos.X - (mousePos.X % TileUnit),
                    mousePos.Y - (mousePos.Y % TileUnit)) -
                    new Vector2(SelectTileOffset, SelectTileOffset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            // Game
            spriteBatch.Begin(transformMatrix: Camera.TransformationMatrix);

            var cryptBoundsToDraw = CryptBoundsToDraw();

            var crypt = Game.Gameplay.Crypt;
            
            for (int row = cryptBoundsToDraw.Y; row < cryptBoundsToDraw.Height; row++)
                for (int col = cryptBoundsToDraw.X; col < cryptBoundsToDraw.Width; col++)
                {
                    var feature = crypt[new Simulation.Crypt.Coords(row, col)];
                    feature.Draw(spriteBatch, new Vector2(col * TileUnit, row * TileUnit) + WorldOffset);
                }

            _selectTexture = _selectTexture ?? ContentStore.Get<Texture2D>("Sprites/PlayerSelect");
            //var selectPosition = Game.Gameplay.Player.CryptSelectCoords.WorldCoord - new Vector2(SelectTileOffset, SelectTileOffset);
            spriteBatch.Draw(_selectTexture, _selectPosition, Color.White);


            spriteBatch.End();

            // Hud
            spriteBatch.Begin();
            spriteBatch.End();
        }

        private Rectangle CryptBoundsToDraw()
        {
            var cameraLookingAt = Camera.ToWorldCoords(Camera.Transform.Position);
            var tilePointBeingLookedAt = new Point(
                (int)Math.Floor(cameraLookingAt.X) / TileUnit,
                (int)Math.Floor(cameraLookingAt.Y) / TileUnit);
            var tileDrawBounds = new Rectangle(
                tilePointBeingLookedAt.X - (SquareTilesToDraw / 2),
                tilePointBeingLookedAt.Y - (SquareTilesToDraw / 2),
                SquareTilesToDraw, SquareTilesToDraw);
            return tileDrawBounds;
        }
    }
}
