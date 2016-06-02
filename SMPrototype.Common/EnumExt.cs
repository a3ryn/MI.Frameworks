using System;
using System.Collections.Generic;
using System.Linq;
using imVision.Core.Common.Extensions;

namespace SMPrototype.Common
{
    public static class EnumExt
    {
        public static TAttrib GetEnumAttribute<TEnum, TAttrib>(this TEnum eItem, bool ifNotFoundReturnName = true)
            where TAttrib : class
        {
            var et = eItem.GetType();
            var em = et.GetMember(eItem.ToString());

            if (em.Length == 0 || em.Length > 1)
                throw new ApplicationException($"None or multiple enum matches found for enum member {eItem}");

            var em0 = em[0];
            var attribs = em0.GetCustomAttributes<TAttrib>();
            if (attribs != null) return (attribs.First());
            throw new ApplicationException($"Attribute {typeof(TAttrib).Name} on enum member {eItem} not found!");
        }

        //TODO: move method below to core library under extension
        public static TAttrib GetEnumAttribute<TEnum, TAttrib, TIAttrib>(this TEnum eItem, bool ifNotFoundReturnName = true)
            where TAttrib : class, TIAttrib
        {
            var et = eItem.GetType();
            var em = et.GetMember(eItem.ToString());

            if (em.Length == 0 || em.Length > 1)
                throw new ApplicationException($"None or multiple enum matches found for enum member {eItem}");

            var em0 = em[0];
            var attribs = em0.GetCustomAttributes<TAttrib>();
            if (attribs != null) return (attribs.First());
            throw new ApplicationException($"Attribute {typeof(TAttrib).Name} on enum member {eItem} not found!");
        }
    }
}
