namespace Microsoft.Transactions.Bridge
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StringCount
    {
        public string Name;
        public int Count;
        private static StringCount nullCount;
        public StringCount(string name)
        {
            this.Name = name;
            this.Count = 1;
        }

        public static StringCount Null =>
            nullCount;
        static StringCount()
        {
            nullCount = new StringCount(null);
        }
    }
}

