using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class Unsubscriber<T> : IDisposable
    {
        private readonly UnsubscribeTarget<T> _target;
        private readonly T _observer;

        public Unsubscriber(UnsubscribeTarget<T> observers, T observer)
        {
            _target = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _target != null)
                _target.Unsubscribe(_observer);
        }
    }

    public interface UnsubscribeTarget<T>
    {
        public void Unsubscribe(T observer);
    }
}