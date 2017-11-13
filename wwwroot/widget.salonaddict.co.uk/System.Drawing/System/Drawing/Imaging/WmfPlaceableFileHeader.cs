namespace System.Drawing.Imaging
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class WmfPlaceableFileHeader
    {
        private int key = -1698247209;
        private short hmf;
        private short bboxLeft;
        private short bboxTop;
        private short bboxRight;
        private short bboxBottom;
        private short inch;
        private int reserved;
        private short checksum;
        public int Key
        {
            get => 
                this.key;
            set
            {
                this.key = value;
            }
        }
        public short Hmf
        {
            get => 
                this.hmf;
            set
            {
                this.hmf = value;
            }
        }
        public short BboxLeft
        {
            get => 
                this.bboxLeft;
            set
            {
                this.bboxLeft = value;
            }
        }
        public short BboxTop
        {
            get => 
                this.bboxTop;
            set
            {
                this.bboxTop = value;
            }
        }
        public short BboxRight
        {
            get => 
                this.bboxRight;
            set
            {
                this.bboxRight = value;
            }
        }
        public short BboxBottom
        {
            get => 
                this.bboxBottom;
            set
            {
                this.bboxBottom = value;
            }
        }
        public short Inch
        {
            get => 
                this.inch;
            set
            {
                this.inch = value;
            }
        }
        public int Reserved
        {
            get => 
                this.reserved;
            set
            {
                this.reserved = value;
            }
        }
        public short Checksum
        {
            get => 
                this.checksum;
            set
            {
                this.checksum = value;
            }
        }
    }
}

