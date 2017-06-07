using System;
using System.Collections.Generic;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly Queue<Action> _actions = new Queue<Action>();
        private bool _executionStarted;
        private bool _currentlyExecuting;

        public BackgroundActions(Action action) => AddAction(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actions = new Queue<Action>(actionsList);

        public BackgroundActions AddAction(Action newAction)
        {
            _actions.Enqueue(newAction);
            if (_executionStarted && !_currentlyExecuting)
            {
                ExecuteQueue();
            }
            return this;
        }

        public void StartExecution()
        {
            _executionStarted = true;
            ExecuteQueue();
        }

        private void ExecuteQueue()
        {
            _currentlyExecuting = true;
            while (_actions.Count > 0)
            {
                var actionToExecute = _actions.Dequeue();
                actionToExecute();
            }
            _currentlyExecuting = false;
        }
    }
}