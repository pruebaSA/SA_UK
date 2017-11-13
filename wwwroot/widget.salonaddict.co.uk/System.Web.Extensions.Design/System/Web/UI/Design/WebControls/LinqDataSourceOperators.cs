namespace System.Web.UI.Design.WebControls
{
    using System;

    internal sealed class LinqDataSourceOperators
    {
        private string _symbol;
        public static LinqDataSourceOperators EQ = new LinqDataSourceOperators("==");
        public static LinqDataSourceOperators GE = new LinqDataSourceOperators(">=");
        public static LinqDataSourceOperators GT = new LinqDataSourceOperators(">");
        public static LinqDataSourceOperators LE = new LinqDataSourceOperators("<=");
        public static LinqDataSourceOperators LT = new LinqDataSourceOperators("<");
        public static LinqDataSourceOperators NE = new LinqDataSourceOperators("!=");
        public static LinqDataSourceOperators None = new LinqDataSourceOperators(string.Empty);

        private LinqDataSourceOperators(string symbol)
        {
            this._symbol = symbol;
        }

        public static LinqDataSourceOperators GetOperator(string symbol)
        {
            if (string.Equals("=", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return EQ;
            }
            if (string.Equals("==", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return EQ;
            }
            if (string.Equals("!=", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return NE;
            }
            if (string.Equals("<>", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return NE;
            }
            if (string.Equals(">", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return GT;
            }
            if (string.Equals(">=", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return GE;
            }
            if (string.Equals("<", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return LT;
            }
            if (string.Equals("<=", symbol, StringComparison.OrdinalIgnoreCase))
            {
                return LE;
            }
            return None;
        }

        public override string ToString() => 
            this._symbol;
    }
}

