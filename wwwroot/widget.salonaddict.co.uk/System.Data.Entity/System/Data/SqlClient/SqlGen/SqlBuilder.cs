namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;

    internal sealed class SqlBuilder : ISqlFragment
    {
        private List<object> _sqlFragments;

        public void Append(object s)
        {
            this.sqlFragments.Add(s);
        }

        public void AppendLine()
        {
            this.sqlFragments.Add("\r\n");
        }

        public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
        {
            if (this._sqlFragments != null)
            {
                foreach (object obj2 in this._sqlFragments)
                {
                    string str = obj2 as string;
                    if (str == null)
                    {
                        ISqlFragment fragment = obj2 as ISqlFragment;
                        if (fragment == null)
                        {
                            throw new InvalidOperationException();
                        }
                        fragment.WriteSql(writer, sqlGenerator);
                    }
                    else
                    {
                        writer.Write(str);
                    }
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (this._sqlFragments != null)
                {
                    return (0 == this._sqlFragments.Count);
                }
                return true;
            }
        }

        private List<object> sqlFragments
        {
            get
            {
                if (this._sqlFragments == null)
                {
                    this._sqlFragments = new List<object>();
                }
                return this._sqlFragments;
            }
        }
    }
}

