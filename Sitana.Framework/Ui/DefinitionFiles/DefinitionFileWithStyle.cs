using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public struct DefinitionFileWithStyle
    {
        DefinitionFile _current;
        DefinitionFile _style;

        public DefinitionFileWithStyle(DefinitionFile current, Type targetType)
        {
            _current = current;

            string name = current["Style"] as string;
            _style = StylesManager.Instance.FindStyle(name, targetType);
        }

        public object this[string id]
        {
            get
            {
                if (_current.HasKey(id))
                {
                    object value = _current[id];

                    if (value != null)
                    {
                        return value;
                    }
                }

                if (_style != null)
                {
                    return _style[id];
                }

                return null;
            }
        }
    }
}
