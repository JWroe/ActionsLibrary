using System;
using System.Collections.Generic;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly Queue<Action> _actions = new Queue<Action>();

        public BackgroundActions(Action action) => AddAction(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actions = new Queue<Action>(actionsList);

        public BackgroundActions AddAction(Action newAction)
        {
            _actions.Enqueue(newAction);
            return this;
        }

        public void Execute()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }
    }
}