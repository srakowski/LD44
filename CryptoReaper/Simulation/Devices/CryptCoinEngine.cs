namespace CryptoReaper.Simulation.Devices
{
    class CryptCoinEngine : Device, IInputs<HellFire, Soul>, IOutputs<CryptCoin>
    {
        public IOutputs<HellFire> Input1From => throw new System.NotImplementedException();

        public IODirection Input1FromDirection => throw new System.NotImplementedException();

        public IOutputs<HellFire> Input2From => throw new System.NotImplementedException();

        public IODirection Input2FromDirection => throw new System.NotImplementedException();


        public IInputs<CryptCoin> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => IODirection.Any;
    }
}
