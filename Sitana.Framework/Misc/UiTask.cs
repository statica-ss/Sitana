using System;
using System.Collections.Generic;
using Sitana.Framework.Cs;

namespace Sitana.Framework
{
    public static class UiTask
    {
		static Queue<EmptyArgsVoidDelegate> _tasks = new Queue<EmptyArgsVoidDelegate>();

		static object _lock = new object();

        public static void BeginInvoke( EmptyArgsVoidDelegate lambda )
        {
			lock (_lock)
			{
				_tasks.Enqueue(lambda);
			}
        }

        internal static void Process()
        {
			EmptyArgsVoidDelegate func = null;

			lock (_lock)
			{
				if (_tasks.Count > 0)
				{
					func = _tasks.Dequeue();
				}
			}

			while(func != null)
			{
#if !DEBUG
				try
				{
                	func.Invoke();
				}
				catch(Exception ex)
				{
#if !WINDOWS_PHONE_APP && !WINDOWS_UWP
					throw new Exception(string.Format("UiTask invoke exception at {0}.{1}", func.Method.DeclaringType.FullName, func.Method.Name), ex);
#else
                    throw ex;
#endif
				}
#else
                func.Invoke();
#endif

				func = null;

				lock (_lock)
				{
					if (_tasks.Count > 0)
					{
						func = _tasks.Dequeue();
					}
				}
            }
        }
    }
}
