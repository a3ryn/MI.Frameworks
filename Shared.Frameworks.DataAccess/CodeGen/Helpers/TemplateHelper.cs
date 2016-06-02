using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Shared.Frameworks.DataAccess.CodeGen.Common;
using Shared.Frameworks.DataAccess.CodeGen.TemplateTypes;

namespace Shared.Frameworks.DataAccess.CodeGen.Helpers
{
    internal static class TemplateHelper
    {
        internal static readonly Assembly AssemblyResourceProvider = Assembly.GetExecutingAssembly();

        internal static readonly Lazy<IReadOnlyDictionary<TemplateType, IEnumerable<PropertyInfo>>> PropertiesByTemplateType =
            new Lazy<IReadOnlyDictionary<TemplateType, IEnumerable<PropertyInfo>>>(PopulatePropertiesForTemplates);

        private static IReadOnlyDictionary<TemplateType, IEnumerable<PropertyInfo>> PopulatePropertiesForTemplates()
        {
            var types = AssemblyResourceProvider
                .GetTypes()
                .Where(x => x.GetCustomAttribute<TemplateTypeAttribute>() != null)
                .ToDictionary(
                        type => type.GetCustomAttribute<TemplateTypeAttribute>().Type,
                        type => type.GetProperties().AsEnumerable());
            return types;
        }

        internal static Script ReplaceParametersInTemplate<T>(Script source, T data )
            where T : ITemplateData
        {
            source = source
                .ScriptSimpleProperties(data) //does the parameters with $()
                .ScriptAttributedProperties(data); //does the parameters with @ which require template expansion themselves

            return source;
        }

        private static Script ScriptSimpleProperties<T>(this Script source, T data) where T : ITemplateData
        {
            var props = GetProperties(data.TemplateType, false); //exclude properties that require embedded template
            source = props.Aggregate(source,
                (input, pi) => 
                    input.SpecifyScriptParameter(pi.Name, pi.GetValue(data)?.ToString() ?? string.Empty));
            return source;
        }

        private static Script ScriptAttributedProperties<T>(this Script source, T data) where T : ITemplateData
        {
            var props = GetProperties(data.TemplateType, true); //only properties with embedded template; assumes these are collections!
            source = props.Aggregate(source,
                (input, pi) =>
                    input.SpecifyScriptParameter(pi.Name,
                        (pi.GetValue(data) as IEnumerable<ITemplateData>)?.Select(x => new Script(x.Expand())).Combine()));

            return source;
        }

        private static IEnumerable<PropertyInfo> GetProperties(TemplateType tt, bool withTemplateAttribute)
        {
            Predicate<PropertyInfo> p = x => withTemplateAttribute
                ? x.GetCustomAttribute<TemplateTypeAttribute>() != null
                : x.GetCustomAttribute<TemplateTypeAttribute>() == null;

            return PropertiesByTemplateType.Value[tt]
                .Where(x => p(x)); //exclude properties that require embedded template
        }

        private static Script Combine(this IEnumerable<Script> items)
        {
            var sb = new StringBuilder();
            sb = items.Aggregate(sb, (input, item) => sb.Append(item));
            return sb.ToString().TrimEnd();
        }

    }
}
