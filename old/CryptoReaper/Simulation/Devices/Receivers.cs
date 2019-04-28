namespace CryptoReaper.Simulation.Devices
{
    class HellFireReceiver : GameToken, IOutputs<HellFire>
    {
        public HellFireReceiver() : base("Sprites/HellFireReceiver")
        {
        }

        public IInputs<HellFire> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }

    class SoulReceiver : GameToken, IOutputs<Soul>
    {
        public SoulReceiver() : base("Sprites/SoulReceiver")
        {
        }

        public IInputs<Soul> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }
}
