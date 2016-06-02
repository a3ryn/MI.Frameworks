using Shared.Frameworks.DataAccess.CodeGen.Common;

namespace Shared.Frameworks.DataAccess.CodeGen.TemplateTypes
{
    [TemplateType(TemplateType.Property)]
    public class PropertyTemplate : ScriptTemplate<PropertyTemplate>
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
    }
}
