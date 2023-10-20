using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public interface IUpdatableCollection<T>
    {
        public IEvent<IUpdatable<T>> UnitAdded { get; }

        public IEvent<(T previous, IUpdatable<T> actual)> UnitUpdated { get; }

        public IEvent<IUpdatable<T>> UnitRemoving { get; }

        public IReadOnlyCollection<IUpdatable<T>> Collection { get; }

        public bool Get(int id, out IUpdatable<T> item);
    }
}