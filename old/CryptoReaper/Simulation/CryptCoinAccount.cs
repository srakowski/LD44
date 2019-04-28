namespace CryptoReaper.Simulation
{
    class CryptCoinAccount : IInputs<CryptCoin>
    {
        public IOutputs<CryptCoin> InputFrom => throw new System.NotImplementedException();

        public IODirection InputFromDirection => throw new System.NotImplementedException();
    }
}
