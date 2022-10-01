using System;
using System.Linq;

namespace workspacer
{
    public static class StringExtensions
    {
        public static string Capitalize(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => "",
                _ => input.First().ToString().ToUpper() + input[1..]
            };
    }
}
