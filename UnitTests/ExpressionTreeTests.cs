using ManagedAutoHotkeyParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class ExpressionTreeTests
    {
        [Fact]
        public void Ctor_InitializesRootNode()
        {
            ExpressionTree tree = new ExpressionTree();
            Assert.NotNull(tree.rootNode);
        }

        [Fact]
        public void AddExpression_ThrowsArgumentNullException()
        {
            ExpressionTree tree = new ExpressionTree();
            Assert.Throws<ArgumentNullException>(() => tree.AddExpression(null));
        }

        [Fact]
        public void AddExpression_AddsExpression()
        {
            ExpressionTree tree = new ExpressionTree();
            MockExpression expr = new MockExpression(1);
            tree.AddExpression(expr);

            Assert.NotEmpty(tree.rootNode.Children);
            Assert.NotNull(tree.rootNode.Children.FirstOrDefault());
            Assert.Equal(expr, tree.rootNode.Children.Select(c => c as ExpressionNode).FirstOrDefault(c => c != null).Expression);
        }

        [Fact]
        public void ContainsExpression_ThrowsArgumentNullException()
        {
            ExpressionTree tree = new ExpressionTree();
            Assert.Throws<ArgumentNullException>(() => tree.ContainsExpression(null));
        }

        [Fact]
        public void ContainsExpression_ReturnsFalseIfDoesntContainExpression()
        {
            ExpressionTree tree = new ExpressionTree();
            Assert.False(tree.ContainsExpression(new MockExpression(1)));
        }

        [Fact]
        public void ContainsExpress_ReturnsTrueIfContainsExpression()
        {
            ExpressionTree tree = new ExpressionTree();
            var expr = new MockExpression(1);
            tree.AddExpression(expr);
            Assert.True(tree.ContainsExpression(expr));
        }
    }

    public class MockExpression : Expression
    {
        public MockExpression(int id)
        {
            this.Text = id.ToString();
        }

        public override string Text { get; }
    }
}
