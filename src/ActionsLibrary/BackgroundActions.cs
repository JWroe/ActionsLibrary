using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private bool _shouldExecute;
        private readonly object _executionLock = new object();

        public BackgroundActions(Action action) => AddAction(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actions = new ConcurrentQueue<Action>(actionsList);

        public BackgroundActions AddAction(Action newAction)
        {
            _actions.Enqueue(newAction);

            if (_shouldExecute)
            {
                ExecuteQueue();
            }

            return this;
        }

        public void StartExecution()
        {
            _shouldExecute = true;
            ExecuteQueue();
        }

        private void ExecuteQueue()
        {
            lock (_executionLock)
            {
                Action actionToExecute;
                while (_actions.TryDequeue(out actionToExecute))
                {
                    actionToExecute();
                }
            }
        }
    }
}