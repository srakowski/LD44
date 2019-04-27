namespace CryptoReaper.Simulation
{
    interface IInputs<T> where T : Unit
    {
        IOutputs<T> InputFrom { get; }
        IODirection InputFromDirection { get; }
    }

    interface IInputs<T, T2> where T : Unit where T2 : Unit
    {
        IOutputs<T> Input1From { get; }
        IODirection Input1FromDirection { get; }

        IOutputs<T> Input2From { get; }
        IODirection Input2FromDirection { get; }
    }

    interface IOutputs<T> where T : Unit
    {
        IInputs<T> OutputTo { get; }
        IODirection OutputToDirection { get; }
    }

    enum IODirection
    {
        Any,
        Up,
        Right,
        Down,
        Left
    }
}
