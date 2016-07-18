using System;
using System.Collections.Generic;
using Sitana.Framework.Cs;
using System.Threading.Tasks;
using System.Threading;

namespace Sitana.Framework
{
    public static class UiTask
    {
        struct TaskOnUi
        {
            public EmptyArgsVoidDelegate function;
            public AutoResetEvent doneEvent;
        }

		static Queue<TaskOnUi> _tasks = new Queue<TaskOnUi>();

		static object _lock = new object();

        public static void BeginInvoke( EmptyArgsVoidDelegate lambda )
        {
			lock (_lock)
			{
				_tasks.Enqueue(new TaskOnUi()
                {
                    function = lambda,
                    doneEvent = null
                });
			}
        }

        public static async Task DoOnUiThread(EmptyArgsVoidDelegate lambda)
        {
            AutoResetEvent doneEvent = new AutoResetEvent(false);

            lock (_lock)
            {
                _tasks.Enqueue(new TaskOnUi()
                {
                    function = lambda,
                    doneEvent = doneEvent
                });
            }

            await Task.Run(() =>
               {
                   doneEvent.WaitOne();
               });
        }

        internal static void Process()
        {
			EmptyArgsVoidDelegate func = null;
            AutoResetEvent doneEvent = null;

            lock (_lock)
			{
				if (_tasks.Count > 0)
				{
                    TaskOnUi taskOnUi = _tasks.Dequeue();

                    func = taskOnUi.function;
                    doneEvent = taskOnUi.doneEvent;
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
                finally
                {
                    if (doneEvent != null)
                    {
                        doneEvent.Set();
                    }
                }
#else
                func.Invoke();

                if (doneEvent != null)
                {
                    doneEvent.Set();
                }
#endif



                    func = null;

				lock (_lock)
				{
					if (_tasks.Count > 0)
					{
                        TaskOnUi taskOnUi = _tasks.Dequeue();

                        func = taskOnUi.function;
                        doneEvent = taskOnUi.doneEvent;
                    }
				}
            }
        }
    }
}
