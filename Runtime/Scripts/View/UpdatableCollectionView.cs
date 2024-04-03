using System;
using System.Collections.Generic;
using System.Linq;
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

        private IUpdatableCollection<TData> _dataToSubscribe;
        private IUpdatableCollection<TData> _currentData;

        public IReadOnlyDictionary<int, TView> SpawnedUnits => _spawnedUnits;

        public void Init(IUpdatableCollection<TData> data)
        {
            Clear();

            if (gameObject.activeInHierarchy && gameObject.activeInHierarchy)
                Subscribe(data);
            else
                _dataToSubscribe = data;
        }

        protected virtual void OnSpawned(TView unit) { }

        protected virtual void OnRemoved(TView unit) { }

        protected virtual bool NeedSpawn(TData data) => true;

        protected virtual TView GetPrefab(TData data) => _prefab;

        protected virtual (Vector3 Position, Quaternion Rotation) ComputeSpawnPosition(TData data) => (_root.position, _root.rotation);

        protected virtual Transform ComputeParent(TData data) => _root;

        protected virtual TView InstantiateView(TView prefab, Vector3 position, Quaternion rotation, Transform parent) => Instantiate(prefab, position, rotation, parent);

        /// <summary>!!! Need to call base.OnEnable !!!</summary>
        protected virtual void OnEnable()
        {
            if (_dataToSubscribe == null)
                return;

            Subscribe(_dataToSubscribe);
            _dataToSubscribe = null;
        }

        /// <summary>!!! Need to call base.OnDisable !!!</summary>
        protected virtual void OnDisable()
        {
            _dataToSubscribe = _currentData;
            Clear();
        }

        private void Subscribe(IUpdatableCollection<TData> data)
        {
            Clear();
            _currentData = data;
            _subs.Add(_currentData.UnitAdded.Subscribe(Spawn));
            _subs.Add(_currentData.UnitRemoving.Subscribe(Remove));

            foreach (var unit in _currentData.Collection)
                Spawn(unit);
        }

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

        private void Remove(IUpdatable<TData> updatable) => Remove(updatable.Current.ID);

        private void Remove(int id)
        {
            if (_spawnedUnits.TryGetValue(id, out var unit) == false)
                return;

            if (unit != null && unit.gameObject != null)
                Destroy(unit.gameObject);
            _spawnedUnits.Remove(id);

            OnRemoved(unit);
        }

        private void Clear()
        {
            _currentData = null;
            _subs.TryDispose();

            while (_spawnedUnits.Count != 0)
                Remove(_spawnedUnits.Keys.First());
        }
    }
}