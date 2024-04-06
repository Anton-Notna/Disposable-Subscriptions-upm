using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class Event : IEvent, UnsubscribeTarget<Action>
    {
        private readonly List<Action> _observers = new List<Action>();
        private readonly CopiedList<Action> _toListObservers = new CopiedList<Action>();
        private int _observersHash;

        public IDisposable Subscribe(Action observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Contains(observer) == false)
            {
                _observers.Add(observer);
                _observersHash++;
            }

            return new Unsubscriber<Action>(this, observer);
        }

        public void Unsubscribe(Action observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
                _observersHash++;
            }
        }

        public void Update()
        {
            if (_observers.Count == 0)
                return;

            _toListObservers.CopyFrom(_observers, _observersHash);
            int count = _toListObservers.Count;
            for (int i = 0; i < count; i++)
                _toListObservers[i]?.Invoke();
        }
    }
}