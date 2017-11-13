namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct ArrayWithOffset
    {
        private object m_array;
        private int m_offset;
        private int m_count;
        public ArrayWithOffset(object array, int offset)
        {
            this.m_array = array;
            this.m_offset = offset;
            this.m_count = 0;
            this.m_count = this.CalculateCount();
        }

        public object GetArray() => 
            this.m_array;

        public int GetOffset() => 
            this.m_offset;

        public override int GetHashCode() => 
            (this.m_count + this.m_offset);

        public override bool Equals(object obj) => 
            ((obj is ArrayWithOffset) && this.Equals((ArrayWithOffset) obj));

        public bool Equals(ArrayWithOffset obj) => 
            (((obj.m_array == this.m_array) && (obj.m_offset == this.m_offset)) && (obj.m_count == this.m_count));

        public static bool operator ==(ArrayWithOffset a, ArrayWithOffset b) => 
            a.Equals(b);

        public static bool operator !=(ArrayWithOffset a, ArrayWithOffset b) => 
            !(a == b);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern int CalculateCount();
    }
}

