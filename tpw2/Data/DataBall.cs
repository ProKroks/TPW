using System.Numerics;

namespace Data
{
    internal class DataBall : IDataBall
    {
        public override event EventHandler<DataEventArgs>? ChangedPosition;

        private Vector2 _position;
        private Vector2 _velocity;

        public override Vector2 Position
        {
            get
            {
                lock (_positionLock)
                {
                    return _position;
                }
            }
        }

        public override Vector2 Velocity
        {
            get
            {
                lock (_velocityLock)
                {
                    return _velocity;
                }
            }
            set
            {
                lock (_velocityLock)
                {
                    _velocity = value;
                }
            }
        }

        public override bool HasCollided { get; set; }
        public override bool ContinueMoving { get; set; }

        private readonly object _generalLock = new();
        private readonly object _positionLock = new();
        private readonly object _velocityLock = new();

        public DataBall(int x, int y, int radius, int mass, int velocityX, int velocityY)
        {
            _position = new Vector2(x, y);
            Velocity = new Vector2(velocityX, velocityY);
            ContinueMoving = true;
            HasCollided = false;

            
            Task.Run(StartSimulation);
        }

       
        private async void StartSimulation()
        {
            while (ContinueMoving)
            {
                MoveBall();
                HasCollided = false;
                await Task.Delay(10);
            }
        }

        private void MoveBall()
        {
            lock (_generalLock)
            {
                _position += Velocity;
                ChangedPosition?.Invoke(this, new DataEventArgs(this));
            }
        }
    }
}
