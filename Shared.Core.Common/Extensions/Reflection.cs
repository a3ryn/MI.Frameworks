using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shared.Core.Common.Extensions
{
    public static class Reflection
    {
        public static bool ImplementsInterface(this Type t, Type iType)
        {
            return t.GetInterfaces().Any(y => y == iType);
        }

        public static bool ExtendsBaseClass(this Type t, Type baseType)
        {
            if (t.BaseType == null)
                return false; //stop condition
            return 
                t.BaseType == baseType || t.BaseType.ExtendsBaseClass(baseType);
        }

        public static IEnumerable<TAttrib> GetCustomAttributes<TAttrib>(this MemberInfo mi) where TAttrib : class
        {
            var t = typeof(TAttrib);
            var result = mi.GetCustomAttributes(t, false);
            return result.Select(x => x as TAttrib);
        }

        public static string FindTextResource(this Assembly asm, string resourceName)
        {
            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ApplicationException($"Resource named {resourceName} could not be found!");
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
