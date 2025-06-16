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
            IDataBall ball = _table.CreateDataBall(10, 20, 5, 1, 2, 3, 1);
            Assert.IsNotNull(ball);
        }

        [Test]
        public void BallVelocity_ShouldBeSetCorrectly()
        {
            IDataBall ball = _table.CreateDataBall(0, 0, 3, 1, 5, -3, 1);
            Vector2 expected = new Vector2(5, -3);
            Assert.AreEqual(expected, ball.Velocity);
        }

        [Test]
        public void ClearTable_ShouldRemoveAllBalls()
        {
            _table.CreateDataBall(0, 0, 3, 1, 1, 1, 1);
            _table.CreateDataBall(100, 100, 3, 1, 2, 2, 2);
            Assert.AreEqual(2, _table.GetBalls().Count);

            _table.ClearTable();
            Assert.AreEqual(0, _table.GetBalls().Count);
        }

        [Test]
        public async Task BallPosition_ShouldChangeOverTime()
        {
            IDataBall ball = _table.CreateDataBall(0, 0, 3, 1, 1, 0, 1);
            Vector2 initialPosition = ball.Position;

            bool changed = false;
            for (int i = 0; i < 50; i++)
            {
                await Task.Delay(10);
                if (ball.Position != initialPosition)
                {
                    changed = true;
                    break;
                }
            }
            Assert.IsTrue(changed, "Pozycja pi³ki nie zmieni³a siê w oczekiwanym czasie.");
        }

        [Test]
        public async Task BallEvents_ShouldTriggerOnMove()
        {
            var ball = _table.CreateDataBall(0, 0, 3, 1, 1, 0, 1);

            var tcs = new TaskCompletionSource();
            ball.ChangedPosition += (sender, args) =>
            {
                tcs.TrySetResult();
            };

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(500));
            Assert.IsTrue(tcs.Task.IsCompleted, "Zdarzenie ChangedPosition nie zosta³o wywo³ane.");
        }
    }
}
