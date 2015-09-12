using ManagedAutoHotkeyParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ParserTests
    {
        public ParserTests() { }

        [Fact]
        public void Parse_ThrowsArgumentNullException()
        {
            Parser parser = new Parser();
            Assert.Throws<ArgumentNullException>(() => parser.TryParse(null));
        }

        [Fact]
        public void Parse_CreatesEmptyObjectWithEmptyInput()
        {
            Parser parser = new Parser();
            Assert.True(parser.TryParse(string.Empty));
        }

        [Fact]
        public void Parse_ParsesCommentString()
        {
            Parser parser = new Parser();
            const string commentString = CommentExpression.CommentToken + " this is a comment";
            CommentExpression expr = new CommentExpression(commentString);
            Assert.True(parser.TryParse(commentString));
            Assert.True(parser.Script.ContainsExpression(expr));
        }

        [Fact]
        public void Parse_ParsesHotkey()
        {
            // HotkeyExpression tests are in ExpressionTests/HotkeyTests.cs
            Parser parser = new Parser();
            const string hotkey = "^x::"; // ctrl+x
            HotkeyExpression expr = new HotkeyExpression(hotkey);
            expr.Modifiers1 = HotkeySymbolModifiers.Ctrl;
            expr.Key1 = "x";

            Assert.True(parser.TryParse(hotkey));
            Assert.True(parser.Script.ContainsExpression(expr));
        }
    }
}
