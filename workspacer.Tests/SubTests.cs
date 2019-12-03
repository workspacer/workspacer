using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace workspacer.Tests
{
    public class SubTests
    {
        [Fact]
        public void LeftCtrl_Does_Not_Equal_RightControl()
        {
            TestEquality(KeyModifiers.LControl, KeyModifiers.RControl, KeyModifiers.Control);
        }

        [Fact]
        public void RightAlt_Does_Not_Equal_LeftAlt()
        {
            TestEquality(KeyModifiers.LAlt, KeyModifiers.RAlt, KeyModifiers.Alt);
        }

        [Fact]
        public void RightWin_Does_Not_Equal_LeftWin()
        {
            TestEquality(KeyModifiers.LWin, KeyModifiers.RWin, KeyModifiers.Win);
        }

        private void TestEquality(KeyModifiers lKey, KeyModifiers rKey, KeyModifiers bothKey)
        {
            var left = new Sub(lKey, Keys.D1);
            var right = new Sub(rKey, Keys.D1);
            var both = new Sub(bothKey, Keys.D1);

            Assert.NotEqual(left, right);
            Assert.Equal(left, both);
            Assert.Equal(right, both);
        }
    }
}
