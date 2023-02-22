using System;
using Xunit;

namespace workspacer.Shared.Tests
{
    public class ColorTests
    {
        [Fact]
        public void Creating_Color_With_Valid_RGB_Components_Succeeds()
        {
            var black = new Color(0, 0, 0);
            var white = new Color(0xFF, 0xFF, 0xFF);
        }

        [Fact]
        public void Creating_Color_With_Invalid_RGB_Components_Throws()
        {
            var sets = new[]
            {
                // negative values
                new[] { -1,  0, 0 },
                new[] {  0, -1, 0 },
                new[] {  0,  0,-1 },

                // beyond 8-bit
                new[] {  256,  0,   0 },
                new[] {  0,  256,   0 },
                new[] {  0,    0, 256 },
            };
            foreach (var set in sets)
            {
                var r = set[0];
                var g = set[1];
                var b = set[2];

                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var result = new Color(r, g, b);
                });
            }
        }

        [Fact]
        public void Different_Colors_Does_Not_Have_Hash_Collision()
        {
            var hash1 = new Color(0, 0, 1).GetHashCode();
            var hash2 = new Color(0, 7, 0).GetHashCode();

            Assert.NotEqual(hash1, hash2);
        }
    }
}