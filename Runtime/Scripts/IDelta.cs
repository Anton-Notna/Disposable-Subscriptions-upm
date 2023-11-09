namespace DisposableSubscriptions
{
    public interface IDelta<out T>
    {
        public T Previous { get; }

        public IUpdatable<T> Current { get; }
    }
}

