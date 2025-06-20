namespace LibraryApi.Common.Utill
{
    public static class ListUtil
    {
        public static bool IsEmpty<T>(IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsNotEmpty<T>(IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static int Length<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                return 0;

            if (collection is ICollection<T> coll)
                return coll.Count;

            return collection.Count();
        }
    }
}
