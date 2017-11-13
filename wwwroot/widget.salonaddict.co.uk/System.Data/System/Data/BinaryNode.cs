namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlTypes;
    using System.Globalization;

    internal class BinaryNode : ExpressionNode
    {
        internal ExpressionNode left;
        internal int op;
        internal ExpressionNode right;

        internal BinaryNode(DataTable table, int op, ExpressionNode left, ExpressionNode right) : base(table)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        internal int BinaryCompare(object vLeft, object vRight, StorageType resultType, int op) => 
            this.BinaryCompare(vLeft, vRight, resultType, op, null);

        internal int BinaryCompare(object vLeft, object vRight, StorageType resultType, int op, CompareInfo comparer)
        {
            int num2 = 0;
            try
            {
                if (!DataStorage.IsSqlType(resultType))
                {
                    switch (resultType)
                    {
                        case StorageType.Boolean:
                            if ((op != 7) && (op != 12))
                            {
                                break;
                            }
                            return (Convert.ToInt32(DataExpression.ToBoolean(vLeft), base.FormatProvider) - Convert.ToInt32(DataExpression.ToBoolean(vRight), base.FormatProvider));

                        case StorageType.Char:
                            return Convert.ToInt32(vLeft, base.FormatProvider).CompareTo(Convert.ToInt32(vRight, base.FormatProvider));

                        case StorageType.SByte:
                        case StorageType.Byte:
                        case StorageType.Int16:
                        case StorageType.UInt16:
                        case StorageType.Int32:
                            return Convert.ToInt32(vLeft, base.FormatProvider).CompareTo(Convert.ToInt32(vRight, base.FormatProvider));

                        case StorageType.UInt32:
                        case StorageType.Int64:
                        case StorageType.UInt64:
                        case StorageType.Decimal:
                            return decimal.Compare(Convert.ToDecimal(vLeft, base.FormatProvider), Convert.ToDecimal(vRight, base.FormatProvider));

                        case StorageType.Single:
                            return Convert.ToSingle(vLeft, base.FormatProvider).CompareTo(Convert.ToSingle(vRight, base.FormatProvider));

                        case StorageType.Double:
                            return Convert.ToDouble(vLeft, base.FormatProvider).CompareTo(Convert.ToDouble(vRight, base.FormatProvider));

                        case StorageType.DateTime:
                            return DateTime.Compare(Convert.ToDateTime(vLeft, base.FormatProvider), Convert.ToDateTime(vRight, base.FormatProvider));

                        case StorageType.String:
                            return base.table.Compare(Convert.ToString(vLeft, base.FormatProvider), Convert.ToString(vRight, base.FormatProvider), comparer);

                        case StorageType.Guid:
                        {
                            Guid guid2 = (Guid) vLeft;
                            return guid2.CompareTo((Guid) vRight);
                        }
                        case StorageType.DateTimeOffset:
                            return DateTimeOffset.Compare((DateTimeOffset) vLeft, (DateTimeOffset) vRight);
                    }
                }
                else
                {
                    switch (resultType)
                    {
                        case StorageType.SByte:
                        case StorageType.Byte:
                        case StorageType.Int16:
                        case StorageType.UInt16:
                        case StorageType.Int32:
                        case StorageType.SqlByte:
                        case StorageType.SqlInt16:
                        case StorageType.SqlInt32:
                            return SqlConvert.ConvertToSqlInt32(vLeft).CompareTo(SqlConvert.ConvertToSqlInt32(vRight));

                        case StorageType.UInt32:
                        case StorageType.Int64:
                        case StorageType.SqlInt64:
                            return SqlConvert.ConvertToSqlInt64(vLeft).CompareTo(SqlConvert.ConvertToSqlInt64(vRight));

                        case StorageType.UInt64:
                        case StorageType.SqlDecimal:
                            return SqlConvert.ConvertToSqlDecimal(vLeft).CompareTo(SqlConvert.ConvertToSqlDecimal(vRight));

                        case StorageType.SqlBinary:
                            return SqlConvert.ConvertToSqlBinary(vLeft).CompareTo(SqlConvert.ConvertToSqlBinary(vRight));

                        case StorageType.SqlBoolean:
                            goto Label_0349;

                        case StorageType.SqlDateTime:
                            return SqlConvert.ConvertToSqlDateTime(vLeft).CompareTo(SqlConvert.ConvertToSqlDateTime(vRight));

                        case StorageType.SqlDouble:
                            return SqlConvert.ConvertToSqlDouble(vLeft).CompareTo(SqlConvert.ConvertToSqlDouble(vRight));

                        case StorageType.SqlGuid:
                        {
                            SqlGuid guid = (SqlGuid) vLeft;
                            return guid.CompareTo(vRight);
                        }
                        case StorageType.SqlMoney:
                            return SqlConvert.ConvertToSqlMoney(vLeft).CompareTo(SqlConvert.ConvertToSqlMoney(vRight));

                        case StorageType.SqlSingle:
                            return SqlConvert.ConvertToSqlSingle(vLeft).CompareTo(SqlConvert.ConvertToSqlSingle(vRight));

                        case StorageType.SqlString:
                            return base.table.Compare(vLeft.ToString(), vRight.ToString());
                    }
                }
                goto Label_0463;
            Label_0349:
                if ((op == 7) || (op == 12))
                {
                    num2 = 1;
                    if (((vLeft.GetType() == typeof(SqlBoolean)) && ((vRight.GetType() == typeof(SqlBoolean)) || (vRight.GetType() == typeof(bool)))) || ((vRight.GetType() == typeof(SqlBoolean)) && ((vLeft.GetType() == typeof(SqlBoolean)) || (vLeft.GetType() == typeof(bool)))))
                    {
                        return SqlConvert.ConvertToSqlBoolean(vLeft).CompareTo(SqlConvert.ConvertToSqlBoolean(vRight));
                    }
                }
            }
            catch (ArgumentException exception5)
            {
                ExceptionBuilder.TraceExceptionWithoutRethrow(exception5);
            }
            catch (FormatException exception4)
            {
                ExceptionBuilder.TraceExceptionWithoutRethrow(exception4);
            }
            catch (InvalidCastException exception3)
            {
                ExceptionBuilder.TraceExceptionWithoutRethrow(exception3);
            }
            catch (OverflowException exception2)
            {
                ExceptionBuilder.TraceExceptionWithoutRethrow(exception2);
            }
            catch (EvaluateException exception)
            {
                ExceptionBuilder.TraceExceptionWithoutRethrow(exception);
            }
        Label_0463:
            this.SetTypeMismatchError(op, vLeft.GetType(), vRight.GetType());
            return num2;
        }

        internal override void Bind(DataTable table, List<DataColumn> list)
        {
            base.BindTable(table);
            this.left.Bind(table, list);
            this.right.Bind(table, list);
        }

        internal override bool DependsOn(DataColumn column) => 
            (this.left.DependsOn(column) || this.right.DependsOn(column));

        internal override object Eval() => 
            this.Eval(null, DataRowVersion.Default);

        internal override object Eval(int[] recordNos) => 
            this.EvalBinaryOp(this.op, this.left, this.right, null, DataRowVersion.Default, recordNos);

        internal override object Eval(DataRow row, DataRowVersion version) => 
            this.EvalBinaryOp(this.op, this.left, this.right, row, version, null);

        private static object Eval(ExpressionNode expr, DataRow row, DataRowVersion version, int[] recordNos)
        {
            if (recordNos == null)
            {
                return expr.Eval(row, version);
            }
            return expr.Eval(recordNos);
        }

        private object EvalBinaryOp(int op, ExpressionNode left, ExpressionNode right, DataRow row, DataRowVersion version, int[] recordNos)
        {
            object obj2;
            object obj3;
            StorageType empty;
            if ((((op != 0x1b) && (op != 0x1a)) && ((op != 5) && (op != 13))) && (op != 0x27))
            {
                obj2 = Eval(left, row, version, recordNos);
                obj3 = Eval(right, row, version, recordNos);
                Type dataType = obj2.GetType();
                Type type4 = obj3.GetType();
                StorageType storageType = DataStorage.GetStorageType(dataType);
                StorageType type2 = DataStorage.GetStorageType(type4);
                bool flag3 = DataStorage.IsSqlType(storageType);
                bool flag2 = DataStorage.IsSqlType(type2);
                if (flag3 && DataStorage.IsObjectSqlNull(obj2))
                {
                    return obj2;
                }
                if (flag2 && DataStorage.IsObjectSqlNull(obj3))
                {
                    return obj3;
                }
                if ((obj2 == DBNull.Value) || (obj3 == DBNull.Value))
                {
                    return DBNull.Value;
                }
                if (flag3 || flag2)
                {
                    empty = this.ResultSqlType(storageType, type2, left is ConstNode, right is ConstNode, op);
                }
                else
                {
                    empty = this.ResultType(storageType, type2, left is ConstNode, right is ConstNode, op);
                }
                if (empty == StorageType.Empty)
                {
                    this.SetTypeMismatchError(op, dataType, type4);
                }
            }
            else
            {
                obj2 = obj3 = DBNull.Value;
                empty = StorageType.Empty;
            }
            object isTrue = DBNull.Value;
            bool flag = false;
            try
            {
                double num4;
                switch (op)
                {
                    case 5:
                        if (!(right is FunctionNode))
                        {
                            throw ExprException.InWithoutParentheses();
                        }
                        goto Label_164C;

                    case 7:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 == this.BinaryCompare(obj2, obj3, empty, 7));
                        }
                        return DBNull.Value;

                    case 8:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 < this.BinaryCompare(obj2, obj3, empty, op));
                        }
                        return DBNull.Value;

                    case 9:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 > this.BinaryCompare(obj2, obj3, empty, op));
                        }
                        return DBNull.Value;

                    case 10:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 <= this.BinaryCompare(obj2, obj3, empty, op));
                        }
                        return DBNull.Value;

                    case 11:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 >= this.BinaryCompare(obj2, obj3, empty, op));
                        }
                        return DBNull.Value;

                    case 12:
                        if (((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2))) && ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3))))
                        {
                            return (0 != this.BinaryCompare(obj2, obj3, empty, op));
                        }
                        return DBNull.Value;

                    case 13:
                        obj2 = Eval(left, row, version, recordNos);
                        if ((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2)))
                        {
                            return false;
                        }
                        return true;

                    case 15:
                        switch (empty)
                        {
                            case StorageType.Char:
                            case StorageType.String:
                                goto Label_03F7;

                            case StorageType.SByte:
                                goto Label_025B;

                            case StorageType.Int16:
                                goto Label_028F;

                            case StorageType.UInt16:
                                goto Label_02C3;

                            case StorageType.Int32:
                                goto Label_02F7;

                            case StorageType.UInt32:
                                goto Label_031B;

                            case StorageType.Int64:
                                goto Label_0363;

                            case StorageType.UInt64:
                                goto Label_033F;

                            case StorageType.Single:
                                goto Label_03AF;

                            case StorageType.Double:
                                goto Label_03D3;

                            case StorageType.Decimal:
                                goto Label_0387;

                            case StorageType.DateTime:
                                goto Label_041A;

                            case StorageType.TimeSpan:
                                goto Label_047A;

                            case StorageType.SqlByte:
                                goto Label_055A;

                            case StorageType.SqlDateTime:
                                goto Label_0592;

                            case StorageType.SqlDecimal:
                                goto Label_0522;

                            case StorageType.SqlDouble:
                                goto Label_04EA;

                            case StorageType.SqlInt16:
                                goto Label_0496;

                            case StorageType.SqlInt32:
                                goto Label_04B2;

                            case StorageType.SqlInt64:
                                goto Label_04CE;

                            case StorageType.SqlMoney:
                                goto Label_053E;

                            case StorageType.SqlSingle:
                                goto Label_0506;

                            case StorageType.SqlString:
                                goto Label_0576;
                        }
                        goto Label_0618;

                    case 0x10:
                        switch (empty)
                        {
                            case StorageType.SByte:
                                goto Label_0709;

                            case StorageType.Byte:
                                goto Label_06B9;

                            case StorageType.Int16:
                                goto Label_073D;

                            case StorageType.UInt16:
                                goto Label_078D;

                            case StorageType.Int32:
                                goto Label_07C1;

                            case StorageType.UInt32:
                                goto Label_0801;

                            case StorageType.Int64:
                                goto Label_0825;

                            case StorageType.UInt64:
                                goto Label_0865;

                            case StorageType.Single:
                                goto Label_08CD;

                            case StorageType.Double:
                                goto Label_090D;

                            case StorageType.Decimal:
                                goto Label_0889;

                            case StorageType.DateTime:
                                goto Label_0969;

                            case StorageType.TimeSpan:
                                goto Label_0985;

                            case StorageType.SqlByte:
                                goto Label_06ED;

                            case StorageType.SqlDateTime:
                                goto Label_09C5;

                            case StorageType.SqlDecimal:
                                goto Label_08B1;

                            case StorageType.SqlDouble:
                                goto Label_0931;

                            case StorageType.SqlInt16:
                                goto Label_0771;

                            case StorageType.SqlInt32:
                                goto Label_07E5;

                            case StorageType.SqlInt64:
                                goto Label_0849;

                            case StorageType.SqlMoney:
                                goto Label_094D;

                            case StorageType.SqlSingle:
                                goto Label_08F1;
                        }
                        goto Label_0A4B;

                    case 0x11:
                        switch (empty)
                        {
                            case StorageType.SByte:
                                goto Label_0B3C;

                            case StorageType.Byte:
                                goto Label_0AEC;

                            case StorageType.Int16:
                                goto Label_0B70;

                            case StorageType.UInt16:
                                goto Label_0BC0;

                            case StorageType.Int32:
                                goto Label_0BF4;

                            case StorageType.UInt32:
                                goto Label_0C34;

                            case StorageType.Int64:
                                goto Label_0C58;

                            case StorageType.UInt64:
                                goto Label_0C98;

                            case StorageType.Single:
                                goto Label_0D00;

                            case StorageType.Double:
                                goto Label_0D5C;

                            case StorageType.Decimal:
                                goto Label_0CBC;

                            case StorageType.SqlByte:
                                goto Label_0B20;

                            case StorageType.SqlDecimal:
                                goto Label_0CE4;

                            case StorageType.SqlDouble:
                                goto Label_0D80;

                            case StorageType.SqlInt16:
                                goto Label_0BA4;

                            case StorageType.SqlInt32:
                                goto Label_0C18;

                            case StorageType.SqlInt64:
                                goto Label_0C7C;

                            case StorageType.SqlMoney:
                                goto Label_0D40;

                            case StorageType.SqlSingle:
                                goto Label_0D24;
                        }
                        goto Label_0D9C;

                    case 0x12:
                        switch (empty)
                        {
                            case StorageType.SByte:
                                goto Label_0E8D;

                            case StorageType.Byte:
                                goto Label_0E3D;

                            case StorageType.Int16:
                                goto Label_0EC1;

                            case StorageType.UInt16:
                                goto Label_0F11;

                            case StorageType.Int32:
                                goto Label_0F45;

                            case StorageType.UInt32:
                                goto Label_0F85;

                            case StorageType.Int64:
                                goto Label_0FCD;

                            case StorageType.UInt64:
                                goto Label_0FA9;

                            case StorageType.Single:
                                goto Label_1051;

                            case StorageType.Double:
                                goto Label_10AD;

                            case StorageType.Decimal:
                                goto Label_100D;

                            case StorageType.SqlByte:
                                goto Label_0E71;

                            case StorageType.SqlDecimal:
                                goto Label_1035;

                            case StorageType.SqlDouble:
                                goto Label_10D5;

                            case StorageType.SqlInt16:
                                goto Label_0EF5;

                            case StorageType.SqlInt32:
                                goto Label_0F69;

                            case StorageType.SqlInt64:
                                goto Label_0FF1;

                            case StorageType.SqlMoney:
                                goto Label_1091;

                            case StorageType.SqlSingle:
                                goto Label_1075;
                        }
                        goto Label_10F1;

                    case 20:
                        if (!ExpressionNode.IsIntegerSql(empty))
                        {
                            goto Label_1636;
                        }
                        if (empty != StorageType.UInt64)
                        {
                            goto Label_1592;
                        }
                        isTrue = Convert.ToUInt64(obj2, base.FormatProvider) % Convert.ToUInt64(obj3, base.FormatProvider);
                        goto Label_1705;

                    case 0x1a:
                        obj2 = Eval(left, row, version, recordNos);
                        if ((obj2 != DBNull.Value) && (!left.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj2)))
                        {
                            goto Label_13AF;
                        }
                        return DBNull.Value;

                    case 0x1b:
                        obj2 = Eval(left, row, version, recordNos);
                        if ((obj2 == DBNull.Value) || DataStorage.IsObjectSqlNull(obj2))
                        {
                            goto Label_14DF;
                        }
                        if ((obj2 is bool) || (obj2 is SqlBoolean))
                        {
                            goto Label_14CB;
                        }
                        obj3 = Eval(right, row, version, recordNos);
                        flag = true;
                        goto Label_1705;

                    case 0x27:
                        obj2 = Eval(left, row, version, recordNos);
                        if ((obj2 == DBNull.Value) || (left.IsSqlColumn && DataStorage.IsObjectSqlNull(obj2)))
                        {
                            return false;
                        }
                        return true;

                    default:
                        throw ExprException.UnsupportedOperator(op);
                }
                isTrue = Convert.ToByte(Convert.ToByte(obj2, base.FormatProvider) + Convert.ToByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_025B:
                isTrue = Convert.ToSByte(Convert.ToSByte(obj2, base.FormatProvider) + Convert.ToSByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_028F:
                isTrue = Convert.ToInt16(Convert.ToInt16(obj2, base.FormatProvider) + Convert.ToInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_02C3:
                isTrue = Convert.ToUInt16(Convert.ToUInt16(obj2, base.FormatProvider) + Convert.ToUInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_02F7:
                isTrue = Convert.ToInt32(obj2, base.FormatProvider) + Convert.ToInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_031B:
                isTrue = Convert.ToUInt32(obj2, base.FormatProvider) + Convert.ToUInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_033F:
                isTrue = Convert.ToUInt64(obj2, base.FormatProvider) + Convert.ToUInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0363:
                isTrue = Convert.ToInt64(obj2, base.FormatProvider) + Convert.ToInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0387:
                isTrue = Convert.ToDecimal(obj2, base.FormatProvider) + Convert.ToDecimal(obj3, base.FormatProvider);
                goto Label_1705;
            Label_03AF:
                isTrue = Convert.ToSingle(obj2, base.FormatProvider) + Convert.ToSingle(obj3, base.FormatProvider);
                goto Label_1705;
            Label_03D3:
                isTrue = Convert.ToDouble(obj2, base.FormatProvider) + Convert.ToDouble(obj3, base.FormatProvider);
                goto Label_1705;
            Label_03F7:
                isTrue = Convert.ToString(obj2, base.FormatProvider) + Convert.ToString(obj3, base.FormatProvider);
                goto Label_1705;
            Label_041A:
                if ((obj2 is TimeSpan) && (obj3 is DateTime))
                {
                    isTrue = ((DateTime) obj3) + ((TimeSpan) obj2);
                }
                else if ((obj2 is DateTime) && (obj3 is TimeSpan))
                {
                    isTrue = ((DateTime) obj2) + ((TimeSpan) obj3);
                }
                else
                {
                    flag = true;
                }
                goto Label_1705;
            Label_047A:
                isTrue = ((TimeSpan) obj2) + ((TimeSpan) obj3);
                goto Label_1705;
            Label_0496:
                isTrue = SqlConvert.ConvertToSqlInt16(obj2) + SqlConvert.ConvertToSqlInt16(obj3);
                goto Label_1705;
            Label_04B2:
                isTrue = SqlConvert.ConvertToSqlInt32(obj2) + SqlConvert.ConvertToSqlInt32(obj3);
                goto Label_1705;
            Label_04CE:
                isTrue = SqlConvert.ConvertToSqlInt64(obj2) + SqlConvert.ConvertToSqlInt64(obj3);
                goto Label_1705;
            Label_04EA:
                isTrue = SqlConvert.ConvertToSqlDouble(obj2) + SqlConvert.ConvertToSqlDouble(obj3);
                goto Label_1705;
            Label_0506:
                isTrue = SqlConvert.ConvertToSqlSingle(obj2) + SqlConvert.ConvertToSqlSingle(obj3);
                goto Label_1705;
            Label_0522:
                isTrue = SqlConvert.ConvertToSqlDecimal(obj2) + SqlConvert.ConvertToSqlDecimal(obj3);
                goto Label_1705;
            Label_053E:
                isTrue = SqlConvert.ConvertToSqlMoney(obj2) + SqlConvert.ConvertToSqlMoney(obj3);
                goto Label_1705;
            Label_055A:
                isTrue = SqlConvert.ConvertToSqlByte(obj2) + SqlConvert.ConvertToSqlByte(obj3);
                goto Label_1705;
            Label_0576:
                isTrue = SqlConvert.ConvertToSqlString(obj2) + SqlConvert.ConvertToSqlString(obj3);
                goto Label_1705;
            Label_0592:
                if ((obj2 is TimeSpan) && (obj3 is SqlDateTime))
                {
                    isTrue = SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(obj3).Value + ((TimeSpan) obj2));
                }
                else if ((obj2 is SqlDateTime) && (obj3 is TimeSpan))
                {
                    isTrue = SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(obj2).Value + ((TimeSpan) obj3));
                }
                else
                {
                    flag = true;
                }
                goto Label_1705;
            Label_0618:
                flag = true;
                goto Label_1705;
            Label_06B9:
                isTrue = Convert.ToByte(Convert.ToByte(obj2, base.FormatProvider) - Convert.ToByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_06ED:
                isTrue = SqlConvert.ConvertToSqlByte(obj2) - SqlConvert.ConvertToSqlByte(obj3);
                goto Label_1705;
            Label_0709:
                isTrue = Convert.ToSByte(Convert.ToSByte(obj2, base.FormatProvider) - Convert.ToSByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_073D:
                isTrue = Convert.ToInt16(Convert.ToInt16(obj2, base.FormatProvider) - Convert.ToInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0771:
                isTrue = SqlConvert.ConvertToSqlInt16(obj2) - SqlConvert.ConvertToSqlInt16(obj3);
                goto Label_1705;
            Label_078D:
                isTrue = Convert.ToUInt16(Convert.ToUInt16(obj2, base.FormatProvider) - Convert.ToUInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_07C1:
                isTrue = Convert.ToInt32(obj2, base.FormatProvider) - Convert.ToInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_07E5:
                isTrue = SqlConvert.ConvertToSqlInt32(obj2) - SqlConvert.ConvertToSqlInt32(obj3);
                goto Label_1705;
            Label_0801:
                isTrue = Convert.ToUInt32(obj2, base.FormatProvider) - Convert.ToUInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0825:
                isTrue = Convert.ToInt64(obj2, base.FormatProvider) - Convert.ToInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0849:
                isTrue = SqlConvert.ConvertToSqlInt64(obj2) - SqlConvert.ConvertToSqlInt64(obj3);
                goto Label_1705;
            Label_0865:
                isTrue = Convert.ToUInt64(obj2, base.FormatProvider) - Convert.ToUInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0889:
                isTrue = Convert.ToDecimal(obj2, base.FormatProvider) - Convert.ToDecimal(obj3, base.FormatProvider);
                goto Label_1705;
            Label_08B1:
                isTrue = SqlConvert.ConvertToSqlDecimal(obj2) - SqlConvert.ConvertToSqlDecimal(obj3);
                goto Label_1705;
            Label_08CD:
                isTrue = Convert.ToSingle(obj2, base.FormatProvider) - Convert.ToSingle(obj3, base.FormatProvider);
                goto Label_1705;
            Label_08F1:
                isTrue = SqlConvert.ConvertToSqlSingle(obj2) - SqlConvert.ConvertToSqlSingle(obj3);
                goto Label_1705;
            Label_090D:
                isTrue = Convert.ToDouble(obj2, base.FormatProvider) - Convert.ToDouble(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0931:
                isTrue = SqlConvert.ConvertToSqlDouble(obj2) - SqlConvert.ConvertToSqlDouble(obj3);
                goto Label_1705;
            Label_094D:
                isTrue = SqlConvert.ConvertToSqlMoney(obj2) - SqlConvert.ConvertToSqlMoney(obj3);
                goto Label_1705;
            Label_0969:
                isTrue = ((DateTime) obj2) - ((TimeSpan) obj3);
                goto Label_1705;
            Label_0985:
                if (obj2 is DateTime)
                {
                    isTrue = (TimeSpan) (((DateTime) obj2) - ((DateTime) obj3));
                }
                else
                {
                    isTrue = ((TimeSpan) obj2) - ((TimeSpan) obj3);
                }
                goto Label_1705;
            Label_09C5:
                if ((obj2 is TimeSpan) && (obj3 is SqlDateTime))
                {
                    isTrue = SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(obj3).Value - ((TimeSpan) obj2));
                }
                else if ((obj2 is SqlDateTime) && (obj3 is TimeSpan))
                {
                    isTrue = SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(obj2).Value - ((TimeSpan) obj3));
                }
                else
                {
                    flag = true;
                }
                goto Label_1705;
            Label_0A4B:
                flag = true;
                goto Label_1705;
            Label_0AEC:
                isTrue = Convert.ToByte(Convert.ToByte(obj2, base.FormatProvider) * Convert.ToByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0B20:
                isTrue = SqlConvert.ConvertToSqlByte(obj2) * SqlConvert.ConvertToSqlByte(obj3);
                goto Label_1705;
            Label_0B3C:
                isTrue = Convert.ToSByte(Convert.ToSByte(obj2, base.FormatProvider) * Convert.ToSByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0B70:
                isTrue = Convert.ToInt16(Convert.ToInt16(obj2, base.FormatProvider) * Convert.ToInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0BA4:
                isTrue = SqlConvert.ConvertToSqlInt16(obj2) * SqlConvert.ConvertToSqlInt16(obj3);
                goto Label_1705;
            Label_0BC0:
                isTrue = Convert.ToUInt16(Convert.ToUInt16(obj2, base.FormatProvider) * Convert.ToUInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0BF4:
                isTrue = Convert.ToInt32(obj2, base.FormatProvider) * Convert.ToInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0C18:
                isTrue = SqlConvert.ConvertToSqlInt32(obj2) * SqlConvert.ConvertToSqlInt32(obj3);
                goto Label_1705;
            Label_0C34:
                isTrue = Convert.ToUInt32(obj2, base.FormatProvider) * Convert.ToUInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0C58:
                isTrue = Convert.ToInt64(obj2, base.FormatProvider) * Convert.ToInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0C7C:
                isTrue = SqlConvert.ConvertToSqlInt64(obj2) * SqlConvert.ConvertToSqlInt64(obj3);
                goto Label_1705;
            Label_0C98:
                isTrue = Convert.ToUInt64(obj2, base.FormatProvider) * Convert.ToUInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0CBC:
                isTrue = Convert.ToDecimal(obj2, base.FormatProvider) * Convert.ToDecimal(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0CE4:
                isTrue = SqlConvert.ConvertToSqlDecimal(obj2) * SqlConvert.ConvertToSqlDecimal(obj3);
                goto Label_1705;
            Label_0D00:
                isTrue = Convert.ToSingle(obj2, base.FormatProvider) * Convert.ToSingle(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0D24:
                isTrue = SqlConvert.ConvertToSqlSingle(obj2) * SqlConvert.ConvertToSqlSingle(obj3);
                goto Label_1705;
            Label_0D40:
                isTrue = SqlConvert.ConvertToSqlMoney(obj2) * SqlConvert.ConvertToSqlMoney(obj3);
                goto Label_1705;
            Label_0D5C:
                isTrue = Convert.ToDouble(obj2, base.FormatProvider) * Convert.ToDouble(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0D80:
                isTrue = SqlConvert.ConvertToSqlDouble(obj2) * SqlConvert.ConvertToSqlDouble(obj3);
                goto Label_1705;
            Label_0D9C:
                flag = true;
                goto Label_1705;
            Label_0E3D:
                isTrue = Convert.ToByte(Convert.ToByte(obj2, base.FormatProvider) / Convert.ToByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0E71:
                isTrue = SqlConvert.ConvertToSqlByte(obj2) / SqlConvert.ConvertToSqlByte(obj3);
                goto Label_1705;
            Label_0E8D:
                isTrue = Convert.ToSByte(Convert.ToSByte(obj2, base.FormatProvider) / Convert.ToSByte(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0EC1:
                isTrue = Convert.ToInt16(Convert.ToInt16(obj2, base.FormatProvider) / Convert.ToInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0EF5:
                isTrue = SqlConvert.ConvertToSqlInt16(obj2) / SqlConvert.ConvertToSqlInt16(obj3);
                goto Label_1705;
            Label_0F11:
                isTrue = Convert.ToUInt16(Convert.ToUInt16(obj2, base.FormatProvider) / Convert.ToUInt16(obj3, base.FormatProvider), base.FormatProvider);
                goto Label_1705;
            Label_0F45:
                isTrue = Convert.ToInt32(obj2, base.FormatProvider) / Convert.ToInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0F69:
                isTrue = SqlConvert.ConvertToSqlInt32(obj2) / SqlConvert.ConvertToSqlInt32(obj3);
                goto Label_1705;
            Label_0F85:
                isTrue = Convert.ToUInt32(obj2, base.FormatProvider) / Convert.ToUInt32(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0FA9:
                isTrue = Convert.ToUInt64(obj2, base.FormatProvider) / Convert.ToUInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0FCD:
                isTrue = Convert.ToInt64(obj2, base.FormatProvider) / Convert.ToInt64(obj3, base.FormatProvider);
                goto Label_1705;
            Label_0FF1:
                isTrue = SqlConvert.ConvertToSqlInt64(obj2) / SqlConvert.ConvertToSqlInt64(obj3);
                goto Label_1705;
            Label_100D:
                isTrue = Convert.ToDecimal(obj2, base.FormatProvider) / Convert.ToDecimal(obj3, base.FormatProvider);
                goto Label_1705;
            Label_1035:
                isTrue = SqlConvert.ConvertToSqlDecimal(obj2) / SqlConvert.ConvertToSqlDecimal(obj3);
                goto Label_1705;
            Label_1051:
                isTrue = Convert.ToSingle(obj2, base.FormatProvider) / Convert.ToSingle(obj3, base.FormatProvider);
                goto Label_1705;
            Label_1075:
                isTrue = SqlConvert.ConvertToSqlSingle(obj2) / SqlConvert.ConvertToSqlSingle(obj3);
                goto Label_1705;
            Label_1091:
                isTrue = SqlConvert.ConvertToSqlMoney(obj2) / SqlConvert.ConvertToSqlMoney(obj3);
                goto Label_1705;
            Label_10AD:
                num4 = Convert.ToDouble(obj3, base.FormatProvider);
                isTrue = Convert.ToDouble(obj2, base.FormatProvider) / num4;
                goto Label_1705;
            Label_10D5:
                isTrue = SqlConvert.ConvertToSqlDouble(obj2) / SqlConvert.ConvertToSqlDouble(obj3);
                goto Label_1705;
            Label_10F1:
                flag = true;
                goto Label_1705;
            Label_13AF:
                if (!(obj2 is bool) && !(obj2 is SqlBoolean))
                {
                    obj3 = Eval(right, row, version, recordNos);
                    flag = true;
                    goto Label_1705;
                }
                if (obj2 is bool)
                {
                    if ((bool) obj2)
                    {
                        goto Label_140D;
                    }
                    isTrue = false;
                    goto Label_1705;
                }
                SqlBoolean flag6 = (SqlBoolean) obj2;
                if (flag6.IsFalse)
                {
                    isTrue = false;
                    goto Label_1705;
                }
            Label_140D:
                obj3 = Eval(right, row, version, recordNos);
                if ((obj3 == DBNull.Value) || (right.IsSqlColumn && DataStorage.IsObjectSqlNull(obj3)))
                {
                    return DBNull.Value;
                }
                if ((obj3 is bool) || (obj3 is SqlBoolean))
                {
                    if (obj3 is bool)
                    {
                        isTrue = (bool) obj3;
                    }
                    else
                    {
                        SqlBoolean flag5 = (SqlBoolean) obj3;
                        isTrue = flag5.IsTrue;
                    }
                }
                else
                {
                    flag = true;
                }
                goto Label_1705;
            Label_14CB:
                if ((bool) obj2)
                {
                    isTrue = true;
                    goto Label_1705;
                }
            Label_14DF:
                obj3 = Eval(right, row, version, recordNos);
                if ((obj3 == DBNull.Value) || DataStorage.IsObjectSqlNull(obj3))
                {
                    return obj2;
                }
                if ((obj2 == DBNull.Value) || DataStorage.IsObjectSqlNull(obj2))
                {
                    return obj3;
                }
                if ((obj3 is bool) || (obj3 is SqlBoolean))
                {
                    isTrue = (obj3 is bool) ? ((bool) obj3) : ((SqlBoolean) obj3).IsTrue;
                }
                else
                {
                    flag = true;
                }
                goto Label_1705;
            Label_1592:
                if (DataStorage.IsSqlType(empty))
                {
                    SqlInt64 num3 = SqlConvert.ConvertToSqlInt64(obj2) % SqlConvert.ConvertToSqlInt64(obj3);
                    switch (empty)
                    {
                        case StorageType.SqlInt32:
                            isTrue = num3.ToSqlInt32();
                            goto Label_1705;

                        case StorageType.SqlInt16:
                            isTrue = num3.ToSqlInt16();
                            goto Label_1705;

                        case StorageType.SqlByte:
                            isTrue = num3.ToSqlByte();
                            goto Label_1705;
                    }
                    isTrue = num3;
                }
                else
                {
                    isTrue = Convert.ToInt64(obj2, base.FormatProvider) % Convert.ToInt64(obj3, base.FormatProvider);
                    isTrue = Convert.ChangeType(isTrue, DataStorage.GetTypeStorage(empty), base.FormatProvider);
                }
                goto Label_1705;
            Label_1636:
                flag = true;
                goto Label_1705;
            Label_164C:
                obj2 = Eval(left, row, version, recordNos);
                if ((obj2 == DBNull.Value) || (left.IsSqlColumn && DataStorage.IsObjectSqlNull(obj2)))
                {
                    return DBNull.Value;
                }
                isTrue = false;
                FunctionNode node = (FunctionNode) right;
                for (int i = 0; i < node.argumentCount; i++)
                {
                    obj3 = node.arguments[i].Eval();
                    if ((obj3 != DBNull.Value) && (!right.IsSqlColumn || !DataStorage.IsObjectSqlNull(obj3)))
                    {
                        empty = DataStorage.GetStorageType(obj2.GetType());
                        if (this.BinaryCompare(obj2, obj3, empty, 7) == 0)
                        {
                            isTrue = true;
                            break;
                        }
                    }
                }
            }
            catch (OverflowException)
            {
                throw ExprException.Overflow(DataStorage.GetTypeStorage(empty));
            }
        Label_1705:
            if (flag)
            {
                this.SetTypeMismatchError(op, obj2.GetType(), obj3.GetType());
            }
            return isTrue;
        }

        private DataTypePrecedence GetPrecedence(StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Boolean:
                    return DataTypePrecedence.Boolean;

                case StorageType.Char:
                    return DataTypePrecedence.Char;

                case StorageType.SByte:
                    return DataTypePrecedence.SByte;

                case StorageType.Byte:
                    return DataTypePrecedence.Byte;

                case StorageType.Int16:
                    return DataTypePrecedence.Int16;

                case StorageType.UInt16:
                    return DataTypePrecedence.UInt16;

                case StorageType.Int32:
                    return DataTypePrecedence.Int32;

                case StorageType.UInt32:
                    return DataTypePrecedence.UInt32;

                case StorageType.Int64:
                    return DataTypePrecedence.Int64;

                case StorageType.UInt64:
                    return DataTypePrecedence.UInt64;

                case StorageType.Single:
                    return DataTypePrecedence.Single;

                case StorageType.Double:
                    return DataTypePrecedence.Double;

                case StorageType.Decimal:
                    return DataTypePrecedence.Decimal;

                case StorageType.DateTime:
                    return DataTypePrecedence.DateTime;

                case StorageType.TimeSpan:
                    return DataTypePrecedence.TimeSpan;

                case StorageType.String:
                    return DataTypePrecedence.String;

                case StorageType.DateTimeOffset:
                    return DataTypePrecedence.DateTimeOffset;

                case StorageType.SqlBinary:
                    return DataTypePrecedence.SqlBinary;

                case StorageType.SqlBoolean:
                    return DataTypePrecedence.SqlBoolean;

                case StorageType.SqlByte:
                    return DataTypePrecedence.SqlByte;

                case StorageType.SqlBytes:
                    return DataTypePrecedence.SqlBytes;

                case StorageType.SqlChars:
                    return DataTypePrecedence.SqlChars;

                case StorageType.SqlDateTime:
                    return DataTypePrecedence.SqlDateTime;

                case StorageType.SqlDecimal:
                    return DataTypePrecedence.SqlDecimal;

                case StorageType.SqlDouble:
                    return DataTypePrecedence.SqlDouble;

                case StorageType.SqlGuid:
                    return DataTypePrecedence.SqlGuid;

                case StorageType.SqlInt16:
                    return DataTypePrecedence.SqlInt16;

                case StorageType.SqlInt32:
                    return DataTypePrecedence.SqlInt32;

                case StorageType.SqlInt64:
                    return DataTypePrecedence.SqlInt64;

                case StorageType.SqlMoney:
                    return DataTypePrecedence.SqlMoney;

                case StorageType.SqlSingle:
                    return DataTypePrecedence.SqlSingle;

                case StorageType.SqlString:
                    return DataTypePrecedence.SqlString;
            }
            return DataTypePrecedence.Error;
        }

        private static StorageType GetPrecedenceType(DataTypePrecedence code)
        {
            switch (code)
            {
                case DataTypePrecedence.SqlBinary:
                    return StorageType.SqlBinary;

                case DataTypePrecedence.Char:
                    return StorageType.Char;

                case DataTypePrecedence.String:
                    return StorageType.String;

                case DataTypePrecedence.SqlString:
                    return StorageType.SqlString;

                case DataTypePrecedence.SqlGuid:
                    return StorageType.SqlGuid;

                case DataTypePrecedence.Boolean:
                    return StorageType.Boolean;

                case DataTypePrecedence.SqlBoolean:
                    return StorageType.SqlBoolean;

                case DataTypePrecedence.SByte:
                    return StorageType.SByte;

                case DataTypePrecedence.SqlByte:
                    return StorageType.SqlByte;

                case DataTypePrecedence.Byte:
                    return StorageType.Byte;

                case DataTypePrecedence.Int16:
                    return StorageType.Int16;

                case DataTypePrecedence.SqlInt16:
                    return StorageType.SqlInt16;

                case DataTypePrecedence.UInt16:
                    return StorageType.UInt16;

                case DataTypePrecedence.Int32:
                    return StorageType.Int32;

                case DataTypePrecedence.SqlInt32:
                    return StorageType.SqlInt32;

                case DataTypePrecedence.UInt32:
                    return StorageType.UInt32;

                case DataTypePrecedence.Int64:
                    return StorageType.Int64;

                case DataTypePrecedence.SqlInt64:
                    return StorageType.SqlInt64;

                case DataTypePrecedence.UInt64:
                    return StorageType.UInt64;

                case DataTypePrecedence.SqlMoney:
                    return StorageType.SqlMoney;

                case DataTypePrecedence.Decimal:
                    return StorageType.Decimal;

                case DataTypePrecedence.SqlDecimal:
                    return StorageType.SqlDecimal;

                case DataTypePrecedence.Single:
                    return StorageType.Single;

                case DataTypePrecedence.SqlSingle:
                    return StorageType.SqlSingle;

                case DataTypePrecedence.Double:
                    return StorageType.Double;

                case DataTypePrecedence.SqlDouble:
                    return StorageType.SqlDouble;

                case DataTypePrecedence.TimeSpan:
                    return StorageType.TimeSpan;

                case DataTypePrecedence.DateTime:
                    return StorageType.DateTime;

                case DataTypePrecedence.DateTimeOffset:
                    return StorageType.DateTimeOffset;

                case DataTypePrecedence.SqlDateTime:
                    return StorageType.SqlDateTime;
            }
            return StorageType.Empty;
        }

        internal override bool HasLocalAggregate()
        {
            if (!this.left.HasLocalAggregate())
            {
                return this.right.HasLocalAggregate();
            }
            return true;
        }

        internal override bool HasRemoteAggregate()
        {
            if (!this.left.HasRemoteAggregate())
            {
                return this.right.HasRemoteAggregate();
            }
            return true;
        }

        internal override bool IsConstant() => 
            (this.left.IsConstant() && this.right.IsConstant());

        private bool IsMixed(StorageType left, StorageType right) => 
            ((ExpressionNode.IsSigned(left) && ExpressionNode.IsUnsigned(right)) || (ExpressionNode.IsUnsigned(left) && ExpressionNode.IsSigned(right)));

        private bool IsMixedSql(StorageType left, StorageType right) => 
            ((ExpressionNode.IsSignedSql(left) && ExpressionNode.IsUnsignedSql(right)) || (ExpressionNode.IsUnsignedSql(left) && ExpressionNode.IsSignedSql(right)));

        internal override bool IsTableConstant() => 
            (this.left.IsTableConstant() && this.right.IsTableConstant());

        internal override ExpressionNode Optimize()
        {
            this.left = this.left.Optimize();
            if (this.op == 13)
            {
                if (this.right is UnaryNode)
                {
                    UnaryNode right = (UnaryNode) this.right;
                    if (right.op != 3)
                    {
                        throw ExprException.InvalidIsSyntax();
                    }
                    this.op = 0x27;
                    this.right = right.right;
                }
                if (!(this.right is ZeroOpNode))
                {
                    throw ExprException.InvalidIsSyntax();
                }
                if (((ZeroOpNode) this.right).op != 0x20)
                {
                    throw ExprException.InvalidIsSyntax();
                }
            }
            else
            {
                this.right = this.right.Optimize();
            }
            if (!this.IsConstant())
            {
                return this;
            }
            object constant = this.Eval();
            if (constant == DBNull.Value)
            {
                return new ZeroOpNode(0x20);
            }
            if (!(constant is bool))
            {
                return new ConstNode(base.table, System.Data.ValueType.Object, constant, false);
            }
            if ((bool) constant)
            {
                return new ZeroOpNode(0x21);
            }
            return new ZeroOpNode(0x22);
        }

        internal StorageType ResultSqlType(StorageType left, StorageType right, bool lc, bool rc, int op)
        {
            int num = (int) this.GetPrecedence(left);
            if (num == 0)
            {
                return StorageType.Empty;
            }
            int num2 = (int) this.GetPrecedence(right);
            if (num2 == 0)
            {
                return StorageType.Empty;
            }
            if (Operators.IsLogical(op))
            {
                if (((left != StorageType.Boolean) && (left != StorageType.SqlBoolean)) || ((right != StorageType.Boolean) && (right != StorageType.SqlBoolean)))
                {
                    return StorageType.Empty;
                }
                if ((left == StorageType.Boolean) && (right == StorageType.Boolean))
                {
                    return StorageType.Boolean;
                }
                return StorageType.SqlBoolean;
            }
            if (op == 15)
            {
                if ((left == StorageType.SqlString) || (right == StorageType.SqlString))
                {
                    return StorageType.SqlString;
                }
                if ((left == StorageType.String) || (right == StorageType.String))
                {
                    return StorageType.String;
                }
            }
            if (((left == StorageType.SqlBinary) && (right != StorageType.SqlBinary)) || ((left != StorageType.SqlBinary) && (right == StorageType.SqlBinary)))
            {
                return StorageType.Empty;
            }
            if (((left == StorageType.SqlGuid) && (right != StorageType.SqlGuid)) || ((left != StorageType.SqlGuid) && (right == StorageType.SqlGuid)))
            {
                return StorageType.Empty;
            }
            if ((num > 0x13) && (num2 < 20))
            {
                return StorageType.Empty;
            }
            if ((num < 20) && (num2 > 0x13))
            {
                return StorageType.Empty;
            }
            if (num > 0x13)
            {
                if ((op == 15) || (op == 0x10))
                {
                    if (left == StorageType.TimeSpan)
                    {
                        return right;
                    }
                    if (right == StorageType.TimeSpan)
                    {
                        return left;
                    }
                    return StorageType.Empty;
                }
                if (!Operators.IsRelational(op))
                {
                    return StorageType.Empty;
                }
                return left;
            }
            DataTypePrecedence code = (DataTypePrecedence) Math.Max(num, num2);
            StorageType precedenceType = GetPrecedenceType(code);
            precedenceType = GetPrecedenceType((DataTypePrecedence) this.SqlResultType((int) code));
            if ((Operators.IsArithmetical(op) && (precedenceType != StorageType.String)) && ((precedenceType != StorageType.Char) && (precedenceType != StorageType.SqlString)))
            {
                if (!ExpressionNode.IsNumericSql(left))
                {
                    return StorageType.Empty;
                }
                if (!ExpressionNode.IsNumericSql(right))
                {
                    return StorageType.Empty;
                }
            }
            if ((op == 0x12) && ExpressionNode.IsIntegerSql(precedenceType))
            {
                return StorageType.SqlDouble;
            }
            if (((precedenceType == StorageType.SqlMoney) && (left != StorageType.SqlMoney)) && (right != StorageType.SqlMoney))
            {
                precedenceType = StorageType.SqlDecimal;
            }
            if (!this.IsMixedSql(left, right) || !ExpressionNode.IsUnsignedSql(precedenceType))
            {
                return precedenceType;
            }
            if (code >= DataTypePrecedence.UInt64)
            {
                throw ExprException.AmbiguousBinop(op, DataStorage.GetTypeStorage(left), DataStorage.GetTypeStorage(right));
            }
            return GetPrecedenceType(code + 1);
        }

        internal StorageType ResultType(StorageType left, StorageType right, bool lc, bool rc, int op)
        {
            if (((left == StorageType.Guid) && (right == StorageType.Guid)) && Operators.IsRelational(op))
            {
                return left;
            }
            if (((left == StorageType.String) && (right == StorageType.Guid)) && Operators.IsRelational(op))
            {
                return left;
            }
            if (((left == StorageType.Guid) && (right == StorageType.String)) && Operators.IsRelational(op))
            {
                return right;
            }
            int num2 = (int) this.GetPrecedence(left);
            if (num2 == 0)
            {
                return StorageType.Empty;
            }
            int num = (int) this.GetPrecedence(right);
            if (num == 0)
            {
                return StorageType.Empty;
            }
            if (Operators.IsLogical(op))
            {
                if ((left == StorageType.Boolean) && (right == StorageType.Boolean))
                {
                    return StorageType.Boolean;
                }
                return StorageType.Empty;
            }
            if ((left == StorageType.DateTimeOffset) || (right == StorageType.DateTimeOffset))
            {
                if ((Operators.IsRelational(op) && (left == StorageType.DateTimeOffset)) && (right == StorageType.DateTimeOffset))
                {
                    return StorageType.DateTimeOffset;
                }
                return StorageType.Empty;
            }
            if ((op == 15) && ((left == StorageType.String) || (right == StorageType.String)))
            {
                return StorageType.String;
            }
            DataTypePrecedence code = (DataTypePrecedence) Math.Max(num2, num);
            StorageType precedenceType = GetPrecedenceType(code);
            if ((Operators.IsArithmetical(op) && (precedenceType != StorageType.String)) && (precedenceType != StorageType.Char))
            {
                if (!ExpressionNode.IsNumeric(left))
                {
                    return StorageType.Empty;
                }
                if (!ExpressionNode.IsNumeric(right))
                {
                    return StorageType.Empty;
                }
            }
            if ((op == 0x12) && ExpressionNode.IsInteger(precedenceType))
            {
                return StorageType.Double;
            }
            if (!this.IsMixed(left, right))
            {
                return precedenceType;
            }
            if (lc && !rc)
            {
                return right;
            }
            if (!lc && rc)
            {
                return left;
            }
            if (!ExpressionNode.IsUnsigned(precedenceType))
            {
                return precedenceType;
            }
            if (code >= DataTypePrecedence.UInt64)
            {
                throw ExprException.AmbiguousBinop(op, DataStorage.GetTypeStorage(left), DataStorage.GetTypeStorage(right));
            }
            return GetPrecedenceType(code + 1);
        }

        internal void SetTypeMismatchError(int op, Type left, Type right)
        {
            throw ExprException.TypeMismatchInBinop(op, left, right);
        }

        private int SqlResultType(int typeCode)
        {
            switch (typeCode)
            {
                case -8:
                    return -7;

                case -7:
                case -6:
                case -4:
                case -3:
                case -1:
                case 0:
                case 2:
                case 5:
                case 8:
                case 11:
                case 13:
                case 15:
                case 0x11:
                case 0x13:
                    return typeCode;

                case -5:
                    return -4;

                case -2:
                    return -1;

                case 1:
                    return 2;

                case 3:
                case 4:
                    return 5;

                case 6:
                case 7:
                    return 8;

                case 9:
                case 10:
                    return 11;

                case 12:
                    return 13;

                case 14:
                    return 15;

                case 0x10:
                    return 0x11;

                case 0x12:
                    return 0x13;

                case 20:
                    return 0x15;

                case 0x17:
                    return 0x18;
            }
            return typeCode;
        }

        private enum DataTypePrecedence
        {
            Boolean = -2,
            Byte = 3,
            Char = -8,
            DateTime = 0x17,
            DateTimeOffset = 0x18,
            Decimal = 14,
            Double = 0x12,
            Error = 0,
            Int16 = 4,
            Int32 = 7,
            Int64 = 10,
            SByte = 1,
            Single = 0x10,
            SqlBinary = -10,
            SqlBoolean = -1,
            SqlByte = 2,
            SqlBytes = -9,
            SqlChars = -7,
            SqlDateTime = 0x19,
            SqlDecimal = 15,
            SqlDouble = 0x13,
            SqlGuid = -3,
            SqlInt16 = 5,
            SqlInt32 = 8,
            SqlInt64 = 11,
            SqlMoney = 13,
            SqlSingle = 0x11,
            SqlString = -4,
            SqlXml = -6,
            String = -5,
            TimeSpan = 20,
            UInt16 = 6,
            UInt32 = 9,
            UInt64 = 12
        }
    }
}

