namespace System.Data.Query.ResultAssembly
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential)]
    internal struct BitVec
    {
        private readonly int[] m_array;
        private readonly int m_length;
        internal BitVec(int length)
        {
            this.m_array = new int[(length + 0x1f) / 0x20];
            this.m_length = length;
        }

        internal int Count =>
            this.m_length;
        internal void Set(int index)
        {
            this.m_array[index / 0x20] |= ((int) 1) << (index % 0x20);
        }

        internal void ClearAll()
        {
            for (int i = 0; i < this.m_array.Length; i++)
            {
                this.m_array[i] = 0;
            }
        }

        internal bool IsEmpty()
        {
            for (int i = 0; i < this.m_array.Length; i++)
            {
                if (this.m_array[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        internal bool IsSet(int index) => 
            ((this.m_array[index / 0x20] & (((int) 1) << (index % 0x20))) != 0);

        internal void Or(BitVec value)
        {
            for (int i = 0; i < this.m_array.Length; i++)
            {
                this.m_array[i] |= value.m_array[i];
            }
        }

        internal void Minus(BitVec value)
        {
            for (int i = 0; i < this.m_array.Length; i++)
            {
                this.m_array[i] &= ~value.m_array[i];
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(3 * this.Count);
            string str = string.Empty;
            for (int i = 0; i < this.Count; i++)
            {
                if (this.IsSet(i))
                {
                    builder.Append(str);
                    builder.Append(i);
                    str = ",";
                }
            }
            return builder.ToString();
        }
    }
}

