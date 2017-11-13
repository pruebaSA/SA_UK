namespace PdfSharp.Fonts.OpenType
{
    using System;
    using System.Collections.Generic;

    internal class FontDataStock
    {
        private Dictionary<string, FontData> fontDataTable = new Dictionary<string, FontData>();
        private static FontDataStock global;
        private FontData lastEntry;

        private FontDataStock()
        {
        }

        private static uint CalcChecksum(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            uint num = 0;
            uint num2 = 0;
            int length = buffer.Length;
            int num4 = 0;
            while (length > 0)
            {
                int num5 = 0xed8;
                if (num5 > length)
                {
                    num5 = length;
                }
                length -= num5;
                while (--num5 >= 0)
                {
                    num += (uint) (buffer[num4++] & 0xff);
                    num2 += num;
                }
                num = num % 0xfff1;
                num2 = num2 % 0xfff1;
            }
            return ((num2 << 0x10) | num);
        }

        internal FontData[] GetFontDataList()
        {
            FontData[] array = new FontData[this.fontDataTable.Values.Count];
            this.fontDataTable.Values.CopyTo(array, 0);
            return array;
        }

        public FontData RegisterFontData(byte[] data)
        {
            FontData data2;
            uint num = CalcChecksum(data);
            string key = $"??{num:X}";
            if (!this.fontDataTable.TryGetValue(key, out data2))
            {
                lock (typeof(FontDataStock))
                {
                    if (!this.fontDataTable.TryGetValue(key, out data2))
                    {
                        data2 = new FontData(data);
                        this.fontDataTable.Add(key, data2);
                        this.lastEntry = data2;
                    }
                }
            }
            return data2;
        }

        public bool UnregisterFontData(FontData fontData) => 
            false;

        public static FontDataStock Global
        {
            get
            {
                if (global == null)
                {
                    lock (typeof(FontDataStock))
                    {
                        if (global == null)
                        {
                            global = new FontDataStock();
                        }
                    }
                }
                return global;
            }
        }
    }
}

