using System.Reflection;
using Shared.Core.Common.Extensions;
using Shared.Frameworks.DataAccess.CodeGen.Common;
using Shared.Frameworks.DataAccess.CodeGen.Helpers;

namespace Shared.Frameworks.DataAccess.CodeGen.TemplateTypes
{
    public abstract class ScriptTemplate<T> : ITemplateData
    where T : ScriptTemplate<T>
    {
        private readonly string TemplateText;

        public ScriptTemplate()
        {
            this.TemplateText = //TemplateHelper.AssemblyResourceProvider.Get(typeof(T).Assembly).FindTextResource($"{typeof(T).Name}.cs");
                typeof(T).Assembly.FindTextResource($"{typeof(T).FullName}.txt");
        }

        public ScriptTemplate(string TemplateText)
        {
            this.TemplateText = TemplateText;
        }

        public string Expand() => TemplateHelper.ReplaceParametersInTemplate(TemplateText, this);
        //ScriptUtilities.SpecifyParametersWithObject(TemplateText, this);

        public TemplateType TemplateType =>
            typeof(T).GetCustomAttribute<TemplateTypeAttribute>().Type;
    }
}
