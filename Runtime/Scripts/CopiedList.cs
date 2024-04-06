using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public class CopiedList<T>
    {
        private T[] _copied = new T[0];
        private int _previousHash = int.MinValue;

        public T this[int i] => _copied[i];

        public int Count { get; private set; }

        public void CopyFrom(IReadOnlyList<T> source, int sourceHash)
        {
            if (_previousHash == sourceHash)
                return;

            _previousHash = sourceHash;

            if (source.Count != Count)
            {
                Count = source.Count;

                if (source.Count > _copied.Length)
                    Array.Resize(ref _copied, source.Count);
            }

            for (int i = 0; i < Count; i++)
                _copied[i] = source[i];
        }
    }
}