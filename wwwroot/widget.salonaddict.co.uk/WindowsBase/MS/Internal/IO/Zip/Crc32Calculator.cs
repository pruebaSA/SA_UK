namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;

    internal class Crc32Calculator
    {
        private static uint[] _Crc32Table;
        private static object _globalSync = new object();
        private const uint _InitialResidueValue = uint.MaxValue;
        private static byte[][] _maskingBitTable;
        private uint _residue = uint.MaxValue;

        static Crc32Calculator()
        {
            byte[][] bufferArray = new byte[0x20][];
            bufferArray[0] = new byte[] { 2 };
            byte[] buffer2 = new byte[2];
            buffer2[1] = 3;
            bufferArray[1] = buffer2;
            byte[] buffer3 = new byte[3];
            buffer3[1] = 1;
            buffer3[2] = 4;
            bufferArray[2] = buffer3;
            bufferArray[3] = new byte[] { 1, 2, 5 };
            bufferArray[4] = new byte[] { 0, 2, 3, 6 };
            bufferArray[5] = new byte[] { 1, 3, 4, 7 };
            bufferArray[6] = new byte[] { 4, 5 };
            byte[] buffer5 = new byte[3];
            buffer5[1] = 5;
            buffer5[2] = 6;
            bufferArray[7] = buffer5;
            bufferArray[8] = new byte[] { 1, 6, 7 };
            bufferArray[9] = new byte[] { 7 };
            bufferArray[10] = new byte[] { 2 };
            bufferArray[11] = new byte[] { 3 };
            byte[] buffer9 = new byte[2];
            buffer9[1] = 4;
            bufferArray[12] = buffer9;
            byte[] buffer10 = new byte[3];
            buffer10[1] = 1;
            buffer10[2] = 5;
            bufferArray[13] = buffer10;
            bufferArray[14] = new byte[] { 1, 2, 6 };
            bufferArray[15] = new byte[] { 2, 3, 7 };
            bufferArray[0x10] = new byte[] { 0, 2, 3, 4 };
            bufferArray[0x11] = new byte[] { 0, 1, 3, 4, 5 };
            bufferArray[0x12] = new byte[] { 0, 1, 2, 4, 5, 6 };
            bufferArray[0x13] = new byte[] { 1, 2, 3, 5, 6, 7 };
            bufferArray[20] = new byte[] { 3, 4, 6, 7 };
            bufferArray[0x15] = new byte[] { 2, 4, 5, 7 };
            bufferArray[0x16] = new byte[] { 2, 3, 5, 6 };
            bufferArray[0x17] = new byte[] { 3, 4, 6, 7 };
            bufferArray[0x18] = new byte[] { 0, 2, 4, 5, 7 };
            bufferArray[0x19] = new byte[] { 0, 1, 2, 3, 5, 6 };
            bufferArray[0x1a] = new byte[] { 0, 1, 2, 3, 4, 6, 7 };
            bufferArray[0x1b] = new byte[] { 1, 3, 4, 5, 7 };
            bufferArray[0x1c] = new byte[] { 0, 4, 5, 6 };
            bufferArray[0x1d] = new byte[] { 0, 1, 5, 6, 7 };
            bufferArray[30] = new byte[] { 0, 1, 6, 7 };
            bufferArray[0x1f] = new byte[] { 1, 7 };
            _maskingBitTable = bufferArray;
        }

        internal Crc32Calculator()
        {
            lock (_globalSync)
            {
                if (_Crc32Table == null)
                {
                    PrepareTable();
                }
            }
        }

        internal void Accumulate(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < (count + offset); i++)
            {
                this._residue = ((this._residue >> 8) & 0xffffff) ^ _Crc32Table[(int) ((IntPtr) ((this._residue ^ buffer[i]) & 0xff))];
            }
        }

        internal uint CalculateStreamCrc(Stream stream)
        {
            byte[] buffer = new byte[0x1000];
            while (true)
            {
                int count = stream.Read(buffer, 0, buffer.Length);
                if (count <= 0)
                {
                    break;
                }
                this.Accumulate(buffer, 0, count);
            }
            return this.Crc;
        }

        internal void ClearCrc()
        {
            this._residue = uint.MaxValue;
        }

        private static bool GetBit(byte bitOrdinal, uint data) => 
            (((data >> bitOrdinal) & 1) == 1);

        private static void PrepareTable()
        {
            _Crc32Table = new uint[0x100];
            for (uint i = 0; i < _Crc32Table.Length; i++)
            {
                for (byte j = 0; j < 0x20; j = (byte) (j + 1))
                {
                    bool flag = false;
                    foreach (byte num3 in _maskingBitTable[j])
                    {
                        flag ^= GetBit(num3, i);
                    }
                    SetBit(j, ref _Crc32Table[i], flag);
                }
            }
        }

        private static void SetBit(byte bitOrdinal, ref uint data, bool value)
        {
            if (value)
            {
                data |= ((int) 1) << bitOrdinal;
            }
        }

        internal uint Crc =>
            ~this._residue;
    }
}

