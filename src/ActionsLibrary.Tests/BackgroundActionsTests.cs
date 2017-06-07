using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ActionsLibrary.Tests
{
    [TestFixture]
    public class BackgroundActionsTests
    {
        [Test]
        public void CanExecuteSingleAction()
        {
            var actionWasExecuted = false;
            var actions = new BackgroundActions(new List<Action> { () => actionWasExecuted = true });

            actions.Execute();

            Assert.That(actionWasExecuted, Is.True, $"{nameof(actionWasExecuted)} should have been set to true if action was executed as expected");
        }

        [Test]
        public void CanExecuteMultipleActions()
        {
            var countOfActionsExecuted = 0;
            const int expectedExecutionCount = 5;
            var actionsList = Enumerable.Repeat<Action>(() => countOfActionsExecuted++, expectedExecutionCount);

            var actions = new BackgroundActions(actionsList);

            actions.Execute();

            Assert.That(countOfActionsExecuted, Is.EqualTo(expectedExecutionCount), $"{nameof(countOfActionsExecuted)} should have been {expectedExecutionCount}, but was {countOfActionsExecuted}");
        }

        [Test]
        public void ActionsAreExecutedInOrder()
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

            actions.Execute();

            const int expectedResult = 3;
            Assert.That(result, Is.EqualTo(expectedResult), $"{nameof(result)} should have been {expectedResult}, but was {result}");
        }
    }
}