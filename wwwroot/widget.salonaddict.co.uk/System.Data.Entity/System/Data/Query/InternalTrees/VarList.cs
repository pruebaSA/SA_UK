namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    [DebuggerDisplay("{{{ToString()}}}")]
    internal class VarList : List<Var>
    {
        internal VarList()
        {
        }

        internal VarList(IEnumerable<Var> vars) : base(vars)
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (Var var in this)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, var.Id });
                str = ",";
            }
            return builder.ToString();
        }
    }
}

