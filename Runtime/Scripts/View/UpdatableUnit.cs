using System;
using UnityEngine;

namespace DisposableSubscriptions.View
{
    public abstract class UpdatableUnit<TSelf, TData> : MonoBehaviour where TSelf : UpdatableUnit<TSelf, TData>
    {
        private IUpdatable<TData> _data;
        private IDisposable _sub;
        private Action<TSelf> _updated;

        public TSelf Init(IUpdatable<TData> data, Action<TSelf> updated)
        {
            _sub.TryDispose();
            _data = data;
            _updated = updated;
            _sub = data.Subscribe(RefreshData);
            RefreshData(_data.Current);

            return (TSelf)this;
        }

        public abstract void Refresh(TData unit);

        protected void ForceRefresh() => RefreshData(_data.Current);

        protected virtual void OnDestroying() { }

        private void OnDestroy()
        {
            _sub.TryDispose();
            OnDestroying();
        }
        private void RefreshData(TData data)
        {
            Refresh(data);
            _updated.Invoke((TSelf)this);
        }
    }
}