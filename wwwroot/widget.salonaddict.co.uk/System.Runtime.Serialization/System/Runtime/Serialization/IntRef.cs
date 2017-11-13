namespace System.Runtime.Serialization
{
    using System;

    internal class IntRef
    {
        private int value;

        public IntRef(int value)
        {
            this.value = value;
        }

        public int Value =>
            this.value;
    }
}

