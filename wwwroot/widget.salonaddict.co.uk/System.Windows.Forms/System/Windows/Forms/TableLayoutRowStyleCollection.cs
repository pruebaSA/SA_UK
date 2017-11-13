namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Windows.Forms.Layout;

    public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
    {
        internal TableLayoutRowStyleCollection() : base(null)
        {
        }

        internal TableLayoutRowStyleCollection(IArrangedElement Owner) : base(Owner)
        {
        }

        public int Add(RowStyle rowStyle) => 
            ((IList) this).Add(rowStyle);

        public bool Contains(RowStyle rowStyle) => 
            ((IList) this).Contains(rowStyle);

        public int IndexOf(RowStyle rowStyle) => 
            ((IList) this).IndexOf(rowStyle);

        public void Insert(int index, RowStyle rowStyle)
        {
            ((IList) this).Insert(index, rowStyle);
        }

        public void Remove(RowStyle rowStyle)
        {
            ((IList) this).Remove(rowStyle);
        }

        public RowStyle this[int index]
        {
            get => 
                ((RowStyle) this[index]);
            set
            {
                this[index] = value;
            }
        }

        internal override string PropertyName =>
            PropertyNames.RowStyles;
    }
}

