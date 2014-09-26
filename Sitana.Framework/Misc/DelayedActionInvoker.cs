using System;
using System.Collections.Generic;
using Ebatianos.Cs;
namespace Ebatianos
{
    public class DelayedActionInvoker : Singleton<DelayedActionInvoker>
    {
        private struct DelayedAction
        {
            public Single Time;
            public Action<Single> Action;
        }

        private volatile Single _currentTime = 0;
        
        private Object _lock = new Object();

        private List<DelayedAction> _delayedActions = new List<DelayedAction>();
        private List<DelayedAction> _workingActions = new List<DelayedAction>();

        public Single CurrentTime
        {
            get
            {
                return _currentTime;
            }
        }

        public void AddAction(Single delay, Action<Single> action)
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

        public void Update(Single time)
        {
            _currentTime += time;
            _workingActions.Clear();
            lock (_lock)
            {
                for (Int32 idx = _delayedActions.Count - 1; idx >= 0; --idx)
                {
                    var action = _delayedActions[idx];
                    if (action.Time < _currentTime)
                    {
                        _workingActions.Add(action);
                        _delayedActions.RemoveAt(idx);
                    }
                }
            }
            for (Int32 idx = 0; idx < _workingActions.Count; ++idx)
            {
                var action = _workingActions[idx];
                action.Action.Invoke(_currentTime);
            }
            _workingActions.Clear();
        }
    }
}