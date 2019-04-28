using CryptoReaper.Simulation.CryptFeatures;
using Microsoft.Xna.Framework;

namespace CryptoReaper.Simulation
{
    class Gameplay
    {
        protected Gameplay()
        {
            Crypt = new Crypt();
            // Player = new Player(Crypt, new Crypt.Coords(9, 9));

            for (int row = 0; row < 20; row++)
                for (int col = 0; col < 20; col++)
                {
                    Crypt[row, col] = row == 3 && col == 3
                        ? new SoulSpire()
                        : row == 3 && col == 16
                        ? new HellFireSpire()
                        : row == 0 || row == 19 || col == 0 || col == 19
                        ? new Wall() as Crypt.Feature
                        : new OpenSpace();

                    //if (row == 9 && col == 9 && Crypt[row, col] is GameTokenContainer gs)
                    //{
                    //    gs.SetGameToken(Player);
                    //}
                }
        }

        public Crypt Crypt { get; }

        // public Player Player { get; }

        public void Update(GameTime gameTime, InputState input)
        {
            // Player.Update(gameTime, input);
        }

        public static Gameplay Create()
        {
            return new Gameplay();
        }
    }
}
