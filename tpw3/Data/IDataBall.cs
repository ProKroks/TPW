using System.Numerics;

namespace Data
{
    public abstract class IDataBall
    {
        public abstract int ID { get; }
        public abstract float Time { get; set; }
        public abstract Vector2 Position { get; set; }
        public abstract Vector2 Velocity { get; set; }
        public abstract bool HasCollided { get; set; }
        public abstract bool ContinueMoving { get; set; }

        public abstract event EventHandler<DataEventArgs> ChangedPosition;

        public static IDataBall CreateDataBall(int x, int y, int r, int m, int vX, int vY, int id)
        {
            return new DataBall(x, y, r, m, vX, vY, id);
        }
    }
}