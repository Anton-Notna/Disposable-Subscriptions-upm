using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class Unsubscriber<T> : IDisposable
    {
        private readonly List<T> _observers;
        private readonly T _observer;

        public Unsubscriber(List<T> observers, T observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}