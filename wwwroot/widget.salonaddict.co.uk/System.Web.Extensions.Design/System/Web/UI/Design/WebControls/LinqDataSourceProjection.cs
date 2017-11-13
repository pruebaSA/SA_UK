namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;

    internal sealed class LinqDataSourceProjection : ILinqDataSourceProjection
    {
        private LinqDataSourceAggregateFunctions _aggregateFunction;
        private string _alias;
        private ILinqDataSourcePropertyItem _column;
        private bool _isAliasMandatory;

        private LinqDataSourceProjection()
        {
        }

        public LinqDataSourceProjection(ILinqDataSourcePropertyItem column, LinqDataSourceAggregateFunctions aggregateFunction, bool isAliasMandatory)
        {
            this._column = column;
            this._aggregateFunction = aggregateFunction;
            this._isAliasMandatory = isAliasMandatory;
            if (this._isAliasMandatory)
            {
                this._alias = this.GetDefaultAlias();
            }
        }

        public LinqDataSourceProjection(ILinqDataSourcePropertyItem column, LinqDataSourceAggregateFunctions aggregateFunction, bool isAliasMandatory, string alias) : this(column, aggregateFunction, isAliasMandatory)
        {
            this._alias = alias;
        }

        private bool DoesAliasNeedUpdate() => 
            ((this.IsAliasMandatory && string.IsNullOrEmpty(this.Alias)) || string.Equals(this.Alias, this.GetDefaultAlias()));

        internal static string GetAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias) || !char.IsWhiteSpace(alias[0]))
            {
                return null;
            }
            alias = alias.Trim();
            if (!IsValidFieldName(alias, false))
            {
                return null;
            }
            return alias;
        }

        private string GetDefaultAlias()
        {
            if (this.AggregateFunction == LinqDataSourceAggregateFunctions.Count)
            {
                return this.AggregateFunction.ToString();
            }
            return (this.AggregateFunction.ToString() + "_" + this.Column.Name);
        }

        internal static ILinqDataSourcePropertyItem GetField(string fieldName, List<ILinqDataSourcePropertyItem> fields)
        {
            if (string.Equals(LinqDataSourceSelectBuilder.CountField.ToString(), fieldName, StringComparison.Ordinal))
            {
                return LinqDataSourceSelectBuilder.CountField;
            }
            if (string.Equals(LinqDataSourceSelectBuilder.ItField.ToString(), fieldName, StringComparison.Ordinal))
            {
                return LinqDataSourceSelectBuilder.ItField;
            }
            if (string.Equals(LinqDataSourceSelectBuilder.KeyField.ToString(), fieldName, StringComparison.Ordinal))
            {
                return LinqDataSourceSelectBuilder.KeyField;
            }
            foreach (ILinqDataSourcePropertyItem item in fields)
            {
                if (string.Equals(item.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        public static List<LinqDataSourceAggregateFunctions> GetValidAggregates(Type fieldType)
        {
            List<LinqDataSourceAggregateFunctions> list = new List<LinqDataSourceAggregateFunctions>();
            if (fieldType != null)
            {
                list.Add(LinqDataSourceAggregateFunctions.Min);
                list.Add(LinqDataSourceAggregateFunctions.Max);
                if (IsNumeric(fieldType) && (fieldType != typeof(ulong)))
                {
                    list.Add(LinqDataSourceAggregateFunctions.Sum);
                    list.Add(LinqDataSourceAggregateFunctions.Average);
                }
                return list;
            }
            list.Add(LinqDataSourceAggregateFunctions.Count);
            return list;
        }

        private static bool IsNumeric(Type fieldType)
        {
            if (((((fieldType != typeof(byte)) && (fieldType != typeof(sbyte))) && ((fieldType != typeof(ushort)) && (fieldType != typeof(uint)))) && (((fieldType != typeof(ulong)) && (fieldType != typeof(short))) && ((fieldType != typeof(int)) && (fieldType != typeof(long))))) && ((fieldType != typeof(float)) && (fieldType != typeof(double))))
            {
                return (fieldType == typeof(decimal));
            }
            return true;
        }

        internal static bool IsValidFieldName(string name, bool mustBeBlank)
        {
            if (mustBeBlank)
            {
                return string.IsNullOrEmpty(name);
            }
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            if (!char.IsLetter(name[0]) && (name[0] != '_'))
            {
                return false;
            }
            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && (name[i] != '_'))
                {
                    return false;
                }
            }
            return true;
        }

        public static LinqDataSourceProjection MakeProjection(string projection, List<ILinqDataSourcePropertyItem> fields)
        {
            LinqDataSourceProjection p = new LinqDataSourceProjection();
            int index = projection.IndexOf('(');
            int startIndex = -1;
            if (index > -1)
            {
                p._isAliasMandatory = true;
                string name = projection.Substring(0, index).Trim();
                p._aggregateFunction = LinqDataSourceAggregateFunctions.FromString(name);
                int num3 = projection.IndexOf(')', index);
                startIndex = num3 + 1;
                if (num3 <= -1)
                {
                    return null;
                }
                string fieldName = projection.Substring(index + 1, (num3 - index) - 1).Trim();
                if (!SetField(p, fieldName, fields))
                {
                    if (p._aggregateFunction != LinqDataSourceAggregateFunctions.Count)
                    {
                        return null;
                    }
                    p._column = LinqDataSourceSelectBuilder.CountField;
                }
            }
            else
            {
                p._aggregateFunction = LinqDataSourceAggregateFunctions.None;
                p._isAliasMandatory = false;
                projection = projection.Trim();
                int length = 0;
                length = 0;
                while (length < projection.Length)
                {
                    if (char.IsWhiteSpace(projection[length]))
                    {
                        break;
                    }
                    length++;
                }
                string str3 = projection.Substring(0, length);
                if (!SetField(p, str3, fields))
                {
                    return null;
                }
                startIndex = length;
            }
            if (p._aggregateFunction == LinqDataSourceAggregateFunctions.Unknown)
            {
                return null;
            }
            if (!IsValidFieldName(p.Column.Name, p._aggregateFunction == LinqDataSourceAggregateFunctions.Count))
            {
                return null;
            }
            if (startIndex > -1)
            {
                string str4 = projection.Substring(startIndex).Trim();
                if (!string.IsNullOrEmpty(str4))
                {
                    if (str4.StartsWith("as", StringComparison.OrdinalIgnoreCase))
                    {
                        p._alias = GetAlias(str4.Substring(2));
                        if (p._alias == null)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            if (p._isAliasMandatory && string.IsNullOrEmpty(p.Alias))
            {
                return null;
            }
            return p;
        }

        private static bool SetField(LinqDataSourceProjection p, string fieldName, List<ILinqDataSourcePropertyItem> fields)
        {
            if (string.Equals(fieldName, LinqDataSourceSelectBuilder.KeyField.Name, StringComparison.OrdinalIgnoreCase))
            {
                p._column = LinqDataSourceSelectBuilder.KeyField;
            }
            else if (string.Equals(fieldName, LinqDataSourceSelectBuilder.ItField.Name, StringComparison.OrdinalIgnoreCase))
            {
                p._column = LinqDataSourceSelectBuilder.ItField;
            }
            else
            {
                ILinqDataSourcePropertyItem field = GetField(fieldName, fields);
                if (field != null)
                {
                    p._column = field;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            if (this.AggregateFunction == LinqDataSourceAggregateFunctions.None)
            {
                if (string.IsNullOrEmpty(this.Alias))
                {
                    return this.Column.Name;
                }
                return (this.Column.Name + " as " + this.Alias);
            }
            if (string.IsNullOrEmpty(this.Alias))
            {
                return (this.AggregateFunction.ToString() + "(" + this.Column.Name + ")");
            }
            return (this.AggregateFunction.ToString() + "(" + this.Column.Name + ") as " + this.Alias);
        }

        public LinqDataSourceAggregateFunctions AggregateFunction
        {
            get => 
                this._aggregateFunction;
            set
            {
                bool flag = this.DoesAliasNeedUpdate();
                this._aggregateFunction = value;
                if (flag)
                {
                    this.Alias = this.GetDefaultAlias();
                }
            }
        }

        public string Alias
        {
            get => 
                this._alias;
            set
            {
                this._alias = value;
            }
        }

        public ILinqDataSourcePropertyItem Column
        {
            get => 
                this._column;
            set
            {
                bool flag = this.DoesAliasNeedUpdate();
                this._column = value;
                if (flag)
                {
                    this.Alias = this.GetDefaultAlias();
                }
            }
        }

        public bool IsAliasMandatory =>
            this._isAliasMandatory;
    }
}

