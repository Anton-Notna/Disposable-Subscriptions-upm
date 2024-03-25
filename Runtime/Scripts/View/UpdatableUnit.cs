using System;
using UnityEngine;

namespace DisposableSubscriptions.View
{
    public abstract class UpdatableUnit<TSelf, TData> : MonoBehaviour where TSelf : UpdatableUnit<TSelf, TData>
    {
        private IUpdatable<TData> _data;
        private IDisposable _sub;

        public TSelf Init(IUpdatable<TData> data)
        {
            _sub.TryDispose();
            _data = data;
            _sub = data.Subscribe(Refresh);
            Refresh(_data.Current);

            return (TSelf)this;
        }

        public abstract void Refresh(TData unit);

        protected void ForceRefresh() => Refresh(_data.Current);

        protected virtual void OnDestroying() { }

        private void OnDestroy()
        {
            _sub.TryDispose();
            OnDestroying();
        }
    }
}