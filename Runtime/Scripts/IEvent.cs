using System;

namespace DisposableSubscriptions
{
    public interface IEvent
    {
        public IDisposable Subscribe(Action action);
    }
}