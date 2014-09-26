using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public class MethodName
    {
        public string Name;
        public object[] Parameters;
    }

    public class FieldName
    {
        public string Name;
        public object[] Parameters;
    }

    struct ReflectionParameter
    {
        public readonly string Name;

        public ReflectionParameter(string name)
        {
            Name = name;
        }
    }
}
