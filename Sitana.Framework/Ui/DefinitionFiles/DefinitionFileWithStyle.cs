using System;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Definitions;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public struct DefinitionFileWithStyle
    {
        DefinitionFile _current;
        DefinitionFile[] _style;

        public DefinitionFileWithStyle(DefinitionFile current, Type targetType)
        {
            _current = current;

            string name = current["Style"] as string;

            string[] styles = name.Replace(" ", "").Split(',');

            _style = new DefinitionFile[styles.Length];

            for (int idx = 0; idx < styles.Length; ++idx)
            {
                _style[idx] = StylesManager.Instance.FindStyle(styles[idx], targetType);
            }
        }

        public object this[string id]
        {
            get
            {
                IMixable mixable = null;

                if (_current.HasKey(id))
                {
                    object value = _current[id];

                    if(value is IMixable)
                    {
                        mixable = value as IMixable;

                         if(!mixable.IsMixMeaningful)
                         {
                             return mixable;
                         }
                    }
                    else if (value != null)
                    {
                        return value;
                    }
                }

                if (_style != null)
                {
                    for (int idx = 0; idx < _style.Length; ++idx)
                    {
                        if ( _style[idx] != null )
                        {
                            object value = _style[idx][id];

                            if (value is IMixable)
                            {
                                if (mixable == null)
                                {
                                    mixable = value as IMixable;
                                }
                                else
                                {
                                    mixable = mixable.Mix(value as IMixable);
                                }

                                if(!mixable.IsMixMeaningful)
                                {
                                    return mixable;
                                }
                            }
                            else if (value != null)
                            {
                                return value;
                            }
                        }
                    }
                }

                return mixable;
            }
        }
    }
}
