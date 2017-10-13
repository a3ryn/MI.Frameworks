/*
This source file is under MIT License (MIT)
Copyright (c) 2015 Mihaela Iridon
https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shared.Core.Common.Extensions
{
    public static class EnumExt
    {
        public static TEnum EnumFromDisplayName<TEnum>(this string enumDisplayName) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ApplicationException("Invalid enumeration type!");
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            var val = values.FirstOrDefault(x => (x as Enum).EnumDisplayName().Equals(enumDisplayName.Trim(), StringComparison.CurrentCultureIgnoreCase));
            return val;
        }

        public static string EnumDisplayName<TEnum>(this TEnum eItem, bool ifNotFoundReturnName = true)
        {
            var et = eItem.GetType();
            var em = et.GetMember(eItem.ToString());
            
            if (em.Length == 0 || em.Length > 1 )
                throw new ApplicationException($"None or multiple enum matches found for enum member {eItem}");

            var em0 = em[0];
            var attribs = em0.GetCustomAttributes<DisplayAttribute>();
            if (attribs != null) return (attribs.First()).Name;
            if (ifNotFoundReturnName) return eItem.ToString();
            throw new ApplicationException($"Display attribute on enum member {eItem} not found!");
        }

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

        public static Predicate<TEnum> IsValidEnumMember<TEnum>(IEnumerable<TEnum> allowedItems = null) where TEnum : struct
        {
            if (typeof(TEnum).IsEnum)
            {
                return x =>
                {
                    var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
                    if (allowedItems == null || !allowedItems.Any())
                        return values.Contains(x);
                    return allowedItems.Contains(x);
                };
            }
            throw new ApplicationException("The provided type is not an Enum. The rule cannot be applied");
        }

        public static bool IsValid<TEnum>(this TEnum val, params TEnum[] allowedItems)
            where TEnum : struct
            => IsValidEnumMember(allowedItems)(val);

        public static bool IsValid<TEnum>(this int val)
            where TEnum : struct
        {
            var t = typeof(TEnum);
            if (!t.IsEnum) return false;

            var vals = Enum.GetValues(t).Cast<int>();
            return vals.Contains(val);
        }
    }
}
