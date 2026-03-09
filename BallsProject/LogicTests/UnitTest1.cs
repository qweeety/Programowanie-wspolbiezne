using Xunit;

namespace LogicTests
{
    public class SimpleTest
    {
        [Fact]
        public void TestEnvironment()
        {
            int a = 2;
            int b = 2;
            Assert.Equal(4, a + b);
        }
    }
}