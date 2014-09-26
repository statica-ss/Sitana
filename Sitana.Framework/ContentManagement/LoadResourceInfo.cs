using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Content.Generators;
using System.Reflection;

namespace Ebatianos.Content
{
    public class LoadResourceInfo
    {
        public MethodInfo Load;
        public String Path;
        public ContentGenerator Generator;
        public Type ContentType;
    }
}
