namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class PostBindDotNetConverter
    {
        private static readonly string[] dateParts = new string[] { "Year", "Month", "Day", "Hour", "Minute", "Second", "Millisecond", "Microsecond", "Nanosecond" };

        internal static bool CanConvert(SqlNode node)
        {
            SqlUnary uo = node as SqlUnary;
            if ((uo != null) && IsSupportedUnary(uo))
            {
                return true;
            }
            SqlNew snew = node as SqlNew;
            if ((snew != null) && IsSupportedNew(snew))
            {
                return true;
            }
            SqlMember m = node as SqlMember;
            if ((m != null) && IsSupportedMember(m))
            {
                return true;
            }
            SqlMethodCall mc = node as SqlMethodCall;
            return ((mc != null) && (GetMethodSupport(mc) == MethodSupport.Method));
        }

        internal static SqlNode Convert(SqlNode node, SqlFactory sql, SqlProvider.ProviderMode providerMode) => 
            new Visitor(sql, providerMode).Visit(node);

        private static MethodSupport GetCoercionMethodSupport(SqlMethodCall mc)
        {
            if ((!mc.Method.IsStatic || !mc.SqlType.CanBeColumn) || ((mc.Method.Name != "op_Implicit") && (mc.Method.Name != "op_Explicit")))
            {
                return MethodSupport.None;
            }
            return MethodSupport.Method;
        }

        private static MethodSupport GetComparisonMethodSupport(SqlMethodCall mc)
        {
            if ((mc.Method.IsStatic && (mc.Method.Name == "Compare")) && (mc.Method.ReturnType == typeof(int)))
            {
                return MethodSupport.Method;
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetConvertMethodSupport(SqlMethodCall mc)
        {
            if ((mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(System.Convert))) && (mc.Arguments.Count == 1))
            {
                switch (mc.Method.Name)
                {
                    case "ToBoolean":
                    case "ToDecimal":
                    case "ToByte":
                    case "ToChar":
                    case "ToDouble":
                    case "ToInt16":
                    case "ToInt32":
                    case "ToInt64":
                    case "ToSingle":
                    case "ToString":
                        return MethodSupport.Method;

                    case "ToDateTime":
                        if ((mc.Arguments[0].ClrType != typeof(string)) && (mc.Arguments[0].ClrType != typeof(DateTime)))
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;
                }
            }
            return MethodSupport.None;
        }

        private static string GetDatePart(string memberName)
        {
            switch (memberName)
            {
                case "Year":
                case "Month":
                case "Day":
                case "DayOfYear":
                case "Hour":
                case "Minute":
                case "Second":
                case "Millisecond":
                    return memberName;
            }
            return null;
        }

        private static MethodSupport GetDateTimeMethodSupport(SqlMethodCall mc)
        {
            if (!mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(DateTime)))
            {
                switch (mc.Method.Name)
                {
                    case "CompareTo":
                    case "AddTicks":
                    case "AddMonths":
                    case "AddYears":
                    case "AddMilliseconds":
                    case "AddSeconds":
                    case "AddMinutes":
                    case "AddHours":
                    case "AddDays":
                        return MethodSupport.Method;

                    case "Add":
                        if ((mc.Arguments.Count != 1) || (mc.Arguments[0].ClrType != typeof(TimeSpan)))
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetDateTimeOffsetMethodSupport(SqlMethodCall mc)
        {
            if (!mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(DateTimeOffset)))
            {
                switch (mc.Method.Name)
                {
                    case "CompareTo":
                    case "AddTicks":
                    case "AddMonths":
                    case "AddYears":
                    case "AddMilliseconds":
                    case "AddSeconds":
                    case "AddMinutes":
                    case "AddHours":
                    case "AddDays":
                        return MethodSupport.Method;

                    case "Add":
                        if ((mc.Arguments.Count != 1) || (mc.Arguments[0].ClrType != typeof(TimeSpan)))
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetDecimalMethodSupport(SqlMethodCall mc)
        {
            if (mc.Method.IsStatic)
            {
                if (mc.Arguments.Count == 2)
                {
                    string str;
                    if (((str = mc.Method.Name) != null) && ((((str == "Multiply") || (str == "Divide")) || ((str == "Subtract") || (str == "Add"))) || ((str == "Remainder") || (str == "Round"))))
                    {
                        return MethodSupport.Method;
                    }
                }
                else if (mc.Arguments.Count == 1)
                {
                    string str2;
                    if (((str2 = mc.Method.Name) != null) && (((str2 == "Negate") || (str2 == "Floor")) || ((str2 == "Truncate") || (str2 == "Round"))))
                    {
                        return MethodSupport.Method;
                    }
                    if (mc.Method.Name.StartsWith("To", StringComparison.Ordinal))
                    {
                        return MethodSupport.Method;
                    }
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetMathMethodSupport(SqlMethodCall mc)
        {
            if (mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(Math)))
            {
                switch (mc.Method.Name)
                {
                    case "Abs":
                    case "Acos":
                    case "Asin":
                    case "Atan":
                    case "Ceiling":
                    case "Cos":
                    case "Cosh":
                    case "Exp":
                    case "Floor":
                    case "Log10":
                        if (mc.Arguments.Count != 1)
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;

                    case "Log":
                        if ((mc.Arguments.Count != 1) && (mc.Arguments.Count != 2))
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;

                    case "Max":
                    case "Min":
                    case "Pow":
                    case "Atan2":
                    case "BigMul":
                        if (mc.Arguments.Count != 2)
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;

                    case "Round":
                        if ((mc.Arguments[mc.Arguments.Count - 1].ClrType != typeof(MidpointRounding)) || ((mc.Arguments.Count != 2) && (mc.Arguments.Count != 3)))
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;

                    case "Sign":
                    case "Sin":
                    case "Sinh":
                    case "Sqrt":
                    case "Tan":
                    case "Tanh":
                    case "Truncate":
                        if (mc.Arguments.Count != 1)
                        {
                            return MethodSupport.MethodGroup;
                        }
                        return MethodSupport.Method;
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetMethodSupport(SqlMethodCall mc)
        {
            MethodSupport none = MethodSupport.None;
            MethodSupport sqlMethodsMethodSupport = GetSqlMethodsMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetDateTimeMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetDateTimeOffsetMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetTimeSpanMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetConvertMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetDecimalMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetMathMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetStringMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetComparisonMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetNullableMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetCoercionMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetObjectMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            sqlMethodsMethodSupport = GetVbHelperMethodSupport(mc);
            if (sqlMethodsMethodSupport > none)
            {
                none = sqlMethodsMethodSupport;
            }
            return none;
        }

        private static MethodSupport GetNullableMethodSupport(SqlMethodCall mc)
        {
            if ((mc.Method.Name == "GetValueOrDefault") && TypeSystem.IsNullableType(mc.Object.ClrType))
            {
                return MethodSupport.Method;
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetObjectMethodSupport(SqlMethodCall mc)
        {
            string str;
            if (!mc.Method.IsStatic && ((str = mc.Method.Name) != null))
            {
                if (str == "Equals")
                {
                    return MethodSupport.Method;
                }
                if (str == "ToString")
                {
                    if (mc.Object.SqlType.CanBeColumn)
                    {
                        return MethodSupport.Method;
                    }
                    return MethodSupport.None;
                }
                if (str == "GetType")
                {
                    if (mc.Arguments.Count == 0)
                    {
                        return MethodSupport.Method;
                    }
                    return MethodSupport.None;
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetSqlMethodsMethodSupport(SqlMethodCall mc)
        {
            if (mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(SqlMethods)))
            {
                if (mc.Method.Name.StartsWith("DateDiff", StringComparison.Ordinal) && (mc.Arguments.Count == 2))
                {
                    foreach (string str in dateParts)
                    {
                        if (mc.Method.Name == ("DateDiff" + str))
                        {
                            if (mc.Arguments.Count == 2)
                            {
                                return MethodSupport.Method;
                            }
                            return MethodSupport.MethodGroup;
                        }
                    }
                }
                else
                {
                    if (mc.Method.Name == "Like")
                    {
                        if (mc.Arguments.Count == 2)
                        {
                            return MethodSupport.Method;
                        }
                        if (mc.Arguments.Count == 3)
                        {
                            return MethodSupport.Method;
                        }
                        return MethodSupport.MethodGroup;
                    }
                    if (mc.Method.Name == "RawLength")
                    {
                        return MethodSupport.Method;
                    }
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetStringMethodSupport(SqlMethodCall mc)
        {
            if (mc.Method.DeclaringType == typeof(string))
            {
                if (mc.Method.IsStatic)
                {
                    if (mc.Method.Name == "Concat")
                    {
                        return MethodSupport.Method;
                    }
                }
                else
                {
                    switch (mc.Method.Name)
                    {
                        case "Contains":
                        case "StartsWith":
                        case "EndsWith":
                            if (mc.Arguments.Count != 1)
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;

                        case "IndexOf":
                        case "LastIndexOf":
                            if (((mc.Arguments.Count != 1) && (mc.Arguments.Count != 2)) && (mc.Arguments.Count != 3))
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;

                        case "Insert":
                            if (mc.Arguments.Count != 2)
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;

                        case "PadLeft":
                        case "PadRight":
                        case "Remove":
                        case "Substring":
                            if ((mc.Arguments.Count != 1) && (mc.Arguments.Count != 2))
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;

                        case "Replace":
                            return MethodSupport.Method;

                        case "Trim":
                        case "ToLower":
                        case "ToUpper":
                            if (mc.Arguments.Count != 0)
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;

                        case "get_Chars":
                        case "CompareTo":
                            if (mc.Arguments.Count != 1)
                            {
                                return MethodSupport.MethodGroup;
                            }
                            return MethodSupport.Method;
                    }
                }
            }
            return MethodSupport.None;
        }

        private static MethodSupport GetTimeSpanMethodSupport(SqlMethodCall mc)
        {
            string str;
            if (((mc.Method.IsStatic || (mc.Method.DeclaringType != typeof(TimeSpan))) || ((str = mc.Method.Name) == null)) || (((str != "Add") && (str != "Subtract")) && (((str != "CompareTo") && (str != "Duration")) && (str != "Negate"))))
            {
                return MethodSupport.None;
            }
            return MethodSupport.Method;
        }

        private static MethodSupport GetVbHelperMethodSupport(SqlMethodCall mc)
        {
            if ((!IsVbConversionMethod(mc) && !IsVbCompareString(mc)) && !IsVbLike(mc))
            {
                return MethodSupport.None;
            }
            return MethodSupport.Method;
        }

        private static bool IsSupportedBinaryMember(SqlMember m) => 
            ((m.Expression.ClrType == typeof(Binary)) && (m.Member.Name == "Length"));

        private static bool IsSupportedDateTimeMember(SqlMember m)
        {
            if (m.Expression.ClrType == typeof(DateTime))
            {
                string str2;
                if (GetDatePart(m.Member.Name) != null)
                {
                    return true;
                }
                if (((str2 = m.Member.Name) != null) && (((str2 == "Date") || (str2 == "TimeOfDay")) || (str2 == "DayOfWeek")))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSupportedDateTimeNew(SqlNew sox)
        {
            if ((((sox.ClrType == typeof(DateTime)) && (sox.Args.Count >= 3)) && ((sox.Args[0].ClrType == typeof(int)) && (sox.Args[1].ClrType == typeof(int)))) && (sox.Args[2].ClrType == typeof(int)))
            {
                if (sox.Args.Count == 3)
                {
                    return true;
                }
                if (((sox.Args.Count >= 6) && (sox.Args[3].ClrType == typeof(int))) && ((sox.Args[4].ClrType == typeof(int)) && (sox.Args[5].ClrType == typeof(int))))
                {
                    if (sox.Args.Count == 6)
                    {
                        return true;
                    }
                    if ((sox.Args.Count == 7) && (sox.Args[6].ClrType == typeof(int)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsSupportedDateTimeOffsetMember(SqlMember m)
        {
            if (m.Expression.ClrType == typeof(DateTimeOffset))
            {
                string str2;
                if (GetDatePart(m.Member.Name) != null)
                {
                    return true;
                }
                if (((str2 = m.Member.Name) != null) && (((str2 == "Date") || (str2 == "DateTime")) || ((str2 == "TimeOfDay") || (str2 == "DayOfWeek"))))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSupportedMember(SqlMember m)
        {
            if ((!IsSupportedStringMember(m) && !IsSupportedBinaryMember(m)) && (!IsSupportedDateTimeMember(m) && !IsSupportedDateTimeOffsetMember(m)))
            {
                return IsSupportedTimeSpanMember(m);
            }
            return true;
        }

        private static bool IsSupportedNew(SqlNew snew)
        {
            if (snew.ClrType == typeof(string))
            {
                return IsSupportedStringNew(snew);
            }
            if (snew.ClrType == typeof(TimeSpan))
            {
                return IsSupportedTimeSpanNew(snew);
            }
            return ((snew.ClrType == typeof(DateTime)) && IsSupportedDateTimeNew(snew));
        }

        private static bool IsSupportedStringMember(SqlMember m) => 
            ((m.Expression.ClrType == typeof(string)) && (m.Member.Name == "Length"));

        private static bool IsSupportedStringNew(SqlNew snew) => 
            (((snew.Args.Count == 2) && (snew.Args[0].ClrType == typeof(char))) && (snew.Args[1].ClrType == typeof(int)));

        private static bool IsSupportedTimeSpanMember(SqlMember m)
        {
            if (m.Expression.ClrType == typeof(TimeSpan))
            {
                switch (m.Member.Name)
                {
                    case "Ticks":
                    case "TotalMilliseconds":
                    case "TotalSeconds":
                    case "TotalMinutes":
                    case "TotalHours":
                    case "TotalDays":
                    case "Milliseconds":
                    case "Seconds":
                    case "Minutes":
                    case "Hours":
                    case "Days":
                        return true;
                }
            }
            return false;
        }

        private static bool IsSupportedTimeSpanNew(SqlNew sox) => 
            ((sox.Args.Count == 1) || ((sox.Args.Count == 3) || ((sox.Args.Count == 4) || (sox.Args.Count == 5))));

        private static bool IsSupportedUnary(SqlUnary uo) => 
            (((uo.NodeType == SqlNodeType.Convert) && (uo.ClrType == typeof(char))) || (uo.Operand.ClrType == typeof(char)));

        private static bool IsVbCompareString(SqlMethodCall call) => 
            ((call.Method.IsStatic && (call.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators")) && (call.Method.Name == "CompareString"));

        private static bool IsVbConversionMethod(SqlMethodCall mc)
        {
            if (mc.Method.IsStatic && (mc.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Conversions"))
            {
                switch (mc.Method.Name)
                {
                    case "ToBoolean":
                    case "ToSByte":
                    case "ToByte":
                    case "ToChar":
                    case "ToCharArrayRankOne":
                    case "ToDate":
                    case "ToDecimal":
                    case "ToDouble":
                    case "ToInteger":
                    case "ToUInteger":
                    case "ToLong":
                    case "ToULong":
                    case "ToShort":
                    case "ToUShort":
                    case "ToSingle":
                    case "ToString":
                        return true;
                }
            }
            return false;
        }

        private static bool IsVbLike(SqlMethodCall mc) => 
            (((mc.Method.IsStatic && (mc.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.LikeOperator")) && (mc.Method.Name == "LikeString")) || ((mc.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators") && (mc.Method.Name == "LikeString")));

        internal enum MethodSupport
        {
            None,
            MethodGroup,
            Method
        }

        private class SqlSelectionSkipper : SqlVisitor
        {
            private SqlVisitor parent;

            internal SqlSelectionSkipper(SqlVisitor parent)
            {
                this.parent = parent;
            }

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq) => 
                this.parent.VisitClientQuery(cq);

            internal override SqlExpression VisitColumn(SqlColumn col) => 
                this.parent.VisitColumn(col);

            internal override SqlSelect VisitSelect(SqlSelect select) => 
                base.VisitSelect(select);

            internal override SqlExpression VisitSubSelect(SqlSubSelect ss) => 
                this.parent.VisitSubSelect(ss);
        }

        private class Visitor : SqlVisitor
        {
            private SqlProvider.ProviderMode providerMode;
            private PostBindDotNetConverter.SqlSelectionSkipper skipper;
            private SqlFactory sql;

            internal Visitor(SqlFactory sql, SqlProvider.ProviderMode providerMode)
            {
                this.sql = sql;
                this.providerMode = providerMode;
                this.skipper = new PostBindDotNetConverter.SqlSelectionSkipper(this);
            }

            private SqlExpression CreateComparison(SqlExpression a, SqlExpression b, Expression source)
            {
                SqlExpression match = this.sql.Binary(SqlNodeType.LT, a, b);
                SqlExpression expression2 = this.sql.Binary(SqlNodeType.EQ2V, a, b);
                return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(match, this.sql.ValueFromObject(-1, false, source)), new SqlWhen(expression2, this.sql.ValueFromObject(0, false, source)) }, this.sql.ValueFromObject(1, false, source), source);
            }

            private SqlExpression CreateDateTimeFromDateAndMs(SqlExpression sqlDate, SqlExpression ms, Expression source) => 
                this.CreateDateTimeFromDateAndMs(sqlDate, ms, source, false);

            private SqlExpression CreateDateTimeFromDateAndMs(SqlExpression sqlDate, SqlExpression ms, Expression source, bool asNullable)
            {
                SqlExpression expr = this.sql.ConvertToBigint(ms);
                SqlExpression expression2 = this.sql.DATEADD("day", this.sql.Divide(expr, (long) 0x5265c00L), sqlDate, source, asNullable);
                return this.sql.DATEADD("ms", this.sql.Mod(expr, 0x5265c00L), expression2, source, asNullable);
            }

            private SqlExpression CreateDateTimeFromDateAndTicks(SqlExpression sqlDate, SqlExpression sqlTicks, Expression source) => 
                this.CreateDateTimeFromDateAndTicks(sqlDate, sqlTicks, source, false);

            private SqlExpression CreateDateTimeFromDateAndTicks(SqlExpression sqlDate, SqlExpression sqlTicks, Expression source, bool asNullable)
            {
                SqlExpression expr = this.sql.DATEADD("day", this.sql.Divide(sqlTicks, (long) 0xc92a69c000L), sqlDate, source, asNullable);
                return this.sql.DATEADD("ms", this.sql.Mod(this.sql.Divide(sqlTicks, (long) 0x2710L), 0x5265c00L), expr, source, asNullable);
            }

            private SqlExpression CreateDateTimeOffsetFromDateAndMs(SqlExpression sqlDate, SqlExpression ms, Expression source) => 
                this.CreateDateTimeOffsetFromDateAndMs(sqlDate, ms, source, false);

            private SqlExpression CreateDateTimeOffsetFromDateAndMs(SqlExpression sqlDate, SqlExpression ms, Expression source, bool asNullable)
            {
                SqlExpression expr = this.sql.ConvertToBigint(ms);
                SqlExpression expression2 = this.sql.DATETIMEOFFSETADD("day", this.sql.Divide(expr, (long) 0x5265c00L), sqlDate, source, asNullable);
                return this.sql.DATETIMEOFFSETADD("ms", this.sql.Mod(expr, 0x5265c00L), expression2, source, asNullable);
            }

            private SqlExpression CreateDateTimeOffsetFromDateAndTicks(SqlExpression sqlDate, SqlExpression sqlTicks, Expression source) => 
                this.CreateDateTimeOffsetFromDateAndTicks(sqlDate, sqlTicks, source, false);

            private SqlExpression CreateDateTimeOffsetFromDateAndTicks(SqlExpression sqlDate, SqlExpression sqlTicks, Expression source, bool asNullable)
            {
                SqlExpression expr = this.sql.DATETIMEOFFSETADD("day", this.sql.Divide(sqlTicks, (long) 0xc92a69c000L), sqlDate, source, asNullable);
                return this.sql.DATETIMEOFFSETADD("ms", this.sql.Mod(this.sql.Divide(sqlTicks, (long) 0x2710L), 0x5265c00L), expr, source, asNullable);
            }

            private SqlExpression CreateFunctionCallStatic1(Type type, string functionName, List<SqlExpression> arguments, Expression source) => 
                this.sql.FunctionCall(type, functionName, new SqlExpression[] { arguments[0] }, source);

            private SqlExpression CreateFunctionCallStatic2(Type type, string functionName, List<SqlExpression> arguments, Expression source) => 
                this.sql.FunctionCall(type, functionName, new SqlExpression[] { arguments[0], arguments[1] }, source);

            internal static Exception GetMethodSupportException(SqlMethodCall mc)
            {
                if (PostBindDotNetConverter.GetMethodSupport(mc) == PostBindDotNetConverter.MethodSupport.MethodGroup)
                {
                    return System.Data.Linq.SqlClient.Error.MethodFormHasNoSupportConversionToSql(mc.Method.Name, mc.Method);
                }
                return System.Data.Linq.SqlClient.Error.MethodHasNoSupportConversionToSql(mc.Method);
            }

            private SqlExpression TranslateConvertStaticMethod(SqlMethodCall mc)
            {
                SqlExpression expression = null;
                if (mc.Arguments.Count != 1)
                {
                    return expression;
                }
                SqlExpression expr = mc.Arguments[0];
                Type type = null;
                switch (mc.Method.Name)
                {
                    case "ToBoolean":
                        type = typeof(bool);
                        break;

                    case "ToDecimal":
                        type = typeof(decimal);
                        break;

                    case "ToByte":
                        type = typeof(byte);
                        break;

                    case "ToChar":
                        type = typeof(char);
                        if (expr.SqlType.IsChar)
                        {
                            this.sql.TypeProvider.From(type, 1);
                        }
                        break;

                    case "ToDateTime":
                    {
                        Type nonNullableType = TypeSystem.GetNonNullableType(expr.ClrType);
                        if ((nonNullableType != typeof(string)) && (nonNullableType != typeof(DateTime)))
                        {
                            throw System.Data.Linq.SqlClient.Error.ConvertToDateTimeOnlyForDateTimeOrString();
                        }
                        type = typeof(DateTime);
                        break;
                    }
                    case "ToDouble":
                        type = typeof(double);
                        break;

                    case "ToInt16":
                        type = typeof(short);
                        break;

                    case "ToInt32":
                        type = typeof(int);
                        break;

                    case "ToInt64":
                        type = typeof(long);
                        break;

                    case "ToSingle":
                        type = typeof(float);
                        break;

                    case "ToString":
                        type = typeof(string);
                        break;

                    default:
                        throw GetMethodSupportException(mc);
                }
                if ((this.sql.TypeProvider.From(type) != expr.SqlType) || ((expr.ClrType == typeof(bool)) && (type == typeof(int))))
                {
                    return this.sql.ConvertTo(type, expr);
                }
                if (type == null)
                {
                    return expression;
                }
                if (this.sql.TypeProvider.From(type) != expr.SqlType)
                {
                    return this.sql.ConvertTo(type, expr);
                }
                if ((type != expr.ClrType) && (TypeSystem.GetNonNullableType(type) == TypeSystem.GetNonNullableType(expr.ClrType)))
                {
                    return new SqlLift(type, expr, expr.SourceExpression);
                }
                return expr;
            }

            private SqlExpression TranslateDateTimeBinary(SqlBinary bo)
            {
                bool asNullable = TypeSystem.IsNullableType(bo.ClrType);
                Type nonNullableType = TypeSystem.GetNonNullableType(bo.Right.ClrType);
                SqlNodeType nodeType = bo.NodeType;
                if (nodeType != SqlNodeType.Add)
                {
                    if (nodeType != SqlNodeType.Sub)
                    {
                        return bo;
                    }
                    if (nonNullableType == typeof(DateTime))
                    {
                        Type clrType = bo.ClrType;
                        SqlExpression left = bo.Left;
                        SqlExpression expression2 = bo.Right;
                        SqlExpression expression3 = new SqlVariable(typeof(void), null, "DAY", bo.SourceExpression);
                        SqlExpression expression4 = new SqlVariable(typeof(void), null, "MILLISECOND", bo.SourceExpression);
                        SqlExpression expr = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression3, expression2, left }, bo.SourceExpression);
                        SqlExpression expression6 = this.sql.FunctionCall(typeof(DateTime), "DATEADD", new SqlExpression[] { expression3, expr, expression2 }, bo.SourceExpression);
                        SqlExpression expression7 = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression4, expression6, left }, bo.SourceExpression);
                        SqlExpression expression8 = this.sql.Multiply(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(expr), 0x5265c00L), expression7 }), 0x2710L);
                        return this.sql.ConvertTo(clrType, expression8);
                    }
                    if (nonNullableType == typeof(DateTimeOffset))
                    {
                        Type type3 = bo.ClrType;
                        SqlExpression expression9 = bo.Left;
                        SqlExpression expression10 = bo.Right;
                        SqlExpression expression11 = new SqlVariable(typeof(void), null, "DAY", bo.SourceExpression);
                        SqlExpression expression12 = new SqlVariable(typeof(void), null, "MILLISECOND", bo.SourceExpression);
                        SqlExpression expression13 = new SqlVariable(typeof(void), null, "MICROSECOND", bo.SourceExpression);
                        SqlExpression expression14 = new SqlVariable(typeof(void), null, "NANOSECOND", bo.SourceExpression);
                        SqlExpression expression15 = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression11, expression10, expression9 }, bo.SourceExpression);
                        SqlExpression expression16 = this.sql.FunctionCall(typeof(DateTimeOffset), "DATEADD", new SqlExpression[] { expression11, expression15, expression10 }, bo.SourceExpression);
                        SqlExpression expression17 = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression12, expression16, expression9 }, bo.SourceExpression);
                        SqlExpression expression18 = this.sql.FunctionCall(typeof(DateTimeOffset), "DATEADD", new SqlExpression[] { expression12, expression17, expression16 }, bo.SourceExpression);
                        SqlExpression expression19 = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression13, expression18, expression9 }, bo.SourceExpression);
                        SqlExpression expression20 = this.sql.FunctionCall(typeof(DateTimeOffset), "DATEADD", new SqlExpression[] { expression13, expression19, expression18 }, bo.SourceExpression);
                        SqlExpression expression21 = this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression14, expression20, expression9 }, bo.SourceExpression);
                        this.sql.FunctionCall(typeof(DateTimeOffset), "DATEADD", new SqlExpression[] { expression14, expression21, expression20 }, bo.SourceExpression);
                        SqlExpression expression22 = this.sql.Add(new SqlExpression[] { this.sql.Divide(expression21, (long) 100L), this.sql.Multiply(expression19, 10L), this.sql.Multiply(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(expression15), 0x5265c00L), expression17 }), 0x2710L) });
                        return this.sql.ConvertTo(type3, expression22);
                    }
                    if (nonNullableType != typeof(TimeSpan))
                    {
                        return bo;
                    }
                    SqlExpression right = bo.Right;
                    if (this.sql.IsSqlTimeType(bo.Right))
                    {
                        SqlExpression expression24 = this.sql.DATEPART("NANOSECOND", right);
                        SqlExpression expression25 = this.sql.DATEPART("SECOND", right);
                        SqlExpression expression26 = this.sql.DATEPART("MINUTE", right);
                        SqlExpression expression27 = this.sql.DATEPART("HOUR", right);
                        right = this.sql.Add(new SqlExpression[] { this.sql.Divide(expression24, (long) 100L), this.sql.Multiply(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(expression27), 0x36ee80L), this.sql.Multiply(this.sql.ConvertToBigint(expression26), 0xea60L), this.sql.Multiply(this.sql.ConvertToBigint(expression25), 0x3e8L) }), 0x2710L) });
                    }
                    if (TypeSystem.GetNonNullableType(bo.Left.ClrType) != typeof(DateTimeOffset))
                    {
                        return this.CreateDateTimeFromDateAndTicks(bo.Left, this.sql.Unary(SqlNodeType.Negate, right, bo.SourceExpression), bo.SourceExpression, asNullable);
                    }
                    return this.CreateDateTimeOffsetFromDateAndTicks(bo.Left, this.sql.Unary(SqlNodeType.Negate, right, bo.SourceExpression), bo.SourceExpression, asNullable);
                }
                if (nonNullableType != typeof(TimeSpan))
                {
                    return bo;
                }
                if (this.sql.IsSqlTimeType(bo.Right))
                {
                    return this.sql.AddTimeSpan(bo.Left, bo.Right, asNullable);
                }
                if (TypeSystem.GetNonNullableType(bo.Left.ClrType) == typeof(DateTimeOffset))
                {
                    return this.CreateDateTimeOffsetFromDateAndTicks(bo.Left, bo.Right, bo.SourceExpression, asNullable);
                }
                return this.CreateDateTimeFromDateAndTicks(bo.Left, bo.Right, bo.SourceExpression, asNullable);
            }

            private SqlExpression TranslateDateTimeInstanceMethod(SqlMethodCall mc)
            {
                SqlExpression expression = null;
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Method.Name == "CompareTo")
                {
                    return this.CreateComparison(mc.Object, mc.Arguments[0], sourceExpression);
                }
                if ((((mc.Method.Name == "Add") && (mc.Arguments.Count == 1)) && (mc.Arguments[0].ClrType == typeof(TimeSpan))) || (mc.Method.Name == "AddTicks"))
                {
                    SqlExpression sqlTicks = mc.Arguments[0];
                    if (this.sql.IsSqlTimeType(mc.Arguments[0]))
                    {
                        SqlExpression expr = this.sql.DATEPART("NANOSECOND", mc.Arguments[0]);
                        SqlExpression expression5 = this.sql.DATEPART("SECOND", mc.Arguments[0]);
                        SqlExpression expression6 = this.sql.DATEPART("MINUTE", mc.Arguments[0]);
                        SqlExpression expression7 = this.sql.DATEPART("HOUR", mc.Arguments[0]);
                        sqlTicks = this.sql.Add(new SqlExpression[] { this.sql.Divide(expr, (long) 100L), this.sql.Multiply(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(expression7), 0x36ee80L), this.sql.Multiply(this.sql.ConvertToBigint(expression6), 0xea60L), this.sql.Multiply(this.sql.ConvertToBigint(expression5), 0x3e8L) }), 0x2710L) });
                    }
                    return this.CreateDateTimeFromDateAndTicks(mc.Object, sqlTicks, sourceExpression);
                }
                if (mc.Method.Name == "AddMonths")
                {
                    return this.sql.DATEADD("MONTH", mc.Arguments[0], mc.Object);
                }
                if (mc.Method.Name == "AddYears")
                {
                    return this.sql.DATEADD("YEAR", mc.Arguments[0], mc.Object);
                }
                if (mc.Method.Name == "AddMilliseconds")
                {
                    return this.CreateDateTimeFromDateAndMs(mc.Object, mc.Arguments[0], sourceExpression);
                }
                if (mc.Method.Name == "AddSeconds")
                {
                    SqlExpression ms = this.sql.Multiply(mc.Arguments[0], 0x3e8L);
                    return this.CreateDateTimeFromDateAndMs(mc.Object, ms, sourceExpression);
                }
                if (mc.Method.Name == "AddMinutes")
                {
                    SqlExpression expression9 = this.sql.Multiply(mc.Arguments[0], 0xea60L);
                    return this.CreateDateTimeFromDateAndMs(mc.Object, expression9, sourceExpression);
                }
                if (mc.Method.Name == "AddHours")
                {
                    SqlExpression expression10 = this.sql.Multiply(mc.Arguments[0], 0x36ee80L);
                    return this.CreateDateTimeFromDateAndMs(mc.Object, expression10, sourceExpression);
                }
                if (mc.Method.Name == "AddDays")
                {
                    SqlExpression expression11 = this.sql.Multiply(mc.Arguments[0], 0x5265c00L);
                    expression = this.CreateDateTimeFromDateAndMs(mc.Object, expression11, sourceExpression);
                }
                return expression;
            }

            private SqlExpression TranslateDateTimeOffsetInstanceMethod(SqlMethodCall mc)
            {
                SqlExpression expression = null;
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Method.Name == "CompareTo")
                {
                    return this.CreateComparison(mc.Object, mc.Arguments[0], sourceExpression);
                }
                if ((((mc.Method.Name == "Add") && (mc.Arguments.Count == 1)) && (mc.Arguments[0].ClrType == typeof(TimeSpan))) || (mc.Method.Name == "AddTicks"))
                {
                    SqlExpression expr = this.sql.DATEPART("NANOSECOND", mc.Arguments[0]);
                    SqlExpression expression4 = this.sql.DATEPART("SECOND", mc.Arguments[0]);
                    SqlExpression expression5 = this.sql.DATEPART("MINUTE", mc.Arguments[0]);
                    SqlExpression expression6 = this.sql.DATEPART("HOUR", mc.Arguments[0]);
                    SqlExpression sqlTicks = this.sql.Add(new SqlExpression[] { this.sql.Divide(expr, (long) 100L), this.sql.Multiply(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(expression6), 0x36ee80L), this.sql.Multiply(this.sql.ConvertToBigint(expression5), 0xea60L), this.sql.Multiply(this.sql.ConvertToBigint(expression4), 0x3e8L) }), 0x2710L) });
                    return this.CreateDateTimeOffsetFromDateAndTicks(mc.Object, sqlTicks, sourceExpression);
                }
                if (mc.Method.Name == "AddMonths")
                {
                    return this.sql.DATETIMEOFFSETADD("MONTH", mc.Arguments[0], mc.Object);
                }
                if (mc.Method.Name == "AddYears")
                {
                    return this.sql.DATETIMEOFFSETADD("YEAR", mc.Arguments[0], mc.Object);
                }
                if (mc.Method.Name == "AddMilliseconds")
                {
                    return this.CreateDateTimeOffsetFromDateAndMs(mc.Object, mc.Arguments[0], sourceExpression);
                }
                if (mc.Method.Name == "AddSeconds")
                {
                    SqlExpression ms = this.sql.Multiply(mc.Arguments[0], 0x3e8L);
                    return this.CreateDateTimeOffsetFromDateAndMs(mc.Object, ms, sourceExpression);
                }
                if (mc.Method.Name == "AddMinutes")
                {
                    SqlExpression expression9 = this.sql.Multiply(mc.Arguments[0], 0xea60L);
                    return this.CreateDateTimeOffsetFromDateAndMs(mc.Object, expression9, sourceExpression);
                }
                if (mc.Method.Name == "AddHours")
                {
                    SqlExpression expression10 = this.sql.Multiply(mc.Arguments[0], 0x36ee80L);
                    return this.CreateDateTimeOffsetFromDateAndMs(mc.Object, expression10, sourceExpression);
                }
                if (mc.Method.Name == "AddDays")
                {
                    SqlExpression expression11 = this.sql.Multiply(mc.Arguments[0], 0x5265c00L);
                    expression = this.CreateDateTimeOffsetFromDateAndMs(mc.Object, expression11, sourceExpression);
                }
                return expression;
            }

            internal SqlExpression TranslateDecimalMethod(SqlMethodCall mc)
            {
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Method.IsStatic)
                {
                    if (mc.Arguments.Count == 2)
                    {
                        switch (mc.Method.Name)
                        {
                            case "Multiply":
                                return this.sql.Binary(SqlNodeType.Mul, mc.Arguments[0], mc.Arguments[1]);

                            case "Divide":
                                return this.sql.Binary(SqlNodeType.Div, mc.Arguments[0], mc.Arguments[1]);

                            case "Subtract":
                                return this.sql.Binary(SqlNodeType.Sub, mc.Arguments[0], mc.Arguments[1]);

                            case "Add":
                                return this.sql.Binary(SqlNodeType.Add, mc.Arguments[0], mc.Arguments[1]);

                            case "Remainder":
                                return this.sql.Binary(SqlNodeType.Mod, mc.Arguments[0], mc.Arguments[1]);

                            case "Round":
                                return this.sql.FunctionCall(mc.Method.ReturnType, "ROUND", mc.Arguments, mc.SourceExpression);
                        }
                    }
                    else if (mc.Arguments.Count == 1)
                    {
                        switch (mc.Method.Name)
                        {
                            case "Negate":
                                return this.sql.Unary(SqlNodeType.Negate, mc.Arguments[0], sourceExpression);

                            case "Floor":
                            case "Truncate":
                                return this.sql.FunctionCall(mc.Method.ReturnType, "ROUND", new SqlExpression[] { mc.Arguments[0], this.sql.ValueFromObject(0, false, mc.SourceExpression), this.sql.ValueFromObject(1, false, mc.SourceExpression) }, mc.SourceExpression);

                            case "Round":
                                return this.sql.FunctionCall(mc.Method.ReturnType, "ROUND", new SqlExpression[] { mc.Arguments[0], this.sql.ValueFromObject(0, false, mc.SourceExpression) }, mc.SourceExpression);
                        }
                        if (mc.Method.Name.StartsWith("To", StringComparison.Ordinal))
                        {
                            return this.TranslateConvertStaticMethod(mc);
                        }
                    }
                }
                throw GetMethodSupportException(mc);
            }

            private SqlExpression TranslateGetValueOrDefaultMethod(SqlMethodCall mc)
            {
                if (mc.Arguments.Count == 0)
                {
                    Type type = mc.Object.ClrType.GetGenericArguments()[0];
                    return this.sql.Binary(SqlNodeType.Coalesce, mc.Object, this.sql.ValueFromObject(Activator.CreateInstance(type), mc.SourceExpression));
                }
                return this.sql.Binary(SqlNodeType.Coalesce, mc.Object, mc.Arguments[0]);
            }

            private SqlExpression TranslateMathMethod(SqlMethodCall mc)
            {
                Expression sourceExpression = mc.SourceExpression;
                switch (mc.Method.Name)
                {
                    case "Abs":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.sql.FunctionCall(mc.Arguments[0].ClrType, "ABS", new SqlExpression[] { mc.Arguments[0] }, sourceExpression);

                    case "Acos":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "ACOS", mc.Arguments, sourceExpression);

                    case "Asin":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "ASIN", mc.Arguments, sourceExpression);

                    case "Atan":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "ATAN", mc.Arguments, sourceExpression);

                    case "Atan2":
                        if (mc.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic2(typeof(double), "ATN2", mc.Arguments, sourceExpression);

                    case "BigMul":
                        if (mc.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.sql.Multiply(new SqlExpression[] { this.sql.ConvertToBigint(mc.Arguments[0]), this.sql.ConvertToBigint(mc.Arguments[1]) });

                    case "Ceiling":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(mc.Arguments[0].ClrType, "CEILING", mc.Arguments, sourceExpression);

                    case "Cos":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "COS", mc.Arguments, sourceExpression);

                    case "Cosh":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        SqlExpression expression = mc.Arguments[0];
                        SqlExpression expression3 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression }, sourceExpression);
                        SqlExpression expression4 = this.sql.Unary(SqlNodeType.Negate, expression, sourceExpression);
                        SqlExpression expression5 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression4 }, sourceExpression);
                        return this.sql.Divide(this.sql.Add(new SqlExpression[] { expression3, expression5 }), (long) 2L);
                    }
                    case "Exp":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "EXP", mc.Arguments, sourceExpression);

                    case "Floor":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(mc.Arguments[0].ClrType, "FLOOR", mc.Arguments, sourceExpression);

                    case "Log":
                        if (mc.Arguments.Count != 1)
                        {
                            if (mc.Arguments.Count != 2)
                            {
                                break;
                            }
                            SqlExpression first = this.sql.FunctionCall(typeof(double), "LOG", new SqlExpression[] { mc.Arguments[0] }, sourceExpression);
                            SqlExpression second = this.sql.FunctionCall(typeof(double), "LOG", new SqlExpression[] { mc.Arguments[1] }, sourceExpression);
                            return this.sql.Divide(first, second);
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "LOG", mc.Arguments, sourceExpression);

                    case "Log10":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "LOG10", mc.Arguments, sourceExpression);

                    case "Max":
                    {
                        if (mc.Arguments.Count != 2)
                        {
                            break;
                        }
                        SqlExpression left = mc.Arguments[0];
                        SqlExpression right = mc.Arguments[1];
                        SqlExpression match = this.sql.Binary(SqlNodeType.LT, left, right);
                        return new SqlSearchedCase(mc.Method.ReturnType, new SqlWhen[] { new SqlWhen(match, right) }, left, sourceExpression);
                    }
                    case "Min":
                    {
                        if (mc.Arguments.Count != 2)
                        {
                            break;
                        }
                        SqlExpression expression11 = mc.Arguments[0];
                        SqlExpression expression12 = mc.Arguments[1];
                        SqlExpression expression13 = this.sql.Binary(SqlNodeType.LT, expression11, expression12);
                        return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression13, expression11) }, expression12, sourceExpression);
                    }
                    case "Pow":
                        if (mc.Arguments.Count != 2)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic2(mc.ClrType, "POWER", mc.Arguments, sourceExpression);

                    case "Round":
                    {
                        int count = mc.Arguments.Count;
                        if (mc.Arguments[count - 1].ClrType != typeof(MidpointRounding))
                        {
                            throw System.Data.Linq.SqlClient.Error.MathRoundNotSupported();
                        }
                        SqlExpression expr = mc.Arguments[0];
                        SqlExpression expression15 = null;
                        if (count == 2)
                        {
                            expression15 = this.sql.ValueFromObject(0, false, sourceExpression);
                        }
                        else
                        {
                            expression15 = mc.Arguments[1];
                        }
                        SqlExpression expression16 = mc.Arguments[count - 1];
                        if (expression16.NodeType != SqlNodeType.Value)
                        {
                            throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedForRounding();
                        }
                        if (((MidpointRounding) base.Eval(expression16)) == MidpointRounding.AwayFromZero)
                        {
                            return this.sql.FunctionCall(expr.ClrType, "round", new SqlExpression[] { expr, expression15 }, sourceExpression);
                        }
                        Type clrType = expr.ClrType;
                        SqlExpression expression17 = this.sql.FunctionCall(clrType, "round", new SqlExpression[] { expr, expression15 }, sourceExpression);
                        SqlExpression expression18 = this.sql.Multiply(expr, 2L);
                        SqlExpression expression19 = this.sql.FunctionCall(clrType, "round", new SqlExpression[] { expression18, expression15 }, sourceExpression);
                        SqlExpression expression20 = this.sql.AndAccumulate(this.sql.Binary(SqlNodeType.EQ, expression18, expression19), this.sql.Binary(SqlNodeType.NE, expr, expression17));
                        SqlExpression expression21 = this.sql.Multiply(this.sql.FunctionCall(clrType, "round", new SqlExpression[] { this.sql.Divide(expr, (long) 2L), expression15 }, sourceExpression), 2L);
                        return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression20, expression21) }, expression17, sourceExpression);
                    }
                    case "Sign":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.sql.FunctionCall(typeof(int), "SIGN", new SqlExpression[] { mc.Arguments[0] }, sourceExpression);

                    case "Sin":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "SIN", mc.Arguments, sourceExpression);

                    case "Sinh":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        SqlExpression expression22 = mc.Arguments[0];
                        SqlExpression expression23 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression22 }, sourceExpression);
                        SqlExpression expression24 = this.sql.Unary(SqlNodeType.Negate, expression22, sourceExpression);
                        SqlExpression expression25 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression24 }, sourceExpression);
                        return this.sql.Divide(this.sql.Subtract(expression23, expression25), (long) 2L);
                    }
                    case "Sqrt":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "SQRT", mc.Arguments, sourceExpression);

                    case "Tan":
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        return this.CreateFunctionCallStatic1(typeof(double), "TAN", mc.Arguments, sourceExpression);

                    case "Tanh":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        SqlExpression expression26 = mc.Arguments[0];
                        SqlExpression expression27 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression26 }, sourceExpression);
                        SqlExpression expression28 = this.sql.Unary(SqlNodeType.Negate, expression26, sourceExpression);
                        SqlExpression expression29 = this.sql.FunctionCall(typeof(double), "EXP", new SqlExpression[] { expression28 }, sourceExpression);
                        return this.sql.Divide(this.sql.Subtract(expression27, expression29), this.sql.Add(new SqlExpression[] { expression27, expression29 }));
                    }
                    case "Truncate":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            break;
                        }
                        SqlExpression expression30 = mc.Arguments[0];
                        return this.sql.FunctionCall(mc.Method.ReturnType, "ROUND", new SqlExpression[] { expression30, this.sql.ValueFromObject(0, false, sourceExpression), this.sql.ValueFromObject(1, false, sourceExpression) }, sourceExpression);
                    }
                }
                throw GetMethodSupportException(mc);
            }

            private SqlExpression TranslateNewDateTime(SqlNew sox)
            {
                Expression sourceExpression = sox.SourceExpression;
                if ((((sox.ClrType == typeof(DateTime)) && (sox.Args.Count >= 3)) && ((sox.Args[0].ClrType == typeof(int)) && (sox.Args[1].ClrType == typeof(int)))) && (sox.Args[2].ClrType == typeof(int)))
                {
                    SqlExpression expression2 = this.sql.FunctionCall(typeof(void), "NCHAR", new SqlExpression[] { this.sql.ValueFromObject(2, false, sourceExpression) }, sourceExpression);
                    SqlExpression expression3 = this.sql.FunctionCall(typeof(void), "NCHAR", new SqlExpression[] { this.sql.ValueFromObject(4, false, sourceExpression) }, sourceExpression);
                    SqlExpression expression4 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression3, sox.Args[0] }, sourceExpression);
                    SqlExpression expression5 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[1] }, sourceExpression);
                    SqlExpression expression6 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[2] }, sourceExpression);
                    SqlExpression expression7 = new SqlVariable(typeof(void), null, "DATETIME", sourceExpression);
                    if (sox.Args.Count == 3)
                    {
                        SqlExpression expression8 = this.sql.Concat(new SqlExpression[] { expression5, this.sql.ValueFromObject("/", false, sourceExpression), expression6, this.sql.ValueFromObject("/", false, sourceExpression), expression4 });
                        return this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression7, expression8, this.sql.ValueFromObject(0x65, false, sourceExpression) }, sourceExpression);
                    }
                    if (((sox.Args.Count >= 6) && (sox.Args[3].ClrType == typeof(int))) && ((sox.Args[4].ClrType == typeof(int)) && (sox.Args[5].ClrType == typeof(int))))
                    {
                        SqlExpression expression9 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[3] }, sourceExpression);
                        SqlExpression expression10 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[4] }, sourceExpression);
                        SqlExpression expression11 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[5] }, sourceExpression);
                        SqlExpression expression12 = this.sql.Concat(new SqlExpression[] { expression4, this.sql.ValueFromObject("-", false, sourceExpression), expression5, this.sql.ValueFromObject("-", false, sourceExpression), expression6 });
                        SqlExpression expression13 = this.sql.Concat(new SqlExpression[] { expression9, this.sql.ValueFromObject(":", false, sourceExpression), expression10, this.sql.ValueFromObject(":", false, sourceExpression), expression11 });
                        SqlExpression expression14 = this.sql.Concat(new SqlExpression[] { expression12, this.sql.ValueFromObject(' ', false, sourceExpression), expression13 });
                        if (sox.Args.Count == 6)
                        {
                            return this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression7, expression14, this.sql.ValueFromObject(120, false, sourceExpression) }, sourceExpression);
                        }
                        if ((sox.Args.Count == 7) && (sox.Args[6].ClrType == typeof(int)))
                        {
                            SqlExpression expression16;
                            SqlExpression expression15 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression3, this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(0x3e8, false, sourceExpression), sox.Args[6] }) }, sourceExpression);
                            if (this.providerMode == SqlProvider.ProviderMode.SqlCE)
                            {
                                SqlExpression left = this.sql.FunctionCall(typeof(int), "LEN", new SqlExpression[] { expression15 }, sourceExpression);
                                SqlExpression expression18 = this.sql.Binary(SqlNodeType.Sub, left, this.sql.ValueFromObject(2, false, sourceExpression));
                                expression16 = this.sql.FunctionCall(typeof(string), "SUBSTRING", new SqlExpression[] { expression15, expression18, this.sql.ValueFromObject(3, false, sourceExpression) }, sourceExpression);
                            }
                            else
                            {
                                expression16 = this.sql.FunctionCall(typeof(string), "RIGHT", new SqlExpression[] { expression15, this.sql.ValueFromObject(3, false, sourceExpression) }, sourceExpression);
                            }
                            expression14 = this.sql.Concat(new SqlExpression[] { expression14, this.sql.ValueFromObject('.', false, sourceExpression), expression16 });
                            return this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression7, expression14, this.sql.ValueFromObject(0x79, false, sourceExpression) }, sourceExpression);
                        }
                    }
                }
                throw System.Data.Linq.SqlClient.Error.UnsupportedDateTimeConstructorForm();
            }

            private SqlExpression TranslateNewDateTimeOffset(SqlNew sox)
            {
                Expression sourceExpression = sox.SourceExpression;
                if (sox.ClrType == typeof(DateTimeOffset))
                {
                    if ((sox.Args.Count == 1) && (sox.Args[0].ClrType == typeof(DateTime)))
                    {
                        return this.sql.FunctionCall(typeof(DateTimeOffset), "TODATETIMEOFFSET", new SqlExpression[] { sox.Args[0], this.sql.ValueFromObject(0, false, sourceExpression) }, sourceExpression);
                    }
                    if (((sox.Args.Count == 2) && (sox.Args[0].ClrType == typeof(DateTime))) && (sox.Args[1].ClrType == typeof(TimeSpan)))
                    {
                        return this.sql.FunctionCall(typeof(DateTimeOffset), "TODATETIMEOFFSET", new SqlExpression[] { sox.Args[0], this.sql.ConvertToInt(this.sql.ConvertToBigint(this.sql.Divide(this.sql.ConvertTimeToDouble(sox.Args[1]), (long) 0x23c34600L))) }, sourceExpression);
                    }
                    if ((((sox.Args.Count >= 7) && (sox.Args[0].ClrType == typeof(int))) && ((sox.Args[1].ClrType == typeof(int)) && (sox.Args[2].ClrType == typeof(int)))) && (((sox.Args[3].ClrType == typeof(int)) && (sox.Args[4].ClrType == typeof(int))) && (sox.Args[5].ClrType == typeof(int))))
                    {
                        SqlExpression expression15;
                        SqlExpression expression16;
                        int num;
                        SqlExpression expression2 = this.sql.FunctionCall(typeof(void), "NCHAR", new SqlExpression[] { this.sql.ValueFromObject(2, false, sourceExpression) }, sourceExpression);
                        SqlExpression expression3 = this.sql.FunctionCall(typeof(void), "NCHAR", new SqlExpression[] { this.sql.ValueFromObject(4, false, sourceExpression) }, sourceExpression);
                        SqlExpression expression4 = this.sql.FunctionCall(typeof(void), "NCHAR", new SqlExpression[] { this.sql.ValueFromObject(5, false, sourceExpression) }, sourceExpression);
                        SqlExpression expression5 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression4, this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(0x2710, false, sourceExpression), sox.Args[0] }) }, sourceExpression);
                        SqlExpression expression6 = this.sql.FunctionCall(typeof(string), "RIGHT", new SqlExpression[] { expression5, this.sql.ValueFromObject(4, false, sourceExpression) }, sourceExpression);
                        SqlExpression expression7 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[1] }, sourceExpression);
                        SqlExpression expression8 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[2] }, sourceExpression);
                        SqlExpression expression9 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[3] }, sourceExpression);
                        SqlExpression expression10 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[4] }, sourceExpression);
                        SqlExpression expression11 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression2, sox.Args[5] }, sourceExpression);
                        SqlExpression expression12 = this.sql.Concat(new SqlExpression[] { expression6, this.sql.ValueFromObject("-", false, sourceExpression), expression7, this.sql.ValueFromObject("-", false, sourceExpression), expression8 });
                        SqlExpression expression13 = this.sql.Concat(new SqlExpression[] { expression9, this.sql.ValueFromObject(":", false, sourceExpression), expression10, this.sql.ValueFromObject(":", false, sourceExpression), expression11 });
                        SqlExpression expression14 = new SqlVariable(typeof(void), null, "DATETIMEOFFSET", sourceExpression);
                        if ((sox.Args.Count == 7) && (sox.Args[6].ClrType == typeof(TimeSpan)))
                        {
                            num = 6;
                            expression16 = this.sql.Concat(new SqlExpression[] { expression12, this.sql.ValueFromObject(' ', false, sourceExpression), expression13 });
                            expression15 = this.sql.FunctionCall(typeof(DateTimeOffset), "CONVERT", new SqlExpression[] { expression14, expression16, this.sql.ValueFromObject(120, false, sourceExpression) }, sourceExpression);
                        }
                        else
                        {
                            if (((sox.Args.Count != 8) || (sox.Args[6].ClrType != typeof(int))) || (sox.Args[7].ClrType != typeof(TimeSpan)))
                            {
                                throw System.Data.Linq.SqlClient.Error.UnsupportedDateTimeOffsetConstructorForm();
                            }
                            num = 7;
                            SqlExpression expression17 = this.sql.FunctionCall(typeof(string), "CONVERT", new SqlExpression[] { expression3, this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(0x3e8, false, sourceExpression), sox.Args[6] }) }, sourceExpression);
                            SqlExpression expression18 = this.sql.FunctionCall(typeof(string), "RIGHT", new SqlExpression[] { expression17, this.sql.ValueFromObject(3, false, sourceExpression) }, sourceExpression);
                            expression16 = this.sql.Concat(new SqlExpression[] { expression12, this.sql.ValueFromObject(' ', false, sourceExpression), expression13, this.sql.ValueFromObject('.', false, sourceExpression), expression18 });
                            expression15 = this.sql.FunctionCall(typeof(DateTimeOffset), "CONVERT", new SqlExpression[] { expression14, expression16, this.sql.ValueFromObject(0x79, false, sourceExpression) }, sourceExpression);
                        }
                        return this.sql.FunctionCall(typeof(DateTimeOffset), "TODATETIMEOFFSET", new SqlExpression[] { expression15, this.sql.ConvertToInt(this.sql.ConvertToBigint(this.sql.Divide(this.sql.ConvertTimeToDouble(sox.Args[num]), (long) 0x23c34600L))) }, sourceExpression);
                    }
                }
                throw System.Data.Linq.SqlClient.Error.UnsupportedDateTimeOffsetConstructorForm();
            }

            private SqlExpression TranslateNewString(SqlNew sox)
            {
                if (((sox.ClrType != typeof(string)) || (sox.Args.Count != 2)) || ((sox.Args[0].ClrType != typeof(char)) || (sox.Args[1].ClrType != typeof(int))))
                {
                    throw System.Data.Linq.SqlClient.Error.UnsupportedStringConstructorForm();
                }
                return this.sql.FunctionCall(typeof(string), "REPLICATE", new SqlExpression[] { sox.Args[0], sox.Args[1] }, sox.SourceExpression);
            }

            private SqlExpression TranslateNewTimeSpan(SqlNew sox)
            {
                if (sox.Args.Count == 1)
                {
                    return this.sql.ConvertTo(typeof(TimeSpan), sox.Args[0]);
                }
                if (sox.Args.Count == 3)
                {
                    SqlExpression expression = this.sql.ConvertToBigint(sox.Args[0]);
                    SqlExpression expression2 = this.sql.ConvertToBigint(sox.Args[1]);
                    SqlExpression expression3 = this.sql.ConvertToBigint(sox.Args[2]);
                    SqlExpression expression4 = this.sql.Multiply(expression, 0x861c46800L);
                    SqlExpression expression5 = this.sql.Multiply(expression2, 0x23c34600L);
                    SqlExpression expression6 = this.sql.Multiply(expression3, 0x989680L);
                    return this.sql.ConvertTo(typeof(TimeSpan), this.sql.Add(new SqlExpression[] { expression4, expression5, expression6 }));
                }
                SqlExpression expr = this.sql.ConvertToBigint(sox.Args[0]);
                SqlExpression expression8 = this.sql.ConvertToBigint(sox.Args[1]);
                SqlExpression expression9 = this.sql.ConvertToBigint(sox.Args[2]);
                SqlExpression expression10 = this.sql.ConvertToBigint(sox.Args[3]);
                SqlExpression expression11 = this.sql.Multiply(expr, 0xc92a69c000L);
                SqlExpression expression12 = this.sql.Multiply(expression8, 0x861c46800L);
                SqlExpression expression13 = this.sql.Multiply(expression9, 0x23c34600L);
                SqlExpression expression14 = this.sql.Multiply(expression10, 0x989680L);
                SqlExpression expression15 = this.sql.Add(new SqlExpression[] { expression11, expression12, expression13, expression14 });
                if (sox.Args.Count == 4)
                {
                    return this.sql.ConvertTo(typeof(TimeSpan), expression15);
                }
                if (sox.Args.Count != 5)
                {
                    throw System.Data.Linq.SqlClient.Error.UnsupportedTimeSpanConstructorForm();
                }
                SqlExpression expression16 = this.sql.ConvertToBigint(sox.Args[4]);
                SqlExpression expression17 = this.sql.Multiply(expression16, 0x2710L);
                return this.sql.ConvertTo(typeof(TimeSpan), this.sql.Add(new SqlExpression[] { expression15, expression17 }));
            }

            private SqlExpression TranslateSqlMethodsMethod(SqlMethodCall mc)
            {
                Expression sourceExpression = mc.SourceExpression;
                SqlExpression expression2 = null;
                string name = mc.Method.Name;
                if (name.StartsWith("DateDiff", StringComparison.Ordinal) && (mc.Arguments.Count == 2))
                {
                    foreach (string str2 in PostBindDotNetConverter.dateParts)
                    {
                        if (mc.Method.Name == ("DateDiff" + str2))
                        {
                            SqlExpression expression3 = mc.Arguments[0];
                            SqlExpression expression4 = mc.Arguments[1];
                            SqlExpression expression5 = new SqlVariable(typeof(void), null, str2, sourceExpression);
                            return this.sql.FunctionCall(typeof(int), "DATEDIFF", new SqlExpression[] { expression5, expression3, expression4 }, sourceExpression);
                        }
                    }
                    return expression2;
                }
                if (name == "Like")
                {
                    if (mc.Arguments.Count == 2)
                    {
                        return this.sql.Like(mc.Arguments[0], mc.Arguments[1], null, sourceExpression);
                    }
                    if (mc.Arguments.Count != 3)
                    {
                        return expression2;
                    }
                    return this.sql.Like(mc.Arguments[0], mc.Arguments[1], this.sql.ConvertTo(typeof(string), mc.Arguments[2]), sourceExpression);
                }
                if (name == "RawLength")
                {
                    return this.sql.DATALENGTH(mc.Arguments[0]);
                }
                return expression2;
            }

            private SqlExpression TranslateStringMethod(SqlMethodCall mc)
            {
                SqlExpression expression2;
                SqlExpression expression3;
                bool flag;
                Expression sourceExpression = mc.SourceExpression;
                switch (mc.Method.Name)
                {
                    case "Contains":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            goto Label_1B30;
                        }
                        expression2 = mc.Arguments[0];
                        expression3 = null;
                        flag = true;
                        if (expression2.NodeType != SqlNodeType.Value)
                        {
                            if (expression2.NodeType != SqlNodeType.ClientParameter)
                            {
                                throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedFor("String.Contains");
                            }
                            SqlClientParameter parameter = (SqlClientParameter) expression2;
                            expression2 = new SqlClientParameter(parameter.ClrType, parameter.SqlType, Expression.Lambda(Expression.Call(typeof(SqlHelpers), "GetStringContainsPattern", Type.EmptyTypes, new Expression[] { parameter.Accessor.Body, Expression.Constant('~') }), new ParameterExpression[] { parameter.Accessor.Parameters[0] }), parameter.SourceExpression);
                            break;
                        }
                        string stringContainsPattern = SqlHelpers.GetStringContainsPattern((string) ((SqlValue) expression2).Value, '~');
                        flag = stringContainsPattern.Contains("~");
                        expression2 = this.sql.ValueFromObject(stringContainsPattern, true, expression2.SourceExpression);
                        break;
                    }
                    case "StartsWith":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            goto Label_1B30;
                        }
                        SqlExpression pattern = mc.Arguments[0];
                        SqlExpression escape = null;
                        bool flag2 = true;
                        if (pattern.NodeType != SqlNodeType.Value)
                        {
                            if (pattern.NodeType != SqlNodeType.ClientParameter)
                            {
                                throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedFor("String.StartsWith");
                            }
                            SqlClientParameter parameter2 = (SqlClientParameter) pattern;
                            pattern = new SqlClientParameter(parameter2.ClrType, parameter2.SqlType, Expression.Lambda(Expression.Call(typeof(SqlHelpers), "GetStringStartsWithPattern", Type.EmptyTypes, new Expression[] { parameter2.Accessor.Body, Expression.Constant('~') }), new ParameterExpression[] { parameter2.Accessor.Parameters[0] }), parameter2.SourceExpression);
                        }
                        else
                        {
                            string stringStartsWithPattern = SqlHelpers.GetStringStartsWithPattern((string) ((SqlValue) pattern).Value, '~');
                            flag2 = stringStartsWithPattern.Contains("~");
                            pattern = this.sql.ValueFromObject(stringStartsWithPattern, true, pattern.SourceExpression);
                        }
                        if (flag2)
                        {
                            escape = this.sql.ValueFromObject("~", false, sourceExpression);
                        }
                        return this.sql.Like(mc.Object, pattern, escape, sourceExpression);
                    }
                    case "EndsWith":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            goto Label_1B30;
                        }
                        SqlExpression expression6 = mc.Arguments[0];
                        SqlExpression expression7 = null;
                        bool flag3 = true;
                        if (expression6.NodeType != SqlNodeType.Value)
                        {
                            if (expression6.NodeType != SqlNodeType.ClientParameter)
                            {
                                throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedFor("String.EndsWith");
                            }
                            SqlClientParameter parameter3 = (SqlClientParameter) expression6;
                            expression6 = new SqlClientParameter(parameter3.ClrType, parameter3.SqlType, Expression.Lambda(Expression.Call(typeof(SqlHelpers), "GetStringEndsWithPattern", Type.EmptyTypes, new Expression[] { parameter3.Accessor.Body, Expression.Constant('~') }), new ParameterExpression[] { parameter3.Accessor.Parameters[0] }), parameter3.SourceExpression);
                        }
                        else
                        {
                            string stringEndsWithPattern = SqlHelpers.GetStringEndsWithPattern((string) ((SqlValue) expression6).Value, '~');
                            flag3 = stringEndsWithPattern.Contains("~");
                            expression6 = this.sql.ValueFromObject(stringEndsWithPattern, true, expression6.SourceExpression);
                        }
                        if (flag3)
                        {
                            expression7 = this.sql.ValueFromObject("~", false, sourceExpression);
                        }
                        return this.sql.Like(mc.Object, expression6, expression7, sourceExpression);
                    }
                    case "IndexOf":
                    {
                        if (mc.Arguments.Count == 1)
                        {
                            if ((mc.Arguments[0] is SqlValue) && (((SqlValue) mc.Arguments[0]).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            SqlWhen when = new SqlWhen(this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression)), this.sql.ValueFromObject(0, sourceExpression));
                            SqlExpression expression9 = this.sql.Subtract(this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { mc.Arguments[0], mc.Object }, sourceExpression), 1);
                            return this.sql.SearchedCase(new SqlWhen[] { when }, expression9, sourceExpression);
                        }
                        if (mc.Arguments.Count == 2)
                        {
                            if ((mc.Arguments[0] is SqlValue) && (((SqlValue) mc.Arguments[0]).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            if (mc.Arguments[1].ClrType == typeof(StringComparison))
                            {
                                throw System.Data.Linq.SqlClient.Error.IndexOfWithStringComparisonArgNotSupported();
                            }
                            SqlExpression expression10 = this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression));
                            SqlWhen when2 = new SqlWhen(this.sql.AndAccumulate(expression10, this.sql.Binary(SqlNodeType.LE, this.sql.Add(mc.Arguments[1], 1), this.sql.CLRLENGTH(mc.Object))), mc.Arguments[1]);
                            SqlExpression expression11 = this.sql.Subtract(this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { mc.Arguments[0], mc.Object, this.sql.Add(mc.Arguments[1], 1) }, sourceExpression), 1);
                            return this.sql.SearchedCase(new SqlWhen[] { when2 }, expression11, sourceExpression);
                        }
                        if (mc.Arguments.Count != 3)
                        {
                            goto Label_1B30;
                        }
                        if ((mc.Arguments[0] is SqlValue) && (((SqlValue) mc.Arguments[0]).Value == null))
                        {
                            throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                        }
                        if (mc.Arguments[2].ClrType == typeof(StringComparison))
                        {
                            throw System.Data.Linq.SqlClient.Error.IndexOfWithStringComparisonArgNotSupported();
                        }
                        SqlExpression left = this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression));
                        SqlWhen when3 = new SqlWhen(this.sql.AndAccumulate(left, this.sql.Binary(SqlNodeType.LE, this.sql.Add(mc.Arguments[1], 1), this.sql.CLRLENGTH(mc.Object))), mc.Arguments[1]);
                        SqlExpression expression13 = this.sql.FunctionCall(typeof(string), "SUBSTRING", new SqlExpression[] { mc.Object, this.sql.ValueFromObject(1, false, sourceExpression), this.sql.Add(new SqlExpression[] { mc.Arguments[1], mc.Arguments[2] }) }, sourceExpression);
                        SqlExpression @else = this.sql.Subtract(this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { mc.Arguments[0], expression13, this.sql.Add(mc.Arguments[1], 1) }, sourceExpression), 1);
                        return this.sql.SearchedCase(new SqlWhen[] { when3 }, @else, sourceExpression);
                    }
                    case "LastIndexOf":
                    {
                        if (mc.Arguments.Count == 1)
                        {
                            SqlExpression expression15 = mc.Arguments[0];
                            if ((expression15 is SqlValue) && (((SqlValue) expression15).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            SqlExpression expression16 = mc.Object;
                            SqlExpression expression17 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression16 }, sourceExpression);
                            SqlExpression expression18 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression15 }, sourceExpression);
                            SqlExpression expression19 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression15, expression16 }, sourceExpression);
                            SqlExpression expression20 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression18, expression17 }, sourceExpression);
                            SqlExpression match = this.sql.Binary(SqlNodeType.EQ, expression19, this.sql.ValueFromObject(0, false, sourceExpression));
                            SqlExpression expression22 = this.sql.CLRLENGTH(expression16);
                            SqlExpression expression23 = this.sql.CLRLENGTH(expression15);
                            SqlExpression expression24 = this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(1, false, sourceExpression), this.sql.Subtract(expression22, this.sql.Add(new SqlExpression[] { expression23, expression20 })) });
                            SqlWhen when4 = new SqlWhen(match, this.sql.ValueFromObject(-1, false, sourceExpression));
                            SqlWhen when5 = new SqlWhen(this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression)), this.sql.Subtract(this.sql.CLRLENGTH(expression16), 1));
                            return this.sql.SearchedCase(new SqlWhen[] { when5, when4 }, expression24, sourceExpression);
                        }
                        if (mc.Arguments.Count == 2)
                        {
                            if (mc.Arguments[1].ClrType == typeof(StringComparison))
                            {
                                throw System.Data.Linq.SqlClient.Error.LastIndexOfWithStringComparisonArgNotSupported();
                            }
                            SqlExpression expression26 = mc.Object;
                            SqlExpression expression27 = mc.Arguments[0];
                            if ((expression27 is SqlValue) && (((SqlValue) expression27).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            SqlExpression expression28 = mc.Arguments[1];
                            SqlExpression expression29 = this.sql.FunctionCall(typeof(string), "LEFT", new SqlExpression[] { expression26, this.sql.Add(expression28, 1) }, sourceExpression);
                            SqlExpression expression30 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression29 }, sourceExpression);
                            SqlExpression expression31 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression27 }, sourceExpression);
                            SqlExpression expression32 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression27, expression29 }, sourceExpression);
                            SqlExpression expression33 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression31, expression30 }, sourceExpression);
                            SqlExpression expression34 = this.sql.Binary(SqlNodeType.EQ, expression32, this.sql.ValueFromObject(0, false, sourceExpression));
                            SqlExpression expression35 = this.sql.CLRLENGTH(expression29);
                            SqlExpression expression36 = this.sql.CLRLENGTH(expression27);
                            SqlExpression expression37 = this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(1, false, sourceExpression), this.sql.Subtract(expression35, this.sql.Add(new SqlExpression[] { expression36, expression33 })) });
                            SqlWhen when6 = new SqlWhen(expression34, this.sql.ValueFromObject(-1, false, sourceExpression));
                            SqlExpression expression38 = this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression));
                            SqlWhen when7 = new SqlWhen(this.sql.AndAccumulate(expression38, this.sql.Binary(SqlNodeType.LE, this.sql.Add(mc.Arguments[1], 1), this.sql.CLRLENGTH(expression26))), mc.Arguments[1]);
                            return this.sql.SearchedCase(new SqlWhen[] { when7, when6 }, expression37, sourceExpression);
                        }
                        if (mc.Arguments.Count != 3)
                        {
                            goto Label_1B30;
                        }
                        if (mc.Arguments[2].ClrType == typeof(StringComparison))
                        {
                            throw System.Data.Linq.SqlClient.Error.LastIndexOfWithStringComparisonArgNotSupported();
                        }
                        SqlExpression expr = mc.Object;
                        SqlExpression expression40 = mc.Arguments[0];
                        if ((expression40 is SqlValue) && (((SqlValue) expression40).Value == null))
                        {
                            throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                        }
                        SqlExpression expression41 = mc.Arguments[1];
                        SqlExpression second = mc.Arguments[2];
                        SqlExpression expression43 = this.sql.FunctionCall(typeof(string), "LEFT", new SqlExpression[] { expr, this.sql.Add(expression41, 1) }, sourceExpression);
                        SqlExpression expression44 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression43 }, sourceExpression);
                        SqlExpression expression45 = this.sql.FunctionCall(typeof(string), "REVERSE", new SqlExpression[] { expression40 }, sourceExpression);
                        SqlExpression expression46 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression40, expression43 }, sourceExpression);
                        SqlExpression expression47 = this.sql.FunctionCall(typeof(int), "CHARINDEX", new SqlExpression[] { expression45, expression44 }, sourceExpression);
                        SqlExpression first = this.sql.CLRLENGTH(expression43);
                        SqlExpression expression49 = this.sql.CLRLENGTH(expression40);
                        SqlExpression expression50 = this.sql.Add(new SqlExpression[] { this.sql.ValueFromObject(1, false, sourceExpression), this.sql.Subtract(first, this.sql.Add(new SqlExpression[] { expression49, expression47 })) });
                        SqlExpression expression51 = this.sql.Binary(SqlNodeType.EQ, expression46, this.sql.ValueFromObject(0, false, sourceExpression));
                        SqlWhen when8 = new SqlWhen(this.sql.OrAccumulate(expression51, this.sql.Binary(SqlNodeType.LE, expression50, this.sql.Subtract(expression41, second))), this.sql.ValueFromObject(-1, false, sourceExpression));
                        SqlExpression expression52 = this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Arguments[0]), this.sql.ValueFromObject(0, sourceExpression));
                        SqlWhen when9 = new SqlWhen(this.sql.AndAccumulate(expression52, this.sql.Binary(SqlNodeType.LE, this.sql.Add(mc.Arguments[1], 1), this.sql.CLRLENGTH(expr))), mc.Arguments[1]);
                        return this.sql.SearchedCase(new SqlWhen[] { when9, when8 }, expression50, sourceExpression);
                    }
                    case "Insert":
                        if (mc.Arguments.Count == 2)
                        {
                            if ((mc.Arguments[1] is SqlValue) && (((SqlValue) mc.Arguments[1]).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            SqlFunctionCall call = this.sql.FunctionCall(typeof(string), "STUFF", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), this.sql.ValueFromObject(0, false, sourceExpression), mc.Arguments[1] }, sourceExpression);
                            SqlExpression expression53 = this.sql.Binary(SqlNodeType.EQ, this.sql.CLRLENGTH(mc.Object), mc.Arguments[0]);
                            SqlExpression expression54 = this.sql.Concat(new SqlExpression[] { mc.Object, mc.Arguments[1] });
                            return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression53, expression54) }, call, sourceExpression);
                        }
                        goto Label_1B30;

                    case "PadLeft":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            if (mc.Arguments.Count == 2)
                            {
                                SqlExpression expression62 = mc.Object;
                                SqlExpression expression63 = mc.Arguments[0];
                                SqlExpression expression64 = mc.Arguments[1];
                                SqlExpression expression65 = this.sql.Binary(SqlNodeType.GE, this.sql.CLRLENGTH(expression62), expression63);
                                SqlExpression expression66 = this.sql.CLRLENGTH(expression62);
                                SqlExpression expression67 = this.sql.Subtract(expression63, expression66);
                                SqlExpression expression68 = this.sql.FunctionCall(typeof(string), "REPLICATE", new SqlExpression[] { expression64, expression67 }, sourceExpression);
                                SqlExpression expression69 = this.sql.Concat(new SqlExpression[] { expression68, expression62 });
                                return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression65, expression62) }, expression69, sourceExpression);
                            }
                            goto Label_1B30;
                        }
                        SqlExpression expression55 = mc.Object;
                        SqlExpression right = mc.Arguments[0];
                        SqlExpression expression57 = this.sql.CLRLENGTH(expression55);
                        SqlExpression expression58 = this.sql.Binary(SqlNodeType.GE, expression57, right);
                        SqlExpression expression59 = this.sql.Subtract(right, expression57);
                        SqlExpression expression60 = this.sql.FunctionCall(typeof(string), "SPACE", new SqlExpression[] { expression59 }, sourceExpression);
                        SqlExpression expression61 = this.sql.Concat(new SqlExpression[] { expression60, expression55 });
                        return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression58, expression55) }, expression61, sourceExpression);
                    }
                    case "PadRight":
                    {
                        if (mc.Arguments.Count != 1)
                        {
                            if (mc.Arguments.Count == 2)
                            {
                                SqlExpression expression77 = mc.Object;
                                SqlExpression expression78 = mc.Arguments[0];
                                SqlExpression expression79 = mc.Arguments[1];
                                SqlExpression expression80 = this.sql.Binary(SqlNodeType.GE, this.sql.CLRLENGTH(expression77), expression78);
                                SqlExpression expression81 = this.sql.CLRLENGTH(expression77);
                                SqlExpression expression82 = this.sql.Subtract(expression78, expression81);
                                SqlExpression expression83 = this.sql.FunctionCall(typeof(string), "REPLICATE", new SqlExpression[] { expression79, expression82 }, sourceExpression);
                                SqlExpression expression84 = this.sql.Concat(new SqlExpression[] { expression77, expression83 });
                                return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression80, expression77) }, expression84, sourceExpression);
                            }
                            goto Label_1B30;
                        }
                        SqlExpression expression70 = mc.Object;
                        SqlExpression expression71 = mc.Arguments[0];
                        SqlExpression expression72 = this.sql.Binary(SqlNodeType.GE, this.sql.CLRLENGTH(expression70), expression71);
                        SqlExpression expression73 = this.sql.CLRLENGTH(expression70);
                        SqlExpression expression74 = this.sql.Subtract(expression71, expression73);
                        SqlExpression expression75 = this.sql.FunctionCall(typeof(string), "SPACE", new SqlExpression[] { expression74 }, sourceExpression);
                        SqlExpression expression76 = this.sql.Concat(new SqlExpression[] { expression70, expression75 });
                        return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(expression72, expression70) }, expression76, sourceExpression);
                    }
                    case "Remove":
                        if (mc.Arguments.Count != 1)
                        {
                            if (mc.Arguments.Count == 2)
                            {
                                return this.sql.FunctionCall(typeof(string), "STUFF", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), mc.Arguments[1], this.sql.ValueFromObject("", false, sourceExpression) }, sourceExpression);
                            }
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(string), "STUFF", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), this.sql.CLRLENGTH(mc.Object), this.sql.ValueFromObject("", false, sourceExpression) }, sourceExpression);

                    case "Replace":
                        if ((mc.Arguments[0] is SqlValue) && (((SqlValue) mc.Arguments[0]).Value == null))
                        {
                            throw System.Data.Linq.SqlClient.Error.ArgumentNull("old");
                        }
                        if ((mc.Arguments[1] is SqlValue) && (((SqlValue) mc.Arguments[1]).Value == null))
                        {
                            throw System.Data.Linq.SqlClient.Error.ArgumentNull("new");
                        }
                        return this.sql.FunctionCall(typeof(string), "REPLACE", new SqlExpression[] { mc.Object, mc.Arguments[0], mc.Arguments[1] }, sourceExpression);

                    case "Substring":
                        if (mc.Arguments.Count != 1)
                        {
                            if (mc.Arguments.Count == 2)
                            {
                                return this.sql.FunctionCall(typeof(string), "SUBSTRING", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), mc.Arguments[1] }, sourceExpression);
                            }
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(string), "SUBSTRING", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), this.sql.CLRLENGTH(mc.Object) }, sourceExpression);

                    case "Trim":
                        if (mc.Arguments.Count != 0)
                        {
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(string), "LTRIM", new SqlExpression[] { this.sql.FunctionCall(typeof(string), "RTRIM", new SqlExpression[] { mc.Object }, sourceExpression) }, sourceExpression);

                    case "ToLower":
                        if (mc.Arguments.Count != 0)
                        {
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(string), "LOWER", new SqlExpression[] { mc.Object }, sourceExpression);

                    case "ToUpper":
                        if (mc.Arguments.Count != 0)
                        {
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(string), "UPPER", new SqlExpression[] { mc.Object }, sourceExpression);

                    case "get_Chars":
                        if (mc.Arguments.Count != 1)
                        {
                            goto Label_1B30;
                        }
                        return this.sql.FunctionCall(typeof(char), "SUBSTRING", new SqlExpression[] { mc.Object, this.sql.Add(mc.Arguments[0], 1), this.sql.ValueFromObject(1, false, sourceExpression) }, sourceExpression);

                    case "CompareTo":
                        if (mc.Arguments.Count == 1)
                        {
                            if ((mc.Arguments[0] is SqlValue) && (((SqlValue) mc.Arguments[0]).Value == null))
                            {
                                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                            }
                            return this.CreateComparison(mc.Object, mc.Arguments[0], sourceExpression);
                        }
                        goto Label_1B30;

                    default:
                        goto Label_1B30;
                }
                if (flag)
                {
                    expression3 = this.sql.ValueFromObject("~", false, sourceExpression);
                }
                return this.sql.Like(mc.Object, expression2, expression3, sourceExpression);
            Label_1B30:
                throw GetMethodSupportException(mc);
            }

            private SqlExpression TranslateStringStaticMethod(SqlMethodCall mc)
            {
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Method.Name == "Concat")
                {
                    SqlClientArray array = mc.Arguments[0] as SqlClientArray;
                    List<SqlExpression> expressions = null;
                    if (array != null)
                    {
                        expressions = array.Expressions;
                    }
                    else
                    {
                        expressions = mc.Arguments;
                    }
                    if (expressions.Count == 0)
                    {
                        this.sql.ValueFromObject("", false, sourceExpression);
                    }
                    else
                    {
                        SqlExpression expression2;
                        if (expressions[0].SqlType.IsString || expressions[0].SqlType.IsChar)
                        {
                            expression2 = expressions[0];
                        }
                        else
                        {
                            expression2 = this.sql.ConvertTo(typeof(string), expressions[0]);
                        }
                        for (int i = 1; i < expressions.Count; i++)
                        {
                            if (expressions[i].SqlType.IsString || expressions[i].SqlType.IsChar)
                            {
                                expression2 = this.sql.Concat(new SqlExpression[] { expression2, expressions[i] });
                            }
                            else
                            {
                                expression2 = this.sql.Concat(new SqlExpression[] { expression2, this.sql.ConvertTo(typeof(string), expressions[i]) });
                            }
                        }
                    }
                }
                else if ((mc.Method.Name == "Equals") && (mc.Arguments.Count == 2))
                {
                    this.sql.Binary(SqlNodeType.EQ2V, mc.Arguments[0], mc.Arguments[1]);
                }
                else if ((mc.Method.Name == "Compare") && (mc.Arguments.Count == 2))
                {
                    this.CreateComparison(mc.Arguments[0], mc.Arguments[1], sourceExpression);
                }
                throw GetMethodSupportException(mc);
            }

            private SqlExpression TranslateTimeSpanInstanceMethod(SqlMethodCall mc)
            {
                SqlExpression expression = null;
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Method.Name == "Add")
                {
                    return this.sql.Add(new SqlExpression[] { mc.Object, mc.Arguments[0] });
                }
                if (mc.Method.Name == "Subtract")
                {
                    return this.sql.Subtract(mc.Object, mc.Arguments[0]);
                }
                if (mc.Method.Name == "CompareTo")
                {
                    return this.CreateComparison(mc.Object, mc.Arguments[0], sourceExpression);
                }
                if (mc.Method.Name == "Duration")
                {
                    if (this.sql.IsSqlTimeType(mc.Object))
                    {
                        return mc.Object;
                    }
                    return this.sql.FunctionCall(typeof(TimeSpan), "ABS", new SqlExpression[] { mc.Object }, sourceExpression);
                }
                if (mc.Method.Name == "Negate")
                {
                    expression = this.sql.Unary(SqlNodeType.Negate, mc.Object, sourceExpression);
                }
                return expression;
            }

            private SqlExpression TranslateVbCompareString(SqlMethodCall mc)
            {
                if (mc.Arguments.Count < 2)
                {
                    throw GetMethodSupportException(mc);
                }
                return this.CreateComparison(mc.Arguments[0], mc.Arguments[1], mc.SourceExpression);
            }

            private SqlExpression TranslateVbConversionMethod(SqlMethodCall mc)
            {
                Expression sourceExpression = mc.SourceExpression;
                if (mc.Arguments.Count == 1)
                {
                    SqlExpression discriminator = mc.Arguments[0];
                    Type clrType = null;
                    switch (mc.Method.Name)
                    {
                        case "ToBoolean":
                            clrType = typeof(bool);
                            break;

                        case "ToSByte":
                            clrType = typeof(sbyte);
                            break;

                        case "ToByte":
                            clrType = typeof(byte);
                            break;

                        case "ToChar":
                            clrType = typeof(char);
                            break;

                        case "ToCharArrayRankOne":
                            clrType = typeof(char[]);
                            break;

                        case "ToDate":
                            clrType = typeof(DateTime);
                            break;

                        case "ToDecimal":
                            clrType = typeof(decimal);
                            break;

                        case "ToDouble":
                            clrType = typeof(double);
                            break;

                        case "ToInteger":
                            clrType = typeof(int);
                            break;

                        case "ToUInteger":
                            clrType = typeof(uint);
                            break;

                        case "ToLong":
                            clrType = typeof(long);
                            break;

                        case "ToULong":
                            clrType = typeof(ulong);
                            break;

                        case "ToShort":
                            clrType = typeof(short);
                            break;

                        case "ToUShort":
                            clrType = typeof(ushort);
                            break;

                        case "ToSingle":
                            clrType = typeof(float);
                            break;

                        case "ToString":
                            clrType = typeof(string);
                            break;
                    }
                    if (clrType != null)
                    {
                        if (((clrType == typeof(int)) || (clrType == typeof(float))) && (discriminator.ClrType == typeof(bool)))
                        {
                            List<SqlExpression> matches = new List<SqlExpression>();
                            List<SqlExpression> values = new List<SqlExpression>();
                            matches.Add(this.sql.ValueFromObject(true, false, sourceExpression));
                            values.Add(this.sql.ValueFromObject(-1, false, sourceExpression));
                            matches.Add(this.sql.ValueFromObject(false, false, sourceExpression));
                            values.Add(this.sql.ValueFromObject(0, false, sourceExpression));
                            return this.sql.Case(clrType, discriminator, matches, values, sourceExpression);
                        }
                        if (mc.ClrType != mc.Arguments[0].ClrType)
                        {
                            return this.sql.ConvertTo(clrType, discriminator);
                        }
                        return mc.Arguments[0];
                    }
                }
                throw GetMethodSupportException(mc);
            }

            private SqlExpression TranslateVbLikeString(SqlMethodCall mc)
            {
                bool flag = true;
                Expression sourceExpression = mc.SourceExpression;
                SqlExpression pattern = mc.Arguments[1];
                if (pattern.NodeType == SqlNodeType.Value)
                {
                    string str = SqlHelpers.TranslateVBLikePattern((string) ((SqlValue) pattern).Value, '~');
                    pattern = this.sql.ValueFromObject(str, typeof(string), true, sourceExpression);
                    flag = str.Contains("~");
                }
                else
                {
                    if (pattern.NodeType != SqlNodeType.ClientParameter)
                    {
                        throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedFor("LIKE");
                    }
                    SqlClientParameter parameter = (SqlClientParameter) pattern;
                    pattern = new SqlClientParameter(parameter.ClrType, parameter.SqlType, Expression.Lambda(Expression.Call(typeof(SqlHelpers), "TranslateVBLikePattern", Type.EmptyTypes, new Expression[] { parameter.Accessor.Body, Expression.Constant('~') }), new ParameterExpression[] { parameter.Accessor.Parameters[0] }), parameter.SourceExpression);
                }
                SqlExpression escape = flag ? this.sql.ValueFromObject("~", false, mc.SourceExpression) : null;
                return this.sql.Like(mc.Arguments[0], pattern, escape, sourceExpression);
            }

            internal override SqlExpression VisitBinaryOperator(SqlBinary bo)
            {
                bo = (SqlBinary) base.VisitBinaryOperator(bo);
                Type nonNullableType = TypeSystem.GetNonNullableType(bo.Left.ClrType);
                if ((nonNullableType != typeof(DateTime)) && (nonNullableType != typeof(DateTimeOffset)))
                {
                    return bo;
                }
                return this.TranslateDateTimeBinary(bo);
            }

            internal override SqlNode VisitMember(SqlMember m)
            {
                SqlExpression expr = this.VisitExpression(m.Expression);
                MemberInfo member = m.Member;
                Expression sourceExpression = m.SourceExpression;
                Type nonNullableType = TypeSystem.GetNonNullableType(expr.ClrType);
                if ((nonNullableType == typeof(string)) && (member.Name == "Length"))
                {
                    return this.sql.LEN(expr);
                }
                if ((nonNullableType == typeof(Binary)) && (member.Name == "Length"))
                {
                    return this.sql.DATALENGTH(expr);
                }
                if ((nonNullableType == typeof(DateTime)) || (nonNullableType == typeof(DateTimeOffset)))
                {
                    string datePart = PostBindDotNetConverter.GetDatePart(member.Name);
                    if (datePart != null)
                    {
                        return this.sql.DATEPART(datePart, expr);
                    }
                    if (member.Name == "Date")
                    {
                        if (this.providerMode == SqlProvider.ProviderMode.Sql2008)
                        {
                            SqlExpression expression3 = new SqlVariable(typeof(void), null, "DATE", sourceExpression);
                            return this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression3, expr }, sourceExpression);
                        }
                        SqlExpression expression = this.sql.DATEPART("MILLISECOND", expr);
                        SqlExpression expression5 = this.sql.DATEPART("SECOND", expr);
                        SqlExpression expression6 = this.sql.DATEPART("MINUTE", expr);
                        SqlExpression expression7 = this.sql.DATEPART("HOUR", expr);
                        SqlExpression expression8 = expr;
                        expression8 = this.sql.DATEADD("MILLISECOND", this.sql.Unary(SqlNodeType.Negate, expression), expression8);
                        expression8 = this.sql.DATEADD("SECOND", this.sql.Unary(SqlNodeType.Negate, expression5), expression8);
                        expression8 = this.sql.DATEADD("MINUTE", this.sql.Unary(SqlNodeType.Negate, expression6), expression8);
                        return this.sql.DATEADD("HOUR", this.sql.Unary(SqlNodeType.Negate, expression7), expression8);
                    }
                    if (member.Name == "DateTime")
                    {
                        SqlExpression expression9 = new SqlVariable(typeof(void), null, "DATETIME", sourceExpression);
                        return this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression9, expr }, sourceExpression);
                    }
                    if (member.Name == "TimeOfDay")
                    {
                        SqlExpression expression10 = this.sql.DATEPART("HOUR", expr);
                        SqlExpression expression11 = this.sql.DATEPART("MINUTE", expr);
                        SqlExpression expression12 = this.sql.DATEPART("SECOND", expr);
                        SqlExpression expression13 = this.sql.DATEPART("MILLISECOND", expr);
                        SqlExpression expression14 = this.sql.Multiply(this.sql.ConvertToBigint(expression10), 0x861c46800L);
                        SqlExpression expression15 = this.sql.Multiply(this.sql.ConvertToBigint(expression11), 0x23c34600L);
                        SqlExpression expression16 = this.sql.Multiply(this.sql.ConvertToBigint(expression12), 0x989680L);
                        SqlExpression expression17 = this.sql.Multiply(this.sql.ConvertToBigint(expression13), 0x2710L);
                        return this.sql.ConvertTo(typeof(TimeSpan), this.sql.Add(new SqlExpression[] { expression14, expression15, expression16, expression17 }));
                    }
                    if (member.Name == "DayOfWeek")
                    {
                        SqlExpression expression18 = this.sql.DATEPART("dw", expr);
                        return this.sql.ConvertTo(typeof(DayOfWeek), this.sql.Mod(this.sql.Add(new SqlExpression[] { expression18, this.sql.Add(new SqlVariable(typeof(int), this.sql.Default(typeof(int)), "@@DATEFIRST", sourceExpression), 6) }), 7L));
                    }
                }
                else if (nonNullableType == typeof(TimeSpan))
                {
                    switch (member.Name)
                    {
                        case "Ticks":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToBigint(expr);
                            }
                            return this.sql.Divide(this.sql.ConvertToBigint(this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.ConvertToBigint(this.sql.DATEPART("HOUR", expr)), 0x34630b8a000L), this.sql.Multiply(this.sql.ConvertToBigint(this.sql.DATEPART("MINUTE", expr)), 0xdf8475800L), this.sql.Multiply(this.sql.ConvertToBigint(this.sql.DATEPART("SECOND", expr)), 0x3b9aca00L), this.sql.DATEPART("NANOSECOND", expr) })), (long) 100L);

                        case "TotalMilliseconds":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.Divide(this.sql.ConvertToDouble(expr), (long) 0x2710L);
                            }
                            return this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.DATEPART("HOUR", expr), 0x36ee80L), this.sql.Multiply(this.sql.DATEPART("MINUTE", expr), 0xea60L), this.sql.Multiply(this.sql.DATEPART("SECOND", expr), 0x3e8L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.ConvertToBigint(this.sql.DATEPART("NANOSECOND", expr))), (long) 0xf4240L) });

                        case "TotalSeconds":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.Divide(this.sql.ConvertToDouble(expr), (long) 0x989680L);
                            }
                            return this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.DATEPART("HOUR", expr), 0xe10L), this.sql.Multiply(this.sql.DATEPART("MINUTE", expr), 60L), this.sql.DATEPART("SECOND", expr), this.sql.Divide(this.sql.ConvertToDouble(this.sql.ConvertToBigint(this.sql.DATEPART("NANOSECOND", expr))), (long) 0x3b9aca00L) });

                        case "TotalMinutes":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.Divide(this.sql.ConvertToDouble(expr), (long) 0x23c34600L);
                            }
                            return this.sql.Add(new SqlExpression[] { this.sql.Multiply(this.sql.DATEPART("HOUR", expr), 60L), this.sql.DATEPART("MINUTE", expr), this.sql.Divide(this.sql.ConvertToDouble(this.sql.DATEPART("SECOND", expr)), (long) 60L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.ConvertToBigint(this.sql.DATEPART("NANOSECOND", expr))), (long) 0xdf8475800L) });

                        case "TotalHours":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.Divide(this.sql.ConvertToDouble(expr), (long) 0x861c46800L);
                            }
                            return this.sql.Add(new SqlExpression[] { this.sql.DATEPART("HOUR", expr), this.sql.Divide(this.sql.ConvertToDouble(this.sql.DATEPART("MINUTE", expr)), (long) 60L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.DATEPART("SECOND", expr)), (long) 0xe10L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.ConvertToBigint(this.sql.DATEPART("NANOSECOND", expr))), (long) 0x34630b8a000L) });

                        case "TotalDays":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.Divide(this.sql.ConvertToDouble(expr), (long) 0xc92a69c000L);
                            }
                            return this.sql.Divide(this.sql.Add(new SqlExpression[] { this.sql.DATEPART("HOUR", expr), this.sql.Divide(this.sql.ConvertToDouble(this.sql.DATEPART("MINUTE", expr)), (long) 60L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.DATEPART("SECOND", expr)), (long) 0xe10L), this.sql.Divide(this.sql.ConvertToDouble(this.sql.ConvertToBigint(this.sql.DATEPART("NANOSECOND", expr))), (long) 0x34630b8a000L) }), (long) 0x18L);

                        case "Milliseconds":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToInt(this.sql.Mod(this.sql.ConvertToBigint(this.sql.Divide(expr, (long) 0x2710L)), 0x3e8L));
                            }
                            return this.sql.DATEPART("MILLISECOND", expr);

                        case "Seconds":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToInt(this.sql.Mod(this.sql.ConvertToBigint(this.sql.Divide(expr, (long) 0x989680L)), 60L));
                            }
                            return this.sql.DATEPART("SECOND", expr);

                        case "Minutes":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToInt(this.sql.Mod(this.sql.ConvertToBigint(this.sql.Divide(expr, (long) 0x23c34600L)), 60L));
                            }
                            return this.sql.DATEPART("MINUTE", expr);

                        case "Hours":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToInt(this.sql.Mod(this.sql.ConvertToBigint(this.sql.Divide(expr, (long) 0x861c46800L)), 0x18L));
                            }
                            return this.sql.DATEPART("HOUR", expr);

                        case "Days":
                            if (!this.sql.IsSqlTimeType(expr))
                            {
                                return this.sql.ConvertToInt(this.sql.Divide(expr, (long) 0xc92a69c000L));
                            }
                            return this.sql.ValueFromObject(0, false, expr.SourceExpression);
                    }
                    throw System.Data.Linq.SqlClient.Error.MemberCannotBeTranslated(member.DeclaringType, member.Name);
                }
                throw System.Data.Linq.SqlClient.Error.MemberCannotBeTranslated(member.DeclaringType, member.Name);
            }

            internal override SqlExpression VisitMethodCall(SqlMethodCall mc)
            {
                Type declaringType = mc.Method.DeclaringType;
                Expression sourceExpression = mc.SourceExpression;
                SqlExpression expression = null;
                mc.Object = this.VisitExpression(mc.Object);
                int num = 0;
                int count = mc.Arguments.Count;
                while (num < count)
                {
                    mc.Arguments[num] = this.VisitExpression(mc.Arguments[num]);
                    num++;
                }
                if (mc.Method.IsStatic)
                {
                    if ((mc.Method.Name == "op_Explicit") || (mc.Method.Name == "op_Implicit"))
                    {
                        if (mc.SqlType.CanBeColumn && mc.Arguments[0].SqlType.CanBeColumn)
                        {
                            expression = this.sql.ConvertTo(mc.ClrType, mc.Arguments[0]);
                        }
                    }
                    else if (((mc.Method.Name == "Compare") && (mc.Arguments.Count == 2)) && (mc.Method.ReturnType == typeof(int)))
                    {
                        expression = this.CreateComparison(mc.Arguments[0], mc.Arguments[1], mc.SourceExpression);
                    }
                    else if (declaringType == typeof(Math))
                    {
                        expression = this.TranslateMathMethod(mc);
                    }
                    else if (declaringType == typeof(string))
                    {
                        expression = this.TranslateStringStaticMethod(mc);
                    }
                    else if (declaringType == typeof(Convert))
                    {
                        expression = this.TranslateConvertStaticMethod(mc);
                    }
                    else if (declaringType == typeof(SqlMethods))
                    {
                        expression = this.TranslateSqlMethodsMethod(mc);
                    }
                    else if (declaringType == typeof(decimal))
                    {
                        expression = this.TranslateDecimalMethod(mc);
                    }
                    else
                    {
                        if (PostBindDotNetConverter.IsVbConversionMethod(mc))
                        {
                            return this.TranslateVbConversionMethod(mc);
                        }
                        if (PostBindDotNetConverter.IsVbCompareString(mc))
                        {
                            return this.TranslateVbCompareString(mc);
                        }
                        if (PostBindDotNetConverter.IsVbLike(mc))
                        {
                            return this.TranslateVbLikeString(mc);
                        }
                    }
                    if (expression != null)
                    {
                        return expression;
                    }
                }
                else
                {
                    if ((mc.Method.Name == "Equals") && (mc.Arguments.Count == 1))
                    {
                        return this.sql.Binary(SqlNodeType.EQ, mc.Object, mc.Arguments[0]);
                    }
                    if (((mc.Method.Name == "GetValueOrDefault") && mc.Method.DeclaringType.IsGenericType) && (mc.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        return this.TranslateGetValueOrDefaultMethod(mc);
                    }
                    if ((mc.Method.Name == "ToString") && (mc.Arguments.Count == 0))
                    {
                        SqlExpression expr = mc.Object;
                        if (expr.SqlType.IsRuntimeOnlyType)
                        {
                            throw System.Data.Linq.SqlClient.Error.ToStringOnlySupportedForPrimitiveTypes();
                        }
                        return this.sql.ConvertTo(typeof(string), expr);
                    }
                    if (declaringType == typeof(string))
                    {
                        return this.TranslateStringMethod(mc);
                    }
                    if (declaringType == typeof(TimeSpan))
                    {
                        expression = this.TranslateTimeSpanInstanceMethod(mc);
                    }
                    else if (declaringType == typeof(DateTime))
                    {
                        expression = this.TranslateDateTimeInstanceMethod(mc);
                    }
                    else if (declaringType == typeof(DateTimeOffset))
                    {
                        expression = this.TranslateDateTimeOffsetInstanceMethod(mc);
                    }
                    if (expression != null)
                    {
                        return expression;
                    }
                }
                throw GetMethodSupportException(mc);
            }

            internal override SqlExpression VisitNew(SqlNew sox)
            {
                sox = (SqlNew) base.VisitNew(sox);
                if (sox.ClrType == typeof(string))
                {
                    return this.TranslateNewString(sox);
                }
                if (sox.ClrType == typeof(TimeSpan))
                {
                    return this.TranslateNewTimeSpan(sox);
                }
                if (sox.ClrType == typeof(DateTime))
                {
                    return this.TranslateNewDateTime(sox);
                }
                if (sox.ClrType == typeof(DateTimeOffset))
                {
                    return this.TranslateNewDateTimeOffset(sox);
                }
                return sox;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                select = this.VisitSelectCore(select);
                select.Selection = this.skipper.VisitExpression(select.Selection);
                return select;
            }

            internal override SqlExpression VisitTypeCase(SqlTypeCase tc)
            {
                tc.Discriminator = base.VisitExpression(tc.Discriminator);
                List<SqlExpression> matches = new List<SqlExpression>();
                List<SqlExpression> values = new List<SqlExpression>();
                bool flag = true;
                foreach (SqlTypeCaseWhen when in tc.Whens)
                {
                    SqlExpression item = this.VisitExpression(when.Match);
                    SqlExpression expression2 = this.VisitExpression(when.TypeBinding);
                    flag = flag && (expression2 is SqlNew);
                    matches.Add(item);
                    values.Add(expression2);
                }
                if (!flag)
                {
                    return this.sql.Case(tc.ClrType, tc.Discriminator, matches, values, tc.SourceExpression);
                }
                int num = 0;
                int count = tc.Whens.Count;
                while (num < count)
                {
                    SqlTypeCaseWhen when2 = tc.Whens[num];
                    when2.Match = matches[num];
                    when2.TypeBinding = (SqlNew) values[num];
                    num++;
                }
                return tc;
            }

            internal override SqlExpression VisitUnaryOperator(SqlUnary uo)
            {
                if (uo.NodeType == SqlNodeType.Convert)
                {
                    Type clrType = uo.ClrType;
                    SqlExpression operand = uo.Operand;
                    if ((clrType == typeof(char)) || (operand.ClrType == typeof(char)))
                    {
                        operand = this.VisitExpression(uo.Operand);
                        uo.Operand = operand;
                        return this.sql.ConvertTo(clrType, uo.SqlType, operand);
                    }
                }
                return base.VisitUnaryOperator(uo);
            }
        }
    }
}

