using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedAutoHotkeyParser
{
    public class CommentExpression : Expression, IEquatable<CommentExpression>
    {
        public const string CommentToken = ";";

        public CommentExpression(string text)
        {
            this.Text = text;
        }

        public override string Text { get; }

        public static bool TryParse(string text, out Expression expr)
        {
            expr = null;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            
            int commentIndex = text.IndexOf(CommentExpression.CommentToken);
            if (commentIndex < 0)
            {
                return false;
            }

            expr = new CommentExpression(text.Substring(commentIndex));
            return true;
        }

        public bool Equals(CommentExpression other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Text.Equals(other.Text);
        }

        public override bool Equals(object obj)
        {
            var objExpr = obj as CommentExpression;
            if (objExpr != null)
            {
                return this.Equals(objExpr);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }
    }
}
