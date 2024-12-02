using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DisposableSubscriptions
{
    public class UpdatableCollection<T> : IUpdatableCollection<T> where T : IIdentifiable
    {
        private class Delta : IDelta<T>
        {
            public T Previous { get; set; }

            public IUpdatable<T> Current { get; set; }

            public Delta Set(T previous, IUpdatable<T> current)
            {
                Current = current;
                Previous = previous;
                return this;
            }
        }

        private readonly Dictionary<int, Updatable<T>> _units = new Dictionary<int, Updatable<T>>();
        private readonly List<IUpdatable<T>> _collection = new List<IUpdatable<T>>();
        private readonly Event<IUpdatable<T>> _unitCreated = new Event<IUpdatable<T>>();
        private readonly Event<IUpdatable<T>> _unitRemoving = new Event<IUpdatable<T>>();
        private readonly Event<IUpdatable<T>> _unitRemoved = new Event<IUpdatable<T>>();

        private readonly Event<IDelta<T>> _unitUpdated = new Event<IDelta<T>>();
        private readonly Delta _delta = new Delta();

        private bool _dirtyCollection = true;

        public IReadOnlyList<IUpdatable<T>> Collection
        {
            get
            {
                if (_dirtyCollection)
                {
                    if (_collection.Count > 0)
                        _collection.Clear();

                    _collection.AddRange(_units.Values);

                    _dirtyCollection = false;
                }

                return _collection;
            }
        }

        public IEvent<IUpdatable<T>> UnitAdded => _unitCreated;

        public IEvent<IUpdatable<T>> UnitRemoving => _unitRemoving;

        public IEvent<IUpdatable<T>> UnitRemoved => _unitRemoved;

        public IEvent<IDelta<T>> UnitUpdated => _unitUpdated;

        public IEnumerator<IUpdatable<T>> GetEnumerator() => Collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(int id) => _units.ContainsKey(id);

        public IUpdatable<T> Get(int id)
        {
            if (_units.TryGetValue(id, out Updatable<T> value))
                return value;

            return null;
        }

        public void UpdateAndRemoveUnused(IEnumerable<T> values)
        {
            HashSet<int> unused = new HashSet<int>(_units.Keys);

            foreach (T value in values)
            {
                UpdateUnit(value);
                unused.Remove(value.ID);
            }

            foreach (int unusedId in unused)
                Remove(unusedId);
        }

        public void UpdateUnits(IEnumerable<T> values)
        {
            foreach (T value in values)
                UpdateUnit(value);
        }

        public void UpdateUnits(params T[] values)
        {
            for (int i = 0; i < values.Length; i++)
                UpdateUnit(values[i]);
        }

        public bool ChangeExistingUnit(int id, Func<T, T> change)
        {
            if (_units.TryGetValue(id, out Updatable<T> current) == false)
                return false;

            T value = current.Current;
            value = change.Invoke(value);
            UpdateUnit(id, value);

            return true;
        }

        public void UpdateUnit(T value)
        {
            if (UpdateUnit(value.ID, value) == false)
                CreateUnit(value);
        }

        public bool Remove(T unit) 
        {
            if (unit == null)
                return false;

            return Remove(unit.ID); 
        }

        public bool Remove(int id)
        {
            if (_units.TryGetValue(id, out var unit) == false)
                return false;

            _unitRemoving.Update(unit);
            _units.Remove(id);
            _dirtyCollection = true;
            _unitRemoved.Update(unit);
            return true;
        }

        public void RemoveAll()
        {
            while (_units.Count > 0)
                Remove(_units.Keys.First());
        }

        private void CreateUnit(T value)
        {
            Updatable<T> unit = new Updatable<T>(value);
            _units.Add(value.ID, unit);
            _dirtyCollection = true;
            _unitCreated.Update(unit);
        }

        private bool UpdateUnit(int id, T value)
        {
            if (_units.TryGetValue(id, out var unit) == false)
                return false;

            T previous = unit.Current;
            unit.Update(value);
            _unitUpdated.Update(_delta.Set(previous, unit));
            return true;
        }
    }
}