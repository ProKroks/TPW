using System.Numerics;

namespace Data
{
    public record LogBall
    {
        public int ID;
        public Vector2 Position;
        public Vector2 Velocity;
        public DateTime Time;
        public string? Event { get; }

        public LogBall(Vector2 p, Vector2 v, DateTime t, int id, string? ev = null)
        {
            Position = p;
            Velocity = v;
            Time = t;
            ID = id;
            Event = ev;
        }
    }
}