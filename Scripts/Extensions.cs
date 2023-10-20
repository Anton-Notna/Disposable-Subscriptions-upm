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

            for (int i = 0; i < subs.Count; i++)
                subs[i].TryDispose();

            subs.Clear();
        }

        public static void TryDispose(this IDisposable sub)
        {
            if (sub != null)
                sub.Dispose();
        }
    }
}