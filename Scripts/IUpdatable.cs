namespace DisposableSubscriptions
{
    public interface IUpdatable<out T> : IEvent<T>
    {
        public bool Exists { get; }

        public T Current { get; }
    }
}