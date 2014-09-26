using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content.Generators;
using System.Reflection;

namespace Sitana.Framework.Content
{
    public class LoadResourceInfo
    {
        public MethodInfo Load;
        public String Path;
        public ContentGenerator Generator;
        public Type ContentType;
    }
}
