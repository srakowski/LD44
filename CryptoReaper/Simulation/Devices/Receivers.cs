namespace CryptoReaper.Simulation.Devices
{
    class HellFireReceiver : Device, IOutputs<HellFire>
    {
        public IInputs<HellFire> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }

    class SoulReceiver : Device, IOutputs<Soul>
    {
        public IInputs<Soul> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }
}
