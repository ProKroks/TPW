using Data;
using Logic;
using System.Numerics;

namespace LogicTest
{
    internal class FakeDataBall : IDataBall
    {
        private Vector2 _position;
        private Vector2 _velocity;

        public override Vector2 Position { get => _position; }
        public override Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public override bool HasCollided { get; set; }
        public override bool ContinueMoving { get; set; }

        public override event EventHandler<DataEventArgs> ChangedPosition;

        public FakeDataBall(Vector2 position, Vector2 velocity)
        {
            _position = position;
            _velocity = velocity;
            HasCollided = false;
            ContinueMoving = true;
        }

        public void SimulatePositionChange()
        {
            ChangedPosition?.Invoke(this, new DataEventArgs(this));
        }
    }

    internal class FakeDataAPI : IDataTable
    {
        private List<IDataBall> _balls = new List<IDataBall>();

        public FakeDataAPI(int length, int width)
        {
            Length = length;
            Width = width;
        }

        public override int Length { get; }
        public override int Width { get; }

        public override IDataBall CreateDataBall(int x, int y, int r, int m, int vX, int vY)
        {
            var ball = new FakeDataBall(new Vector2(x, y), new Vector2(vX, vY));
            _balls.Add(ball);
            return ball;
        }

        public override List<IDataBall> GetBalls()
        {
            return _balls;
        }

        public override void ClearTable()
        {
            _balls.Clear();
        }
    }

    public class Tests
    {
        private IDataTable _data;
        private LogicAbstractAPI _table;

        [SetUp]
        public void Init()
        {
            _data = new FakeDataAPI(500, 500);
            _table = LogicAbstractAPI.CreateAPI(_data);
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.IsNotNull(_table);
        }

        [Test]
        public void CreateBallsTest()
        {
            _table.CreateBalls(3, 5);
            Assert.AreEqual(3, _table.GetBalls().Count);
        }

        [Test]
        public void ClearTableTest()
        {
            _table.CreateBalls(3, 5);
            Assert.AreEqual(3, _table.GetBalls().Count);

            _table.ClearTable();
            Assert.AreEqual(0, _table.GetBalls().Count);
        }

        [Test]
        public void ClearingEmptyTableTest()
        {
            _table.ClearTable();
            Assert.AreEqual(0, _table.GetBalls().Count);
        }

        [Test]
        public void BallVelocityInitializationTest()
        {
            _table.CreateBalls(1, 5);
            var dataBall = _data.GetBalls()[0] as FakeDataBall;
            Assert.AreNotEqual(Vector2.Zero, dataBall.Velocity);
        }

        [Test]
        public void MultipleBallsCreationTest()
        {
            _table.CreateBalls(10, 5);
            Assert.AreEqual(10, _table.GetBalls().Count);
            Assert.AreEqual(10, _data.GetBalls().Count);
        }

        [Test]
        public void BallPositionWithinBoundsTest()
        {
            int radius = 10;
            _table.CreateBalls(1, radius);
            var logicBall = _table.GetBalls()[0];

            Assert.GreaterOrEqual(logicBall.Position.X, radius);
            Assert.LessOrEqual(logicBall.Position.X, _data.Length - radius);
            Assert.GreaterOrEqual(logicBall.Position.Y, radius);
            Assert.LessOrEqual(logicBall.Position.Y, _data.Width - radius);
        }

        [Test]
        public void TableDimensionsTest()
        {
            var customData = new FakeDataAPI(600, 400);
            var customTable = LogicAbstractAPI.CreateAPI(customData);

            customTable.CreateBalls(1, 10);
            var ball = customTable.GetBalls()[0];

            Assert.GreaterOrEqual(ball.Position.X, 10);
            Assert.LessOrEqual(ball.Position.X, 590);
            Assert.GreaterOrEqual(ball.Position.Y, 10);
            Assert.LessOrEqual(ball.Position.Y, 390);
        }

        [Test]
        public void ZeroBallsCreationTest()
        {
            _table.CreateBalls(0, 5);
            Assert.AreEqual(0, _table.GetBalls().Count);
        }

        [Test]
        public void NegativeBallsCreationTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _table.CreateBalls(-1, 5));
        }
    }
}