namespace System.Net.NetworkInformation
{
    using System;
    using System.Diagnostics;

    internal class IcmpPacket
    {
        internal byte[] buffer;
        internal ushort checkSum;
        internal static ushort identifier;
        internal ushort sequenceNumber;
        private static ushort staticSequenceNumber;
        internal byte subCode;
        internal byte type = 8;

        internal IcmpPacket(byte[] buffer)
        {
            this.buffer = buffer;
            staticSequenceNumber = (ushort) (staticSequenceNumber + 1);
            this.sequenceNumber = staticSequenceNumber;
            this.checkSum = (ushort) this.GetCheckSum();
        }

        internal byte[] GetBytes()
        {
            byte[] destinationArray = new byte[this.buffer.Length + 8];
            byte[] bytes = BitConverter.GetBytes(this.checkSum);
            byte[] sourceArray = BitConverter.GetBytes(this.Identifier);
            byte[] buffer4 = BitConverter.GetBytes(this.sequenceNumber);
            destinationArray[0] = this.type;
            destinationArray[1] = this.subCode;
            Array.Copy(bytes, 0, destinationArray, 2, 2);
            Array.Copy(sourceArray, 0, destinationArray, 4, 2);
            Array.Copy(buffer4, 0, destinationArray, 6, 2);
            Array.Copy(this.buffer, 0, destinationArray, 8, this.buffer.Length);
            return destinationArray;
        }

        private uint GetCheckSum()
        {
            uint num = (uint) ((this.type + this.Identifier) + this.sequenceNumber);
            for (int i = 0; i < this.buffer.Length; i++)
            {
                num += (uint) (this.buffer[i] + (this.buffer[++i] << 8));
            }
            num = (num >> 0x10) + (num & 0xffff);
            num += num >> 0x10;
            return ~num;
        }

        internal ushort Identifier
        {
            get
            {
                if (identifier == 0)
                {
                    identifier = (ushort) Process.GetCurrentProcess().Id;
                }
                return identifier;
            }
        }
    }
}

