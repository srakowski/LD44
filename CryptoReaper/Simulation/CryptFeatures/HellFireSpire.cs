namespace CryptoReaper.Simulation.CryptFeatures
{
    class HellFireSpire : GameTokenContainer
    {
        public HellFireSpire() : base(
            "Sprites/HellFireSpire",
            new[] {
                typeof(Devices.HellFireReceiver)
            })
        {
        }
    }
}
