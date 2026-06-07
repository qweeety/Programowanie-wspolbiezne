using Xunit;
using Logic;
using Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicTests
{
    internal class FakeDataApi : DataAbstractApi
    {
        public override double Width => 500;
        public override double Height => 400;
        private List<IBall> _balls = new();

        public override IBall CreateBall(double x, double y, double radius, double mass, double vx, double vy)
        {
            return DataAbstractApi.CreateApi(Width, Height).CreateBall(x, y, radius, mass, vx, vy);
        }
    }

    public class LogicTest
    {
        [Fact]
        public void CreateBalls_ShouldAddBalls()
        {
            var fakeData = new FakeDataApi();
            var api = LogicAbstractApi.CreateApi(fakeData);

            api.CreateBalls(3);

            Assert.Equal(3, api.GetBalls().Count);
        }

        [Fact]
        public async Task Start_ShouldMoveBalls_WithinBounds()
        {
            var fakeData = new FakeDataApi();
            var api = LogicAbstractApi.CreateApi(fakeData);
            api.CreateBalls(1);
            var ball = api.GetBalls().First();
            double initialX = ball.X;

            api.Start();
            await Task.Delay(50);
            api.Stop();

            Assert.NotEqual(initialX, ball.X);
            Assert.True(ball.X >= 0 && ball.X <= fakeData.Width);
        }
    }
}