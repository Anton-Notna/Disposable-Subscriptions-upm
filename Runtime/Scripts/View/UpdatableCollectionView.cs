using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisposableSubscriptions.View
{
    public abstract class UpdatableCollectionView<TView, TData> : MonoBehaviour where TView : UpdatableUnit<TView, TData> where TData : IIdentifiable
    {
        [SerializeField]
        private TView _prefab;
        [SerializeField]
        private Transform _root;

        private readonly Dictionary<int, TView> _spawnedUnits = new Dictionary<int, TView>();
        private readonly List<IDisposable> _subs = new List<IDisposable>();
     
        public IReadOnlyDictionary<int, TView> SpawnedUnits => _spawnedUnits;

        public void Init(IUpdatableCollection<TData> data)
        {
            Clear();

            _subs.Add(data.UnitAdded.Subscribe(Spawn));
            _subs.Add(data.UnitRemoving.Subscribe(Remove));

            foreach (var unit in data.Collection)
                Spawn(unit);
        }

        protected virtual void OnSpawned(TView unit) { }

        protected virtual void OnRemoved(TView unit) { }

        protected virtual bool NeedSpawn(TData data) => true;

        protected virtual TView GetPrefab(TData data) => _prefab;

        protected virtual (Vector3 Position, Quaternion Rotation) ComputeSpawnPosition(TData data) => (_root.position, _root.rotation);

        protected virtual Transform ComputeParent(TData data) => _root;

        protected virtual TView InstantiateView(TView prefab, Vector3 position, Quaternion rotation, Transform parent) => Instantiate(prefab, position, rotation, parent);

        private void OnDestroy() => Clear();

        private void Spawn(IUpdatable<TData> updatable)
        {
            TData data = updatable.Current;
            if (NeedSpawn(data) == false)
                return;
            (Vector3 Position, Quaternion Rotation) location = ComputeSpawnPosition(data);
            var unit = InstantiateView(GetPrefab(data), location.Position, location.Rotation, ComputeParent(data)).Init(updatable);
            _spawnedUnits.Add(updatable.Current.ID, unit);
            OnSpawned(unit);
        }

        private void Remove(IUpdatable<TData> updatable)
        {
            if (_spawnedUnits.TryGetValue(updatable.Current.ID, out var unit) == false)
                return;

            if (unit != null && unit.gameObject != null)
                Destroy(unit.gameObject);
            _spawnedUnits.Remove(updatable.Current.ID);

            OnRemoved(unit);
        }

        private void Clear()
        {
            _subs.TryDispose();

            if (_spawnedUnits.Count == 0)
                return;

            foreach (var unit in _spawnedUnits.Values)
            {
                if (unit != null && unit.gameObject != null)
                    Destroy(unit.gameObject);
            }

            _spawnedUnits.Clear();
        }
    }
}