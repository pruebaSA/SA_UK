namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;

    internal static class SqlTypeSystem
    {
        private const int defaultDecimalPrecision = 0x1d;
        private const int defaultDecimalScale = 4;
        internal const short LargeTypeSizeIndicator = -1;
        private static readonly SqlType theBigInt = new SqlType(SqlDbType.BigInt);
        private static readonly SqlType theBit = new SqlType(SqlDbType.Bit);
        private static readonly SqlType theChar = new SqlType(SqlDbType.Char);
        private static readonly SqlType theDate = new SqlType(SqlDbType.Date);
        private static readonly SqlType theDateTime = new SqlType(SqlDbType.DateTime);
        private static readonly SqlType theDateTime2 = new SqlType(SqlDbType.DateTime2);
        private static readonly SqlType theDateTimeOffset = new SqlType(SqlDbType.DateTimeOffset);
        private static readonly SqlType theDefaultDecimal = new SqlType(SqlDbType.Decimal, 0x1d, 4);
        private static readonly SqlType theFloat = new SqlType(SqlDbType.Float);
        private static readonly SqlType theImage = new SqlType(SqlDbType.Image, -1);
        private static readonly SqlType theInt = new SqlType(SqlDbType.Int);
        private static readonly SqlType theMoney = new SqlType(SqlDbType.Money, 0x13, 4);
        private static readonly SqlType theNText = new SqlType(SqlDbType.NText, -1);
        private static readonly SqlType theReal = new SqlType(SqlDbType.Real);
        private static readonly SqlType theSmallDateTime = new SqlType(SqlDbType.SmallDateTime);
        private static readonly SqlType theSmallInt = new SqlType(SqlDbType.SmallInt);
        private static readonly SqlType theSmallMoney = new SqlType(SqlDbType.SmallMoney, 10, 4);
        private static readonly SqlType theText = new SqlType(SqlDbType.Text, -1);
        private static readonly SqlType theTime = new SqlType(SqlDbType.Time);
        private static readonly SqlType theTimestamp = new SqlType(SqlDbType.Timestamp);
        private static readonly SqlType theTinyInt = new SqlType(SqlDbType.TinyInt);
        private static readonly SqlType theUniqueIdentifier = new SqlType(SqlDbType.UniqueIdentifier);
        private static readonly SqlType theXml = new SqlType(SqlDbType.Xml, -1);

        private static ProviderType Create(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.BigInt:
                    return theBigInt;

                case SqlDbType.Bit:
                    return theBit;

                case SqlDbType.Char:
                    return theChar;

                case SqlDbType.DateTime:
                    return theDateTime;

                case SqlDbType.Decimal:
                    return theDefaultDecimal;

                case SqlDbType.Float:
                    return theFloat;

                case SqlDbType.Image:
                    return theImage;

                case SqlDbType.Int:
                    return theInt;

                case SqlDbType.Money:
                    return theMoney;

                case SqlDbType.NText:
                    return theNText;

                case SqlDbType.Real:
                    return theReal;

                case SqlDbType.UniqueIdentifier:
                    return theUniqueIdentifier;

                case SqlDbType.SmallDateTime:
                    return theSmallDateTime;

                case SqlDbType.SmallInt:
                    return theSmallInt;

                case SqlDbType.SmallMoney:
                    return theSmallMoney;

                case SqlDbType.Text:
                    return theText;

                case SqlDbType.Timestamp:
                    return theTimestamp;

                case SqlDbType.TinyInt:
                    return theTinyInt;

                case SqlDbType.Xml:
                    return theXml;

                case SqlDbType.Date:
                    return theDate;

                case SqlDbType.Time:
                    return theTime;

                case SqlDbType.DateTime2:
                    return theDateTime2;

                case SqlDbType.DateTimeOffset:
                    return theDateTimeOffset;
            }
            return new SqlType(type);
        }

        private static ProviderType Create(SqlDbType type, int size) => 
            new SqlType(type, new int?(size));

        private static ProviderType Create(SqlDbType type, int precision, int scale)
        {
            if (((type != SqlDbType.Decimal) && (precision == 0)) && (scale == 0))
            {
                return Create(type);
            }
            if (((type == SqlDbType.Decimal) && (precision == 0x1d)) && (scale == 4))
            {
                return Create(type);
            }
            return new SqlType(type, precision, scale);
        }

        internal static TypeSystemProvider Create2000Provider() => 
            new Sql2000Provider();

        internal static TypeSystemProvider Create2005Provider() => 
            new Sql2005Provider();

        internal static TypeSystemProvider Create2008Provider() => 
            new Sql2008Provider();

        internal static TypeSystemProvider CreateCEProvider() => 
            new SqlCEProvider();

        private static Type GetClosestRuntimeType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.DateTime2:
                    return typeof(DateTime);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal);

                case SqlDbType.Float:
                    return typeof(double);

                case SqlDbType.Int:
                    return typeof(int);

                case SqlDbType.Real:
                    return typeof(float);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);

                case SqlDbType.SmallInt:
                    return typeof(short);

                case SqlDbType.TinyInt:
                    return typeof(byte);

                case SqlDbType.Udt:
                    throw System.Data.Linq.SqlClient.Error.UnexpectedTypeCode(SqlDbType.Udt);

                case SqlDbType.Time:
                    return typeof(TimeSpan);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset);
            }
            return typeof(object);
        }

        private abstract class ProviderBase : TypeSystemProvider
        {
            protected Dictionary<int, SqlTypeSystem.SqlType> applicationTypes = new Dictionary<int, SqlTypeSystem.SqlType>();

            protected ProviderBase()
            {
            }

            internal override ProviderType ChangeTypeFamilyTo(ProviderType type, ProviderType toType)
            {
                if (type.IsSameTypeFamily(toType))
                {
                    return type;
                }
                if (type.IsApplicationType || toType.IsApplicationType)
                {
                    return toType;
                }
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) toType;
                SqlTypeSystem.SqlType type3 = (SqlTypeSystem.SqlType) type;
                if ((type2.Category != SqlTypeSystem.SqlType.TypeCategory.Numeric) || (type3.Category != SqlTypeSystem.SqlType.TypeCategory.Char))
                {
                    return toType;
                }
                SqlDbType sqlDbType = type3.SqlDbType;
                if (sqlDbType != SqlDbType.Char)
                {
                    if (sqlDbType == SqlDbType.NChar)
                    {
                        return SqlTypeSystem.Create(SqlDbType.Int);
                    }
                    return toType;
                }
                return SqlTypeSystem.Create(SqlDbType.SmallInt);
            }

            internal override ProviderType From(object o)
            {
                Type type = (o != null) ? o.GetType() : typeof(object);
                if (type == typeof(string))
                {
                    string str = (string) o;
                    return this.From(type, new int?(str.Length));
                }
                if (type == typeof(bool))
                {
                    return this.From(typeof(int));
                }
                if (type.IsArray)
                {
                    Array array = (Array) o;
                    return this.From(type, new int?(array.Length));
                }
                if (type == typeof(decimal))
                {
                    decimal d = (decimal) o;
                    int num2 = (decimal.GetBits(d)[3] & 0xff0000) >> 0x10;
                    return this.From(type, new int?(num2));
                }
                return this.From(type);
            }

            internal override ProviderType From(Type type) => 
                this.From(type, null);

            internal override ProviderType From(Type type, int? size) => 
                this.From(type, size);

            internal override ProviderType GetApplicationType(int index)
            {
                if (index < 0)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("index");
                }
                SqlTypeSystem.SqlType type = null;
                if (!this.applicationTypes.TryGetValue(index, out type))
                {
                    type = new SqlTypeSystem.SqlType(index);
                    this.applicationTypes.Add(index, type);
                }
                return type;
            }

            private ProviderType[] GetArgumentTypes(SqlFunctionCall fc)
            {
                ProviderType[] typeArray = new ProviderType[fc.Arguments.Count];
                int index = 0;
                int length = typeArray.Length;
                while (index < length)
                {
                    typeArray[index] = fc.Arguments[index].SqlType;
                    index++;
                }
                return typeArray;
            }

            internal override ProviderType GetBestType(ProviderType typeA, ProviderType typeB)
            {
                SqlTypeSystem.SqlType type = (typeA.ComparePrecedenceTo(typeB) > 0) ? ((SqlTypeSystem.SqlType) typeA) : ((SqlTypeSystem.SqlType) typeB);
                if (typeA.IsApplicationType || typeA.IsRuntimeOnlyType)
                {
                    return typeA;
                }
                if (typeB.IsApplicationType || typeB.IsRuntimeOnlyType)
                {
                    return typeB;
                }
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) typeA;
                SqlTypeSystem.SqlType type3 = (SqlTypeSystem.SqlType) typeB;
                if ((type2.HasPrecisionAndScale && type3.HasPrecisionAndScale) && (type.SqlDbType == SqlDbType.Decimal))
                {
                    int precision = type2.Precision;
                    int scale = type2.Scale;
                    int num3 = type3.Precision;
                    int num4 = type3.Scale;
                    if (((precision == 0) && (scale == 0)) && ((num3 == 0) && (num4 == 0)))
                    {
                        return SqlTypeSystem.Create(type.SqlDbType);
                    }
                    if ((precision == 0) && (scale == 0))
                    {
                        return SqlTypeSystem.Create(type.SqlDbType, num3, num4);
                    }
                    if ((num3 == 0) && (num4 == 0))
                    {
                        return SqlTypeSystem.Create(type.SqlDbType, precision, scale);
                    }
                    int num5 = Math.Max((int) (precision - scale), (int) (num3 - num4));
                    int num6 = Math.Max(scale, num4);
                    int num7 = Math.Min(num5 + num6, 0x26);
                    return SqlTypeSystem.Create(type.SqlDbType, num7, num6);
                }
                int? size = null;
                if (type2.Size.HasValue && type3.Size.HasValue)
                {
                    int? nullable4 = type3.Size;
                    int? nullable5 = type2.Size;
                    size = ((nullable4.GetValueOrDefault() > nullable5.GetValueOrDefault()) && (nullable4.HasValue & nullable5.HasValue)) ? type3.Size : type2.Size;
                }
                if ((type3.Size.HasValue && (type3.Size.Value == -1)) || (type2.Size.HasValue && (type2.Size.Value == -1)))
                {
                    size = -1;
                }
                return new SqlTypeSystem.SqlType(type.SqlDbType, size);
            }

            protected ProviderType GetBestType(SqlDbType targetType, int? size)
            {
                int num = 0;
                switch (targetType)
                {
                    case SqlDbType.Binary:
                    case SqlDbType.Char:
                    case SqlDbType.VarBinary:
                    case SqlDbType.VarChar:
                        num = 0x1f40;
                        break;

                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                        num = 0xfa0;
                        break;
                }
                if (!size.HasValue)
                {
                    return SqlTypeSystem.Create(targetType, this.SupportsMaxSize ? -1 : num);
                }
                if (size.Value <= num)
                {
                    return SqlTypeSystem.Create(targetType, size.Value);
                }
                return this.GetBestLargeType(SqlTypeSystem.Create(targetType));
            }

            protected object GetParameterValue(SqlTypeSystem.SqlType type, object value)
            {
                if (value == null)
                {
                    return DBNull.Value;
                }
                Type type2 = value.GetType();
                Type closestRuntimeType = type.GetClosestRuntimeType();
                if (closestRuntimeType == type2)
                {
                    return value;
                }
                return DBConvert.ChangeType(value, closestRuntimeType);
            }

            internal override void InitializeParameter(ProviderType type, DbParameter parameter, object value)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                if (type2.IsRuntimeOnlyType)
                {
                    throw System.Data.Linq.SqlClient.Error.BadParameterType(type2.GetClosestRuntimeType());
                }
                System.Data.SqlClient.SqlParameter parameter2 = parameter as System.Data.SqlClient.SqlParameter;
                if (parameter2 != null)
                {
                    parameter2.SqlDbType = type2.SqlDbType;
                    if (type2.HasPrecisionAndScale)
                    {
                        parameter2.Precision = (byte) type2.Precision;
                        parameter2.Scale = (byte) type2.Scale;
                    }
                }
                else
                {
                    PropertyInfo property = parameter.GetType().GetProperty("SqlDbType");
                    if (property != null)
                    {
                        property.SetValue(parameter, type2.SqlDbType, null);
                    }
                    if (type2.HasPrecisionAndScale)
                    {
                        PropertyInfo info2 = parameter.GetType().GetProperty("Precision");
                        if (info2 != null)
                        {
                            info2.SetValue(parameter, Convert.ChangeType(type2.Precision, info2.PropertyType, CultureInfo.InvariantCulture), null);
                        }
                        PropertyInfo info3 = parameter.GetType().GetProperty("Scale");
                        if (info3 != null)
                        {
                            info3.SetValue(parameter, Convert.ChangeType(type2.Scale, info3.PropertyType, CultureInfo.InvariantCulture), null);
                        }
                    }
                }
                parameter.Value = this.GetParameterValue(type2, value);
                if (!((parameter.Direction == ParameterDirection.Input) && type2.IsFixedSize) && (parameter.Direction == ParameterDirection.Input))
                {
                    return;
                }
                if (type2.Size.HasValue)
                {
                    if (parameter.Size < type2.Size)
                    {
                        goto Label_016B;
                    }
                }
                if (!type2.IsLargeType)
                {
                    return;
                }
            Label_016B:
                parameter.Size = type2.Size.Value;
            }

            internal override ProviderType MostPreciseTypeInFamily(ProviderType type)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                switch (type2.SqlDbType)
                {
                    case SqlDbType.DateTime:
                    case SqlDbType.SmallDateTime:
                    case SqlDbType.Date:
                    case SqlDbType.Time:
                    case SqlDbType.DateTime2:
                        return this.From(typeof(DateTime));

                    case SqlDbType.Decimal:
                    case SqlDbType.Image:
                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                    case SqlDbType.UniqueIdentifier:
                    case SqlDbType.Text:
                    case SqlDbType.Timestamp:
                        return type;

                    case SqlDbType.Float:
                    case SqlDbType.Real:
                        return this.From(typeof(double));

                    case SqlDbType.Int:
                    case SqlDbType.SmallInt:
                    case SqlDbType.TinyInt:
                        return this.From(typeof(int));

                    case SqlDbType.Money:
                    case SqlDbType.SmallMoney:
                        return SqlTypeSystem.Create(SqlDbType.Money);

                    case SqlDbType.DateTimeOffset:
                        return this.From(typeof(DateTimeOffset));
                }
                return type;
            }

            internal override ProviderType Parse(string stype)
            {
                string strA = null;
                string s = null;
                string str3 = null;
                int index = stype.IndexOf('(');
                int num2 = stype.IndexOf(' ');
                int length = ((index != -1) && (num2 != -1)) ? Math.Min(num2, index) : ((index != -1) ? index : ((num2 != -1) ? num2 : -1));
                if (length == -1)
                {
                    strA = stype;
                    length = stype.Length;
                }
                else
                {
                    strA = stype.Substring(0, length);
                }
                int startIndex = length;
                if ((startIndex < stype.Length) && (stype[startIndex] == '('))
                {
                    startIndex++;
                    length = stype.IndexOf(',', startIndex);
                    if (length > 0)
                    {
                        s = stype.Substring(startIndex, length - startIndex);
                        startIndex = length + 1;
                        length = stype.IndexOf(')', startIndex);
                        str3 = stype.Substring(startIndex, length - startIndex);
                    }
                    else
                    {
                        length = stype.IndexOf(')', startIndex);
                        s = stype.Substring(startIndex, length - startIndex);
                    }
                    startIndex = length++;
                }
                if (string.Compare(strA, "rowversion", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    strA = "Timestamp";
                }
                if (string.Compare(strA, "numeric", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    strA = "Decimal";
                }
                if (string.Compare(strA, "sql_variant", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    strA = "Variant";
                }
                if (string.Compare(strA, "filestream", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    strA = "Binary";
                }
                if (!(from n in Enum.GetNames(typeof(SqlDbType)) select n.ToUpperInvariant()).Contains<string>(strA.ToUpperInvariant()))
                {
                    throw System.Data.Linq.SqlClient.Error.InvalidProviderType(strA);
                }
                int size = 0;
                int scale = 0;
                SqlDbType type = (SqlDbType) Enum.Parse(typeof(SqlDbType), strA, true);
                if (s != null)
                {
                    if (string.Compare(s.Trim(), "max", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        size = -1;
                    }
                    else
                    {
                        size = int.Parse(s, CultureInfo.InvariantCulture);
                        if (size == 0x7fffffff)
                        {
                            size = -1;
                        }
                    }
                }
                if (str3 != null)
                {
                    if (string.Compare(str3.Trim(), "max", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        scale = -1;
                    }
                    else
                    {
                        scale = int.Parse(str3, CultureInfo.InvariantCulture);
                        if (scale == 0x7fffffff)
                        {
                            scale = -1;
                        }
                    }
                }
                switch (type)
                {
                    case SqlDbType.Binary:
                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                    case SqlDbType.VarBinary:
                    case SqlDbType.VarChar:
                        return SqlTypeSystem.Create(type, size);

                    case SqlDbType.Decimal:
                    case SqlDbType.Float:
                    case SqlDbType.Real:
                        return SqlTypeSystem.Create(type, size, scale);
                }
                return SqlTypeSystem.Create(type);
            }

            internal override ProviderType PredictTypeForBinary(SqlNodeType binaryOp, ProviderType leftType, ProviderType rightType)
            {
                SqlTypeSystem.SqlType bestType;
                if (leftType.IsSameTypeFamily(this.From(typeof(string))) && rightType.IsSameTypeFamily(this.From(typeof(string))))
                {
                    bestType = (SqlTypeSystem.SqlType) this.GetBestType(leftType, rightType);
                }
                else
                {
                    bestType = (leftType.ComparePrecedenceTo(rightType) > 0) ? ((SqlTypeSystem.SqlType) leftType) : ((SqlTypeSystem.SqlType) rightType);
                }
                switch (binaryOp)
                {
                    case SqlNodeType.BitAnd:
                    case SqlNodeType.BitOr:
                    case SqlNodeType.BitXor:
                        return bestType;

                    case SqlNodeType.And:
                    case SqlNodeType.EQ:
                    case SqlNodeType.EQ2V:
                    case SqlNodeType.LE:
                    case SqlNodeType.LT:
                    case SqlNodeType.GE:
                    case SqlNodeType.GT:
                    case SqlNodeType.NE:
                    case SqlNodeType.NE2V:
                    case SqlNodeType.Or:
                        return SqlTypeSystem.theInt;

                    case SqlNodeType.Add:
                        return bestType;

                    case SqlNodeType.Coalesce:
                        return bestType;

                    case SqlNodeType.Concat:
                    {
                        if (!bestType.HasSizeOrIsLarge)
                        {
                            return bestType;
                        }
                        ProviderType type2 = this.GetBestType(bestType.SqlDbType, null);
                        if ((leftType.IsLargeType || !leftType.Size.HasValue) || (rightType.IsLargeType || !rightType.Size.HasValue))
                        {
                            return type2;
                        }
                        int num2 = leftType.Size.Value + rightType.Size.Value;
                        int num3 = num2;
                        if ((num3 >= type2.Size) && !type2.IsLargeType)
                        {
                            return type2;
                        }
                        return this.GetBestType(bestType.SqlDbType, new int?(num2));
                    }
                    case SqlNodeType.Div:
                        return bestType;

                    case SqlNodeType.Mod:
                    case SqlNodeType.Mul:
                        return bestType;

                    case SqlNodeType.Sub:
                        return bestType;
                }
                throw System.Data.Linq.SqlClient.Error.UnexpectedNode(binaryOp);
            }

            internal override ProviderType PredictTypeForUnary(SqlNodeType unaryOp, ProviderType operandType)
            {
                switch (unaryOp)
                {
                    case SqlNodeType.Avg:
                    case SqlNodeType.Covar:
                    case SqlNodeType.Stddev:
                    case SqlNodeType.Sum:
                        return this.MostPreciseTypeInFamily(operandType);

                    case SqlNodeType.BitNot:
                        return operandType;

                    case SqlNodeType.ClrLength:
                        if (operandType.IsLargeType)
                        {
                            return this.From(typeof(long));
                        }
                        return this.From(typeof(int));

                    case SqlNodeType.LongCount:
                        return this.From(typeof(long));

                    case SqlNodeType.Max:
                        return operandType;

                    case SqlNodeType.Count:
                        return this.From(typeof(int));

                    case SqlNodeType.IsNotNull:
                    case SqlNodeType.IsNull:
                    case SqlNodeType.Not:
                    case SqlNodeType.Not2V:
                        return SqlTypeSystem.theBit;

                    case SqlNodeType.Negate:
                        return operandType;

                    case SqlNodeType.OuterJoinedValue:
                        return operandType;

                    case SqlNodeType.Min:
                        return operandType;

                    case SqlNodeType.Treat:
                    case SqlNodeType.ValueOf:
                        return operandType;
                }
                throw System.Data.Linq.SqlClient.Error.UnexpectedNode(unaryOp);
            }

            internal override ProviderType ReturnTypeOfFunction(SqlFunctionCall functionCall)
            {
                ProviderType[] argumentTypes = this.GetArgumentTypes(functionCall);
                SqlTypeSystem.SqlType type = (SqlTypeSystem.SqlType) argumentTypes[0];
                SqlTypeSystem.SqlType type2 = (argumentTypes.Length > 1) ? ((SqlTypeSystem.SqlType) argumentTypes[1]) : null;
                switch (functionCall.Name)
                {
                    case "LEN":
                    case "DATALENGTH":
                        switch (type.SqlDbType)
                        {
                            case SqlDbType.VarBinary:
                            case SqlDbType.VarChar:
                            case SqlDbType.NVarChar:
                                if (type.IsLargeType)
                                {
                                    return SqlTypeSystem.Create(SqlDbType.BigInt);
                                }
                                return SqlTypeSystem.Create(SqlDbType.Int);
                        }
                        return SqlTypeSystem.Create(SqlDbType.Int);

                    case "ABS":
                    case "SIGN":
                    case "ROUND":
                    case "CEILING":
                    case "FLOOR":
                    case "POWER":
                    {
                        SqlDbType sqlDbType = type.SqlDbType;
                        if (sqlDbType > SqlDbType.Real)
                        {
                            if ((sqlDbType != SqlDbType.SmallInt) && (sqlDbType != SqlDbType.TinyInt))
                            {
                                return type;
                            }
                            break;
                        }
                        switch (sqlDbType)
                        {
                            case SqlDbType.Float:
                            case SqlDbType.Real:
                                return SqlTypeSystem.Create(SqlDbType.Float);

                            case SqlDbType.Image:
                                return type;
                        }
                        return type;
                    }
                    case "PATINDEX":
                    case "CHARINDEX":
                        if (!type2.IsLargeType)
                        {
                            return SqlTypeSystem.Create(SqlDbType.Int);
                        }
                        return SqlTypeSystem.Create(SqlDbType.BigInt);

                    case "SUBSTRING":
                    {
                        if (functionCall.Arguments[2].NodeType != SqlNodeType.Value)
                        {
                            goto Label_02DA;
                        }
                        SqlValue value2 = (SqlValue) functionCall.Arguments[2];
                        if (!(value2.Value is int))
                        {
                            goto Label_02DA;
                        }
                        switch (type.SqlDbType)
                        {
                            case SqlDbType.NChar:
                            case SqlDbType.NVarChar:
                            case SqlDbType.VarChar:
                            case SqlDbType.Char:
                                return SqlTypeSystem.Create(type.SqlDbType, (int) value2.Value);
                        }
                        return null;
                    }
                    case "STUFF":
                    {
                        if (functionCall.Arguments.Count != 4)
                        {
                            goto Label_0375;
                        }
                        SqlValue value3 = functionCall.Arguments[2] as SqlValue;
                        if ((value3 == null) || (((int) value3.Value) != 0))
                        {
                            goto Label_0375;
                        }
                        return this.PredictTypeForBinary(SqlNodeType.Concat, functionCall.Arguments[0].SqlType, functionCall.Arguments[3].SqlType);
                    }
                    case "LOWER":
                    case "UPPER":
                    case "RTRIM":
                    case "LTRIM":
                    case "INSERT":
                    case "REPLACE":
                    case "LEFT":
                    case "RIGHT":
                    case "REVERSE":
                        return type;

                    default:
                        return null;
                }
                return SqlTypeSystem.Create(SqlDbType.Int);
            Label_02DA:
                switch (type.SqlDbType)
                {
                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                        return SqlTypeSystem.Create(SqlDbType.NVarChar);

                    case SqlDbType.VarChar:
                    case SqlDbType.Char:
                        return SqlTypeSystem.Create(SqlDbType.VarChar);

                    default:
                        return null;
                }
            Label_0375:
                return null;
            }

            protected abstract bool SupportsMaxSize { get; }
        }

        private class Sql2000Provider : SqlTypeSystem.ProviderBase
        {
            internal override ProviderType From(Type type, int? size)
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    type = type.GetGenericArguments()[0];
                }
                TypeCode typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Object:
                        if (type != typeof(Guid))
                        {
                            if ((type == typeof(byte[])) || (type == typeof(Binary)))
                            {
                                return base.GetBestType(SqlDbType.VarBinary, size);
                            }
                            if (type == typeof(char[]))
                            {
                                return base.GetBestType(SqlDbType.NVarChar, size);
                            }
                            if (type == typeof(TimeSpan))
                            {
                                return SqlTypeSystem.Create(SqlDbType.BigInt);
                            }
                            if ((type != typeof(XDocument)) && (type != typeof(XElement)))
                            {
                                return new SqlTypeSystem.SqlType(type);
                            }
                            return SqlTypeSystem.theNText;
                        }
                        return SqlTypeSystem.Create(SqlDbType.UniqueIdentifier);

                    case TypeCode.Boolean:
                        return SqlTypeSystem.Create(SqlDbType.Bit);

                    case TypeCode.Char:
                        return SqlTypeSystem.Create(SqlDbType.NChar, 1);

                    case TypeCode.SByte:
                    case TypeCode.Int16:
                        return SqlTypeSystem.Create(SqlDbType.SmallInt);

                    case TypeCode.Byte:
                        return SqlTypeSystem.Create(SqlDbType.TinyInt);

                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                        return SqlTypeSystem.Create(SqlDbType.Int);

                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                        return SqlTypeSystem.Create(SqlDbType.BigInt);

                    case TypeCode.UInt64:
                        return SqlTypeSystem.Create(SqlDbType.Decimal, 20, 0);

                    case TypeCode.Single:
                        return SqlTypeSystem.Create(SqlDbType.Real);

                    case TypeCode.Double:
                        return SqlTypeSystem.Create(SqlDbType.Float);

                    case TypeCode.Decimal:
                    {
                        int? nullable = size;
                        return SqlTypeSystem.Create(SqlDbType.Decimal, 0x1d, nullable.HasValue ? nullable.GetValueOrDefault() : 4);
                    }
                    case TypeCode.DateTime:
                        return SqlTypeSystem.Create(SqlDbType.DateTime);

                    case TypeCode.String:
                        return base.GetBestType(SqlDbType.NVarChar, size);
                }
                throw System.Data.Linq.SqlClient.Error.UnexpectedTypeCode(typeCode);
            }

            internal override ProviderType GetBestLargeType(ProviderType type)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                switch (type2.SqlDbType)
                {
                    case SqlDbType.Binary:
                    case SqlDbType.VarBinary:
                        return SqlTypeSystem.Create(SqlDbType.Image);

                    case SqlDbType.Bit:
                        return type;

                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                        return SqlTypeSystem.Create(SqlDbType.Text);

                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                        return SqlTypeSystem.Create(SqlDbType.NText);

                    case SqlDbType.NText:
                        return type;
                }
                return type;
            }

            protected override bool SupportsMaxSize =>
                false;
        }

        private class Sql2005Provider : SqlTypeSystem.ProviderBase
        {
            internal override ProviderType From(Type type, int? size)
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    type = type.GetGenericArguments()[0];
                }
                TypeCode typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Object:
                        if (type != typeof(Guid))
                        {
                            if ((type == typeof(byte[])) || (type == typeof(Binary)))
                            {
                                return base.GetBestType(SqlDbType.VarBinary, size);
                            }
                            if (type == typeof(char[]))
                            {
                                return base.GetBestType(SqlDbType.NVarChar, size);
                            }
                            if (type == typeof(TimeSpan))
                            {
                                return SqlTypeSystem.Create(SqlDbType.BigInt);
                            }
                            if ((type != typeof(XDocument)) && (type != typeof(XElement)))
                            {
                                return new SqlTypeSystem.SqlType(type);
                            }
                            return SqlTypeSystem.theXml;
                        }
                        return SqlTypeSystem.Create(SqlDbType.UniqueIdentifier);

                    case TypeCode.Boolean:
                        return SqlTypeSystem.Create(SqlDbType.Bit);

                    case TypeCode.Char:
                        return SqlTypeSystem.Create(SqlDbType.NChar, 1);

                    case TypeCode.SByte:
                    case TypeCode.Int16:
                        return SqlTypeSystem.Create(SqlDbType.SmallInt);

                    case TypeCode.Byte:
                        return SqlTypeSystem.Create(SqlDbType.TinyInt);

                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                        return SqlTypeSystem.Create(SqlDbType.Int);

                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                        return SqlTypeSystem.Create(SqlDbType.BigInt);

                    case TypeCode.UInt64:
                        return SqlTypeSystem.Create(SqlDbType.Decimal, 20, 0);

                    case TypeCode.Single:
                        return SqlTypeSystem.Create(SqlDbType.Real);

                    case TypeCode.Double:
                        return SqlTypeSystem.Create(SqlDbType.Float);

                    case TypeCode.Decimal:
                    {
                        int? nullable = size;
                        return SqlTypeSystem.Create(SqlDbType.Decimal, 0x1d, nullable.HasValue ? nullable.GetValueOrDefault() : 4);
                    }
                    case TypeCode.DateTime:
                        return SqlTypeSystem.Create(SqlDbType.DateTime);

                    case TypeCode.String:
                        return base.GetBestType(SqlDbType.NVarChar, size);
                }
                throw System.Data.Linq.SqlClient.Error.UnexpectedTypeCode(typeCode);
            }

            internal override ProviderType GetBestLargeType(ProviderType type)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                switch (type2.SqlDbType)
                {
                    case SqlDbType.Binary:
                    case SqlDbType.Image:
                    case SqlDbType.VarBinary:
                        return SqlTypeSystem.Create(SqlDbType.VarBinary, -1);

                    case SqlDbType.Bit:
                        return type;

                    case SqlDbType.Char:
                    case SqlDbType.Text:
                    case SqlDbType.VarChar:
                        return SqlTypeSystem.Create(SqlDbType.VarChar, -1);

                    case SqlDbType.Int:
                    case SqlDbType.Money:
                        return type;

                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                        return SqlTypeSystem.Create(SqlDbType.NVarChar, -1);

                    case SqlDbType.Timestamp:
                    case SqlDbType.TinyInt:
                        return type;
                }
                return type;
            }

            protected override bool SupportsMaxSize =>
                true;
        }

        private class Sql2008Provider : SqlTypeSystem.Sql2005Provider
        {
            internal override ProviderType From(Type type, int? size)
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    type = type.GetGenericArguments()[0];
                }
                if ((Type.GetTypeCode(type) == TypeCode.Object) && (type == typeof(DateTimeOffset)))
                {
                    return SqlTypeSystem.Create(SqlDbType.DateTimeOffset);
                }
                return base.From(type, size);
            }
        }

        private class SqlCEProvider : SqlTypeSystem.Sql2000Provider
        {
            internal override void InitializeParameter(ProviderType type, DbParameter parameter, object value)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                parameter.GetType().GetProperty("SqlDbType").SetValue(parameter, type2.SqlDbType, null);
                if (type2.HasPrecisionAndScale)
                {
                    PropertyInfo property = parameter.GetType().GetProperty("Precision");
                    if (property != null)
                    {
                        property.SetValue(parameter, Convert.ChangeType(type2.Precision, property.PropertyType, CultureInfo.InvariantCulture), null);
                    }
                    PropertyInfo info3 = parameter.GetType().GetProperty("Scale");
                    if (info3 != null)
                    {
                        info3.SetValue(parameter, Convert.ChangeType(type2.Scale, info3.PropertyType, CultureInfo.InvariantCulture), null);
                    }
                }
                parameter.Value = base.GetParameterValue(type2, value);
                if (((parameter.Direction == ParameterDirection.Input) && type2.IsFixedSize) || (parameter.Direction != ParameterDirection.Input))
                {
                    int size = parameter.Size;
                    if ((size < type2.Size) || type2.IsLargeType)
                    {
                        int? nullable2 = type2.Size;
                        parameter.Size = nullable2.HasValue ? nullable2.GetValueOrDefault() : 0;
                    }
                }
            }
        }

        public class SqlType : ProviderType
        {
            private int? applicationTypeIndex;
            protected int precision;
            private Type runtimeOnlyType;
            protected int scale;
            protected int? size;
            protected System.Data.SqlDbType sqlDbType;

            internal SqlType(System.Data.SqlDbType type)
            {
                this.applicationTypeIndex = null;
                this.sqlDbType = type;
            }

            internal SqlType(int applicationTypeIndex)
            {
                this.applicationTypeIndex = null;
                this.applicationTypeIndex = new int?(applicationTypeIndex);
            }

            internal SqlType(Type type)
            {
                this.applicationTypeIndex = null;
                this.runtimeOnlyType = type;
            }

            internal SqlType(System.Data.SqlDbType type, int? size)
            {
                this.applicationTypeIndex = null;
                this.sqlDbType = type;
                this.size = size;
            }

            internal SqlType(System.Data.SqlDbType type, int precision, int scale)
            {
                this.applicationTypeIndex = null;
                this.sqlDbType = type;
                this.precision = precision;
                this.scale = scale;
            }

            internal override bool AreValuesEqual(object o1, object o2)
            {
                string str;
                if ((o1 == null) || (o2 == null))
                {
                    return false;
                }
                System.Data.SqlDbType sqlDbType = this.sqlDbType;
                if (sqlDbType <= System.Data.SqlDbType.NVarChar)
                {
                    switch (sqlDbType)
                    {
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NVarChar:
                        case System.Data.SqlDbType.Char:
                            goto Label_0039;
                    }
                    goto Label_007D;
                }
                if ((sqlDbType != System.Data.SqlDbType.Text) && (sqlDbType != System.Data.SqlDbType.VarChar))
                {
                    goto Label_007D;
                }
            Label_0039:
                str = o1 as string;
                if (str != null)
                {
                    string str2 = o2 as string;
                    if (str2 != null)
                    {
                        return str.TrimEnd(new char[] { ' ' }).Equals(str2.TrimEnd(new char[] { ' ' }), StringComparison.Ordinal);
                    }
                }
            Label_007D:
                return o1.Equals(o2);
            }

            internal override int ComparePrecedenceTo(ProviderType type)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                int num = this.IsTypeKnownByProvider ? GetTypeCoercionPrecedence(this.SqlDbType) : -2147483648;
                int num2 = type2.IsTypeKnownByProvider ? GetTypeCoercionPrecedence(type2.SqlDbType) : -2147483648;
                return num.CompareTo(num2);
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                SqlTypeSystem.SqlType type = obj as SqlTypeSystem.SqlType;
                return (((type != null) && (((this.runtimeOnlyType == type.runtimeOnlyType) && ((this.applicationTypeIndex == type.applicationTypeIndex) && (this.sqlDbType == type.sqlDbType))) && ((this.Size == type.Size) && (this.precision == type.precision)))) && (this.scale == type.scale));
            }

            internal override Type GetClosestRuntimeType()
            {
                if (this.runtimeOnlyType != null)
                {
                    return this.runtimeOnlyType;
                }
                return SqlTypeSystem.GetClosestRuntimeType(this.sqlDbType);
            }

            public override int GetHashCode()
            {
                int hashCode = 0;
                if (this.runtimeOnlyType != null)
                {
                    hashCode = this.runtimeOnlyType.GetHashCode();
                }
                else if (this.applicationTypeIndex.HasValue)
                {
                    hashCode = this.applicationTypeIndex.Value;
                }
                int? size = this.size;
                return ((((hashCode ^ this.sqlDbType.GetHashCode()) ^ (size.HasValue ? size.GetValueOrDefault() : 0)) ^ this.precision) ^ (this.scale << 8));
            }

            internal override ProviderType GetNonUnicodeEquivalent()
            {
                if (this.IsUnicodeType)
                {
                    switch (this.SqlDbType)
                    {
                        case System.Data.SqlDbType.NChar:
                            return new SqlTypeSystem.SqlType(System.Data.SqlDbType.Char, this.Size);

                        case System.Data.SqlDbType.NText:
                            return new SqlTypeSystem.SqlType(System.Data.SqlDbType.Text);

                        case System.Data.SqlDbType.NVarChar:
                            return new SqlTypeSystem.SqlType(System.Data.SqlDbType.VarChar, this.Size);
                    }
                }
                return this;
            }

            private static int GetTypeCoercionPrecedence(System.Data.SqlDbType type)
            {
                switch (type)
                {
                    case System.Data.SqlDbType.BigInt:
                        return 15;

                    case System.Data.SqlDbType.Binary:
                        return 0;

                    case System.Data.SqlDbType.Bit:
                        return 11;

                    case System.Data.SqlDbType.Char:
                        return 3;

                    case System.Data.SqlDbType.DateTime:
                        return 0x18;

                    case System.Data.SqlDbType.Decimal:
                        return 0x12;

                    case System.Data.SqlDbType.Float:
                        return 20;

                    case System.Data.SqlDbType.Image:
                        return 8;

                    case System.Data.SqlDbType.Int:
                        return 14;

                    case System.Data.SqlDbType.Money:
                        return 0x11;

                    case System.Data.SqlDbType.NChar:
                        return 4;

                    case System.Data.SqlDbType.NText:
                        return 10;

                    case System.Data.SqlDbType.NVarChar:
                        return 5;

                    case System.Data.SqlDbType.Real:
                        return 0x13;

                    case System.Data.SqlDbType.UniqueIdentifier:
                        return 6;

                    case System.Data.SqlDbType.SmallDateTime:
                        return 0x17;

                    case System.Data.SqlDbType.SmallInt:
                        return 13;

                    case System.Data.SqlDbType.SmallMoney:
                        return 0x10;

                    case System.Data.SqlDbType.Text:
                        return 9;

                    case System.Data.SqlDbType.Timestamp:
                        return 7;

                    case System.Data.SqlDbType.TinyInt:
                        return 12;

                    case System.Data.SqlDbType.VarBinary:
                        return 1;

                    case System.Data.SqlDbType.VarChar:
                        return 2;

                    case System.Data.SqlDbType.Variant:
                        return 0x1c;

                    case System.Data.SqlDbType.Xml:
                        return 0x1b;

                    case System.Data.SqlDbType.Udt:
                        return 0x1d;

                    case System.Data.SqlDbType.Date:
                        return 0x15;

                    case System.Data.SqlDbType.Time:
                        return 0x16;

                    case System.Data.SqlDbType.DateTime2:
                        return 0x19;

                    case System.Data.SqlDbType.DateTimeOffset:
                        return 0x1a;
                }
                throw System.Data.Linq.SqlClient.Error.UnexpectedTypeCode(type);
            }

            internal override bool IsApplicationTypeOf(int index)
            {
                if (!this.IsApplicationType)
                {
                    return false;
                }
                int? applicationTypeIndex = this.applicationTypeIndex;
                int num = index;
                return ((applicationTypeIndex.GetValueOrDefault() == num) && applicationTypeIndex.HasValue);
            }

            internal override bool IsSameTypeFamily(ProviderType type)
            {
                SqlTypeSystem.SqlType type2 = (SqlTypeSystem.SqlType) type;
                if (this.IsApplicationType)
                {
                    return false;
                }
                if (type2.IsApplicationType)
                {
                    return false;
                }
                return (this.Category == type2.Category);
            }

            protected static string KeyValue<T>(string key, T value)
            {
                if (value != null)
                {
                    return (key + "=" + value.ToString() + " ");
                }
                return string.Empty;
            }

            protected static string SingleValue<T>(T value)
            {
                if (value != null)
                {
                    return (value.ToString() + " ");
                }
                return string.Empty;
            }

            internal override string ToQueryString() => 
                this.ToQueryString(QueryFormatOptions.None);

            internal override string ToQueryString(QueryFormatOptions formatFlags)
            {
                if (this.runtimeOnlyType != null)
                {
                    return this.runtimeOnlyType.ToString();
                }
                StringBuilder builder = new StringBuilder();
                switch (this.sqlDbType)
                {
                    case System.Data.SqlDbType.BigInt:
                    case System.Data.SqlDbType.Bit:
                    case System.Data.SqlDbType.DateTime:
                    case System.Data.SqlDbType.Image:
                    case System.Data.SqlDbType.Int:
                    case System.Data.SqlDbType.Money:
                    case System.Data.SqlDbType.NText:
                    case System.Data.SqlDbType.UniqueIdentifier:
                    case System.Data.SqlDbType.SmallDateTime:
                    case System.Data.SqlDbType.SmallInt:
                    case System.Data.SqlDbType.SmallMoney:
                    case System.Data.SqlDbType.Text:
                    case System.Data.SqlDbType.Timestamp:
                    case System.Data.SqlDbType.TinyInt:
                    case System.Data.SqlDbType.Xml:
                    case System.Data.SqlDbType.Udt:
                    case System.Data.SqlDbType.Date:
                    case System.Data.SqlDbType.Time:
                    case System.Data.SqlDbType.DateTime2:
                    case System.Data.SqlDbType.DateTimeOffset:
                        builder.Append(this.sqlDbType.ToString());
                        goto Label_0231;

                    case System.Data.SqlDbType.Binary:
                    case System.Data.SqlDbType.Char:
                    case System.Data.SqlDbType.NChar:
                        builder.Append(this.sqlDbType);
                        if ((formatFlags & QueryFormatOptions.SuppressSize) == QueryFormatOptions.None)
                        {
                            builder.Append("(");
                            builder.Append(this.size);
                            builder.Append(")");
                        }
                        goto Label_0231;

                    case System.Data.SqlDbType.Decimal:
                    case System.Data.SqlDbType.Float:
                    case System.Data.SqlDbType.Real:
                        builder.Append(this.sqlDbType);
                        if (this.precision != 0)
                        {
                            builder.Append("(");
                            builder.Append(this.precision);
                            if (this.scale != 0)
                            {
                                builder.Append(",");
                                builder.Append(this.scale);
                            }
                            builder.Append(")");
                        }
                        goto Label_0231;

                    case System.Data.SqlDbType.NVarChar:
                    case System.Data.SqlDbType.VarBinary:
                    case System.Data.SqlDbType.VarChar:
                        builder.Append(this.sqlDbType);
                        if (!this.size.HasValue || ((this.size == 0) || ((formatFlags & QueryFormatOptions.SuppressSize) != QueryFormatOptions.None)))
                        {
                            goto Label_0231;
                        }
                        builder.Append("(");
                        if (this.size != -1)
                        {
                            builder.Append(this.size);
                            break;
                        }
                        builder.Append("MAX");
                        break;

                    case System.Data.SqlDbType.Variant:
                        builder.Append("sql_variant");
                        goto Label_0231;

                    default:
                        goto Label_0231;
                }
                builder.Append(")");
            Label_0231:
                return builder.ToString();
            }

            public override string ToString() => 
                (SingleValue<Type>(this.GetClosestRuntimeType()) + SingleValue<string>(this.ToQueryString()) + KeyValue<bool>("IsApplicationType", this.IsApplicationType) + KeyValue<bool>("IsUnicodeType", this.IsUnicodeType) + KeyValue<bool>("IsRuntimeOnlyType", this.IsRuntimeOnlyType) + KeyValue<bool>("SupportsComparison", this.SupportsComparison) + KeyValue<bool>("SupportsLength", this.SupportsLength) + KeyValue<bool>("IsLargeType", this.IsLargeType) + KeyValue<bool>("IsFixedSize", this.IsFixedSize) + KeyValue<bool>("IsOrderable", this.IsOrderable) + KeyValue<bool>("IsGroupable", this.IsGroupable) + KeyValue<bool>("IsNumeric", this.IsNumeric) + KeyValue<bool>("IsChar", this.IsChar) + KeyValue<bool>("IsString", this.IsString));

            internal override bool CanBeColumn =>
                (!this.IsApplicationType && !this.IsRuntimeOnlyType);

            internal override bool CanBeParameter =>
                (!this.IsApplicationType && !this.IsRuntimeOnlyType);

            internal override bool CanSuppressSizeForConversionToString
            {
                get
                {
                    int num = 30;
                    if (!this.IsLargeType)
                    {
                        if (((!this.IsChar && !this.IsString) && this.IsFixedSize) && (this.Size > 0))
                        {
                            int? size = this.Size;
                            int num2 = num;
                            return ((size.GetValueOrDefault() < num2) && size.HasValue);
                        }
                        switch (this.SqlDbType)
                        {
                            case System.Data.SqlDbType.Real:
                            case System.Data.SqlDbType.SmallInt:
                            case System.Data.SqlDbType.SmallMoney:
                            case System.Data.SqlDbType.TinyInt:
                            case System.Data.SqlDbType.BigInt:
                            case System.Data.SqlDbType.Bit:
                            case System.Data.SqlDbType.Float:
                            case System.Data.SqlDbType.Int:
                            case System.Data.SqlDbType.Money:
                                return true;
                        }
                    }
                    return false;
                }
            }

            internal TypeCategory Category
            {
                get
                {
                    switch (this.SqlDbType)
                    {
                        case System.Data.SqlDbType.BigInt:
                        case System.Data.SqlDbType.Bit:
                        case System.Data.SqlDbType.Decimal:
                        case System.Data.SqlDbType.Float:
                        case System.Data.SqlDbType.Int:
                        case System.Data.SqlDbType.Money:
                        case System.Data.SqlDbType.Real:
                        case System.Data.SqlDbType.SmallInt:
                        case System.Data.SqlDbType.SmallMoney:
                        case System.Data.SqlDbType.TinyInt:
                            return TypeCategory.Numeric;

                        case System.Data.SqlDbType.Binary:
                        case System.Data.SqlDbType.Timestamp:
                        case System.Data.SqlDbType.VarBinary:
                            return TypeCategory.Binary;

                        case System.Data.SqlDbType.Char:
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NVarChar:
                        case System.Data.SqlDbType.VarChar:
                            return TypeCategory.Char;

                        case System.Data.SqlDbType.DateTime:
                        case System.Data.SqlDbType.SmallDateTime:
                        case System.Data.SqlDbType.Date:
                        case System.Data.SqlDbType.Time:
                        case System.Data.SqlDbType.DateTime2:
                        case System.Data.SqlDbType.DateTimeOffset:
                            return TypeCategory.DateTime;

                        case System.Data.SqlDbType.Image:
                            return TypeCategory.Image;

                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.Text:
                            return TypeCategory.Text;

                        case System.Data.SqlDbType.UniqueIdentifier:
                            return TypeCategory.UniqueIdentifier;

                        case System.Data.SqlDbType.Variant:
                            return TypeCategory.Variant;

                        case System.Data.SqlDbType.Xml:
                            return TypeCategory.Xml;

                        case System.Data.SqlDbType.Udt:
                            return TypeCategory.Udt;
                    }
                    throw System.Data.Linq.SqlClient.Error.UnexpectedTypeCode(this);
                }
            }

            internal override bool HasPrecisionAndScale
            {
                get
                {
                    switch (this.SqlDbType)
                    {
                        case System.Data.SqlDbType.Time:
                        case System.Data.SqlDbType.DateTime2:
                        case System.Data.SqlDbType.DateTimeOffset:
                        case System.Data.SqlDbType.SmallMoney:
                        case System.Data.SqlDbType.Decimal:
                        case System.Data.SqlDbType.Float:
                        case System.Data.SqlDbType.Money:
                        case System.Data.SqlDbType.Real:
                            return true;
                    }
                    return false;
                }
            }

            internal override bool HasSizeOrIsLarge
            {
                get
                {
                    if (!this.size.HasValue)
                    {
                        return this.IsLargeType;
                    }
                    return true;
                }
            }

            internal override bool IsApplicationType =>
                this.applicationTypeIndex.HasValue;

            internal override bool IsChar
            {
                get
                {
                    if (!this.IsApplicationType && !this.IsRuntimeOnlyType)
                    {
                        switch (this.SqlDbType)
                        {
                            case System.Data.SqlDbType.NChar:
                            case System.Data.SqlDbType.NVarChar:
                            case System.Data.SqlDbType.VarChar:
                            case System.Data.SqlDbType.Char:
                                return (this.Size == 1);
                        }
                    }
                    return false;
                }
            }

            internal override bool IsFixedSize
            {
                get
                {
                    switch (this.sqlDbType)
                    {
                        case System.Data.SqlDbType.Text:
                        case System.Data.SqlDbType.VarBinary:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.Xml:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.NVarChar:
                        case System.Data.SqlDbType.Image:
                            return false;
                    }
                    return true;
                }
            }

            internal override bool IsGroupable
            {
                get
                {
                    if (this.IsRuntimeOnlyType)
                    {
                        return false;
                    }
                    System.Data.SqlDbType sqlDbType = this.sqlDbType;
                    if (sqlDbType <= System.Data.SqlDbType.NText)
                    {
                        switch (sqlDbType)
                        {
                            case System.Data.SqlDbType.Image:
                            case System.Data.SqlDbType.NText:
                                goto Label_002B;
                        }
                        goto Label_002D;
                    }
                    if ((sqlDbType != System.Data.SqlDbType.Text) && (sqlDbType != System.Data.SqlDbType.Xml))
                    {
                        goto Label_002D;
                    }
                Label_002B:
                    return false;
                Label_002D:
                    return true;
                }
            }

            internal override bool IsLargeType
            {
                get
                {
                    switch (this.sqlDbType)
                    {
                        case System.Data.SqlDbType.Text:
                        case System.Data.SqlDbType.Xml:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.Image:
                            return true;

                        case System.Data.SqlDbType.VarBinary:
                        case System.Data.SqlDbType.VarChar:
                        case System.Data.SqlDbType.NVarChar:
                            return (this.size == -1);
                    }
                    return false;
                }
            }

            internal override bool IsNumeric
            {
                get
                {
                    if (!this.IsApplicationType && !this.IsRuntimeOnlyType)
                    {
                        switch (this.SqlDbType)
                        {
                            case System.Data.SqlDbType.BigInt:
                            case System.Data.SqlDbType.Bit:
                            case System.Data.SqlDbType.Decimal:
                            case System.Data.SqlDbType.Float:
                            case System.Data.SqlDbType.Int:
                            case System.Data.SqlDbType.Money:
                            case System.Data.SqlDbType.Real:
                            case System.Data.SqlDbType.SmallInt:
                            case System.Data.SqlDbType.SmallMoney:
                            case System.Data.SqlDbType.TinyInt:
                                return true;
                        }
                    }
                    return false;
                }
            }

            internal override bool IsOrderable
            {
                get
                {
                    if (this.IsRuntimeOnlyType)
                    {
                        return false;
                    }
                    System.Data.SqlDbType sqlDbType = this.sqlDbType;
                    if (sqlDbType <= System.Data.SqlDbType.NText)
                    {
                        switch (sqlDbType)
                        {
                            case System.Data.SqlDbType.Image:
                            case System.Data.SqlDbType.NText:
                                goto Label_002B;
                        }
                        goto Label_002D;
                    }
                    if ((sqlDbType != System.Data.SqlDbType.Text) && (sqlDbType != System.Data.SqlDbType.Xml))
                    {
                        goto Label_002D;
                    }
                Label_002B:
                    return false;
                Label_002D:
                    return true;
                }
            }

            internal override bool IsRuntimeOnlyType =>
                (this.runtimeOnlyType != null);

            internal override bool IsString
            {
                get
                {
                    int? nullable;
                    if (this.IsApplicationType || this.IsRuntimeOnlyType)
                    {
                        return false;
                    }
                    System.Data.SqlDbType sqlDbType = this.SqlDbType;
                    if (sqlDbType <= System.Data.SqlDbType.NVarChar)
                    {
                        switch (sqlDbType)
                        {
                            case System.Data.SqlDbType.NChar:
                            case System.Data.SqlDbType.NVarChar:
                            case System.Data.SqlDbType.Char:
                                goto Label_0043;

                            case System.Data.SqlDbType.NText:
                                goto Label_0099;
                        }
                        goto Label_009B;
                    }
                    if (sqlDbType == System.Data.SqlDbType.Text)
                    {
                        goto Label_0099;
                    }
                    if (sqlDbType != System.Data.SqlDbType.VarChar)
                    {
                        goto Label_009B;
                    }
                Label_0043:
                    nullable = this.Size;
                    if (((nullable.GetValueOrDefault() != 0) || !nullable.HasValue) && (this.Size <= 1))
                    {
                        return (this.Size == -1);
                    }
                    return true;
                Label_0099:
                    return true;
                Label_009B:
                    return false;
                }
            }

            private bool IsTypeKnownByProvider =>
                (!this.IsApplicationType && !this.IsRuntimeOnlyType);

            internal override bool IsUnicodeType
            {
                get
                {
                    switch (this.SqlDbType)
                    {
                        case System.Data.SqlDbType.NChar:
                        case System.Data.SqlDbType.NText:
                        case System.Data.SqlDbType.NVarChar:
                            return true;
                    }
                    return false;
                }
            }

            internal int Precision =>
                this.precision;

            internal int Scale =>
                this.scale;

            internal override int? Size =>
                this.size;

            internal System.Data.SqlDbType SqlDbType =>
                this.sqlDbType;

            internal override bool SupportsComparison
            {
                get
                {
                    System.Data.SqlDbType sqlDbType = this.sqlDbType;
                    if (sqlDbType <= System.Data.SqlDbType.NText)
                    {
                        switch (sqlDbType)
                        {
                            case System.Data.SqlDbType.Image:
                            case System.Data.SqlDbType.NText:
                                goto Label_0021;
                        }
                        goto Label_0023;
                    }
                    if ((sqlDbType != System.Data.SqlDbType.Text) && (sqlDbType != System.Data.SqlDbType.Xml))
                    {
                        goto Label_0023;
                    }
                Label_0021:
                    return false;
                Label_0023:
                    return true;
                }
            }

            internal override bool SupportsLength
            {
                get
                {
                    System.Data.SqlDbType sqlDbType = this.sqlDbType;
                    if (sqlDbType <= System.Data.SqlDbType.NText)
                    {
                        switch (sqlDbType)
                        {
                            case System.Data.SqlDbType.Image:
                            case System.Data.SqlDbType.NText:
                                goto Label_0021;
                        }
                        goto Label_0023;
                    }
                    if ((sqlDbType != System.Data.SqlDbType.Text) && (sqlDbType != System.Data.SqlDbType.Xml))
                    {
                        goto Label_0023;
                    }
                Label_0021:
                    return false;
                Label_0023:
                    return true;
                }
            }

            internal enum TypeCategory
            {
                Numeric,
                Char,
                Text,
                Binary,
                Image,
                Xml,
                DateTime,
                UniqueIdentifier,
                Variant,
                Udt
            }
        }
    }
}

