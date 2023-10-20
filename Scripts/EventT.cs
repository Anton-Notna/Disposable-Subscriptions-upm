using System;
using System.Collections.Generic;
using System.Linq;

namespace DisposableSubscriptions
{
    public class Event<T> : IEvent<T>
    {
        private readonly List<Action<T>> _observers = new List<Action<T>>();

        public IDisposable Subscribe(Action<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Contains(observer) == false)
                _observers.Add(observer);

            return new Unsubscriber<Action<T>>(_observers, observer);
        }

        public void Update(T value)
        {
            OnUpdating(value);

            if (_observers.Count == 0)
                return;

            foreach (Action<T> observer in _observers.ToList())
                observer?.Invoke(value);
        }

        protected virtual void OnUpdating(T value) { }
    }
}