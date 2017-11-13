namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI.WebControls;

    internal sealed class LinqDataSourceWhereStatement : List<LinqDataSourceWhereExpression>
    {
        private ParameterCollection _parameters = new ParameterCollection();

        public void RemoveExpression(LinqDataSourceWhereExpression expr)
        {
            if (base.Remove(expr))
            {
                foreach (Parameter parameter in this._parameters)
                {
                    if (string.Equals(parameter.Name, expr.ParameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        this._parameters.Remove(parameter);
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (LinqDataSourceWhereExpression expression in this)
            {
                builder.Append(expression.ToString());
                if (num < (base.Count - 1))
                {
                    builder.Append(" && ");
                    num++;
                }
            }
            return builder.ToString();
        }

        public ParameterCollection Parameters =>
            this._parameters;
    }
}

