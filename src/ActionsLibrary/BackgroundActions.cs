using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActionsLibrary
{
    public class BackgroundActions
    {
        private readonly object _executionLock = new object();

        private readonly ConcurrentQueue<Action> _actionsToExecute = new ConcurrentQueue<Action>();
        private readonly ConcurrentDictionary<Task, object> _executionTasks = new ConcurrentDictionary<Task, object>();

        private bool _shouldExecute;

        public BackgroundActions(Action action) => Add(action);

        public BackgroundActions(IEnumerable<Action> actionsList) => _actionsToExecute = new ConcurrentQueue<Action>(actionsList);

        public Task ActionExecution => Task.WhenAll(_executionTasks.Keys);

        public BackgroundActions Add(Action newAction)
        {
            _actionsToExecute.Enqueue(newAction);

            if (_shouldExecute)
            {
                ExecuteQueue();
            }

            return this;
        }

        public async Task BeginExecuting()
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
                while (_actionsToExecute.TryDequeue(out Action actionToExecute))
                {
                    actionToExecute();
                }
            }
        }
    }
}