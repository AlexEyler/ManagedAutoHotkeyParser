using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace ManagedAutoHotkeyParser
{
    [Flags]
    public enum HotkeySymbolModifiers
    {
        None = 0,
        Win = 1,
        Alt = 2,
        Ctrl = 4,
        Shift = 8,
        AltGr = 16,
        Left = 32,
        Right = 64,
        Wildcard = 128,
        DontBlock = 256,
        UseHook = 512,
        Up = 1024
    }

    public class HotkeyExpression : Expression, IEquatable<HotkeyExpression>
    {
        public const string HotkeyToken = "::";

        public static readonly IReadOnlyDictionary<string, HotkeySymbolModifiers> ModifierMap = new Dictionary<string, HotkeySymbolModifiers>()
        {
            { "#", HotkeySymbolModifiers.Win },
            { "!", HotkeySymbolModifiers.Alt },
            { "^", HotkeySymbolModifiers.Ctrl },
            { "+", HotkeySymbolModifiers.Shift },
            { "<", HotkeySymbolModifiers.Left },
            { ">", HotkeySymbolModifiers.Right },
            // { "<^>!", HotkeySymbolModifiers.AltGr },    // currently unsupported
            { "*", HotkeySymbolModifiers.Wildcard },
            { "~", HotkeySymbolModifiers.DontBlock },
            { "$", HotkeySymbolModifiers.UseHook },
            // { "Up", HotkeySymbolModifiers.Up }          // currently unsupported
        };

        public HotkeyExpression(string text)
        {
            this.Text = text;
        }

        public override string Text { get; }

        // set is internal for testing
        public HotkeySymbolModifiers Modifiers1 { get; internal set; }

        // set is internal for testing
        public string Key1 { get; internal set; }

        // set is internal for testing
        public HotkeySymbolModifiers Modifiers2 { get; internal set; }

        // set is internal for testing
        public string Key2 { get; internal set; }

        public static bool TryParse(string text, out Expression expr)
        {
            Requires.NotNull(text, nameof(text));
            expr = null;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            string textWithoutWhiteSpace = text.RemoveAllWhiteSpace();
            int hotkeyIndex = textWithoutWhiteSpace.LastIndexOf(HotkeyExpression.HotkeyToken);

            if (hotkeyIndex < 1) // no empty hotkey
            {
                return false;
            }

            int ampersandIndex = textWithoutWhiteSpace.IndexOf('&');

            var hkExpr = new HotkeyExpression(text);
            if (ampersandIndex < 0)
            {
                // Only one hotkey in sequence
                string key;
                HotkeySymbolModifiers modifiers;
                if (!TryParseHalf(textWithoutWhiteSpace, hotkeyIndex, out key, out modifiers))
                {
                    return false;
                }

                hkExpr.Key1 = key;
                hkExpr.Modifiers1 = modifiers;
            }
            else
            {
                string[] hotkeys = textWithoutWhiteSpace.Split('&');
                if (hotkeys.Length != 2)
                {
                    // Only support two hotkey sequences
                    return false;
                }

                string key1;
                HotkeySymbolModifiers modifiers1;
                if (!TryParseHalf(hotkeys[0], hotkeys[0].Length, out key1, out modifiers1))
                {
                    return false;
                }

                hkExpr.Key1 = key1;
                hkExpr.Modifiers1 = modifiers1;

                string key2;
                HotkeySymbolModifiers modifiers2;
                // We subtract the hotkey token length from total length to get ignore the token
                if (!TryParseHalf(hotkeys[1], hotkeys[1].Length - HotkeyExpression.HotkeyToken.Length, out key2, out modifiers2))
                {
                    return false;
                }

                hkExpr.Key2 = key2;
                hkExpr.Modifiers2 = modifiers2;
            }

            expr = hkExpr;
            return true;
        }

        private static bool TryParseHalf(string text, int maxIndex, out string key, out HotkeySymbolModifiers modifiers)
        {
            key = null;
            modifiers = HotkeySymbolModifiers.None;

            for (int i = 0; i < maxIndex; i++)
            {
                string delim = text[i].ToString();
                HotkeySymbolModifiers modifier;
                if (ModifierMap.TryGetValue(delim, out modifier))
                {
                    if (modifier == HotkeySymbolModifiers.Left || modifier == HotkeySymbolModifiers.Right)
                    {
                        // Left/Right modifiers need to be followed by another modifier
                        if (i < maxIndex - 1)
                        {
                            string nextDelim = text[i + 1].ToString();
                            HotkeySymbolModifiers nextModifier;
                            if (ModifierMap.TryGetValue(nextDelim, out nextModifier))
                            {
                                if (nextModifier != HotkeySymbolModifiers.Left || nextModifier != HotkeySymbolModifiers.Right)
                                {
                                    // This is actually valid, let's move on (and skip the next modifier since we're handling it here.
                                    modifiers |= modifier | nextModifier;
                                    i++;
                                    continue;
                                }

                                // The next modifier is another left/right. This is invalid.
                                return false;
                            }
                            else
                            {
                                // The next character isn't a modifier. This is invalid.
                                return false;
                            }
                        }
                        else
                        {
                            // The last character in the hotkey sequence is a left/right modifier. This is invalid.
                            return false;
                        }
                    }

                    modifiers |= modifier;
                }
                else
                {
                    // Must be the character. If we've already found one, it's invalid.
                    if (!string.IsNullOrEmpty(key))
                    {
                        return false;
                    }

                    key = delim;
                }
            }

            return true;
        }

        public bool Equals(HotkeyExpression other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            return other.Modifiers1 == this.Modifiers1 && string.Equals(other.Key1, this.Key1, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var exprObj = obj as HotkeyExpression;
            if (exprObj != null)
            {
                return this.Equals(exprObj);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 11 + this.Modifiers1.GetHashCode() * 5;
            if (!string.IsNullOrEmpty(this.Key1))
            {
                hash += this.Key1.GetHashCode() * 7;
            }

            return hash;
        }
    }
}
