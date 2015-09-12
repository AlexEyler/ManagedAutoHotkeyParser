using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace ManagedAutoHotkeyParser
{
    public class ExpressionTree
    {
        // internal for testing
        internal Node rootNode;

        public ExpressionTree()
        {
            this.rootNode = new RootNode();
        }

        public void AddExpression(Expression expr)
        {
            Requires.NotNull(expr, nameof(expr));
            this.rootNode.AddChild(new ExpressionNode(expr, this.rootNode));
        }

        public bool ContainsExpression(Expression expr)
        {
            Requires.NotNull(expr, nameof(expr));

            HashSet<Node> visited = new HashSet<Node>(new[] { this.rootNode });
            Queue<Node> nodes = new Queue<Node>(new[] { this.rootNode });

            while (nodes.Any())
            {
                var node = nodes.Dequeue();

                if (node == null)
                {
                    return false;
                }

                var expressionNode = node as ExpressionNode;
                if (expressionNode != null)
                {
                    if (expressionNode.Expression.Equals(expr))
                    {
                        return true;
                    }
                }

                foreach (var child in node.Children)
                {
                    if (!visited.Contains(child))
                    {
                        visited.Add(child);
                        nodes.Enqueue(child);
                    }
                }
            }

            return false;
        }
    }

    public class ExpressionNode : Node
    {
        public Expression Expression { get; }

        public ExpressionNode(Expression expr, Node parent)
        {
            Requires.NotNull(expr, nameof(expr));
            Requires.NotNull(parent, nameof(parent));

            this.Expression = expr;
            this.Parent = parent;
        }
    }

    public sealed class RootNode : Node
    {
        public RootNode()
        {
            this.Parent = null;
        }
    }

    public class Node
    {
        public Node Parent { get; protected set; }

        public IReadOnlyList<Node> Children { get; private set; } = new List<Node>();

        public void AddChild(Node child)
        {
            Requires.NotNull(child, nameof(child));
            var list = Children.ToList();
            list.Add(child);
            this.Children = list.AsReadOnly();
        }
    }
}
