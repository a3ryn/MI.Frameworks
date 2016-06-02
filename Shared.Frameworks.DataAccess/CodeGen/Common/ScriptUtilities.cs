using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared.Frameworks.DataAccess.CodeGen.Common
{
    using static Core.Common.corefunc;

    /// <summary>
    /// Defines utilities for working with scripts
    /// </summary>
    public static class ScriptUtilities
    {
        /// <summary>
        /// Identifies script parameters of the form $(Parameter)
        /// </summary>
        private static readonly Regex ParamType1 = regex(@"\$\((?<Name>(\w)*)\)");

        /// <summary>
        /// Identifies script parameters of the form @Parameter
        /// </summary>
        private static readonly Regex ParamType2 = regex(@"@(?<Name>[a-zA-Z]*)");

        /// <summary>
        /// Gets the names of the script parameters implied by the script body
        /// </summary>
        /// <param name="script">The script</param>
        /// <param name="skipSecondary">Whether type-2 parameters are to be ignored</param>
        /// <returns></returns>
        public static IEnumerable<string> GetScriptParameterNames(this Script script, bool skipSecondary)
        {
            var names = new HashSet<string>();
            foreach (Match match in ParamType1.Matches(script))
            {
                names.Add(match.Groups["Name"].Value);
            }
            if (!skipSecondary)
            {
                foreach (Match match in ParamType2.Matches(script))
                {
                    names.Add(match.Groups["Name"].Value);
                }
            }

            return names;
        }

        public static Script SpecifyScriptParameter(this Script script, string paramName, string paramValue) =>
            script.Body.Replace($"$({paramName})", paramValue).Replace($"@{paramName}", paramValue);

        public static Script SpecifyParametersWithObject(this Script script, object paramobj)
        {
            var result = script;
            foreach (var property in props(paramobj))
            {
                var paramName = property.Name;
                var paramValue = property.GetValue(paramobj)?.ToString() ?? String.Empty;
                result = SpecifyScriptParameter(result, paramName, paramValue);
            }
            return result;
        }

        public static Script SpecifyParameters<T>(Script script, IReadOnlyDictionary<string, T> values, bool skipSecondary) =>
            GetScriptParameterNames(script, skipSecondary).Aggregate(script,
                (input, name) => SpecifyScriptParameter(input, name, values.ContainsKey(name) ? toString(values[name]) : String.Empty));

        public static Script SpecifyParametersFromEnvironment(Script script, bool skipSecondary) =>
            GetScriptParameterNames(script, skipSecondary).Aggregate(script,
                (input, name) => SpecifyScriptParameter(input, name, Environment.GetEnvironmentVariable(name)));


    }
}
