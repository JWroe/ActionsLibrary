using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly IImmutableList<Action> _actions;

        public BackgroundActions(Action action) : this(ImmutableList.Create(action))
        {
        }

        public BackgroundActions(IEnumerable<Action> actions) : this(actions.ToImmutableList())
        {
        }

        private BackgroundActions(IImmutableList<Action> actions) => _actions = actions;

        public BackgroundActions AddAction(Action newAction) => new BackgroundActions(_actions.Add(newAction));

        public void Execute()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }
    }
}