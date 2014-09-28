using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Content;
using System.Reflection;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public static class DefinitionResolver
    {
        public static object GetFieldValue(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            FieldName field = (FieldName)definition;

            object context = field.Binding ? binding : controller;

            PropertyInfo info = context.GetType().GetProperty(field.Name);

            int[] indices = null;

            if (field.Parameters != null)
            {
                indices = new int[field.Parameters.Length];

                for (int idx = 0; idx < indices.Length; ++idx)
                {
                    indices[idx] = (int)ObtainParameter(invokeParameters, field.Parameters[idx]);
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

        public static object InvokeMethod(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            MethodName method = (MethodName)definition;

            object context = method.Binding ? binding : controller;

            Type[] types = method.Parameters.Length > 0 ? new Type[method.Parameters.Length] : null;
            object[] parameters = method.Parameters.Length > 0 ? new object[method.Parameters.Length] : null;

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; ++idx)
                {
                    parameters[idx] = ObtainParameter(invokeParameters, method.Parameters[idx]);
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

        public static T InvokeMethod<T>(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            object result = InvokeMethod(controller, binding, definition, invokeParameters);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public static object GetValueFromMethodOrField(UiController controller, object binding, object definition)
        {
            if ( definition is FieldName )
            {
                return GetFieldValue(controller, binding, definition, null);
            }

            return InvokeMethod(controller, binding, definition, null);
        }

        public static T GetValueFromMethodOrField<T>(UiController controller, object binding, object definition)
        {
            object result = GetValueFromMethodOrField(controller, binding, definition);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        static object ObtainParameter(InvokeParameters invokeParameters, object parameter)
        {
            if ( parameter is ReflectionParameter)
            {
                ReflectionParameter rp = (ReflectionParameter)parameter;
                return invokeParameters[rp.Name];
            }

            return parameter;
        }

        public static string GetString(UiController controller, object binding, object definition)
        {
            if (definition is string)
            {
                return definition as string;
            }

            object result = GetValueFromMethodOrField(controller, binding, definition);

            if ((result is StringBuilder) || (result is SharedString))
            {
                return result.ToString();
            }

            if (result is string)
            {
                return result as string;
            }

            return null;
        }

        public static SharedString GetSharedString(UiController controller, object binding, object definition)
        {
            if ( definition is string )
            {
                return new SharedString((string)definition);
            }

            object result = GetValueFromMethodOrField(controller, binding, definition);

            if (result is SharedString)
            {
                return result as SharedString;
            }

            if (result is string)
            {
                return new SharedString((string)result);
            }

            return null;
        }

        public static bool GetBoolean(UiController controller, object binding, object definition)
        {
            if (definition == null)
            {
                return false;
            }

            if (definition is bool)
            {
                return (bool)definition;
            }

            return GetValueFromMethodOrField<bool>(controller, binding, definition);
        }

        public static Color? GetColor(UiController controller, object binding, object definition)
        {
            if (definition == null)
            {
                return null;
            }

            if (definition is Color)
            {
                return (Color)definition;
            }

            return GetValueFromMethodOrField<Color>(controller, binding, definition);
        }

        public static ColorWrapper GetColorWrapper(UiController controller, object binding, object definition)
        {
            if ( definition == null )
            {
                return null;
            }

            if (definition is Color)
            {
                return new ColorWrapper((Color)definition);
            }

            return GetValueFromMethodOrField<ColorWrapper>(controller, binding, definition);
        }

        public static T Get<T>(UiController controller, object binding, object definition)
        {
            if (definition == null)
            {
                return default(T);
            }

            if ( definition is T)
            {
                return (T)definition;
            }

            return GetValueFromMethodOrField<T>(controller, binding, definition);
        }
    }
}
