using System;
using System.Collections.Generic;
using Shared.Frameworks.DataAccess.CodeGen.Common;

namespace Shared.Frameworks.DataAccess.CodeGen.TemplateTypes
{
    [TemplateType(TemplateType.Type)]
    public class TypeTemplate : ScriptTemplate<TypeTemplate>
    {
        public string Typename { get; set; }
        public string Namespace { get; set; }

        public string Date => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [TemplateType(TemplateType.Property)]
        public IEnumerable<ITemplateData> Properties { get; set; }
    }
}
