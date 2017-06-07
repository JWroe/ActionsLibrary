using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
        private bool _shouldExecute;
        private readonly object _executionLock = new object();
        private readonly ConcurrentDictionary<Task, object> _executionTasks = new ConcurrentDictionary<Task, object>();

        public BackgroundActions(Action action) => AddAction(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actions = new ConcurrentQueue<Action>(actionsList);

        public Task ActionExecution => Task.WhenAll(_executionTasks.Keys);

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
            await ActionExecution;
        }

        private void ExecuteQueueAsync() => AddExecutionTask(Task.Run(() => ExecuteQueue()));

        private void AddExecutionTask(Task task)
        {
            _executionTasks.TryAdd(task, null);
            task.ContinueWith(t => _executionTasks.TryRemove(t, out object val));
        }

        private void ExecuteQueue()
        {
            lock (_executionLock)
            {
                while (_actions.TryDequeue(out Action actionToExecute))
                {
                    actionToExecute();
                }
            }
        }
    }
}