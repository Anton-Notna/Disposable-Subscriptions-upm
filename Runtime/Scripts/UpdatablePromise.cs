using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class UpdatablePromise<T> : IUpdatable<T>, UnsubscribeTarget<Action<T>>, IDisposable where T : IIdentifiable
    {
        private readonly List<IDisposable> _collectionSubs = new List<IDisposable>();
        private readonly List<Action<T>> _observers = new List<Action<T>>();
        private readonly CopiedList<Action<T>> _toListObservers = new CopiedList<Action<T>>();
        private readonly int _id;

        private IUpdatable<T> _unit;
        private IDisposable _unitSub;
        private bool _disposed;
        private int _observersHash;

        public bool Exists => _unit != null && _unit.Exists;

        public T Current => _unit.Current;

        public UpdatablePromise(IUpdatableCollection<T> collection, int id)
        {
            _id = id;
            _collectionSubs.Add(collection.UnitAdded.Subscribe(CheckAdded));
            _collectionSubs.Add(collection.UnitRemoved.Subscribe(CheckRemoved));
            if (collection.Get(id, out var item))
                CheckAdded(item);
        }

        public void Dispose()
        {
            _collectionSubs.TryDispose();
            _unitSub.TryDispose();
            _unit = null;
            _disposed = true;
        }

        public IDisposable Subscribe(Action<T> observer)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Contains(observer) == false)
            {
                _observers.Add(observer);
                _observersHash++;
            }

            return new Unsubscriber<Action<T>>(this, observer);
        }

        public void Unsubscribe(Action<T> observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
                _observersHash++;
            }
        }

        private void CheckRemoved(IUpdatable<T> updatable)
        {
            if (_unit == null)
                return;

            if (updatable != _unit)
                return;

            _unitSub.Dispose();
            _unit = null;
        }

        private void CheckAdded(IUpdatable<T> updatable)
        {
            if (_unit != null)
                return;

            if (updatable.Current.ID != _id)
                return;

            _unitSub.TryDispose();

            _unit = updatable;
            _unitSub = _unit.Subscribe(CallInnerSubscribes);
            if (_unit.Exists)
                CallInnerSubscribes(_unit.Current);
        }

        private void CallInnerSubscribes(T current)
        {
            if (_observers.Count == 0)
                return;

            _toListObservers.CopyFrom(_observers, _observersHash);
            int count = _toListObservers.Count;
            for (int i = 0; i < count; i++)
                _toListObservers[i]?.Invoke(current);
        }
    }

    public static class UpdatablePromiseExtensions
    {
        public static UpdatablePromise<T> GetPromise<T>(this IUpdatableCollection<T> collection, int key) where T : IIdentifiable => new UpdatablePromise<T>(collection, key);
    }
}