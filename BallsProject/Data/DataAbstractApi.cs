using System;
using System.Collections.Generic;

namespace Data
{
    public abstract class DataAbstractApi
    {
        // Обновили сигнатуру создания
        public abstract IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy);
        public abstract double Width { get; }
        public abstract double Height { get; }

        public static DataAbstractApi CreateApi(double width, double height)
            => new DataApi(width, height);
    }

    internal class DataApi : DataAbstractApi
    {
        public override double Width { get; }
        public override double Height { get; }

        public DataApi(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy)
            => new Ball(x, y, radius, mass, vx, vy);
    }
}