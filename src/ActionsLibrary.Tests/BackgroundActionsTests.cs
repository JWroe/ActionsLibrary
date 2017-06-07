using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ActionsLibrary.Tests
{
    [TestFixture]
    public class BackgroundActionsTests
    {
        [Test]
        public async Task CanExecuteSingleAction()
        {
            var actionWasExecuted = false;
            var actions = new BackgroundActions(() => actionWasExecuted = true);

            await actions.BeginExecuting();

            Assert.That(actionWasExecuted, Is.True, $"{nameof(actionWasExecuted)} should have been set to true if action was executed as expected");
        }

        [Test]
        public async Task CanExecuteMultipleActions()
        {
            var countOfActionsExecuted = 0;
            const int expectedExecutionCount = 5;
            var actionsList = Enumerable.Repeat<Action>(() => countOfActionsExecuted++, expectedExecutionCount);

            var actions = new BackgroundActions(actionsList);

            await actions.BeginExecuting();

            Assert.That(countOfActionsExecuted, Is.EqualTo(expectedExecutionCount), $"{nameof(countOfActionsExecuted)} should have been {expectedExecutionCount}, but was {countOfActionsExecuted}");
        }

        [Test]
        public async Task ActionsAreExecutedInOrder()
        {
            var result = 0;

            var actionsList = new List<Action>
                              {
                                  () => result += 5,
                                  () => result -= 4,
                                  () => result *= 6,
                                  () => result /= 2,
                              };

            var actions = new BackgroundActions(actionsList);

            await actions.BeginExecuting();

            const int expectedResult = 3;
            Assert.That(result, Is.EqualTo(expectedResult), $"{nameof(result)} should have been {expectedResult}, but was {result}");
        }

        [Test]
        public async Task ActionsCanBeAddedAfterObjectCreation()
        {
            var result = 0;

            await new BackgroundActions(() => result += 5)
                .Add(() => result -= 4)
                .Add(() => result *= 6)
                .Add(() => result /= 2)
                .BeginExecuting();

            const int expectedResult = 3;
            Assert.That(result, Is.EqualTo(expectedResult), $"{nameof(result)} should have been {expectedResult}, but was {result}");
        }

        [Test]
        public async Task ActionsAddedAfterExecutionStartsAreStillExecutedAsync()
        {
            var result = 0;

            var actions = new BackgroundActions(() => result += 5);
            actions.BeginExecuting();

            actions.Add(() => result -= 4)
                   .Add(() => result *= 6)
                   .Add(() => result /= 2);

            await actions.ActionExecution;

            const int expectedResult = 3;
            Assert.That(result, Is.EqualTo(expectedResult), $"{nameof(result)} should have been {expectedResult}, but was {result}");
        }

        [Test]
        public void MultipleThreadsCanAddActions()
        {
            var result = 0;

            var actions = new BackgroundActions(() => result += 10);
            actions.BeginExecuting();

            var taskOne = Task.Run(() =>
                                   {
                                       for (var i = 0; i < 10000; i++)
                                       {
                                           actions.Add(() => result += i);
                                       }
                                   });

            var taskTwo = Task.Run(() =>
                                   {
                                       for (var i = 0; i < 10000; i++)
                                       {
                                           actions.Add(() => result -= i);
                                       }
                                   });
            Task.WaitAll(taskOne, taskTwo, actions.ActionExecution);

            const int expectedResult = 10;
            Assert.That(result, Is.EqualTo(expectedResult), $"{nameof(result)} should have been {expectedResult}, but was {result}");
        }

        [Test]
        public void BackgroundActionsExecutesOnABackgroundThread()
        {
            var watch = Stopwatch.StartNew();
            var waitTime = TimeSpan.FromSeconds(1);
            var actions = new BackgroundActions(() => Thread.Sleep(waitTime));
            actions.BeginExecuting();

            Assert.That(watch.Elapsed, Is.LessThan(waitTime), $"Elapsed time should have been less than {nameof(waitTime)} ({waitTime}), but was {watch.Elapsed}. Is the call blocking on execution?");
        }
    }
}