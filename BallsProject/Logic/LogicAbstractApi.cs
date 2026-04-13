using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public abstract class LogicAbstractApi
    {
        public abstract void CreateBalls(int count);
        public abstract void Start();
        public abstract void Stop();
        public abstract List<IBall> GetBalls();

        public static LogicAbstractApi CreateApi(DataAbstractApi dataApi = null)
            => new LogicLayer(dataApi ?? DataAbstractApi.CreateApi(500, 400));
    }

    internal class LogicLayer : LogicAbstractApi
    {
        private readonly DataAbstractApi _data;
        private readonly List<IBall> _balls = new();
        private readonly List<(double vx, double vy)> _velocities = new();
        private bool _moving = false;

        public LogicLayer(DataAbstractApi data) => _data = data;

        public override void CreateBalls(int count)
        {
            _balls.Clear();
            _velocities.Clear();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                double r = 10;
                double x = rand.NextDouble() * (_data.Width - 2 * r);
                double y = rand.NextDouble() * (_data.Height - 2 * r);
                _balls.Add(_data.CreateBall(x, y, r));
                _velocities.Add((rand.NextDouble() * 2 + 1, rand.NextDouble() * 2 + 1));
            }
        }

        public override async void Start()
        {
            _moving = true;
            while (_moving)
            {
                for (int i = 0; i < _balls.Count; i++)
                {
                    var ball = _balls[i];
                    var (vx, vy) = _velocities[i];

                    double newX = ball.X + vx;
                    double newY = ball.Y + vy;

                    if (newX <= 0 || newX >= _data.Width - ball.Radius * 2) vx *= -1;
                    if (newY <= 0 || newY >= _data.Height - ball.Radius * 2) vy *= -1;

                    _velocities[i] = (vx, vy);
                    ball.Move(newX, newY);
                }
                await Task.Delay(16); 
            }
        }

        public override void Stop() => _moving = false;
        public override List<IBall> GetBalls() => _balls;
    }
}