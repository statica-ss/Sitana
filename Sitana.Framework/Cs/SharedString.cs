// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Cs
{
    /// <summary>
    /// Synchronized StringBuilder equivalent.
    /// When using string content such as: Length, Character at index or StringBuilder reference,
    /// synchronize instance of this class with lock.
    /// </summary>
    public class SharedString
    {
        readonly StringBuilder _stringBuilder = new StringBuilder();

        // Remember to lock SharedString instance while using StringBuilder instance.
        public StringBuilder StringBuilder
        {
            get
            {
                return _stringBuilder;
            }
        }

        // Remember to lock SharedString instance when getting character.
        public char this[int index]
        {
            get
            {
                lock (this)
                {
                    return _stringBuilder[index];
                }
            }
        }

        // Remember to lock SharedString instance when getting length.
        public int Length
        {
            get
            {
                lock (this)
                {
                    return _stringBuilder.Length;
                }
            }
        }

        // Be aware! Getting string from SharedString will always create new instance of string.
        // Setting string to SharedString is as fast as Append in StringBuilder.
        public string StringValue
        {
            get
            {
                lock(this)
                {
                    return _stringBuilder.ToString();
                }
            }

            set
            {
                lock(this)
                {
                    _stringBuilder.Clear();
                    _stringBuilder.Append(value);
                }
            }
        }

        public SharedString()
        {
        }

        public SharedString(string value)
        {
            _stringBuilder.Append(value);
        }

        public void Format(string format, params object[] args)
        {
            // Usually lock(this) is a bad practice, but here it works as it should. 
            // User of this class can lock instance of this class and be sure that
            // internal StringBuilder's content is synchronized.
            lock (this)
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendFormat(format, args);
            }
        }

        public void Format(IFormatProvider formatProvider, string format, params object[] args)
        {
            // lock(this) - see explanation above.
            lock (this)
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendFormat(formatProvider, format, args);
            }
        }

        public void AppendFormat(string format, params object[] args)
        {
            // lock(this) - see explanation above.
            lock (this)
            {
                _stringBuilder.AppendFormat(format, args);
            }
        }

        public void AppendFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            // lock(this) - see explanation above.
            lock (this)
            {
                _stringBuilder.AppendFormat(formatProvider, format, args);
            }
        }

        public void Copy(StringBuilder other)
        {
            lock(this)
            {
                _stringBuilder.Clear();
                for(int idx = 0; idx < other.Length; ++idx)
                {
                    _stringBuilder.Append(other[idx]);
                }
            }
        }

        public void Append(string value)
        {
            // lock(this) - see explanation above.
            lock (this)
            {
                _stringBuilder.Append(value);
            }
        }

        public void Clear()
        {
            // lock(this) - see explanation above.
            lock (this)
            {
                _stringBuilder.Clear();
            }
        }

        public override string ToString()
        {
            lock (this)
            {
                return _stringBuilder.ToString();
            }
        }

        public override int GetHashCode()
        {
            lock (this)
            {
                return _stringBuilder.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if ( obj is SharedString )
            {
                return _stringBuilder.Equals(((SharedString)obj)._stringBuilder);
            }
            
            return _stringBuilder.Equals(obj);
        }

        public static implicit operator SharedString(string text)  // explicit byte to digit conversion operator
        {
            return new SharedString(text);
        }
    }
}
