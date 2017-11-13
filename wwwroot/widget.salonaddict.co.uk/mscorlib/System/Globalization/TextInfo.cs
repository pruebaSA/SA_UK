namespace System.Globalization
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public class TextInfo : ICloneable, IDeserializationCallback
    {
        private const string CASING_EXCEPTIONS_FILE_NAME = "l_except.nlp";
        private const string CASING_FILE_NAME = "l_intl.nlp";
        [OptionalField(VersionAdded=2)]
        private string customCultureName;
        [NonSerialized]
        private TextInfo m_casingTextInfo;
        [NonSerialized]
        private CultureTableRecord m_cultureTableRecord;
        private static int m_exceptionCount;
        private static long[] m_exceptionNativeTextInfo;
        private unsafe static ExceptionTableItem* m_exceptionTable;
        [OptionalField(VersionAdded=2)]
        private bool m_isReadOnly;
        [OptionalField(VersionAdded=2)]
        private string m_listSeparator;
        [NonSerialized]
        private string m_name;
        internal int m_nDataItem;
        private unsafe static byte* m_pDataTable;
        private unsafe static void* m_pDefaultCasingTable;
        private unsafe static byte* m_pExceptionFile;
        private unsafe static void* m_pInvariantNativeTextInfo;
        [NonSerialized]
        private unsafe void* m_pNativeTextInfo;
        [NonSerialized]
        private int m_textInfoID;
        internal bool m_useUserOverride;
        internal int m_win32LangID;
        private static object s_InternalSyncObject;
        internal const int TurkishAnsiCodepage = 0x4e6;
        private const int wordSeparatorMask = 0x1ffcf800;

        static unsafe TextInfo()
        {
            byte* globalizationResourceBytePtr = GlobalizationAssembly.GetGlobalizationResourceBytePtr(typeof(TextInfo).Assembly, "l_intl.nlp");
            Thread.MemoryBarrier();
            m_pDataTable = globalizationResourceBytePtr;
            TextInfoDataHeader* pDataTable = (TextInfoDataHeader*) m_pDataTable;
            m_exceptionCount = pDataTable->exceptionCount;
            m_exceptionTable = (ExceptionTableItem*) &pDataTable->exceptionLangId;
            m_exceptionNativeTextInfo = new long[m_exceptionCount];
            m_pDefaultCasingTable = AllocateDefaultCasingTable(m_pDataTable);
        }

        internal unsafe TextInfo(CultureTableRecord table)
        {
            this.m_cultureTableRecord = table;
            this.m_textInfoID = this.m_cultureTableRecord.ITEXTINFO;
            if (table.IsSynthetic)
            {
                this.m_pNativeTextInfo = InvariantNativeTextInfo;
            }
            else
            {
                this.m_pNativeTextInfo = GetNativeTextInfo(this.m_textInfoID);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* AllocateDefaultCasingTable(byte* ptr);
        internal unsafe void ChangeCaseSurrogate(char highSurrogate, char lowSurrogate, out char resultHighSurrogate, out char resultLowSurrogate, bool isToUpper)
        {
            fixed (char* chRef = ((char*) resultHighSurrogate))
            {
                fixed (char* chRef2 = ((char*) resultLowSurrogate))
                {
                    nativeChangeCaseSurrogate(this.m_pNativeTextInfo, highSurrogate, lowSurrogate, chRef, chRef2, isToUpper);
                }
            }
        }

        [ComVisible(false)]
        public virtual object Clone()
        {
            object obj2 = base.MemberwiseClone();
            ((TextInfo) obj2).SetReadOnlyState(false);
            return obj2;
        }

        internal static unsafe int CompareOrdinalIgnoreCase(string str1, string str2) => 
            nativeCompareOrdinalIgnoreCase(InvariantNativeTextInfo, str1, str2);

        internal static unsafe int CompareOrdinalIgnoreCaseEx(string strA, int indexA, string strB, int indexB, int length) => 
            nativeCompareOrdinalIgnoreCaseEx(InvariantNativeTextInfo, strA, indexA, strB, indexB, length);

        public override bool Equals(object obj)
        {
            TextInfo info = obj as TextInfo;
            return ((info != null) && this.CultureName.Equals(info.CultureName));
        }

        internal unsafe int GetCaseInsensitiveHashCode(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (this.m_pNativeTextInfo == null)
            {
                this.OnDeserialized();
            }
            switch (this.m_textInfoID)
            {
                case 0x41f:
                case 0x42c:
                    str = nativeChangeCaseString(this.m_textInfoID, this.m_pNativeTextInfo, str, true);
                    break;
            }
            return nativeGetCaseInsHash(str, this.m_pNativeTextInfo);
        }

        public override int GetHashCode() => 
            this.CultureName.GetHashCode();

        internal static unsafe int GetHashCodeOrdinalIgnoreCase(string s) => 
            nativeGetHashCodeOrdinalIgnoreCase(InvariantNativeTextInfo, s);

        internal static unsafe void* GetNativeTextInfo(int cultureID)
        {
            if ((cultureID == 0x7f) && (Environment.OSVersion.Platform == PlatformID.Win32NT))
            {
                void* voidPtr = nativeGetInvariantTextInfo();
                if (voidPtr == null)
                {
                    throw new TypeInitializationException(typeof(TextInfo).ToString(), null);
                }
                return voidPtr;
            }
            void* pDefaultCasingTable = m_pDefaultCasingTable;
            for (int i = 0; i < m_exceptionCount; i++)
            {
                if (m_exceptionTable[i].langID == cultureID)
                {
                    if (m_exceptionNativeTextInfo[i] == 0L)
                    {
                        lock (InternalSyncObject)
                        {
                            if (m_pExceptionFile == null)
                            {
                                m_pExceptionFile = GlobalizationAssembly.GetGlobalizationResourceBytePtr(typeof(TextInfo).Assembly, "l_except.nlp");
                            }
                            long num2 = (long) ((ulong) InternalAllocateCasingTable(m_pExceptionFile, m_exceptionTable[i].exceptIndex));
                            Thread.MemoryBarrier();
                            m_exceptionNativeTextInfo[i] = num2;
                        }
                    }
                    return (void*) m_exceptionNativeTextInfo[i];
                }
            }
            return pDefaultCasingTable;
        }

        internal static unsafe int IndexOfStringOrdinalIgnoreCase(string source, string value, int startIndex, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return nativeIndexOfStringOrdinalIgnoreCase(InvariantNativeTextInfo, source, value, startIndex, count);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* InternalAllocateCasingTable(byte* ptr, int exceptionIndex);
        private bool IsLetterCategory(UnicodeCategory uc)
        {
            if (((uc != UnicodeCategory.UppercaseLetter) && (uc != UnicodeCategory.LowercaseLetter)) && ((uc != UnicodeCategory.TitlecaseLetter) && (uc != UnicodeCategory.ModifierLetter)))
            {
                return (uc == UnicodeCategory.OtherLetter);
            }
            return true;
        }

        private bool IsWordSeparator(UnicodeCategory category) => 
            ((0x1ffcf800 & (((int) 1) << category)) != 0);

        internal static unsafe int LastIndexOfStringOrdinalIgnoreCase(string source, string value, int startIndex, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return nativeLastIndexOfStringOrdinalIgnoreCase(InvariantNativeTextInfo, source, value, startIndex, count);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe char nativeChangeCaseChar(int win32LangID, void* pNativeTextInfo, char ch, bool isToUpper);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe string nativeChangeCaseString(int win32LangID, void* pNativeTextInfo, string str, bool isToUpper);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe void nativeChangeCaseSurrogate(void* pNativeTextInfo, char highSurrogate, char lowSurrogate, char* resultHighSurrogate, char* resultLowSurrogate, bool isToUpper);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeCompareOrdinalIgnoreCase(void* pNativeTextInfo, string str1, string str2);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeCompareOrdinalIgnoreCaseEx(void* pNativeTextInfo, string strA, int indexA, string strB, int indexB, int length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeGetCaseInsHash(string str, void* pNativeTextInfo);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeGetHashCodeOrdinalIgnoreCase(void* pNativeTextInfo, string s);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* nativeGetInvariantTextInfo();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe char nativeGetTitleCaseChar(void* pNativeTextInfo, char ch);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int nativeIndexOfCharOrdinalIgnoreCase(void* pNativeTextInfo, string str, char value, int startIndex, int count);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeIndexOfStringOrdinalIgnoreCase(void* pNativeTextInfo, string str, string value, int startIndex, int count);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe int nativeLastIndexOfCharOrdinalIgnoreCase(void* pNativeTextInfo, string str, char value, int startIndex, int count);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe int nativeLastIndexOfStringOrdinalIgnoreCase(void* pNativeTextInfo, string str, string value, int startIndex, int count);
        private unsafe void OnDeserialized()
        {
            if (this.m_cultureTableRecord == null)
            {
                if (this.m_win32LangID == 0)
                {
                    this.m_win32LangID = CultureTableRecord.IdFromEverettDataItem(this.m_nDataItem);
                }
                if (this.customCultureName != null)
                {
                    this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.customCultureName, this.m_useUserOverride);
                }
                else
                {
                    this.m_cultureTableRecord = CultureTableRecord.GetCultureTableRecord(this.m_win32LangID, this.m_useUserOverride);
                }
                this.m_textInfoID = this.m_cultureTableRecord.ITEXTINFO;
                if (this.m_cultureTableRecord.IsSynthetic)
                {
                    this.m_pNativeTextInfo = InvariantNativeTextInfo;
                }
                else
                {
                    this.m_pNativeTextInfo = GetNativeTextInfo(this.m_textInfoID);
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            this.OnDeserialized();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext ctx)
        {
            this.m_cultureTableRecord = null;
            this.m_win32LangID = 0;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctx)
        {
            this.m_nDataItem = this.m_cultureTableRecord.EverettDataItem();
            this.m_useUserOverride = this.m_cultureTableRecord.UseUserOverride;
            if (CultureTableRecord.IsCustomCultureId(this.m_cultureTableRecord.CultureID))
            {
                this.customCultureName = this.m_cultureTableRecord.SNAME;
                this.m_win32LangID = this.m_textInfoID;
            }
            else
            {
                this.customCultureName = null;
                this.m_win32LangID = this.m_cultureTableRecord.CultureID;
            }
        }

        [ComVisible(false)]
        public static TextInfo ReadOnly(TextInfo textInfo)
        {
            if (textInfo == null)
            {
                throw new ArgumentNullException("textInfo");
            }
            if (textInfo.IsReadOnly)
            {
                return textInfo;
            }
            TextInfo info = (TextInfo) textInfo.MemberwiseClone();
            info.SetReadOnlyState(true);
            return info;
        }

        internal void SetReadOnlyState(bool readOnly)
        {
            this.m_isReadOnly = readOnly;
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this.OnDeserialized();
        }

        public virtual unsafe char ToLower(char c)
        {
            if (this.m_cultureTableRecord.IsSynthetic)
            {
                return this.CasingTextInfo.ToLower(c);
            }
            return nativeChangeCaseChar(this.m_textInfoID, this.m_pNativeTextInfo, c, false);
        }

        public virtual unsafe string ToLower(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (this.m_cultureTableRecord.IsSynthetic)
            {
                return this.CasingTextInfo.ToLower(str);
            }
            return nativeChangeCaseString(this.m_textInfoID, this.m_pNativeTextInfo, str, false);
        }

        public override string ToString() => 
            ("TextInfo - " + this.m_textInfoID);

        public unsafe string ToTitleCase(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (this.m_cultureTableRecord.IsSynthetic)
            {
                if (this.ANSICodePage == 0x4e6)
                {
                    return CultureInfo.GetCultureInfo("tr-TR").TextInfo.ToTitleCase(str);
                }
                return CultureInfo.GetCultureInfo("en-US").TextInfo.ToTitleCase(str);
            }
            int length = str.Length;
            if (length == 0)
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            string str2 = null;
            for (int i = 0; i < length; i++)
            {
                int num3;
                UnicodeCategory uc = CharUnicodeInfo.InternalGetUnicodeCategory(str, i, out num3);
                if (char.CheckLetter(uc))
                {
                    if (num3 == 1)
                    {
                        builder.Append(nativeGetTitleCaseChar(this.m_pNativeTextInfo, str[i]));
                    }
                    else
                    {
                        char ch;
                        char ch2;
                        this.ChangeCaseSurrogate(str[i], str[i + 1], out ch, out ch2, true);
                        builder.Append(ch);
                        builder.Append(ch2);
                    }
                    i += num3;
                    int startIndex = i;
                    bool flag = uc == UnicodeCategory.LowercaseLetter;
                    while (i < length)
                    {
                        uc = CharUnicodeInfo.InternalGetUnicodeCategory(str, i, out num3);
                        if (this.IsLetterCategory(uc))
                        {
                            if (uc == UnicodeCategory.LowercaseLetter)
                            {
                                flag = true;
                            }
                            i += num3;
                        }
                        else
                        {
                            if (str[i] == '\'')
                            {
                                i++;
                                if (flag)
                                {
                                    if (str2 == null)
                                    {
                                        str2 = this.ToLower(str);
                                    }
                                    builder.Append(str2, startIndex, i - startIndex);
                                }
                                else
                                {
                                    builder.Append(str, startIndex, i - startIndex);
                                }
                                startIndex = i;
                                flag = true;
                                continue;
                            }
                            if (this.IsWordSeparator(uc))
                            {
                                break;
                            }
                            i += num3;
                        }
                    }
                    int count = i - startIndex;
                    if (count > 0)
                    {
                        if (flag)
                        {
                            if (str2 == null)
                            {
                                str2 = this.ToLower(str);
                            }
                            builder.Append(str2, startIndex, count);
                        }
                        else
                        {
                            builder.Append(str, startIndex, count);
                        }
                    }
                    if (i < length)
                    {
                        if (num3 == 1)
                        {
                            builder.Append(str[i]);
                        }
                        else
                        {
                            builder.Append(str[i++]);
                            builder.Append(str[i]);
                        }
                    }
                    continue;
                }
                if (num3 == 1)
                {
                    builder.Append(str[i]);
                }
                else
                {
                    builder.Append(str[i++]);
                    builder.Append(str[i]);
                }
            }
            return builder.ToString();
        }

        public virtual unsafe char ToUpper(char c)
        {
            if (this.m_cultureTableRecord.IsSynthetic)
            {
                return this.CasingTextInfo.ToUpper(c);
            }
            return nativeChangeCaseChar(this.m_textInfoID, this.m_pNativeTextInfo, c, true);
        }

        public virtual unsafe string ToUpper(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (this.m_cultureTableRecord.IsSynthetic)
            {
                return this.CasingTextInfo.ToUpper(str);
            }
            return nativeChangeCaseString(this.m_textInfoID, this.m_pNativeTextInfo, str, true);
        }

        private void VerifyWritable()
        {
            if (this.m_isReadOnly)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
            }
        }

        public virtual int ANSICodePage =>
            this.m_cultureTableRecord.IDEFAULTANSICODEPAGE;

        internal TextInfo CasingTextInfo
        {
            get
            {
                if (this.m_casingTextInfo == null)
                {
                    if (this.ANSICodePage == 0x4e6)
                    {
                        this.m_casingTextInfo = CultureInfo.GetCultureInfo("tr-TR").TextInfo;
                    }
                    else
                    {
                        this.m_casingTextInfo = CultureInfo.GetCultureInfo("en-US").TextInfo;
                    }
                }
                return this.m_casingTextInfo;
            }
        }

        [ComVisible(false)]
        public string CultureName
        {
            get
            {
                if (this.m_name == null)
                {
                    this.m_name = CultureInfo.GetCultureInfo(this.m_textInfoID).Name;
                }
                return this.m_name;
            }
        }

        public virtual int EBCDICCodePage =>
            this.m_cultureTableRecord.IDEFAULTEBCDICCODEPAGE;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        internal static void* InvariantNativeTextInfo
        {
            get
            {
                if (m_pInvariantNativeTextInfo == null)
                {
                    lock (InternalSyncObject)
                    {
                        if (m_pInvariantNativeTextInfo == null)
                        {
                            m_pInvariantNativeTextInfo = GetNativeTextInfo(0x7f);
                        }
                    }
                }
                return m_pInvariantNativeTextInfo;
            }
        }

        [ComVisible(false)]
        public bool IsReadOnly =>
            this.m_isReadOnly;

        [ComVisible(false)]
        public bool IsRightToLeft =>
            ((this.m_cultureTableRecord.ILINEORIENTATIONS & 0x8000) != 0);

        [ComVisible(false)]
        public int LCID =>
            this.m_textInfoID;

        public virtual string ListSeparator
        {
            get
            {
                if (this.m_listSeparator == null)
                {
                    this.m_listSeparator = this.m_cultureTableRecord.SLIST;
                }
                return this.m_listSeparator;
            }
            [ComVisible(false)]
            set
            {
                this.VerifyWritable();
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_String"));
                }
                this.m_listSeparator = value;
            }
        }

        public virtual int MacCodePage =>
            this.m_cultureTableRecord.IDEFAULTMACCODEPAGE;

        public virtual int OEMCodePage =>
            this.m_cultureTableRecord.IDEFAULTOEMCODEPAGE;

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        internal struct ExceptionTableItem
        {
            internal ushort langID;
            internal ushort exceptIndex;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct TextInfoDataHeader
        {
            [FieldOffset(180)]
            internal ushort exceptionCount;
            [FieldOffset(0xb6)]
            internal ushort exceptionLangId;
            [FieldOffset(0x2c)]
            internal uint OffsetToLowerCasingTable;
            [FieldOffset(0x30)]
            internal uint OffsetToTitleCaseTable;
            [FieldOffset(40)]
            internal uint OffsetToUpperCasingTable;
            [FieldOffset(0x34)]
            internal uint PlaneOffset;
            [FieldOffset(0)]
            internal char TableName;
            [FieldOffset(0x20)]
            internal ushort version;
        }
    }
}

