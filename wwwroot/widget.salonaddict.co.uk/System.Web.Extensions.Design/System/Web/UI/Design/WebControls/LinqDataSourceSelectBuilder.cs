namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web.Resources.Design;

    internal class LinqDataSourceSelectBuilder
    {
        private static ILinqDataSourcePropertyItem _countField;
        private static ILinqDataSourcePropertyItem _itField;
        private static ILinqDataSourcePropertyItem _keyField;

        public ParseResult CreateProjection(LinqDataSourceState state, List<ILinqDataSourcePropertyItem> tableFields)
        {
            ParseResult result = new ParseResult {
                Projections = null,
                Mode = LinqDataSourceGroupByMode.GroupByCustom
            };
            string str = state.GroupBy.Trim();
            string str2 = state.Select.Trim();
            if ((state.GroupByParameters.Count <= 0) && (state.SelectParameters.Count <= 0))
            {
                bool flag = false;
                if (str2.StartsWith("new", StringComparison.OrdinalIgnoreCase))
                {
                    string str3 = str2.Substring(3).Trim();
                    if (str3.StartsWith("(", StringComparison.Ordinal) && str3.EndsWith(")", StringComparison.Ordinal))
                    {
                        str2 = str3.Substring(1, str3.Length - 2);
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    if (string.IsNullOrEmpty(str2))
                    {
                        return result;
                    }
                }
                else if (!string.IsNullOrEmpty(str2))
                {
                    return result;
                }
                string[] selects = str2.Split(new char[] { ',' });
                if (string.IsNullOrEmpty(str))
                {
                    return this.CreateProjectionGroupByNone(selects, tableFields);
                }
                if (tableFields.Count <= 0)
                {
                    return result;
                }
                foreach (ILinqDataSourcePropertyItem item in tableFields)
                {
                    if (string.Equals(item.Name, str, StringComparison.OrdinalIgnoreCase))
                    {
                        return this.CreateProjectionGroupByField(selects, tableFields);
                    }
                }
            }
            return result;
        }

        internal ParseResult CreateProjectionGroupByField(string[] selects, List<ILinqDataSourcePropertyItem> tableFields)
        {
            ParseResult result = new ParseResult {
                Projections = null,
                Mode = LinqDataSourceGroupByMode.GroupByCustom
            };
            if ((selects.Length == 1) && string.IsNullOrEmpty(selects[0]))
            {
                return result;
            }
            ParseResult result2 = new ParseResult();
            int num = 0;
            int num2 = 0;
            List<string> list = new List<string>();
            List<ILinqDataSourceProjection> list2 = new List<ILinqDataSourceProjection>();
            result2.Mode = LinqDataSourceGroupByMode.GroupByField;
            result2.Projections = list2;
            foreach (string str in selects)
            {
                LinqDataSourceProjection projection = LinqDataSourceProjection.MakeProjection(str, tableFields);
                if (projection == null)
                {
                    return result;
                }
                list2.Add(projection);
                string str2 = string.IsNullOrEmpty(projection.Alias) ? projection.Column.Name : projection.Alias;
                if (list.Contains(str2))
                {
                    return result;
                }
                list.Add(str2);
                if (KeyField == projection.Column)
                {
                    if (projection.AggregateFunction != LinqDataSourceAggregateFunctions.None)
                    {
                        return result;
                    }
                    if (string.IsNullOrEmpty(projection.Alias))
                    {
                        result2.Mode = LinqDataSourceGroupByMode.GroupByCustom;
                    }
                    num++;
                    continue;
                }
                if (ItField == projection.Column)
                {
                    if (projection.AggregateFunction != LinqDataSourceAggregateFunctions.None)
                    {
                        result2.Mode = LinqDataSourceGroupByMode.GroupByCustom;
                    }
                    if (string.IsNullOrEmpty(projection.Alias))
                    {
                        return result;
                    }
                    num2++;
                    continue;
                }
                bool flag = false;
                if (projection.AggregateFunction == LinqDataSourceAggregateFunctions.Count)
                {
                    flag = true;
                }
                else
                {
                    foreach (ILinqDataSourcePropertyItem item in tableFields)
                    {
                        if (item == projection.Column)
                        {
                            if (!LinqDataSourceProjection.GetValidAggregates(item.Type).Contains(projection.AggregateFunction) || string.IsNullOrEmpty(projection.Alias))
                            {
                                return result;
                            }
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    return result;
                }
            }
            if ((num != 1) || (num2 != 1))
            {
                result2.Mode = LinqDataSourceGroupByMode.GroupByCustom;
            }
            return result2;
        }

        internal ParseResult CreateProjectionGroupByNone(string[] selects, List<ILinqDataSourcePropertyItem> tableFields)
        {
            ParseResult result = new ParseResult {
                Projections = null,
                Mode = LinqDataSourceGroupByMode.GroupByCustom
            };
            ParseResult result2 = new ParseResult();
            List<string> list = new List<string>();
            List<ILinqDataSourceProjection> list2 = new List<ILinqDataSourceProjection>();
            result2.Mode = LinqDataSourceGroupByMode.GroupByNone;
            result2.Projections = list2;
            if ((selects.Length != 1) || !string.IsNullOrEmpty(selects[0]))
            {
                foreach (string str in selects)
                {
                    LinqDataSourceProjection projection = LinqDataSourceProjection.MakeProjection(str, tableFields);
                    if (projection == null)
                    {
                        return result;
                    }
                    list2.Add(projection);
                    if ((projection.Column == null) || (projection.AggregateFunction != LinqDataSourceAggregateFunctions.None))
                    {
                        return result;
                    }
                    if (projection.Alias != null)
                    {
                        result2.Mode = LinqDataSourceGroupByMode.GroupByCustom;
                    }
                    string str2 = string.IsNullOrEmpty(projection.Alias) ? projection.Column.Name : projection.Alias;
                    if (list.Contains(str2))
                    {
                        return result;
                    }
                    list.Add(str2);
                    bool flag = false;
                    if (ItField == projection.Column)
                    {
                        result2.Mode = LinqDataSourceGroupByMode.GroupByCustom;
                        if (string.IsNullOrEmpty(projection.Alias))
                        {
                            return result;
                        }
                        flag = true;
                    }
                    else
                    {
                        foreach (ILinqDataSourcePropertyItem item in tableFields)
                        {
                            if (item == projection.Column)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        return result;
                    }
                }
            }
            return result2;
        }

        public static ILinqDataSourcePropertyItem CountField
        {
            get
            {
                if (_countField == null)
                {
                    _countField = new LinqDataSourcePropertyItem(null, AtlasWebDesign.LinqDataSourceSelectBuilder_CountFieldDisplay);
                }
                return _countField;
            }
        }

        public static ILinqDataSourcePropertyItem ItField
        {
            get
            {
                if (_itField == null)
                {
                    _itField = new LinqDataSourcePropertyItem("it", "it");
                }
                return _itField;
            }
        }

        public static ILinqDataSourcePropertyItem KeyField
        {
            get
            {
                if (_keyField == null)
                {
                    _keyField = new LinqDataSourcePropertyItem("key", "key");
                }
                return _keyField;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ParseResult
        {
            public LinqDataSourceGroupByMode Mode;
            public List<ILinqDataSourceProjection> Projections;
        }
    }
}

