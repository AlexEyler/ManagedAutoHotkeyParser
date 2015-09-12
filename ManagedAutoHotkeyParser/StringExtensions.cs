using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace ManagedAutoHotkeyParser
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitByLine(this string text)
        {
            Requires.NotNull(text, nameof(text));
            return text.Split('\n', '\r');
        }
    }
}
