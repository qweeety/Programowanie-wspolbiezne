using Xunit;
using Logic;
using System.Linq;

namespace LogicTests
{
    public class LogicTest
    {
        [Fact]
        public void CreateBalls_ShouldAddBalls()
        {
            var api = LogicAbstractApi.CreateApi();
            api.CreateBalls(3);
            Assert.Equal(3, api.GetBalls().Count);
        }

        [Fact]
        public void Start_ShouldMoveBalls()
        {
            var api = LogicAbstractApi.CreateApi();
            api.CreateBalls(1);
            var ball = api.GetBalls().First();

            Assert.True(ball.X >= 0 && ball.X <= 500);
        }
    }
}