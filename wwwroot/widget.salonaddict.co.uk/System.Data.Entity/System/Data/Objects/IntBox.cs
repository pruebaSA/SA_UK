namespace System.Data.Objects
{
    using System;

    internal sealed class IntBox
    {
        private int val;

        internal IntBox(int val)
        {
            this.val = val;
        }

        internal int Value
        {
            get => 
                this.val;
            set
            {
                this.val = value;
            }
        }
    }
}

