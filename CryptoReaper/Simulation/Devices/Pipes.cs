namespace CryptoReaper.Simulation.Devices
{
    abstract class Pipe<T> : Device, IInputs<T>, IOutputs<T> where T : Unit
    {
        public IOutputs<T> InputFrom => throw new System.NotImplementedException();

        public IODirection InputFromDirection => throw new System.NotImplementedException();

        public IInputs<T> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }

    abstract class SoulPipe : Pipe<Soul>
    {
    }

    class StraightSoulPipe : SoulPipe { }

    class AngledSoulPipe : SoulPipe { }

    abstract class HellFirePipe : Pipe<HellFire> { }

    class StraightHellFirePipe : HellFirePipe { }

    class AngledHellFirePipe : HellFirePipe { }
}
