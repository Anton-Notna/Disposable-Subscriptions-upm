namespace DisposableSubscriptions
{
    public class Updatable<T> : Event<T>, IUpdatable<T>
    {
        private static readonly bool _referenceType = typeof(T).IsClass;
        private bool _exists;
        private T _current;

        public bool Exists => _exists && (_referenceType ? _current != null : true);

        public T Current => _current;

        public Updatable() { }

        public Updatable(T value)
        {
            _exists = true;
            _current = value;
        }

        protected override void OnUpdating(T value)
        {
            _exists = true;
            _current = value;
        }
    }
}