using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        // naive implementation since DistinctBy is not available before .net 6
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (!source.Any())
            {
                yield break;
            }

            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (var val in source)
            {
                TKey key = keySelector(val);
                if (seenKeys.Contains(key))
                    continue;

                seenKeys.Add(key);
                yield return val;
            }
        }
    }
}
