﻿namespace Data
{
    internal class DataTable : IDataTable
    {
        public override int Length { get; }
        public override int Width { get; }

        private readonly List<IDataBall> _balls = new();

        public DataTable(int length, int width)
        {
            Length = length;
            Width = width;
        }

        public override List<IDataBall> GetBalls()
        {
            return _balls;
        }

        public override void ClearTable()
        {
            _balls.Clear();
        }

        public override IDataBall CreateDataBall(int x, int y, int r, int m, int vX, int vY, int id)
        {
            IDataBall ballData = IDataBall.CreateDataBall(x, y, r, m, vX, vY, id);
            _balls.Add(ballData);
            return ballData;
        }
    }
}
