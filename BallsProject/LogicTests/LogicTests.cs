using NUnit.Framework;
using Logic;
using Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace Logic.Tests
{
    internal class FakeBall : IBall
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }
        public double Diameter => Radius * 2;
        public double Mass { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Move(double delta)
        {
            X += Vx * delta;
            Y += Vy * delta;
        }
    }

    internal class FakeDataApi : DataAbstractApi
    {
        public override double Width => 500;
        public override double Height => 400;

        public override IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy)
        {
            return new FakeBall { X = x, Y = y, Radius = radius, Mass = mass, Vx = vx, Vy = vy };
        }

        public override void Log(string message) { }
        public override void StopLogging() { }
    }

    [TestFixture]
    public class LogicLayerTests
    {
        [Test]
        public void TestCreateBalls()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbstractApi.CreateApi(fakeData);

            logic.CreateBalls(5);
            var balls = logic.GetBalls();

            NUnit.Framework.Assert.AreEqual(5, balls.Count);
        }

        [Test]
        public void TestBallsMovementStatus()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbstractApi.CreateApi(fakeData);

            logic.CreateBalls(1);
            var balls = logic.GetBalls();

            double initialX = balls[0].X;
            double initialY = balls[0].Y;

            logic.Start();
            System.Threading.Thread.Sleep(100);
            logic.Stop();

            NUnit.Framework.Assert.AreNotEqual(initialX, balls[0].X);
            NUnit.Framework.Assert.AreNotEqual(initialY, balls[0].Y);
        }
    }
}