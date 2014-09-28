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
        public bool Binding;
    }

    public class FieldName
    {
        public string Name;
        public object[] Parameters;
        public bool Binding;
    }

    struct ReflectionParameter
    {
        public readonly string Name;

        public ReflectionParameter(string name)
        {
            Name = name;
        }
    }

    public struct InvokeParam
    {
        public readonly string Id;
        public readonly object Value;

        public InvokeParam(string id, object value)
        {
            Id = id;
            Value = value;
        }
    }

    public class InvokeParameters: Dictionary<string, object>
    {
        public void Set(InvokeParam param)
        {
            Remove(param.Id);
            Add(param.Id, param.Value);
        }
    }
}
