using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Core.Common.Extensions
{
    public static class EnumerableExt
    {
        public static int? IndexOf<T>(this IEnumerable<T> items, T item) where T : IEquatable<T>
        {
            var itemsList = items.ToList();
            for (var i = 0; i < itemsList.Count; i++)
            {
                if (itemsList[i].Equals(item))
                    return i;
            }
            return null;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> a)
        {
            if (a != null)
                items?.ToList().ForEach(a);
        }
    }
}
