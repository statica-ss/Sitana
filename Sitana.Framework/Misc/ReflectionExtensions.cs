using System;
using System.Collections.Generic;
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

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static PropertyInfo[] GetProperties(this Type type, BindingFlags flags)
        {
            Console.WriteLine("[SLOW] ReflectionExtensions.GetProperties");
            return type.GetTypeInfo().DeclaredProperties.ToArray();
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethod(name);
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            MethodInfo[] methods;
            if (!_methods.TryGetValue(type, out methods))
            {
                methods = type.GetTypeInfo().DeclaredMethods.ToArray();
                _methods.Add(type, methods);
            }

            return methods;
        }

        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredField(name);
        }

        public static bool IsSubclassOf(this Type type, Type baseType)
        {
            return type.GetTypeInfo().IsSubclassOf(baseType);
        }

        public static bool IsAssignableFrom(this Type type, Type from)
        {
            return type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
        }
    }
}
