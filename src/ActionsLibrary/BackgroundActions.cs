using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private bool _shouldExecute;
        private readonly object _executionLock = new object();
        private HashSet<Task> _tasks = new HashSet<Task>();

        public BackgroundActions(Action action) => AddAction(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actions = new ConcurrentQueue<Action>(actionsList);

        public Task QueueExecution => Task.WhenAll(_tasks);

        public BackgroundActions AddAction(Action newAction)
        {
            _actions.Enqueue(newAction);

            if (_shouldExecute)
            {
                ExecuteQueue();
            }

            return this;
        }

        public async Task StartExecution()
        {
            _shouldExecute = true;
            ExecuteQueueAsync();
            await QueueExecution;
        }

        private void ExecuteQueueAsync() => _tasks.Add(Task.Run(() => ExecuteQueue()));

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