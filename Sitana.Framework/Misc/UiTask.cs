using System;
using System.Collections.Generic;
using Sitana.Framework.Cs;

namespace Sitana.Framework
{
    public static class UiTask
    {
        static List<Tuple<long, EmptyArgsVoidDelegate>> _tasks = new List<Tuple<long, EmptyArgsVoidDelegate>>();

		static object _lock = new object();

        public static void BeginInvoke( EmptyArgsVoidDelegate lambda )
        {
			lock (_lock)
			{
				_tasks.Add(new Tuple<long, EmptyArgsVoidDelegate>(0, lambda));
			}
        }

        public static void BeginInvoke(double delay, EmptyArgsVoidDelegate lambda)
        {
			lock (_lock)
			{
				_tasks.Add(new Tuple<long, EmptyArgsVoidDelegate>(
					DateTime.Now.AddSeconds(delay).Ticks,
					lambda));
			}
        }

        internal static void Process()
        {
            long ticks = 0;
            if (_tasks.Count > 0)
            {
                ticks = DateTime.Now.Ticks;
            }

            for (int idx = 0; idx < _tasks.Count; )
            {
                var pair = _tasks[idx];
                if (pair.Item1 <= ticks)
                {
					EmptyArgsVoidDelegate func = pair.Item2;

					if (func == null)
					{
						throw new NullReferenceException("Delegate.");
					}

					try
					{
                    	func.Invoke();
					}
					catch(Exception ex)
					{
						throw new Exception(string.Format("UiTask invoke exception at {0}.{1}", func.Method.DeclaringType.FullName, func.Method.Name), ex);
					}

                    _tasks.RemoveAt(idx);
                }
                else
                {
                    ++idx;
                }
            }
        }
    }
}
