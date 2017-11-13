namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    internal class VarMap : Dictionary<Var, Var>
    {
        internal VarMap()
        {
        }

        internal VarMap GetReverseMap()
        {
            VarMap map = new VarMap();
            foreach (KeyValuePair<Var, Var> pair in this)
            {
                Var var;
                if (!map.TryGetValue(pair.Value, out var))
                {
                    map[pair.Value] = pair.Key;
                }
            }
            return map;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (Var var in base.Keys)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", new object[] { str, var.Id, base[var].Id });
                str = ",";
            }
            return builder.ToString();
        }
    }
}

