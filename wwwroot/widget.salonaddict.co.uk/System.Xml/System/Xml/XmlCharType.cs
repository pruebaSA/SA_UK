namespace System.Xml
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XmlCharType
    {
        internal const int fWhitespace = 1;
        internal const int fLetter = 2;
        internal const int fNCStartName = 4;
        internal const int fNCName = 8;
        internal const int fCharData = 0x10;
        internal const int fPublicId = 0x20;
        internal const int fText = 0x40;
        internal const int fAttrValue = 0x80;
        private const uint CharPropertiesSize = 0x10000;
        private static object s_Lock;
        private unsafe static byte* s_CharProperties;
        internal unsafe byte* charProperties;
        private static object StaticLock
        {
            get
            {
                if (s_Lock == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_Lock, obj2, null);
                }
                return s_Lock;
            }
        }
        private static unsafe void InitInstance()
        {
            lock (StaticLock)
            {
                if (s_CharProperties == null)
                {
                    UnmanagedMemoryStream manifestResourceStream = (UnmanagedMemoryStream) Assembly.GetExecutingAssembly().GetManifestResourceStream("XmlCharType.bin");
                    byte* positionPointer = manifestResourceStream.PositionPointer;
                    Thread.MemoryBarrier();
                    s_CharProperties = positionPointer;
                }
            }
        }

        private unsafe XmlCharType(byte* charProperties)
        {
            this.charProperties = charProperties;
        }

        internal static XmlCharType Instance
        {
            get
            {
                if (s_CharProperties == null)
                {
                    InitInstance();
                }
                return new XmlCharType(s_CharProperties);
            }
        }
        public unsafe bool IsWhiteSpace(char ch) => 
            ((this.charProperties[ch] & 1) != 0);

        public unsafe bool IsLetter(char ch) => 
            ((this.charProperties[ch] & 2) != 0);

        public bool IsExtender(char ch) => 
            (ch == '\x00b7');

        public unsafe bool IsNCNameChar(char ch) => 
            ((this.charProperties[ch] & 8) != 0);

        public unsafe bool IsStartNCNameChar(char ch) => 
            ((this.charProperties[ch] & 4) != 0);

        public unsafe bool IsCharData(char ch) => 
            ((this.charProperties[ch] & 0x10) != 0);

        public unsafe bool IsPubidChar(char ch) => 
            ((this.charProperties[ch] & 0x20) != 0);

        internal unsafe bool IsTextChar(char ch) => 
            ((this.charProperties[ch] & 0x40) != 0);

        internal unsafe bool IsAttributeValueChar(char ch) => 
            ((this.charProperties[ch] & 0x80) != 0);

        public bool IsNameChar(char ch)
        {
            if (!this.IsNCNameChar(ch))
            {
                return (ch == ':');
            }
            return true;
        }

        public bool IsStartNameChar(char ch)
        {
            if (!this.IsStartNCNameChar(ch))
            {
                return (ch == ':');
            }
            return true;
        }

        public bool IsDigit(char ch) => 
            ((ch >= '0') && (ch <= '9'));

        public bool IsHexDigit(char ch)
        {
            if (((ch < '0') || (ch > '9')) && ((ch < 'a') || (ch > 'f')))
            {
                return ((ch >= 'A') && (ch <= 'F'));
            }
            return true;
        }

        internal bool IsOnlyWhitespace(string str) => 
            (this.IsOnlyWhitespaceWithPos(str) == -1);

        internal unsafe int IsOnlyWhitespaceWithPos(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((this.charProperties[str[i]] & 1) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal bool IsName(string str)
        {
            if ((str.Length == 0) || !this.IsStartNameChar(str[0]))
            {
                return false;
            }
            for (int i = 1; i < str.Length; i++)
            {
                if (!this.IsNameChar(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal unsafe bool IsNmToken(string str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (((this.charProperties[str[i]] & 8) == 0) && (str[i] != ':'))
                {
                    return false;
                }
            }
            return true;
        }

        internal unsafe int IsOnlyCharData(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((this.charProperties[str[i]] & 0x10) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal unsafe int IsPublicId(string str)
        {
            if (str != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if ((this.charProperties[str[i]] & 0x20) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}

