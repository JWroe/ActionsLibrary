using System;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly Action _action;

        public BackgroundActions(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }
}