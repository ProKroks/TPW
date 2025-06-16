using System.Numerics;
using static Data.IDataBall;

namespace Data
{
    internal record LogBall
    {
        public int ID;
        public Vector2 Position;
        public Vector2 Velocity;
        public DateTime Time;
        public LogBall(Vector2 p, Vector2 v, DateTime t, int id)
        {
            Position = p;
            Velocity = v;
            Time = t;
            ID = id;
        }
    }
}