/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Xml;
using System.Globalization;

namespace Ebatianos
{
    /// <summary>
    /// Reprezents collection of parameters indexed by string.
    /// </summary>
    public class ParametersCollection
    {
        private Dictionary<String, String> _parameters = new Dictionary<String, String>();
        private Object _valueSource = null;
        private Boolean _canMerge = false;

        public Int32 MethodCallIndexArgument { get; set; }

        private ColorsManager _colorsManager = null;

        public IEnumerable<String> Keys
        {
            get
            {
                return _parameters.Keys;
            }
        }

        public ParametersCollection(Boolean canMerge)
        {
            _canMerge = canMerge;
        }

        /// <summary>
        /// Creates parameters collection with set value source object.
        /// </summary>
        /// <param name="valueSource"></param>
        public ParametersCollection(Object valueSource, Boolean canMerge)
        {
            _valueSource = valueSource;
            _canMerge = canMerge;
        }

        /// <summary>
        /// Sets value source.
        /// </summary>
        public Object ValueSource
        {
            set
            {
                _valueSource = value;
            }

            get
            {
                return _valueSource;
            }
        }

        public ColorsManager ColorsManager
        {
            set
            {
                _colorsManager = value;
            }

            get
            {
                return _colorsManager;
            }
        }

        /// <summary>
        /// Adds parameter to collection.
        /// </summary>
        /// <param name="key">Key to identify parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        public void Add(String key, String value)
        {
            _parameters.Add(key, value);
        }

        /// <summary>
        /// Updates parameter's value in collection.
        /// </summary>
        /// <param name="key">Key to identify parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        public void Update(String key, String value)
        {
            if ( _parameters.ContainsKey(key) )
            {
                _parameters.Remove(key);
            }
            
            _parameters.Add(key, value);
        }

        

        /// <summary>
        /// Sums this collection with another one.
        /// </summary>
        /// <param name="other">Collection to sum its parameters with.</param>
        public void Add(ParametersCollection other)
        {
            if (!_canMerge)
            {
                throw new Exception("Cannot merge parameters with another container, because merging is locked for this collection.");
            }

            foreach (var param in other._parameters)
            {
                if (!_parameters.ContainsKey(param.Key))
                {
                    _parameters.Add(param.Key, param.Value);
                }
            }
        }

        public ParametersCollection Clone()
        {
            ParametersCollection collection = new ParametersCollection(true);
            collection.Add(this);
            collection._valueSource = _valueSource;

            return collection;
        }

        private String ParseFunctionAndArgumens(String key, out Object[] arguments)
        {
            String text = GetAt(key);

            arguments = null;

            if (text.StartsWith("[") && text.EndsWith("]"))
            {
                text = text.Substring(1, text.Length - 2);

                String[] funcAndArgs = text.Split(':');

                if (funcAndArgs.Length == 0)
                {
                    return null;
                }

                if (String.IsNullOrEmpty(funcAndArgs[0]))
                {
                    return null;
                }

                if ( funcAndArgs.Length > 2)
                {
                    for ( Int32 idx = 2; idx < funcAndArgs.Length; ++idx )
                    {
                        funcAndArgs[1] += ":" + funcAndArgs[idx];
                        funcAndArgs[idx] = null;
                    }
                }

                if (funcAndArgs.Length >= 2 && !String.IsNullOrEmpty(funcAndArgs[1]))
                {
                    String[] args = funcAndArgs[1].Split(',');
                    arguments = new Object[args.Length];

                    for ( Int32 idx = 0; idx < args.Length; ++idx )
                    {
                        if ( args[idx] == "$$" )
                        {
                            arguments[idx] = MethodCallIndexArgument;
                        }
                        else
                        {
                            Single floatVal;
                            Int32 intVal;
                            Boolean boolVal;

                            if (Int32.TryParse(args[idx], out intVal))
                            {
                                arguments[idx] = intVal;
                            }
                            else if ( Single.TryParse(args[idx], NumberStyles.Any, CultureInfo.InvariantCulture, out floatVal) )
                            {
                                arguments[idx] = floatVal;
                            }
                            else if ( Boolean.TryParse(args[idx], out boolVal))
                            {
                                arguments[idx] = boolVal;
                            }
                            else
                            {
                                arguments[idx] = args[idx];
                            }
                        }
                    }

                }

                return funcAndArgs[0];
            }

            return null;
        }

        private Boolean GetFromSource<T>(String key, out T value)
        {
            value = default(T);

            if (_valueSource != null)
            {
                Object[] args = null;
                String functionName = ParseFunctionAndArgumens(key, out args);

                if (functionName != null)
                {
                    try
                    {
                        Object retVal = _valueSource.GetType().InvokeMember(functionName, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, _valueSource, args);
                        value = (T)retVal;
                        return true;
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets parameter value at specified key.
        /// </summary>
        /// <param name="key">Parameter's key.</param>
        /// <returns>Value at specyfied key or empty string.</returns>
        public String GetAt(String key)
        {
            String value;

            if (_parameters.TryGetValue(key, out value))
            {
                return value;
            }

            return "";
        }

        /// <summary>
        /// Gets parameter as invokable action.
        /// </summary>
        /// <param name="key">Key of parameter.</param>
        /// <param name="sender">Sender of action invoke.</param>
        /// <returns>Action object.</returns>
        public Action AsAction(String key, Object sender)
        {
            if (_valueSource != null)
            {
                Object[] args = null;
                String funcName = ParseFunctionAndArgumens(key, out args);

                if (funcName != null)
                {
                    Object[] arguments = new Object[(args != null ? args.Length : 0) + 1];
                    arguments[0] = sender;

                    if (args != null)
                    {
                        for (Int32 idx = 0; idx < args.Length; ++idx)
                        {
                            arguments[idx + 1] = args[idx];
                        }
                    }

                    return () => _valueSource.GetType().InvokeMember(funcName, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, _valueSource, arguments);
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if collection contains key.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True is parameters with given key exists, False otherwise.</returns>
        public Boolean HasKey(String key)
        {
            return _parameters.ContainsKey(key);
        }


        public String AsString(String key)
        {
            String value = String.Empty;

            if (!GetFromSource<String>(key, out value))
            {
                value = GetAt(key);
            }

            return value;
        }

        public T AsObject<T>(String key)
        {
            T value = default(T);
            GetFromSource<T>(key, out value);
            return value;
        }

        public Int32 AsInt32(String key)
        {
            Int32 value = 0;

            if (!GetFromSource<Int32>(key, out value))
            {
                Int32.TryParse(GetAt(key), out value);
            }

            return value;
        }

        public Single AsSingle(String key)
        {
            Single value = 0;

            if (!GetFromSource<Single>(key, out value))
            {
                Single.TryParse(GetAt(key), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            }

            return value;
        }

        public Boolean AsBoolean(String key, Boolean defaultValue)
        {
            if ( HasKey(key) )
            {
                return AsBoolean(key);
            }

            return defaultValue;
        }

        public Boolean AsBoolean(String key)
        {
            Boolean value = false;

            if (!GetFromSource<Boolean>(key, out value))
            {
                String val = GetAt(key).ToLowerInvariant();
                value = (val == "yes" || val == "true");
            }

            return value;
        }

        public T AsEnum<T>(String key, T defaultValue)
        {
            T value = default(T);

            if (!GetFromSource<T>(key, out value))
            {
                try
                {
                    value = (T)Enum.Parse(typeof(T), GetAt(key), true);
                }
                catch (System.Exception)
                {
                    value = defaultValue;
                }
            }

            return value;
        }

        public Color? AsColorIfExists(String key, Boolean premultiplied)
        {
            Color value = Color.White;

            if (GetFromSource<Color>(key, out value))
            {
                return value;
            }

            String val = AsString(key);

            if (_colorsManager != null)
            {
                Color? colorIfExists = _colorsManager[val];

                if (colorIfExists.HasValue)
                {
                    return colorIfExists.Value;
                }
            }

            String[] rgb = val.Split(",".ToCharArray());

            if (rgb.Length == 3)
            {
                return new Color(Int32.Parse(rgb[0]), Int32.Parse(rgb[1]), Int32.Parse(rgb[2]));
            }

            if (rgb.Length == 4)
            {
                if (premultiplied)
                {
                    return new Color(Int32.Parse(rgb[1]), Int32.Parse(rgb[2]), Int32.Parse(rgb[3])) * ((Single)Int32.Parse(rgb[0]) / 255.0f);
                }
                else
                {
                    return new Color(Int32.Parse(rgb[1]), Int32.Parse(rgb[2]), Int32.Parse(rgb[3]), Int32.Parse(rgb[0]));
                }
            }

            return null;
        }

        public Color? AsColorIfExists(String key)
        {
            return AsColorIfExists(key, true);
        }

        public Object AsObject(String key)
        {
            Object value;

            if (GetFromSource<Object>(key, out value))
            {
                return value;
            }

            return null;
        }

        public Color AsColor(String key)
        {
            Color value = Color.White;

            if (GetFromSource<Color>(key, out value))
            {
                return value;
            }

            String val = AsString(key);

            if (_colorsManager != null)
            {
                Color? colorIfExists = _colorsManager[val];

                if (colorIfExists.HasValue)
                {
                    return colorIfExists.Value;
                }
            }

            String[] rgb = val.Split(",".ToCharArray());

            if (rgb.Length == 3)
            {
                return new Color(Int32.Parse(rgb[0]), Int32.Parse(rgb[1]), Int32.Parse(rgb[2]));
            }

            if (rgb.Length == 4)
            {
                return new Color(Int32.Parse(rgb[1]), Int32.Parse(rgb[2]), Int32.Parse(rgb[3])) * ((Single)Int32.Parse(rgb[0]) / 255.0f);
            }

            return Color.White;
        }

        public Align AsAlign(String horz, String vert, Align def)
        {
            Align al = def & (Align.Right | Align.Center);
            Align val = def & (Align.Middle | Align.Bottom);

            String align = GetAt(horz);
            String valign = GetAt(vert);

            valign = valign.ToLowerInvariant();

            if (valign.Contains("center"))
            {
                valign = valign.Replace("center", "middle");
            }

            Align placement = 0;

            if (align == "")
            {
                placement |= al;
            }

            if (valign == "")
            {
                placement |= val;
            }

            String newAlign = valign + ", " + align;
            newAlign = newAlign.Trim(',', ' ');

            Align result = Align.Top;

            if (Enum.TryParse<Align>(newAlign, true, out result))
            {
                placement |= result;
            }

            return placement;
        }

        public Align AsAlign(String horz, String vert)
        {
            return AsAlign(horz, vert, Align.Left | Align.Top);
        }

        public Rectangle ParseRectangle(String key)
        {
            Rectangle value = Rectangle.Empty;

            if (GetFromSource<Rectangle>(key, out value))
            {
                return value;
            }

            String val = GetAt(key);

            if (String.IsNullOrEmpty(val))
            {
                return Rectangle.Empty;
            }

            String[] coords = val.Split(',');

            if (coords.Length != 4)
            {
                throw new InvalidOperationException("Not enough values for rectangle.");
            }

            return new Rectangle(Int32.Parse(coords[0]), Int32.Parse(coords[1]), Int32.Parse(coords[2]), Int32.Parse(coords[3]));
        }

        public Point ParsePoint(String key)
        {
            Point value = Point.Zero;

            if (GetFromSource<Point>(key, out value))
            {
                return value;
            }

            if (GetFromSource<Int32>(key, out value.X))
            {
                return new Point(0, value.X);
            }

            String val = GetAt(key);

            if ( String.IsNullOrEmpty(val) )
            {
                return Point.Zero;
            }

            String[] coords = val.Split(',');
            
            if (coords.Length == 1)
            {
                return new Point(Int32.Parse(coords[0]), Int32.Parse(coords[0]));
            }
            else if (coords.Length == 2)
            {
                return new Point(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
            }

            return Point.Zero;
        }

        public void Remove(String id)
        {
            _parameters.Remove(id);
        }

        public ColorWrapper FindColorWrapper(String id, Boolean mustExist = true)
        {
            ColorWrapper wrapper = AsObject<ColorWrapper>(id);

            if (wrapper != null)
            {
                return wrapper;
            }

            Color? color = AsColorIfExists(id);

            if (mustExist && !color.HasValue)
            {
                return new ColorWrapper(Color.White);
            }

            return color.HasValue ? new ColorWrapper(color.Value) : null;
        }
    }
}
