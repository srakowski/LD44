namespace CryptoReaper.Simulation.Devices
{
    abstract class Pipe<T> : GameToken, IInputs<T>, IOutputs<T> where T : Unit
    {
        protected Pipe(string textureKey) : base(textureKey)
        {
        }

        public IOutputs<T> InputFrom => throw new System.NotImplementedException();

        public IODirection InputFromDirection => throw new System.NotImplementedException();

        public IInputs<T> OutputTo => throw new System.NotImplementedException();

        public IODirection OutputToDirection => throw new System.NotImplementedException();
    }

    abstract class SoulPipe : Pipe<Soul>
    {
        protected SoulPipe(string textureKey) : base(textureKey)
        {
        }
    }

    class StraightSoulPipe : SoulPipe
    {
        public StraightSoulPipe() : base("Sprites/StraightSoulPipe")
        {
        }
    }

    class AngledSoulPipe : SoulPipe
    {
        public AngledSoulPipe() : base("Sprites/AngledSoulPipe")
        {
        }
    }

    abstract class HellFirePipe : Pipe<HellFire>
    {
        protected HellFirePipe(string textureKey) : base(textureKey)
        {
        }
    }

    class StraightHellFirePipe : HellFirePipe
    {
        public StraightHellFirePipe() : base("Sprites/StraightHellFirePipe")
        {
        }
    }

    class AngledHellFirePipe : HellFirePipe
    {
        public AngledHellFirePipe() : base("Sprites/AngledHellFirePipe")
        {
        }
    }
}
