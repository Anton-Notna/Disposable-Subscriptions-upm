using System;

namespace DisposableSubscriptions
{
    public interface IEvent<out T>
    {
        public IDisposable Subscribe(Action<T> action);
    }
}