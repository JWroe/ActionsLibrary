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
            var actions = new BackgroundActions(() => actionWasExecuted = true);

            actions.Execute();

            Assert.That(actionWasExecuted, Is.True, $"{nameof(actionWasExecuted)} should have been set to true if action was executed as expected");
        }
    }
}