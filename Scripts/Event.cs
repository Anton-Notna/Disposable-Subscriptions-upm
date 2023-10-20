using System;
using System.Collections.Generic;
using System.Linq;

namespace DisposableSubscriptions
{
    public class Event : IEvent
    {
        private readonly List<Action> _observers = new List<Action>();

        public IDisposable Subscribe(Action observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Contains(observer) == false)
                _observers.Add(observer);

            return new Unsubscriber<Action>(_observers, observer);
        }

        public void Update()
        {
            if (_observers.Count == 0)
                return;

            foreach (Action observer in _observers.ToList())
                observer?.Invoke();
        }
    }
}