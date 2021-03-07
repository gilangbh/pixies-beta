using System;
using System.Collections.Generic;
using System.Linq;

namespace pixies_nft_console
{
    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
        // public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
        // {
        //     return items.Select((item, inx) => new { item, inx })
        //                 .GroupBy(x => x.inx / maxItems)
        //                 .Select(g => g.Select(x => x.item));
        // }
    }
}