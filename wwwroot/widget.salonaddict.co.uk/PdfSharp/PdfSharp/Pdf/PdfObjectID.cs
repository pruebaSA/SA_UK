namespace PdfSharp.Pdf
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), DebuggerDisplay("({ObjectNumber}, {GenerationNumber})")]
    public struct PdfObjectID : IComparable
    {
        private int objectNumber;
        private ushort generationNumber;
        public PdfObjectID(int objectNumber)
        {
            this.objectNumber = objectNumber;
            this.generationNumber = 0;
        }

        public PdfObjectID(int objectNumber, int generationNumber)
        {
            this.objectNumber = objectNumber;
            this.generationNumber = (ushort) generationNumber;
        }

        public int ObjectNumber
        {
            get => 
                this.objectNumber;
            set
            {
                this.objectNumber = value;
            }
        }
        public int GenerationNumber
        {
            get => 
                this.generationNumber;
            set
            {
                this.generationNumber = (ushort) value;
            }
        }
        public bool IsEmpty =>
            (this.objectNumber == 0);
        public override bool Equals(object obj)
        {
            if (obj is PdfObjectID)
            {
                PdfObjectID tid = (PdfObjectID) obj;
                if (this.objectNumber == tid.objectNumber)
                {
                    return (this.generationNumber == tid.generationNumber);
                }
            }
            return false;
        }

        public override int GetHashCode() => 
            (this.objectNumber ^ this.generationNumber);

        public static bool operator ==(PdfObjectID left, PdfObjectID right) => 
            left.Equals(right);

        public static bool operator !=(PdfObjectID left, PdfObjectID right) => 
            !left.Equals(right);

        public override string ToString() => 
            (this.objectNumber.ToString(CultureInfo.InvariantCulture) + " " + this.generationNumber.ToString(CultureInfo.InvariantCulture));

        public static PdfObjectID Empty =>
            new PdfObjectID();
        public int CompareTo(object obj)
        {
            if (!(obj is PdfObjectID))
            {
                return 1;
            }
            PdfObjectID tid = (PdfObjectID) obj;
            if (this.objectNumber == tid.objectNumber)
            {
                return (this.generationNumber - tid.generationNumber);
            }
            return (this.objectNumber - tid.objectNumber);
        }
    }
}

