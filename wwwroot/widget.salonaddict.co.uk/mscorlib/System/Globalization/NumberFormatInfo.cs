namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public sealed class NumberFormatInfo : ICloneable, IFormatProvider
    {
        internal string ansiCurrencySymbol;
        internal int currencyDecimalDigits;
        internal string currencyDecimalSeparator;
        internal string currencyGroupSeparator;
        internal int[] currencyGroupSizes;
        internal int currencyNegativePattern;
        internal int currencyPositivePattern;
        internal string currencySymbol;
        [OptionalField(VersionAdded=2)]
        internal int digitSubstitution;
        private const NumberStyles InvalidNumberStyles = ~(NumberStyles.HexNumber | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowExponent | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.AllowLeadingSign);
        private static NumberFormatInfo invariantInfo;
        internal bool isReadOnly;
        internal int m_dataItem;
        internal bool m_useUserOverride;
        internal string nanSymbol;
        [OptionalField(VersionAdded=2)]
        internal string[] nativeDigits;
        internal string negativeInfinitySymbol;
        internal string negativeSign;
        internal int numberDecimalDigits;
        internal string numberDecimalSeparator;
        internal string numberGroupSeparator;
        internal int[] numberGroupSizes;
        internal int numberNegativePattern;
        internal int percentDecimalDigits;
        internal string percentDecimalSeparator;
        internal string percentGroupSeparator;
        internal int[] percentGroupSizes;
        internal int percentNegativePattern;
        internal int percentPositivePattern;
        internal string percentSymbol;
        internal string perMilleSymbol;
        internal string positiveInfinitySymbol;
        internal string positiveSign;
        internal bool validForParseAsCurrency;
        internal bool validForParseAsNumber;

        public NumberFormatInfo() : this(null)
        {
        }

        internal NumberFormatInfo(CultureTableRecord cultureTableRecord)
        {
            this.numberGroupSizes = new int[] { 3 };
            this.currencyGroupSizes = new int[] { 3 };
            this.percentGroupSizes = new int[] { 3 };
            this.positiveSign = "+";
            this.negativeSign = "-";
            this.numberDecimalSeparator = ".";
            this.numberGroupSeparator = ",";
            this.currencyGroupSeparator = ",";
            this.currencyDecimalSeparator = ".";
            this.currencySymbol = "\x00a4";
            this.nanSymbol = "NaN";
            this.positiveInfinitySymbol = "Infinity";
            this.negativeInfinitySymbol = "-Infinity";
            this.percentDecimalSeparator = ".";
            this.percentGroupSeparator = ",";
            this.percentSymbol = "%";
            this.perMilleSymbol = "‰";
            this.nativeDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            this.numberDecimalDigits = 2;
            this.currencyDecimalDigits = 2;
            this.numberNegativePattern = 1;
            this.percentDecimalDigits = 2;
            this.digitSubstitution = 1;
            this.validForParseAsNumber = true;
            this.validForParseAsCurrency = true;
            if (cultureTableRecord != null)
            {
                cultureTableRecord.GetNFIOverrideValues(this);
                if ((0x3a4 == cultureTableRecord.IDEFAULTANSICODEPAGE) || (0x3b5 == cultureTableRecord.IDEFAULTANSICODEPAGE))
                {
                    this.ansiCurrencySymbol = @"\";
                }
                this.negativeInfinitySymbol = cultureTableRecord.SNEGINFINITY;
                this.positiveInfinitySymbol = cultureTableRecord.SPOSINFINITY;
                this.nanSymbol = cultureTableRecord.SNAN;
            }
        }

        internal void CheckGroupSize(string propName, int[] groupSize)
        {
            for (int i = 0; i < groupSize.Length; i++)
            {
                if (groupSize[i] < 1)
                {
                    if ((i != (groupSize.Length - 1)) || (groupSize[i] != 0))
                    {
                        throw new ArgumentException(propName, Environment.GetResourceString("Argument_InvalidGroupSize"));
                    }
                    return;
                }
                if (groupSize[i] > 9)
                {
                    throw new ArgumentException(propName, Environment.GetResourceString("Argument_InvalidGroupSize"));
                }
            }
        }

        public object Clone()
        {
            NumberFormatInfo info = (NumberFormatInfo) base.MemberwiseClone();
            info.isReadOnly = false;
            return info;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType != typeof(NumberFormatInfo))
            {
                return null;
            }
            return this;
        }

        public static NumberFormatInfo GetInstance(IFormatProvider formatProvider)
        {
            NumberFormatInfo numInfo;
            CultureInfo info2 = formatProvider as CultureInfo;
            if ((info2 != null) && !info2.m_isInherited)
            {
                numInfo = info2.numInfo;
                if (numInfo != null)
                {
                    return numInfo;
                }
                return info2.NumberFormat;
            }
            numInfo = formatProvider as NumberFormatInfo;
            if (numInfo != null)
            {
                return numInfo;
            }
            if (formatProvider != null)
            {
                numInfo = formatProvider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
                if (numInfo != null)
                {
                    return numInfo;
                }
            }
            return CurrentInfo;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (this.nativeDigits == null)
            {
                this.nativeDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            }
            if (this.digitSubstitution < 0)
            {
                this.digitSubstitution = 1;
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext ctx)
        {
            this.nativeDigits = null;
            this.digitSubstitution = -1;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            if (this.numberDecimalSeparator != this.numberGroupSeparator)
            {
                this.validForParseAsNumber = true;
            }
            else
            {
                this.validForParseAsNumber = false;
            }
            if (((this.numberDecimalSeparator != this.numberGroupSeparator) && (this.numberDecimalSeparator != this.currencyGroupSeparator)) && ((this.currencyDecimalSeparator != this.numberGroupSeparator) && (this.currencyDecimalSeparator != this.currencyGroupSeparator)))
            {
                this.validForParseAsCurrency = true;
            }
            else
            {
                this.validForParseAsCurrency = false;
            }
        }

        public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
        {
            if (nfi == null)
            {
                throw new ArgumentNullException("nfi");
            }
            if (nfi.IsReadOnly)
            {
                return nfi;
            }
            NumberFormatInfo info = (NumberFormatInfo) nfi.MemberwiseClone();
            info.isReadOnly = true;
            return info;
        }

        internal static void ValidateParseStyleFloatingPoint(NumberStyles style)
        {
            if ((style & ~(NumberStyles.HexNumber | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowExponent | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.AllowLeadingSign)) != NumberStyles.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidNumberStyles"), "style");
            }
            if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_HexStyleNotSupported"));
            }
        }

        internal static void ValidateParseStyleInteger(NumberStyles style)
        {
            if ((style & ~(NumberStyles.HexNumber | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowExponent | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign | NumberStyles.AllowLeadingSign)) != NumberStyles.None)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidNumberStyles"), "style");
            }
            if (((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None) && ((style & ~NumberStyles.HexNumber) != NumberStyles.None))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_InvalidHexStyle"));
            }
        }

        private void VerifyDecimalSeparator(string decSep, string propertyName)
        {
            if (decSep == null)
            {
                throw new ArgumentNullException(propertyName, Environment.GetResourceString("ArgumentNull_String"));
            }
            if (decSep.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyDecString"));
            }
        }

        private void VerifyDigitSubstitution(DigitShapes digitSub, string propertyName)
        {
            switch (digitSub)
            {
                case DigitShapes.Context:
                case DigitShapes.None:
                case DigitShapes.NativeNational:
                    return;
            }
            throw new ArgumentException(propertyName, Environment.GetResourceString("Argument_InvalidDigitSubstitution"));
        }

        private void VerifyGroupSeparator(string groupSep, string propertyName)
        {
            if (groupSep == null)
            {
                throw new ArgumentNullException(propertyName, Environment.GetResourceString("ArgumentNull_String"));
            }
        }

        private void VerifyNativeDigits(string[] nativeDig, string propertyName)
        {
            if (nativeDig == null)
            {
                throw new ArgumentNullException(propertyName, Environment.GetResourceString("ArgumentNull_Array"));
            }
            if (nativeDig.Length != 10)
            {
                throw new ArgumentException(propertyName, Environment.GetResourceString("Argument_InvalidNativeDigitCount"));
            }
            for (int i = 0; i < nativeDig.Length; i++)
            {
                if (nativeDig[i] == null)
                {
                    throw new ArgumentNullException(propertyName, Environment.GetResourceString("ArgumentNull_ArrayValue"));
                }
                if (nativeDig[i].Length != 1)
                {
                    if (nativeDig[i].Length != 2)
                    {
                        throw new ArgumentException(propertyName, Environment.GetResourceString("Argument_InvalidNativeDigitValue"));
                    }
                    if (!char.IsSurrogatePair(nativeDig[i][0], nativeDig[i][1]))
                    {
                        throw new ArgumentException(propertyName, Environment.GetResourceString("Argument_InvalidNativeDigitValue"));
                    }
                }
                if ((CharUnicodeInfo.GetDecimalDigitValue(nativeDig[i], 0) != i) && (CharUnicodeInfo.GetUnicodeCategory(nativeDig[i], 0) != UnicodeCategory.PrivateUse))
                {
                    throw new ArgumentException(propertyName, Environment.GetResourceString("Argument_InvalidNativeDigitValue"));
                }
            }
        }

        private void VerifyWritable()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
            }
        }

        public int CurrencyDecimalDigits
        {
            get => 
                this.currencyDecimalDigits;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 0x63))
                {
                    throw new ArgumentOutOfRangeException("CurrencyDecimalDigits", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 0x63 }));
                }
                this.currencyDecimalDigits = value;
            }
        }

        public string CurrencyDecimalSeparator
        {
            get => 
                this.currencyDecimalSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyDecimalSeparator(value, "CurrencyDecimalSeparator");
                this.currencyDecimalSeparator = value;
            }
        }

        public string CurrencyGroupSeparator
        {
            get => 
                this.currencyGroupSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyGroupSeparator(value, "CurrencyGroupSeparator");
                this.currencyGroupSeparator = value;
            }
        }

        public int[] CurrencyGroupSizes
        {
            get => 
                ((int[]) this.currencyGroupSizes.Clone());
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("CurrencyGroupSizes", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                int[] groupSize = (int[]) value.Clone();
                this.CheckGroupSize("CurrencyGroupSizes", groupSize);
                this.currencyGroupSizes = groupSize;
            }
        }

        public int CurrencyNegativePattern
        {
            get => 
                this.currencyNegativePattern;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 15))
                {
                    throw new ArgumentOutOfRangeException("CurrencyNegativePattern", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 15 }));
                }
                this.currencyNegativePattern = value;
            }
        }

        public int CurrencyPositivePattern
        {
            get => 
                this.currencyPositivePattern;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 3))
                {
                    throw new ArgumentOutOfRangeException("CurrencyPositivePattern", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 3 }));
                }
                this.currencyPositivePattern = value;
            }
        }

        public string CurrencySymbol
        {
            get => 
                this.currencySymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("CurrencySymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.currencySymbol = value;
            }
        }

        public static NumberFormatInfo CurrentInfo
        {
            get
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                if (!currentCulture.m_isInherited)
                {
                    NumberFormatInfo numInfo = currentCulture.numInfo;
                    if (numInfo != null)
                    {
                        return numInfo;
                    }
                }
                return (NumberFormatInfo) currentCulture.GetFormat(typeof(NumberFormatInfo));
            }
        }

        [ComVisible(false)]
        public DigitShapes DigitSubstitution
        {
            get => 
                ((DigitShapes) this.digitSubstitution);
            set
            {
                this.VerifyWritable();
                this.VerifyDigitSubstitution(value, "DigitSubstitution");
                this.digitSubstitution = (int) value;
            }
        }

        public static NumberFormatInfo InvariantInfo
        {
            get
            {
                if (invariantInfo == null)
                {
                    invariantInfo = ReadOnly(new NumberFormatInfo());
                }
                return invariantInfo;
            }
        }

        public bool IsReadOnly =>
            this.isReadOnly;

        public string NaNSymbol
        {
            get => 
                this.nanSymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("NaNSymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.nanSymbol = value;
            }
        }

        [ComVisible(false)]
        public string[] NativeDigits
        {
            get => 
                this.nativeDigits;
            set
            {
                this.VerifyWritable();
                this.VerifyNativeDigits(value, "NativeDigits");
                this.nativeDigits = value;
            }
        }

        public string NegativeInfinitySymbol
        {
            get => 
                this.negativeInfinitySymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("NegativeInfinitySymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.negativeInfinitySymbol = value;
            }
        }

        public string NegativeSign
        {
            get => 
                this.negativeSign;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("NegativeSign", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.negativeSign = value;
            }
        }

        public int NumberDecimalDigits
        {
            get => 
                this.numberDecimalDigits;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 0x63))
                {
                    throw new ArgumentOutOfRangeException("NumberDecimalDigits", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 0x63 }));
                }
                this.numberDecimalDigits = value;
            }
        }

        public string NumberDecimalSeparator
        {
            get => 
                this.numberDecimalSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyDecimalSeparator(value, "NumberDecimalSeparator");
                this.numberDecimalSeparator = value;
            }
        }

        public string NumberGroupSeparator
        {
            get => 
                this.numberGroupSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyGroupSeparator(value, "NumberGroupSeparator");
                this.numberGroupSeparator = value;
            }
        }

        public int[] NumberGroupSizes
        {
            get => 
                ((int[]) this.numberGroupSizes.Clone());
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("NumberGroupSizes", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                int[] groupSize = (int[]) value.Clone();
                this.CheckGroupSize("NumberGroupSizes", groupSize);
                this.numberGroupSizes = groupSize;
            }
        }

        public int NumberNegativePattern
        {
            get => 
                this.numberNegativePattern;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 4))
                {
                    throw new ArgumentOutOfRangeException("NumberNegativePattern", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 4 }));
                }
                this.numberNegativePattern = value;
            }
        }

        public int PercentDecimalDigits
        {
            get => 
                this.percentDecimalDigits;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 0x63))
                {
                    throw new ArgumentOutOfRangeException("PercentDecimalDigits", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 0x63 }));
                }
                this.percentDecimalDigits = value;
            }
        }

        public string PercentDecimalSeparator
        {
            get => 
                this.percentDecimalSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyDecimalSeparator(value, "PercentDecimalSeparator");
                this.percentDecimalSeparator = value;
            }
        }

        public string PercentGroupSeparator
        {
            get => 
                this.percentGroupSeparator;
            set
            {
                this.VerifyWritable();
                this.VerifyGroupSeparator(value, "PercentGroupSeparator");
                this.percentGroupSeparator = value;
            }
        }

        public int[] PercentGroupSizes
        {
            get => 
                ((int[]) this.percentGroupSizes.Clone());
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("PercentGroupSizes", Environment.GetResourceString("ArgumentNull_Obj"));
                }
                int[] groupSize = (int[]) value.Clone();
                this.CheckGroupSize("PercentGroupSizes", groupSize);
                this.percentGroupSizes = groupSize;
            }
        }

        public int PercentNegativePattern
        {
            get => 
                this.percentNegativePattern;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 11))
                {
                    throw new ArgumentOutOfRangeException("PercentNegativePattern", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 11 }));
                }
                this.percentNegativePattern = value;
            }
        }

        public int PercentPositivePattern
        {
            get => 
                this.percentPositivePattern;
            set
            {
                this.VerifyWritable();
                if ((value < 0) || (value > 3))
                {
                    throw new ArgumentOutOfRangeException("PercentPositivePattern", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Range"), new object[] { 0, 3 }));
                }
                this.percentPositivePattern = value;
            }
        }

        public string PercentSymbol
        {
            get => 
                this.percentSymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("PercentSymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.percentSymbol = value;
            }
        }

        public string PerMilleSymbol
        {
            get => 
                this.perMilleSymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("PerMilleSymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.perMilleSymbol = value;
            }
        }

        public string PositiveInfinitySymbol
        {
            get => 
                this.positiveInfinitySymbol;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("PositiveInfinitySymbol", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.positiveInfinitySymbol = value;
            }
        }

        public string PositiveSign
        {
            get => 
                this.positiveSign;
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("PositiveSign", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.positiveSign = value;
            }
        }
    }
}

