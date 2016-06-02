using Shared.Frameworks.DataAccess.CodeGen.Common;

namespace Shared.Frameworks.DataAccess.CodeGen.TemplateTypes
{
    public interface ITemplateData
    {
        TemplateType TemplateType { get; }

        string Expand();
    }
}
