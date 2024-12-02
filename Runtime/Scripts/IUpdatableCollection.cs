using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public interface IUpdatableCollection<out T> : IEnumerable<IUpdatable<T>>
    {
        public IEvent<IUpdatable<T>> UnitAdded { get; }

        public IEvent<IDelta<T>> UnitUpdated { get; }

        /// <summary>
        /// Calls BEFORE unit removed from Collection
        /// </summary>
        public IEvent<IUpdatable<T>> UnitRemoving { get; }

        /// <summary>
        /// Calls AFTER unit removed from Collection
        /// </summary>
        public IEvent<IUpdatable<T>> UnitRemoved { get; }

        /// <summary>
        /// You can use IUpdatableCollection.GetEnumerator as alternative
        /// </summary>
        public IReadOnlyCollection<IUpdatable<T>> Collection { get; }

        public bool Contains(int id);

        public IUpdatable<T> Get(int id);
    }
}

