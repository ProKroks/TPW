using Data;
using System.Numerics;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DataTests
{
    public class DataTests
    {
        private IDataTable _table;

        [SetUp]
        public void Setup()
        {
            _table = IDataTable.CreateAPI();
        }

        [Test]
        public void CreateBall_ShouldReturnNotNull()
        {
            IDataBall ball = _table.CreateDataBall(10, 20, 5, 1, 2, 3);
            Assert.IsNotNull(ball);
        }

        [Test]
        public void BallVelocity_ShouldBeSetCorrectly()
        {
            IDataBall ball = _table.CreateDataBall(0, 0, 3, 1, 5, -3);
            Vector2 expected = new Vector2(5, -3);
            Assert.AreEqual(expected, ball.Velocity);
        }

        [Test]
        public void ClearTable_ShouldRemoveAllBalls()
        {
            _table.CreateDataBall(0, 0, 3, 1, 1, 1);
            _table.CreateDataBall(100, 100, 3, 1, 2, 2);
            Assert.AreEqual(2, _table.GetBalls().Count);

            _table.ClearTable();
            Assert.AreEqual(0, _table.GetBalls().Count);
        }

        [Test]
        public async Task BallPosition_ShouldChangeOverTime()
        {
            IDataBall ball = _table.CreateDataBall(0, 0, 3, 1, 1, 0);
            Vector2 initialPosition = ball.Position;

            await Task.Delay(50); 
            Vector2 newPosition = ball.Position;

            Assert.AreNotEqual(initialPosition, newPosition);
        }

        [Test]
        public void BallEvents_ShouldTriggerOnMove()
        {
            var ball = _table.CreateDataBall(0, 0, 3, 1, 1, 0);

            bool eventFired = false;
            ball.ChangedPosition += (sender, args) =>
            {
                eventFired = true;
            };

            Task.Delay(50).Wait();
            Assert.IsTrue(eventFired);
        }
    }
}
