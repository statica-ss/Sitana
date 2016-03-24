using System;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public static class DefinitionResolver
    {
        public static object GetFieldValue(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            FieldName field = (FieldName)definition;

            object context = field.Binding ? binding : controller;

            if (field.Name == "*")
            {
                return context;
            }

            string name = field.Name;

            if (name.StartsWith(":"))
            {
                name = name.Substring(1);
                controller = controller.Parent;
            }

			int[] indices = null;

			if (field.Parameters != null)
			{
				indices = new int[field.Parameters.Length];

				for (int idx = 0; idx < indices.Length; ++idx)
				{
					indices[idx] = (int)ObtainParameter(invokeParameters, field.Parameters[idx]);
				}
			}

            PropertyInfo info = context.GetType().GetProperty(name);

			if (info != null)
			{
				object value = info.GetValue(context, null);

				if (indices != null)
				{
					if (value is Array)
					{
						return (value as Array).GetValue(indices);
					}

					throw new Exception(String.Format("Value is not an array: {0}[{1}]", name, indices.ToString()));
				} else
				{
					return value;
				}
			}
			else
			{
                FieldInfo finfo = context.GetType().GetField(name);

				if (finfo != null)
				{
					object value = finfo.GetValue(context);

					if (indices != null)
					{
						if (value is Array)
						{
							return (value as Array).GetValue(indices);
						}

						throw new Exception(String.Format("Value is not an array: {0}[{1}]", name, indices.ToString()));
					}
					else
					{
						return value;
					}
				}
			}

            throw new Exception(String.Format("Cannot find field: {0}{1}", field.Name, indices != null ? '['+indices.ToString()+']' : ""));
        }

        public static object InvokeMethod(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            MethodName method = (MethodName)definition;

            string name = method.Name;

            if (name.StartsWith(":"))
            {
                name = name.Substring(1);
                controller = controller.Parent;
            }

            object context = method.Binding ? binding : controller;

            object[] parameters = new object[method.Parameters.Length];

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                parameters[idx] = ObtainParameter(invokeParameters, method.Parameters[idx]);
            }
            
            foreach (var info in context.GetType().GetMethods())
            {
                if (info.Name == name)
                {
                    ParameterInfo[] methodParams = info.GetParameters();

                    if (parameters.Length == methodParams.Length)
                    {
                        return info.Invoke(context, parameters); 
                    }
                }
            }

            throw new Exception(String.Format("Cannot find method: {0}({1})", name, parameters.ToString()));
        }

        public static T InvokeMethod<T>(UiController controller, object binding, object definition, InvokeParameters invokeParameters)
        {
            object result = InvokeMethod(controller, binding, definition, invokeParameters);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public static object GetValueFromMethodOrField(UiController controller, object binding, object definition)
        {
            if(definition is GlobalVariable)
            {
                return GlobalVariables.Instance[(definition as GlobalVariable).Name];
            }

            if ( definition is FieldName )
            {
                return GetFieldValue(controller, binding, definition, null);
            }

            return InvokeMethod(controller, binding, definition, null);
        }

        public static T GetValueFromMethodOrField<T>(UiController controller, object binding, object definition)
        {
            object result = GetValueFromMethodOrField(controller, binding, definition);

            if(result is T)
            {
                return (T)result;
            }

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
            if (definition == null)
            {
                return null;
            }

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
            if (definition == null)
            {
                return null;
            }

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

            if (definition is ColorWrapper)
            {
                return ((ColorWrapper)definition).Value;
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

            if (definition is ColorWrapper)
            {
                return ((ColorWrapper)definition);
            }

            object obj = GetValueFromMethodOrField(controller, binding, definition);

            if(obj is Color)
            {
                return new ColorWrapper((Color)obj);
            }

            if(obj is ColorWrapper)
            {
                return obj as ColorWrapper;
            }

            return null;
        }

        public static SharedValue<T> GetShared<T>(UiController controller, object binding, object definition, T defaultValue)
        {
            if (definition == null)
            {
                return new SharedValue<T>(defaultValue);
            }

            if (definition is T)
            {
                return new SharedValue<T>((T)definition);
            }

            if (typeof(T) == typeof(NinePatchImage) || typeof(T) == typeof(Texture2D) || typeof(T) == typeof(SoundEffect) || typeof(T) == typeof(PartialTexture2D))
            {
                if (definition is string)
                {
                    return new SharedValue<T>(
                        ContentLoader.Current.Load<T>(definition as string));
                }
                else if (definition is MethodName || definition is FieldName || definition is GlobalVariable)
                {
                    object value = GetValueFromMethodOrField(controller, binding, definition);

                    if (value is string)
                    {
                        return new SharedValue<T>(ContentLoader.Current.Load<T>(value as string));
                    }

                    if (value == null)
                    {
                        return new SharedValue<T>(defaultValue);
                    }
                }
            }

            if (definition is MethodName || definition is FieldName || definition is GlobalVariable)
            {
                object value = GetValueFromMethodOrField(controller, binding, definition);

                if (value is T)
                {
                    return new SharedValue<T>((T)value);
                }

                if (value is SharedValue<T>)
                {
                    return (SharedValue<T>)value;
                }
            }

            throw new Exception("Unable to get shared value for type.");
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

            if (typeof(T) == typeof(NinePatchImage) || typeof(T) == typeof(Texture2D) || typeof(T) == typeof(SoundEffect) || typeof(T) == typeof(PartialTexture2D))
            {
                if (definition is string)
                {
                    return ContentLoader.Current.Load<T>(definition as string);
                }
                else
                {
                    object value = GetValueFromMethodOrField(controller, binding, definition);

                    if(value is string)
                    {
                        return ContentLoader.Current.Load<T>(value as string);
                    }
                    else if(value is T)
                    {
                        return (T)value;
                    }

                    return default(T);
                }
            }

            return GetValueFromMethodOrField<T>(controller, binding, definition);
        }
    }
}
