using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static CryptoReaper.Simulation.Player.Direction;

namespace CryptoReaper.Simulation
{
    class Player : GameToken
    {
        private readonly Crypt _crypt;
        private Crypt.Coords _cryptCoords;
        private Direction _facing;
        private Crypt.Coords _selectCoords;

        public Player(Crypt crypt, Crypt.Coords cryptCoords) : base("Sprites/Player")
        {
            _crypt = crypt;
            _cryptCoords = cryptCoords;
            Face(Up);
        }

        public Crypt.Coords CryptCoords => _cryptCoords;

        public Crypt.Coords CryptSelectCoords => _selectCoords;

        public void Update(GameTime gameTime, InputState input)
        {
            if (input.KeyPress(Keys.W, Keys.Up))
            {
                if (IsFacing(Up))
                    Move(Negative, Static);
                else
                    Face(Up);
            }

            if (input.KeyPress(Keys.A, Keys.Left))
            {
                if (IsFacing(Left))
                    Move(Static, Negative);
                else
                    Face(Left);
            }

            if (input.KeyPress(Keys.S, Keys.Down))
            {
                if (IsFacing(Down))
                    Move(Positive, Static);
                else
                    Face(Down);
            }

            if (input.KeyPress(Keys.D, Keys.Right))
            {
                if (IsFacing(Right))
                    Move(Static, Positive);
                else
                    Face(Right);
            }
        }

        private bool IsFacing(Direction direction) => _facing == direction;

        private void Face(Direction direction)
        {
            _facing = direction;
            Select();
        }

        private void Move(Func<int, int> changeRow, Func<int, int> changeCol)
        {
            var coords = GetNewCoords(changeRow, changeCol);
            var feature = _crypt[coords];
            if (feature.CanPlaceToken(this))
            {
                _crypt.RemoveGameToken(_cryptCoords);
                _cryptCoords = coords;
                _crypt.PlaceGameToken(_cryptCoords, this);
                Select();
            }
        }

        private void Select()
        {
            _selectCoords =
                _facing == Up
                ? GetNewCoords(Negative, Static)
                : _facing == Right
                ? GetNewCoords(Static, Positive)
                : _facing == Down
                ? GetNewCoords(Positive, Static)
                : GetNewCoords(Static, Negative);
        }

        private Crypt.Coords GetNewCoords(Func<int, int> changeRow, Func<int, int> changeCol)
        {
            return new Crypt.Coords(changeRow(_cryptCoords.Row), changeCol(_cryptCoords.Col));
        }

        public enum Direction
        {
            Up,
            Right,
            Down,
            Left,
        }

        private int Static(int value) => value;
        private int Negative(int value) => value - 1;
        private int Positive(int value) => value + 1;

    }
}
