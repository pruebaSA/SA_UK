namespace System.Windows.Forms
{
    using System;

    internal class DataGridViewIntLinkedListElement
    {
        private int integer;
        private DataGridViewIntLinkedListElement next;

        public DataGridViewIntLinkedListElement(int integer)
        {
            this.integer = integer;
        }

        public int Int
        {
            get => 
                this.integer;
            set
            {
                this.integer = value;
            }
        }

        public DataGridViewIntLinkedListElement Next
        {
            get => 
                this.next;
            set
            {
                this.next = value;
            }
        }
    }
}

