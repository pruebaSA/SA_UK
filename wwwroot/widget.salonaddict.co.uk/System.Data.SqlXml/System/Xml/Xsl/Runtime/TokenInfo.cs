namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Diagnostics;

    internal class TokenInfo
    {
        public string formatString;
        public int length;
        public char startChar;
        public int startIdx;

        private TokenInfo()
        {
        }

        [Conditional("DEBUG")]
        public void AssertSeparator(bool isSeparator)
        {
        }

        public static TokenInfo CreateFormat(string formatString, int startIdx, int tokLen)
        {
            TokenInfo info = new TokenInfo {
                formatString = null,
                length = 1
            };
            bool flag = false;
            char ch = formatString[startIdx];
            switch (ch)
            {
                case '1':
                case 'A':
                case 'I':
                case 'a':
                case 'i':
                    break;

                default:
                    if (!CharUtil.IsDecimalDigitOne(ch))
                    {
                        if (CharUtil.IsDecimalDigitOne((char) (ch + '\x0001')))
                        {
                            int num = startIdx;
                            do
                            {
                                info.length++;
                            }
                            while ((--tokLen > 0) && (ch == formatString[++num]));
                            if (formatString[num] == (ch = (char) (ch + '\x0001')))
                            {
                                break;
                            }
                        }
                        flag = true;
                    }
                    break;
            }
            if (tokLen != 1)
            {
                flag = true;
            }
            if (flag)
            {
                info.startChar = '1';
                info.length = 1;
                return info;
            }
            info.startChar = ch;
            return info;
        }

        public static TokenInfo CreateSeparator(string formatString, int startIdx, int tokLen) => 
            new TokenInfo { 
                startIdx = startIdx,
                formatString = formatString,
                length = tokLen
            };
    }
}

