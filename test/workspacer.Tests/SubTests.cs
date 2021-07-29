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

        [Fact]
        public void LeftCtrl_RightCtrl_And_Ctrl_Has_Same_HashCode()
        {
            TestHashEquality(KeyModifiers.LControl, KeyModifiers.RControl, KeyModifiers.Control);
        }

        [Fact]
        public void RightAlt_LeftAlt_And_Alt_Has_Same_HashCode()
        {
            TestHashEquality(KeyModifiers.LAlt, KeyModifiers.RAlt, KeyModifiers.Alt);
        }

        [Fact]
        public void RightWin_LeftWin_And_WinHas_Same_HashCode()
        {
            TestHashEquality(KeyModifiers.LWin, KeyModifiers.RWin, KeyModifiers.Win);
        }

        private void TestHashEquality(KeyModifiers lKey, KeyModifiers rKey, KeyModifiers bothKey)
        {
            var left = new Sub(lKey, Keys.D1).GetHashCode();
            var right = new Sub(rKey, Keys.D1).GetHashCode();
            var both = new Sub(bothKey, Keys.D1).GetHashCode();

            Assert.Equal(left, both);
            Assert.Equal(left, right);
        }

        [Fact]
        public void Does_Not_Have_Hash_Collisions_Between_Different_Keys()
        {
            var subHash1 = new Sub(KeyModifiers.None, Keys.D2).GetHashCode();
            var subHash2 = new Sub(KeyModifiers.Alt, Keys.D1).GetHashCode();

            Assert.NotEqual(subHash1, subHash2);
        }
    }
}
