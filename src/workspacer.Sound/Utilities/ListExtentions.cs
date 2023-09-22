using System.Collections.Generic;

namespace workspacer.Sound
{
    public static class ListExtentions
    {
        public static T GetItemFromValueSet<T>(this IList<T> set, int value, int max, int min)
        {
            var fragmentSize = max / (set.Count - 1);
            var index = value / fragmentSize;
            return set[index];
        }
    }
}
