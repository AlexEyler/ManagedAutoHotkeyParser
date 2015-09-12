using ManagedAutoHotkeyParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ExpressionTests
{
    public class HotkeyTests
    {
        [Fact]
        public void TryParse_ThrowsArgumentNullException()
        {
            Expression expr;
            Assert.Throws<ArgumentNullException>(() => HotkeyExpression.TryParse(null, out expr));
        }

        [Fact]
        public void TryParse_EmptyStringIsInvalid()
        {
            Expression expr;
            Assert.False(HotkeyExpression.TryParse(string.Empty, out expr));
            Assert.Null(expr);
        }

        [Fact]
        public void TryParse_WhiteSpaceStringIsInvalid()
        {
            Expression expr;
            Assert.False(HotkeyExpression.TryParse(" \t", out expr));
            Assert.Null(expr);
        }

        [Fact]
        public void TryParse_EmptyModifiersIsInvalid()
        {
            const string text = HotkeyExpression.HotkeyToken;
            Expression expr;
            Assert.False(HotkeyExpression.TryParse(text, out expr));
            Assert.Null(expr);
        }

        [Fact]
        public void TryParse_OnlyModifierIsValidIfNotLeftOrRight()
        {
            foreach (var key in HotkeyExpression.ModifierMap.Keys)
            {
                var modifier = HotkeyExpression.ModifierMap[key];

                Expression expr;

                if (modifier == HotkeySymbolModifiers.Left || modifier == HotkeySymbolModifiers.Right)
                {
                    Assert.False(HotkeyExpression.TryParse(key + "::", out expr));
                    Assert.Null(expr);
                }
                else
                {
                    Assert.True(HotkeyExpression.TryParse(key + "::", out expr));
                    Assert.NotNull(expr);

                    HotkeyExpression hkExpr = expr as HotkeyExpression;
                    Assert.NotNull(hkExpr);
                    Assert.Equal(modifier, hkExpr.Modifiers1);
                    Assert.True(string.IsNullOrEmpty(hkExpr.Key1));

                    // Make sure we didn't set Key2 or Modifiers2
                    Assert.Equal(HotkeySymbolModifiers.None, hkExpr.Modifiers2);
                    Assert.True(string.IsNullOrEmpty(hkExpr.Key2));
                }
            }
        }

        [Fact]
        public void TryParse_OnlyKeyIsValid()
        {
            const string key = "k";
            Expression expr;
            Assert.True(HotkeyExpression.TryParse(key + "::", out expr));
            Assert.NotNull(expr);

            HotkeyExpression hkExpr = expr as HotkeyExpression;
            Assert.NotNull(hkExpr);
            Assert.True(hkExpr.Modifiers1.Equals(HotkeySymbolModifiers.None));
            Assert.Equal(key, hkExpr.Key1);

            // Make sure we didn't set Key2 or Modifiers2
            Assert.Equal(HotkeySymbolModifiers.None, hkExpr.Modifiers2);
            Assert.True(string.IsNullOrEmpty(hkExpr.Key2));
        }

        [Fact]
        public void TryParse_SingleModifierSingleKeyIsValid()
        {
            const string key = "k";
            foreach (var modKey in HotkeyExpression.ModifierMap.Keys)
            {
                Expression expr;
                var modifier = HotkeyExpression.ModifierMap[modKey];

                if (modifier == HotkeySymbolModifiers.Left || modifier == HotkeySymbolModifiers.Right)
                {
                    Assert.False(HotkeyExpression.TryParse(modKey + key + "::", out expr));
                    Assert.Null(expr);
                }
                else
                {
                    Assert.True(HotkeyExpression.TryParse(modKey + key + "::", out expr));
                    Assert.NotNull(expr);

                    HotkeyExpression hkExpr = expr as HotkeyExpression;
                    Assert.NotNull(hkExpr);
                    Assert.True(hkExpr.Modifiers1.Equals(modifier));
                    Assert.Equal(key, hkExpr.Key1);

                    // Make sure we didn't set Key2 or Modifiers2
                    Assert.Equal(HotkeySymbolModifiers.None, hkExpr.Modifiers2);
                    Assert.True(string.IsNullOrEmpty(hkExpr.Key2));
                }
            }
        }

        [Fact]
        public void TryParse_MultipleModifiersSingleKeyIsValid()
        {
            const string key = "k";
            const string expressionString = "^!k::";
            const HotkeySymbolModifiers modifiers = HotkeySymbolModifiers.Alt | HotkeySymbolModifiers.Ctrl;
            Expression expr;
            Assert.True(HotkeyExpression.TryParse(expressionString, out expr));
            Assert.NotNull(expr);

            HotkeyExpression hkExpr = expr as HotkeyExpression;
            Assert.NotNull(hkExpr);
            Assert.Equal(modifiers, hkExpr.Modifiers1);
            Assert.Equal(key, hkExpr.Key1);

            // Make sure we didn't set Key2 or Modifiers2
            Assert.Equal(HotkeySymbolModifiers.None, hkExpr.Modifiers2);
            Assert.True(string.IsNullOrEmpty(hkExpr.Key2));
        }

        [Fact]
        public void TryParse_MultipleKeysWithoutAmpersandIsInvalid()
        {
            const string expressionString = "ab";
            Expression expr;
            Assert.False(HotkeyExpression.TryParse(expressionString, out expr));
            Assert.Null(expr);
        }

        [Fact]
        public void TryParse_MultipleKeysWithAmpersandIsValid()
        {
            const string expressionString = "^k & ^m::"; // ctrl + k, ctrl + m
            Expression expr;
            Assert.True(HotkeyExpression.TryParse(expressionString, out expr));
            Assert.NotNull(expr);

            HotkeyExpression hkExpr = expr as HotkeyExpression;
            Assert.NotNull(hkExpr);
            Assert.Equal(hkExpr.Key1, "k");
            Assert.Equal(hkExpr.Modifiers1, HotkeySymbolModifiers.Ctrl);

            Assert.Equal(hkExpr.Key2, "m");
            Assert.Equal(hkExpr.Modifiers2, HotkeySymbolModifiers.Ctrl);
        }
    }
}
