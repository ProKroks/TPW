using Data;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    internal class Table : LogicAbstractAPI
    {
        private readonly int _length;
        private readonly int _width;
        private int _ballRadius;
        public List<ILogicBall> Balls { get; }
        private readonly IDataTable _dataAPI;
        private readonly Random _random = new Random();
        private readonly object _locker = new object();
        private readonly object _collisionLock = new object();

        private readonly DataLogger _logger = DataLogger.GetInstance();

        public Table(IDataTable api)
        {
            _length = api.Length;
            _width = api.Width;
            Balls = new List<ILogicBall>();
            _dataAPI = api;
        }

        public override void CreateBalls(int n, int r)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), "Number of balls cannot be negative");
            if (r <= 0)
                throw new ArgumentOutOfRangeException(nameof(r), "Ball radius must be positive");

            _ballRadius = r;

            for (int i = 0; i < n; i++)
            {
                int x = _random.Next(r, _length - r);
                int y = _random.Next(r, _width - r);
                int m = 1;

                int vX, vY;
                do { vX = _random.Next(-3, 4); } while (vX == 0);
                do { vY = _random.Next(-3, 4); } while (vY == 0);

                IDataBall dataBall = _dataAPI.CreateDataBall(x, y, r, m, vX, vY, i);
                LogicBall ball = new LogicBall(x, y);
                dataBall.ChangedPosition += ball.UpdateBall;
                dataBall.ChangedPosition += CheckCollisionWithWall;
                dataBall.ChangedPosition += CheckBallsCollision;
                Balls.Add(ball);

                _logger.AddBall(new LogBall(dataBall.Position, dataBall.Velocity, DateTime.UtcNow, dataBall.ID, "Ball Created"));
            }
        }

        private void CheckCollisionWithWall(Object s, DataEventArgs e)
        {
            IDataBall ball = e.Ball;
            if (ball.HasCollided) return;

            var predictedPositionX = ball.Position.X + ball.Velocity.X * ball.Time;
            var predictedPositionY = ball.Position.Y + ball.Velocity.Y * ball.Time;

            bool collision = false;

            if (predictedPositionX > _dataAPI.Length - _ballRadius || predictedPositionX < _ballRadius)
            {
                ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                collision = true;
            }
            if (predictedPositionY > _dataAPI.Width - _ballRadius || predictedPositionY < _ballRadius)
            {
                ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                collision = true;
            }

            if (collision)
            {
                ball.HasCollided = true;
                _logger.AddBall(new LogBall(ball.Position, ball.Velocity, DateTime.UtcNow, ball.ID, "Wall Collision"));
            }
        }

        private void CheckBallsCollision(object sender, DataEventArgs e)
        {
            IDataBall me = (IDataBall)sender;
            lock (_collisionLock)
            {
                if (!me.HasCollided)
                {
                    foreach (IDataBall other in _dataAPI.GetBalls().ToArray())
                    {
                        if (other != me)
                        {
                            double distance = Vector2.Distance(me.Position, other.Position);
                            if (distance <= _ballRadius + 0.01)
                            {
                                HandleBallCollision(me, other);
                            }
                        }
                    }
                }
            }
        }

        private void HandleBallCollision(IDataBall ball, IDataBall other)
        {
            float mass = 1f;

            var vx1 = (2 * mass * other.Velocity.X) / (2 * mass);
            var vx2 = (2 * mass * ball.Velocity.X) / (2 * mass);
            var vy1 = (2 * mass * other.Velocity.Y) / (2 * mass);
            var vy2 = (2 * mass * ball.Velocity.Y) / (2 * mass);

            ball.Velocity = new Vector2(vx1, vy1);
            other.Velocity = new Vector2(vx2, vy2);

            ball.HasCollided = true;
            other.HasCollided = true;

            _logger.AddBall(new LogBall(ball.Position, ball.Velocity, DateTime.UtcNow, ball.ID, $"Collision With Ball {other.ID}"));
            _logger.AddBall(new LogBall(other.Position, other.Velocity, DateTime.UtcNow, other.ID, $"Collision With Ball {ball.ID}"));
        }

        public override void ClearTable()
        {
            foreach (IDataBall ball in _dataAPI.GetBalls().ToArray())
            {
                ball.ContinueMoving = false;
            }
            Balls.Clear();
            _dataAPI.ClearTable();
        }

        public override List<ILogicBall> GetBalls()
        {
            return Balls;
        }
    }
}