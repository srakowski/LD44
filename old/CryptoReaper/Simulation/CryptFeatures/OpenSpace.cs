using CryptoReaper.Simulation.Devices;

namespace CryptoReaper.Simulation.CryptFeatures
{
    class OpenSpace : GameTokenContainer
    {
        public OpenSpace() : base(
            "Sprites/OpenSpace",
            new[] {
                typeof(Player),
                typeof(StraightSoulPipe),
                typeof(AngledSoulPipe),
                typeof(StraightHellFirePipe),
                typeof(AngledHellFirePipe)
            })
        {
        }
    }
}
