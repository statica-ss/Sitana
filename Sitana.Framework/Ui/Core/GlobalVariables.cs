using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Ui.Core
{
    public class GlobalVariables : Singleton<GlobalVariables>
    {
        Dictionary<string, object> _variables = new Dictionary<string, object>();

        public object this[string id]
        {
            get
            {
                return _variables[id];
            }
        }

        public T Get<T>(string id)
        {
            return (T)(_variables[id]);
        }

        public void SetShared<T>(string id, T value)
        {
            object variable;

            if (_variables.TryGetValue(id, out variable))
            {
                SharedValue<T> shared = variable as SharedValue<T>;

                if (shared != null)
                {
                    shared.Value = value;
                }
                else
                {
                    throw new Exception(string.Format("Variable already has type and it is different than SharedValue<{0}>.", typeof(T).Name));
                }
            }
            else
            {
                _variables.Add(id, new SharedValue<T>(value));
            }
        }

        public void SetValue<T>(string id, T value)
        {
            object variable;

            if (_variables.TryGetValue(id, out variable))
            {
                if (variable is T)
                {
                    _variables.Remove(id);
                    _variables.Add(id, value);
                }
                else
                {
                    throw new Exception(string.Format("Variable already has type and it is different than {0}.", typeof(T).Name));
                }
            }
            else
            {
                _variables.Add(id, value);
            }
        }
    }
}
