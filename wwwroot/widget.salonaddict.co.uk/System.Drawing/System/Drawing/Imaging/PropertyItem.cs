namespace System.Drawing.Imaging
{
    using System;

    public sealed class PropertyItem
    {
        private int id;
        private int len;
        private short type;
        private byte[] value;

        internal PropertyItem()
        {
        }

        public int Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        public int Len
        {
            get => 
                this.len;
            set
            {
                this.len = value;
            }
        }

        public short Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }

        public byte[] Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

