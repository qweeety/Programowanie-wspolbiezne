using Data;
using System;
using System.Collections.Generic;
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

        // ТА САМАЯ СЕКЦИЯ КРИТИЧЕСКАЯ
        private readonly object _lock = new object();

        // Список потоков, чтобы дождаться их завершения
        private readonly List<Task> _tasks = new();

        public LogicLayer(DataAbstractApi data) => _data = data;

        public override void CreateBalls(int count)
        {
            _balls.Clear();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                double r = rand.Next(10, 20); // Разный радиус
                double mass = r * r;          // Масса зависит от размера
                double x = rand.NextDouble() * (_data.Width - 2 * r);
                double y = rand.NextDouble() * (_data.Height - 2 * r);
                double vx = rand.NextDouble() * 4 - 2; // Скорость от -2 до 2
                double vy = rand.NextDouble() * 4 - 2;

                _balls.Add(_data.CreateBall(x, y, r, mass, vx, vy));
            }
        }

        public override void Start()
        {
            if (_moving) return;
            _moving = true;
            _tasks.Clear();

            // ЗАПУСКАЕМ ОТДЕЛЬНЫЙ ПОТОК ДЛЯ КАЖДОГО ШАРА
            foreach (var ball in _balls)
            {
                Task task = Task.Run(() => BallLoop(ball));
                _tasks.Add(task);
            }
        }

        public override void Stop()
        {
            _moving = false;
            Task.WaitAll(_tasks.ToArray()); // Ждем, пока все потоки остановятся
        }

        public override List<IBall> GetBalls() => _balls;

        // Жизненный цикл одного шара
        private void BallLoop(IBall ball)
        {
            while (_moving)
            {
                // БЛОКИРУЕМ доступ, чтобы другой шар в эту миллисекунду не сломал нам координаты
                lock (_lock)
                {
                    ball.Move();
                    CheckWallCollisions(ball);
                    CheckBallCollisions(ball);
                }
                Thread.Sleep(16); // Засыпаем на 16мс (~60 кадров в секунду)
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

                // Считаем центры шаров
                double c1X = ball.X + ball.Radius;
                double c1Y = ball.Y + ball.Radius;
                double c2X = other.X + other.Radius;
                double c2Y = other.Y + other.Radius;

                // Расстояние между центрами
                double dx = c2X - c1X;
                double dy = c2Y - c1Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // Если произошло столкновение (пересечение)
                if (distance <= ball.Radius + other.Radius)
                {
                    double nx = dx / distance;
                    double ny = dy / distance;

                    // ПРОТИВОЛИПКА: Считаем относительную скорость шаров
                    double relVx = ball.Vx - other.Vx;
                    double relVy = ball.Vy - other.Vy;

                    // Проверяем, двигаются ли они друг на друга (скалярное произведение векторов)
                    // Если результат больше 0, значит они сближаются. Если меньше - уже разлетаются!
                    if (relVx * nx + relVy * ny > 0)
                    {
                        // Формула упругого удара (Elastic collision)
                        double p = 2 * (ball.Vx * nx + ball.Vy * ny - other.Vx * nx - other.Vy * ny) / (ball.Mass + other.Mass);

                        ball.Vx = ball.Vx - p * other.Mass * nx;
                        ball.Vy = ball.Vy - p * other.Mass * ny;
                        other.Vx = other.Vx + p * ball.Mass * nx;
                        other.Vy = other.Vy + p * ball.Mass * ny;

                        // Принудительные сдвиги (Move) отсюда убираем, они больше не нужны,
                        // так как шары спокойно вылетят друг из друга на следующих тиках.
                    }
                }
            }
        }
    }
}