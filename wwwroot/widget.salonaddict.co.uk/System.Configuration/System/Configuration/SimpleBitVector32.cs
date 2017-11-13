namespace System.Configuration
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    internal struct SimpleBitVector32
    {
        private int data;
        internal SimpleBitVector32(int data)
        {
            this.data = data;
        }

        internal int Data =>
            this.data;
        internal bool this[int bit]
        {
            get => 
                ((this.data & bit) == bit);
            set
            {
                int data = this.data;
                if (value)
                {
                    this.data = data | bit;
                }
                else
                {
                    this.data = data & ~bit;
                }
            }
        }
    }
}

