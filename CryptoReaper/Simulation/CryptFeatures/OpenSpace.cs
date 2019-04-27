using CryptoReaper.Simulation.Devices;

namespace CryptoReaper.Simulation.CryptFeatures
{
    class OpenSpace : DeviceContainer
    {
        public OpenSpace() : base(new[] {
                typeof(StraightSoulPipe),
                typeof(AngledSoulPipe),
                typeof(StraightHellFirePipe),
                typeof(AngledHellFirePipe),
            })
        {
        }
    }
}
