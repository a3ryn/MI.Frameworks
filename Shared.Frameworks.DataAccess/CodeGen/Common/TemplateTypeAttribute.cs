using System;

namespace Shared.Frameworks.DataAccess.CodeGen.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    internal class TemplateTypeAttribute : Attribute
    {
        internal TemplateTypeAttribute(TemplateType type)
        {
            Type = type;
        }

        internal TemplateType Type { get; }
    }
}
