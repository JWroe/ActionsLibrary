using System;
using System.Collections.Generic;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly IEnumerable<Action> _actions;

        public BackgroundActions(IEnumerable<Action> actions)
        {
            _actions = actions;
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