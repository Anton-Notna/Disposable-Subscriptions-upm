using System;
using System.Collections.Generic;

namespace DisposableSubscriptions
{
    public static class Extensions
    {
        public static void TryDispose(this List<IDisposable> subs)
        {
            if (subs == null)
                return;

            if (subs.Count == 0)
                return;

            for (int i = 0; i < subs.Count; i++)
                subs[i].TryDispose();

            subs.Clear();
        }

        public static void TryDispose(this IDisposable sub)
        {
            if (sub != null)
                sub.Dispose();
        }

        public static void SubscribeToAnyUpdate<T>(this IUpdatableCollection<T> collection, List<IDisposable> subs, Action action)
        {
            subs.Add(collection.UnitAdded.Subscribe(_ => action.Invoke()));
            subs.Add(collection.UnitRemoved.Subscribe(_ => action.Invoke()));
            subs.Add(collection.UnitUpdated.Subscribe(_ => action.Invoke()));
        }
    }
}