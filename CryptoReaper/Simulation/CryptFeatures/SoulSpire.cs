namespace CryptoReaper.Simulation.CryptFeatures
{
    class SoulSpire : GameTokenContainer
    {
        public SoulSpire() : base(
            "Sprites/SoulSpire",
            new[] {
                typeof(Devices.SoulReceiver)
            })
        {
        }
    }
}
