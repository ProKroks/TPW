using System.Numerics;
using System.Threading.Tasks;

namespace Data
{
    internal class DataBall : IDataBall, IDisposable
    {
        public override event EventHandler<DataEventArgs>? ChangedPosition;

        private Vector2 _position;
        private Vector2 _velocity;
        private readonly object _locker = new object();
        private bool _continueMoving;
        private const float TIME_INTERVAL_SECONDS = 1f / 60f;
        private const int MOVEMENT_SPEED_MULTIPLIER = 65; 

        public override int ID { get; }
        public override float Time { get; set; }

        public override Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public override Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public override bool HasCollided { get; set; }
        public override bool ContinueMoving
        {
            get => _continueMoving;
            set => _continueMoving = value;
        }

        public int Radius { get; private set; }
        public int Mass { get; private set; }

        public DataBall(int x, int y, int radius, int mass, int velocityX, int velocityY, int id)
        {
            _position = new Vector2(x, y);
            _velocity = new Vector2(velocityX, velocityY);
            Radius = radius;
            Mass = mass;
            ID = id;
            HasCollided = false;
            _continueMoving = true;
            Task.Run(StartSimulation);
        }

        private async void StartSimulation()
        {
            while (_continueMoving)
            {
                Time = TIME_INTERVAL_SECONDS;
                MoveBall(TIME_INTERVAL_SECONDS);
                await Task.Delay((int)(TIME_INTERVAL_SECONDS * 1000));
            }
        }

        private void MoveBall(float elapsedTime)
        {
            lock (_locker)
            {
                HasCollided = false;
                _position += _velocity * elapsedTime * MOVEMENT_SPEED_MULTIPLIER;
                ChangedPosition?.Invoke(this, new DataEventArgs(this));
            }
        }

        public void Dispose()
        {
            _continueMoving = false;
        }
    }
}