using System;
using System.Collections.Generic;

namespace Data
{
    public abstract class DataAbstractApi
    {
        public abstract IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy);
        public abstract double Width { get; }
        public abstract double Height { get; }

        public abstract void Log(string message);
        public abstract void StopLogging();

        public static DataAbstractApi CreateApi(double width, double height)
            => new DataApi(width, height);
    }

    internal class DataApi : DataAbstractApi
    {
        public override double Width { get; }
        public override double Height { get; }

        private readonly Logger _logger;

        public DataApi(double width, double height)
        {
            Width = width;
            Height = height;
            _logger = new Logger();
        }

        public override IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy)
            => new Ball(x, y, radius, mass, vx, vy);

        public override void Log(string message) => _logger.Log(message);
        public override void StopLogging() => _logger.Stop();
    }
}