namespace PdfSharp.Pdf.IO
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class ShiftStack
    {
        private List<PdfItem> items = new List<PdfItem>();
        private int sp;

        public int GetInteger(int relativeIndex)
        {
            if ((relativeIndex >= 0) || (-relativeIndex > this.sp))
            {
                throw new ArgumentOutOfRangeException("index", relativeIndex, "Value out of stack range.");
            }
            return ((PdfInteger) this.items[this.sp + relativeIndex]).Value;
        }

        public PdfItem GetItem(int relativeIndex)
        {
            if ((relativeIndex >= 0) || (-relativeIndex > this.sp))
            {
                throw new ArgumentOutOfRangeException("index", relativeIndex, "Value out of stack range.");
            }
            return this.items[this.sp + relativeIndex];
        }

        public void Reduce(int count)
        {
            if (count > this.sp)
            {
                throw new ArgumentException("count causes stack underflow.");
            }
            this.items.RemoveRange(this.sp - count, count);
            this.sp -= count;
        }

        public void Reduce(PdfItem item, int count)
        {
            this.Reduce(count);
            this.items.Add(item);
            this.sp++;
        }

        public void Shift(PdfItem item)
        {
            this.items.Add(item);
            this.sp++;
        }

        public PdfItem[] ToArray(int start, int length)
        {
            PdfItem[] itemArray = new PdfItem[length];
            int index = 0;
            for (int i = start; index < length; i++)
            {
                itemArray[index] = this.items[i];
                index++;
            }
            return itemArray;
        }

        public PdfItem this[int index]
        {
            get
            {
                if (index >= this.sp)
                {
                    throw new ArgumentOutOfRangeException("index", index, "Value greater than stack index.");
                }
                return this.items[index];
            }
        }

        public int SP =>
            this.sp;
    }
}

