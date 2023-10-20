using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class UpdatableCollection<T> : IUpdatableCollection<T> where T : IIdentifiable
    {
        private readonly Dictionary<int, Updatable<T>> _units = new Dictionary<int, Updatable<T>>();
        private readonly Event<IUpdatable<T>> _unitCreated = new Event<IUpdatable<T>>();
        private readonly Event<IUpdatable<T>> _unitRemoving = new Event<IUpdatable<T>>();
        private readonly Event<(T, IUpdatable<T>)> _unitUpdated = new Event<(T, IUpdatable<T>)>();

        public IReadOnlyCollection<IUpdatable<T>> Collection => _units.Values;

        public IEvent<IUpdatable<T>> UnitAdded => _unitCreated;

        public IEvent<IUpdatable<T>> UnitRemoving => _unitRemoving;

        public IEvent<(T, IUpdatable<T>)> UnitUpdated => _unitUpdated;

        public bool Get(int id, out IUpdatable<T> item)
        {
            if (_units.TryGetValue(id, out Updatable<T> value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
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
            return true;
        }

        public void RemoveAll()
        {
            foreach (var unit in _units.Values)
                _unitRemoving.Update(unit);
            
            _units.Clear();
        }

        private void CreateUnit(T value)
        {
            Updatable<T> unit = new Updatable<T>(value);
            _units.Add(value.ID, unit);
            _unitCreated.Update(unit);
        }

        private bool UpdateUnit(int id, T value)
        {
            if (_units.TryGetValue(id, out var unit) == false)
                return false;

            T previous = unit.Current;
            unit.Update(value);
            _unitUpdated.Update((previous, unit));
            return true;
        }
    }
}