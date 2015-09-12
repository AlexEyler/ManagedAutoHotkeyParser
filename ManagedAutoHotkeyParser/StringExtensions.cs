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

        public static string RemoveAllWhiteSpace(this string text)
        {
            Requires.NotNull(text, nameof(text));
            return text.RemoveAll(char.IsWhiteSpace);
        }

        public static string RemoveAll(this string text, Predicate<char> shouldRemovePredicate)
        {
            Requires.NotNull(text, nameof(text));
            Requires.NotNull(shouldRemovePredicate, nameof(shouldRemovePredicate));

            char[] textArr = text.Where(c => !shouldRemovePredicate(c)).ToArray();
            return new string(textArr);
        }
    }
}
