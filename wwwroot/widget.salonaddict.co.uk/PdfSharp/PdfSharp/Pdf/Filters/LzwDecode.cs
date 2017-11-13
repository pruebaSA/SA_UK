namespace PdfSharp.Pdf.Filters
{
    using System;
    using System.IO;

    public class LzwDecode : PdfSharp.Pdf.Filters.Filter
    {
        private readonly int[] andTable = new int[] { 0x1ff, 0x3ff, 0x7ff, 0xfff };
        private int bitsToGet = 9;
        private int bytePointer;
        private byte[] data;
        private int nextBits;
        private int nextData;
        private byte[][] stringTable;
        private int tableIndex;

        private void AddEntry(byte[] oldstring, byte newstring)
        {
            int length = oldstring.Length;
            byte[] destinationArray = new byte[length + 1];
            Array.Copy(oldstring, 0, destinationArray, 0, length);
            destinationArray[length] = newstring;
            this.stringTable[this.tableIndex++] = destinationArray;
            if (this.tableIndex == 0x1ff)
            {
                this.bitsToGet = 10;
            }
            else if (this.tableIndex == 0x3ff)
            {
                this.bitsToGet = 11;
            }
            else if (this.tableIndex == 0x7ff)
            {
                this.bitsToGet = 12;
            }
        }

        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            int nextCode;
            if ((data[0] == 0) && (data[1] == 1))
            {
                throw new Exception("LZW flavour not supported.");
            }
            MemoryStream stream = new MemoryStream();
            this.InitializeDictionary();
            this.data = data;
            this.bytePointer = 0;
            this.nextData = 0;
            this.nextBits = 0;
            int index = 0;
            while ((nextCode = this.NextCode) != 0x101)
            {
                if (nextCode == 0x100)
                {
                    this.InitializeDictionary();
                    nextCode = this.NextCode;
                    if (nextCode == 0x101)
                    {
                        break;
                    }
                    stream.Write(this.stringTable[nextCode], 0, this.stringTable[nextCode].Length);
                    index = nextCode;
                }
                else
                {
                    byte[] buffer;
                    if (nextCode < this.tableIndex)
                    {
                        buffer = this.stringTable[nextCode];
                        stream.Write(buffer, 0, buffer.Length);
                        this.AddEntry(this.stringTable[index], buffer[0]);
                        index = nextCode;
                        continue;
                    }
                    buffer = this.stringTable[index];
                    stream.Write(buffer, 0, buffer.Length);
                    this.AddEntry(buffer, buffer[0]);
                    index = nextCode;
                }
            }
            if (stream.Length >= 0L)
            {
                stream.Capacity = (int) stream.Length;
                return stream.GetBuffer();
            }
            return null;
        }

        public override byte[] Encode(byte[] data)
        {
            throw new NotImplementedException("PDFsharp does not support LZW encoding.");
        }

        private void InitializeDictionary()
        {
            this.stringTable = new byte[0x2000][];
            for (int i = 0; i < 0x100; i++)
            {
                this.stringTable[i] = new byte[] { (byte) i };
            }
            this.tableIndex = 0x102;
            this.bitsToGet = 9;
        }

        private int NextCode
        {
            get
            {
                try
                {
                    this.nextData = (this.nextData << 8) | (this.data[this.bytePointer++] & 0xff);
                    this.nextBits += 8;
                    if (this.nextBits < this.bitsToGet)
                    {
                        this.nextData = (this.nextData << 8) | (this.data[this.bytePointer++] & 0xff);
                        this.nextBits += 8;
                    }
                    int num = (this.nextData >> (this.nextBits - this.bitsToGet)) & this.andTable[this.bitsToGet - 9];
                    this.nextBits -= this.bitsToGet;
                    return num;
                }
                catch
                {
                    return 0x101;
                }
            }
        }
    }
}

