using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework
{
    [Flags]
    public enum BindingFlags
    {
        Public = 1,
        GetProperty = 2,
        Static = 4
    }

    public static class ReflectionExtensions
    {
        static Dictionary<Type, MethodInfo[]> _methods = new Dictionary<Type, MethodInfo[]>();
        static Dictionary<Type, PropertyInfo[]> _properties = new Dictionary<Type, PropertyInfo[]>();

        public static FieldInfo GetField(this Type type, string name)
        {
            Type baseType = type;
            FieldInfo info = type.GetTypeInfo().GetDeclaredField(name);
            while(info == null)
            {
                baseType = baseType.GetTypeInfo().BaseType;

                if(baseType == null || baseType.GetTypeInfo().IsPrimitive)
                {
                    return null;
                }

                info = baseType.GetTypeInfo().GetDeclaredField(name);
            }

            return info;
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            Type baseType = type;
            PropertyInfo info = type.GetTypeInfo().GetDeclaredProperty(name);
            while (info == null)
            {
                baseType = baseType.GetTypeInfo().BaseType;

                if (baseType == null || baseType.GetTypeInfo().IsPrimitive)
                {
                    return null;
                }

                info = baseType.GetTypeInfo().GetDeclaredProperty(name);
            }

            return info;
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
            PropertyInfo[] properties;
            if (!_properties.TryGetValue(type, out properties))
            {
                List<PropertyInfo> list = new List<PropertyInfo>();

                Type baseType = type;

                for (; ; )
                {
                    list.AddRange(baseType.GetTypeInfo().DeclaredProperties);

                    baseType = baseType.GetTypeInfo().BaseType;

                    if (baseType == null || baseType.GetTypeInfo().IsPrimitive)
                    {
                        break;
                    }
                }

                properties = list.ToArray();
                _properties.Add(type, properties);
            }
            return properties;
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            MethodInfo[] methods;
            if(!_methods.TryGetValue(type, out methods))
            {
                List<MethodInfo> list = new List<MethodInfo>();

                Type baseType = type;

                for (;;)
                {
                    list.AddRange(baseType.GetTypeInfo().DeclaredMethods);

                    baseType = baseType.GetTypeInfo().BaseType;

                    if (baseType == null || baseType.GetTypeInfo().IsPrimitive)
                    {
                        break;
                    }
                }

                methods = list.ToArray();
                _methods.Add(type, methods);
            }
            return methods;
        }
    }
}
