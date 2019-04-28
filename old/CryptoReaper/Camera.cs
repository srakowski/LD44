using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CryptoReaper.Constants;

namespace CryptoReaper
{
    class Camera
    {
        public Camera(CryptoReaperGame game)
        {
            Game = game;
        }

        public CryptoReaperGame Game { get; }

        public Vector2 Offset { get; internal set; }

        public Transform Transform { get; set; } = new Transform();

        internal Matrix TransformationMatrix =>
            Matrix.Identity *
            Matrix.CreateRotationZ(Transform.Rotation) *
            Matrix.CreateScale(Transform.Scale) *
            Matrix.CreateTranslation(-Transform.Position.X, -Transform.Position.Y, 0f) *
            Matrix.CreateTranslation(
                (GraphicsDevice.Viewport.Width * 0.5f) + Offset.X,
                (GraphicsDevice.Viewport.Height * 0.5f) + Offset.Y,
                0f);

        public void Update(GameTime gameTime, InputState input)
        {
            //var playerCoords = Game.Gameplay.Player.CryptCoords;
            //var targetLocation = new Vector2(playerCoords.Col * TileUnit, playerCoords.Row * TileUnit);
            //this.Transform.Position = targetLocation;
        }

        public Vector2 ToWorldCoords(Vector2 coords) =>
            Vector2.Transform(coords, Matrix.Invert(this.TransformationMatrix));

        public Vector2 ToScreenCoords(Vector2 coords) =>
            Vector2.Transform(coords, this.TransformationMatrix);

        public void SetPosition(Vector2 pos) => Transform.Position = pos;

        private GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
    }
}
