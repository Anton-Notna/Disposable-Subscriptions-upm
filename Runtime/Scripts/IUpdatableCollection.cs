using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public interface IUpdatableCollection<out T>
    {
        public IEvent<IUpdatable<T>> UnitAdded { get; }

        public IEvent<IDelta<T>> UnitUpdated { get; }

        public IEvent<IUpdatable<T>> UnitRemoving { get; }

        public IReadOnlyCollection<IUpdatable<T>> Collection { get; }

        public bool Contains(int id);

        public IUpdatable<T> Get(int id);
    }
}

