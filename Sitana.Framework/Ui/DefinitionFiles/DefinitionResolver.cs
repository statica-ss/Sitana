using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;
using Ebatianos.Ui.DefinitionFiles;
using Ebatianos.Content;
using System.Reflection;

namespace Ebatianos.Ui.DefinitionFiles
{
    public static class DefinitionResolver
    {
        private static readonly Dictionary<string, object> _parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public static object Parameter(string id)
        {
            object val;

            if ( !_parameters.TryGetValue(id, out val) )
            {
                throw new Exception(String.Format("Unable to find parameter {0}.", id));
            }
            return val;
        }

        public static void Parameter(string id, object value)
        {
            _parameters.Remove(id);
            _parameters.Add(id, value);
        }

        public static object GetFieldValue(object context, object definition)
        {
            FieldName field = (FieldName)definition;

            PropertyInfo info = context.GetType().GetProperty(field.Name);

            int[] indices = null;

            if (field.Parameters != null)
            {
                indices = new int[field.Parameters.Length];

                for (int idx = 0; idx < indices.Length; ++idx)
                {
                    indices[idx] = (int)ObtainParameter(field.Parameters[idx]);
                }
            }

            if ( info != null )
            {
                object value = info.GetValue(context, null);

                if (indices != null)
                {
                    if (value is Array)
                    {
                        return (value as Array).GetValue(indices);
                    }

                    throw new Exception(String.Format("Value is not an array: {0}[{1}]", field.Name, indices.ToString()));
                }
                else
                {
                    return value;
                }
            }

            throw new Exception(String.Format("Cannot find field: {0}{1}", field.Name, indices != null ? '['+indices.ToString()+']' : ""));
        }

        public static object InvokeMethod(object context, object definition)
        {
            MethodName method = (MethodName)definition;

            Type[] types = method.Parameters.Length > 0 ? new Type[method.Parameters.Length] : null;
            object[] parameters = method.Parameters.Length > 0 ? new object[method.Parameters.Length] : null;

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; ++idx)
                {
                    parameters[idx] = ObtainParameter(method.Parameters[idx]);
                    types[idx] = parameters[idx] != null ? parameters[idx].GetType() : typeof(object);
                }
            }

            MethodInfo info;
            
            if (types != null)
            {
                info = context.GetType().GetMethod(method.Name, types);
            }
            else
            {
                info = context.GetType().GetMethod(method.Name);
            }

            if (info != null)
            {
                return info.Invoke(context, parameters);
            }

            throw new Exception(String.Format("Cannot find method: {0}({1})", method.Name, parameters.ToString()));
        }

        public static T InvokeMethod<T>(object context, object definition)
        {
            object result = InvokeMethod(context, definition);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public static object GetValueFromMethodOrField(object context, object definition)
        {
            if ( definition is FieldName )
            {
                return GetFieldValue(context, definition);
            }

            return InvokeMethod(context, definition);
        }

        public static T GetValueFromMethodOrField<T>(object context, object definition)
        {
            object result = GetValueFromMethodOrField(context, definition);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        static object ObtainParameter(object parameter)
        {
            if ( parameter is ReflectionParameter)
            {
                return Parameter(((ReflectionParameter)parameter).Name);
            }

            return parameter;
        }

        public static string GetString(object context, object definition)
        {
            if (definition is string)
            {
                return definition as string;
            }

            object result = GetValueFromMethodOrField(context, definition);

            if (result is StringBuilder)
            {
                return (result as StringBuilder).ToString();
            }

            if (result is string)
            {
                return result as string;
            }

            return null;
        }

        public static StringBuilder GetStringBuilder(object context, object definition)
        {
            if ( definition is string )
            {
                return new StringBuilder((string)definition);
            }

            object result = GetValueFromMethodOrField(context, definition);

            if (result is StringBuilder)
            {
                return result as StringBuilder;
            }

            if (result is string)
            {
                return new StringBuilder((string)result);
            }

            return null;
        }

        public static bool GetBoolean(object context, object definition)
        {
            if (definition == null)
            {
                return false;
            }

            if (definition is bool)
            {
                return (bool)definition;
            }

            return GetValueFromMethodOrField<bool>(context, definition);
        }

        public static Color? GetColor(object context, object definition)
        {
            if (definition == null)
            {
                return null;
            }

            if (definition is Color)
            {
                return (Color)definition;
            }

            return GetValueFromMethodOrField<Color>(context, definition);
        }

        public static ColorWrapper GetColorWrapper(object context, object definition)
        {
            if ( definition == null )
            {
                return null;
            }

            if (definition is Color)
            {
                return new ColorWrapper((Color)definition);
            }

            return GetValueFromMethodOrField<ColorWrapper>(context, definition);
        }

        public static T Get<T>(object context, object definition)
        {
            if (definition == null)
            {
                return default(T);
            }

            if ( definition is T)
            {
                return (T)definition;
            }

            return GetValueFromMethodOrField<T>(context, definition);
        }
    }
}
