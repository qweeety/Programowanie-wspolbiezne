using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Logic
{
    public abstract class LogicAbstractApi
    {
        public abstract void CreateBalls(int count);
        public abstract void Start();
        public abstract void Stop();
        public abstract List<IBall> GetBalls();

        public static LogicAbstractApi CreateApi(DataAbstractApi? dataApi = null)
            => new LogicLayer(dataApi ?? DataAbstractApi.CreateApi(500, 400));
    }

    internal class LogicLayer : LogicAbstractApi
    {
        private readonly DataAbstractApi _data;
        private readonly List<IBall> _balls = new();
        private bool _moving = false;

        private readonly object _lock = new object();

        private readonly List<Task> _tasks = new();
        private Task? _loggerTask;

        public LogicLayer(DataAbstractApi data) => _data = data;

        public override void CreateBalls(int count)
        {
            _balls.Clear();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                double r = rand.Next(10, 20);
                double mass = r * r;
                double x = rand.NextDouble() * (_data.Width - 2 * r);
                double y = rand.NextDouble() * (_data.Height - 2 * r);
                double vx = rand.NextDouble() * 4 - 2;
                double vy = rand.NextDouble() * 4 - 2;

                _balls.Add(_data.CreateBall(x, y, r, mass, vx, vy));
            }
        }

        public override void Start()
        {
            if (_moving) return;
            _moving = true;
            _tasks.Clear();

            foreach (var ball in _balls)
            {
                Task task = Task.Run(() => BallLoop(ball));
                _tasks.Add(task);
            }

            _loggerTask = Task.Run(DiagnosticLoop);
        }

        public override void Stop()
        {
            _moving = false;
            Task.WaitAll(_tasks.ToArray());
            _loggerTask?.Wait();
            _data.StopLogging();
        }

        public override List<IBall> GetBalls() => _balls;

        private void DiagnosticLoop()
        {
            while (_moving)
            {
                List<object> snapshot = new List<object>();

                lock (_lock)
                {
                    foreach (var ball in _balls)
                    {
                        snapshot.Add(new
                        {
                            Time = DateTime.Now.ToString("HH:mm:ss.fff"),
                            ball.X,
                            ball.Y,
                            ball.Vx,
                            ball.Vy
                        });
                    }
                }

                string json = JsonSerializer.Serialize(snapshot);
                _data.Log(json);

                Thread.Sleep(100);
            }
        }

        private void BallLoop(IBall ball)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (_moving)
            {
                double delta = sw.ElapsedMilliseconds / 16.0;
                if (delta == 0) delta = 0.1;
                sw.Restart();

                lock (_lock)
                {
                    ball.Move(delta);
                    CheckWallCollisions(ball);
                    CheckBallCollisions(ball);
                }
                Thread.Sleep(16);
            }
        }

        private void CheckWallCollisions(IBall ball)
        {
            if (ball.X <= 0 || ball.X >= _data.Width - ball.Radius * 2) ball.Vx *= -1;
            if (ball.Y <= 0 || ball.Y >= _data.Height - ball.Radius * 2) ball.Vy *= -1;
        }

        private void CheckBallCollisions(IBall ball)
        {
            foreach (var other in _balls)
            {
                if (ball == other) continue;

                double c1X = ball.X + ball.Radius;
                double c1Y = ball.Y + ball.Radius;
                double c2X = other.X + other.Radius;
                double c2Y = other.Y + other.Radius;

                double dx = c2X - c1X;
                double dy = c2Y - c1Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= ball.Radius + other.Radius)
                {
                    double nx = dx / distance;
                    double ny = dy / distance;

                    double relVx = ball.Vx - other.Vx;
                    double relVy = ball.Vy - other.Vy;

                    if (relVx * nx + relVy * ny > 0)
                    {
                        double p = 2 * (ball.Vx * nx + ball.Vy * ny - other.Vx * nx - other.Vy * ny) / (ball.Mass + other.Mass);

                        ball.Vx = ball.Vx - p * other.Mass * nx;
                        ball.Vy = ball.Vy - p * other.Mass * ny;
                        other.Vx = other.Vx + p * ball.Mass * nx;
                        other.Vy = other.Vy + p * ball.Mass * ny;
                    }
                }
            }
        }
    }
}