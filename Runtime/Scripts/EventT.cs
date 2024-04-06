using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class Event<T> : IEvent<T>, UnsubscribeTarget<Action<T>>
    {
        private readonly List<Action<T>> _observers = new List<Action<T>>();
        private readonly CopiedList<Action<T>> _toListObservers = new CopiedList<Action<T>>();
        private int _observersHash;

        public IDisposable Subscribe(Action<T> observer)
        {
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

        public void Update(T value)
        {
            OnUpdating(value);

            if (_observers.Count == 0)
                return;

            _toListObservers.CopyFrom(_observers, _observersHash);
            int count = _toListObservers.Count;
            for (int i = 0; i < count; i++)
                _toListObservers[i]?.Invoke(value);
        }

        protected virtual void OnUpdating(T value) { }
    }
}