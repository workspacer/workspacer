using Xunit;

namespace workspacer.Shared.Tests
{
    public class FocusLayoutTest
    {
        [Theory]
        [InlineData(5, 2, 2, 1)]
        [InlineData(6, 2, 2, 2)]
        [InlineData(5, 1, 2, 2)]
        [InlineData(4, 1, 2, 1)]
        [InlineData(2, 1, 1, 0)]
        [InlineData(1, 1, 0, 0)]
        [InlineData(2, 2, 0, 0)]
        [InlineData(3, 2, 1, 0)]
        public void CheckNumberWindows(int numWindows, int numInPrimary, int expectedLeft, int expectedRight)
        {
            FocusLayoutEngine focusLayoutEngine = new FocusLayoutEngine();
            Assert.Equal(expectedLeft, focusLayoutEngine.GetNbLeftWindows(numWindows, numInPrimary));
            Assert.Equal(expectedRight, focusLayoutEngine.GetNbRightWindows(numWindows, numInPrimary));
        }
    }
}
