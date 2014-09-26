using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Cs
{
    public class DelegatesMap
    {
        class DelegateInfo
        {
            public string Name;
            public Delegate Function;
            public object Target;
            public Type   DelegateType;
        }

        Dictionary<Enum, DelegateInfo> _delegates = new Dictionary<Enum, DelegateInfo>();

        public void RegisterDelegate(Enum id, Type delegateType, string name)
        {
            _delegates.Add(id, new DelegateInfo()
                {
                    Name = name,
                    Function = null,
                    Target = null,
                    DelegateType = delegateType
                });
        }

        public Delegate FindMethod(Enum id, object target)
        {
            DelegateInfo del;

            if ( _delegates.TryGetValue(id, out del) )
            {
                if ( del.Target != target )
                {
                    if (target == null)
                    {
                        del.Function = null;
                    }
                    else
                    {
                        try
                        {
                            del.Function = Delegate.CreateDelegate(del.DelegateType, target, del.Name);
                        }
                        catch
                        {
                            del.Function = null;
                        }
                    }

                    del.Target = target;
                }

                return del.Function;
            }

            return null;
        }
    }
}
