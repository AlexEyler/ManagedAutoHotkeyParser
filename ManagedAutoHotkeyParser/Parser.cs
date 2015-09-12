using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace ManagedAutoHotkeyParser
{
    public class Parser
    {
        public Script Script { get; }

        public Parser()
        {
            this.Script = new Script();
        }

        public bool TryParse(string text)
        {
            Requires.NotNull(text, nameof(text));
            bool success = true;
            foreach (var line in text.SplitByLine())
            {
                Expression expr;
                success |= this.TryParseLine(line, out expr);
                if (expr != null)
                {
                    this.Script.AddExpression(expr);
                }
            }

            return success;
        }

        private bool TryParseLine(string line, out Expression expr)
        {
            Requires.NotNull(line, nameof(line));

            expr = null;
            if (CommentExpression.TryParse(line, out expr))
            {
                return true;
            }
            else if (HotkeyExpression.TryParse(line, out expr))
            {
                return true;
            }

            return false;
        }
    }
}
