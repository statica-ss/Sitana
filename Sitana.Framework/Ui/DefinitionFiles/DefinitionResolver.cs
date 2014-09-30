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
using Microsoft.Xna.Framework.Graphics;

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

            object[] parameters = new object[method.Parameters.Length];

            
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                parameters[idx] = ObtainParameter(invokeParameters, method.Parameters[idx]);
            }

            MethodInfo[] infos = context.GetType().GetMethods();

            if (infos != null)
            {
                foreach (var info in infos)
                {
                    if (info.Name == method.Name)
                    {
                        ParameterInfo[] methodParams = info.GetParameters();

                        if (parameters.Length == methodParams.Length)
                        {
                            return info.Invoke(context, parameters);
                        }
                    }
                }
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

        public static T Get<T>(UiController controller, object binding, object definition, T defaultValue)
        {
            if (definition == null)
            {
                return defaultValue;
            }

            if ( definition is T)
            {
                return (T)definition;
            }

            if (typeof(T) == typeof(NinePatchImage) || typeof(T) == typeof(Texture2D))
            {
                if (definition is string)
                {
                    return ContentLoader.Current.Load<T>(definition as string);
                }
            }

            return GetValueFromMethodOrField<T>(controller, binding, definition);
        }
    }
}
