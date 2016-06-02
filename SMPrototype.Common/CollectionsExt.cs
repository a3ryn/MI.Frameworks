using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMPrototype.Common
{
    public static class CollectionsExt
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
    }
}
