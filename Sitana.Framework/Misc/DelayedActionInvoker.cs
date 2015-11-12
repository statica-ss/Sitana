using System;
using System.Collections.Generic;
using Sitana.Framework.Cs;
namespace Sitana.Framework
{
    public class DelayedActionInvoker : Singleton<DelayedActionInvoker>
    {
        private struct DelayedAction
        {
            public float Time;
            public Action<float> Action;
        }

        private volatile float _currentTime = 0;
        
        private Object _lock = new Object();

        private List<DelayedAction> _delayedActions = new List<DelayedAction>();
        private List<DelayedAction> _workingActions = new List<DelayedAction>();

        public float CurrentTime
        {
            get
            {
                return _currentTime;
            }
        }

        public void AddAction(float delay, Action<float> action)
        {
            lock (_lock)
            {
                _delayedActions.Add(new DelayedAction()
                {
                    Time = delay + _currentTime,
                    Action = action
                });
            }
        }

        public void Update(float time)
        {
            _currentTime += time;
            _workingActions.Clear();
            lock (_lock)
            {
                for (int idx = _delayedActions.Count - 1; idx >= 0; --idx)
                {
                    var action = _delayedActions[idx];
                    if (action.Time < _currentTime)
                    {
                        _workingActions.Add(action);
                        _delayedActions.RemoveAt(idx);
                    }
                }
            }
            for (int idx = 0; idx < _workingActions.Count; ++idx)
            {
                var action = _workingActions[idx];

				try
				{
                	action.Action.Invoke(_currentTime);
				}
				catch(Exception ex)
				{
					var func = action.Action;

#if !WINDOWS_PHONE_APP
					throw new Exception(string.Format("DelayedActionInvoker invoke exception at {0}.{1}", func.Target.GetType().FullName, func.Method.Name), ex);
#else
                    throw ex;
#endif
				}
            }
            _workingActions.Clear();
        }
    }
}