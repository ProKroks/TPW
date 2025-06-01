using System.Numerics;

namespace Data
{
    internal class DataBall : IDataBall
    {
        #region ctor
        public DataBall(int x, int y, int radius, int mass, int velocityX, int velocityY)
        {
            _position = new Vector2(x, y);
            Velocity = new Vector2(velocityX, velocityY);
            Radius = radius;
            Mass = mass;
            HasCollided = false;
            ContinueMoving = true;

            // uruchomienie wątku w tle
            _thread = new Thread(MoveLoop)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        public void Stop()
        {
            ContinueMoving = false;
            _thread?.Join(); 
        }
        #endregion ctor

        #region IDataBall
        public override event EventHandler<DataEventArgs>? ChangedPosition;

        public override Vector2 Velocity { get; set; }

        public override bool HasCollided { get; set; }

        // volatile daje visibility między wątkami
        private volatile bool _continueMoving;
        public override bool ContinueMoving
        {
            get => _continueMoving;
            set => _continueMoving = value;
        }

        public override Vector2 Position => _position;

        private readonly Thread _thread;
        #endregion IDataBall

        #region private
        private Vector2 _position;
        public int Radius { get; private set; }
        public int Mass { get; private set; }

        private void RaisePositionChangeNotification()
        {
            ChangedPosition?.Invoke(this, new DataEventArgs(this));
        }

        private void MoveLoop()
        {
            while (ContinueMoving)
            {
                Move();
                HasCollided = false; 
                Thread.Sleep(10);    
            }
        }

        private void Move()
        {
            _position = new Vector2(_position.X + Velocity.X, _position.Y + Velocity.Y);
            RaisePositionChangeNotification();
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetPosition(float x, float y)
        {
            _position = new Vector2(x, y);
            RaisePositionChangeNotification();
        }

        public void SetPosition(Vector2 newPosition)
        {
            _position = newPosition;
            RaisePositionChangeNotification();
        }
        #endregion private

        #region IDisposable
        public void Dispose()
        {
            Stop();
        }
        #endregion IDisposable
    }
}