using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Graphics
{
    public struct StringProvider
    {
        private string _stringText;
        private StringBuilder _builderText;

        public StringProvider(string text)
        {
            _stringText = text;
            _builderText = null;
        }

        public StringProvider(StringBuilder text)
        {
            _builderText = text;
            _stringText = null;
        }

        public char this[int index]
        {
            get
            {
                return _stringText != null ? _stringText[index] : _builderText[index];
            }
        }

        public int Length
        {
            get
            {
                return _stringText != null ? _stringText.Length : (_builderText != null ? _builderText.Length : 0);
            }
        }
    }
}
