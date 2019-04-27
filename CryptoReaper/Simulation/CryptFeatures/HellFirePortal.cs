namespace CryptoReaper.Simulation.CryptFeatures
{
    class HellFirePortal : DeviceContainer
    {
        public HellFirePortal() : base(new[] {
                typeof(Devices.HellFireReceiver)
            })
        {
        }
    }
}
