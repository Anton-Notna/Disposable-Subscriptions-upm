namespace DisposableSubscriptions
{
    public static class IUpdatableCollectionExtensions
    {
        public static bool Get<T>(this IUpdatableCollection<T> collection, int id, out IUpdatable<T> value)
        {
            if (collection != null && collection.Contains(id))
            {
                value = collection.Get(id);
                return true;
            }

            value = default;
            return false;
        }
    }
}

