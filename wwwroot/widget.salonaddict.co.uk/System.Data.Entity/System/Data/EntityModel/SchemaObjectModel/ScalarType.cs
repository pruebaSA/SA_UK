namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    internal sealed class ScalarType : System.Data.EntityModel.SchemaObjectModel.SchemaType
    {
        private static readonly Regex _BinaryValueValidator = new Regex("^0[xX][0-9a-fA-F]+$", RegexOptions.Compiled);
        private static readonly Regex _GuidValueValidator = new Regex("[0-9a-fA-F]{8,8}(-[0-9a-fA-F]{4,4}){3,3}-[0-9a-fA-F]{12,12}", RegexOptions.Compiled);
        private PrimitiveType _primitiveType;
        internal const string DateTimeFormat = @"yyyy-MM-dd HH\:mm\:ss.fffZ";
        internal const string DateTimeOffsetFormat = @"yyyy-MM-dd HH\:mm\:ss.fffffffz";
        internal const string TimeFormat = @"HH\:mm\:ss.fffffffZ";

        internal ScalarType(Schema parentElement, string typeName, PrimitiveType primitiveType) : base(parentElement)
        {
            this.Name = typeName;
            this._primitiveType = primitiveType;
        }

        public bool TryParse(string text, out object value)
        {
            switch (this._primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    return TryParseBinary(text, out value);

                case PrimitiveTypeKind.Boolean:
                    return TryParseBoolean(text, out value);

                case PrimitiveTypeKind.Byte:
                    return TryParseByte(text, out value);

                case PrimitiveTypeKind.DateTime:
                    return TryParseDateTime(text, out value);

                case PrimitiveTypeKind.Decimal:
                    return TryParseDecimal(text, out value);

                case PrimitiveTypeKind.Double:
                    return TryParseDouble(text, out value);

                case PrimitiveTypeKind.Guid:
                    return TryParseGuid(text, out value);

                case PrimitiveTypeKind.Single:
                    return TryParseSingle(text, out value);

                case PrimitiveTypeKind.SByte:
                    return TryParseSByte(text, out value);

                case PrimitiveTypeKind.Int16:
                    return TryParseInt16(text, out value);

                case PrimitiveTypeKind.Int32:
                    return TryParseInt32(text, out value);

                case PrimitiveTypeKind.Int64:
                    return TryParseInt64(text, out value);

                case PrimitiveTypeKind.String:
                    return TryParseString(text, out value);

                case PrimitiveTypeKind.Time:
                    return TryParseTime(text, out value);

                case PrimitiveTypeKind.DateTimeOffset:
                    return TryParseDateTimeOffset(text, out value);
            }
            throw EntityUtil.NotSupported(this._primitiveType.FullName);
        }

        private static bool TryParseBinary(string text, out object value)
        {
            if (!_BinaryValueValidator.IsMatch(text))
            {
                value = null;
                return false;
            }
            int length = 2;
            if ((text.Length % 2) == 1)
            {
                length = 1;
            }
            int num2 = (text.Length - 1) / 2;
            byte[] buffer = new byte[num2];
            int startIndex = 2;
            for (int i = 0; startIndex < text.Length; i++)
            {
                buffer[i] = byte.Parse(text.Substring(startIndex, length), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                startIndex += length;
                length = 2;
            }
            value = buffer;
            return true;
        }

        private static bool TryParseBoolean(string text, out object value)
        {
            bool flag;
            if (!bool.TryParse(text, out flag))
            {
                value = null;
                return false;
            }
            value = flag;
            return true;
        }

        private static bool TryParseByte(string text, out object value)
        {
            byte num;
            if (!byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseDateTime(string text, out object value)
        {
            DateTime time;
            if (!DateTime.TryParseExact(text, @"yyyy-MM-dd HH\:mm\:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time))
            {
                value = null;
                return false;
            }
            value = time;
            return true;
        }

        private static bool TryParseDateTimeOffset(string text, out object value)
        {
            DateTimeOffset offset;
            if (!DateTimeOffset.TryParse(text, out offset))
            {
                value = null;
                return false;
            }
            value = offset;
            return true;
        }

        private static bool TryParseDecimal(string text, out object value)
        {
            decimal num;
            if (!decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseDouble(string text, out object value)
        {
            double num;
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseGuid(string text, out object value)
        {
            if (!_GuidValueValidator.IsMatch(text))
            {
                value = null;
                return false;
            }
            value = new Guid(text);
            return true;
        }

        private static bool TryParseInt16(string text, out object value)
        {
            short num;
            if (!short.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseInt32(string text, out object value)
        {
            int num;
            if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseInt64(string text, out object value)
        {
            long num;
            if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseSByte(string text, out object value)
        {
            sbyte num;
            if (!sbyte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseSingle(string text, out object value)
        {
            float num;
            if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
            {
                value = null;
                return false;
            }
            value = num;
            return true;
        }

        private static bool TryParseString(string text, out object value)
        {
            value = text;
            return true;
        }

        private static bool TryParseTime(string text, out object value)
        {
            DateTime time;
            if (!DateTime.TryParseExact(text, @"HH\:mm\:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time))
            {
                value = null;
                return false;
            }
            value = new TimeSpan(time.Ticks);
            return true;
        }

        public PrimitiveType Type =>
            this._primitiveType;

        public PrimitiveTypeKind TypeKind =>
            this._primitiveType.PrimitiveTypeKind;
    }
}

