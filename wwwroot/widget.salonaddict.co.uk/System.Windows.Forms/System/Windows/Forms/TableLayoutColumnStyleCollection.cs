﻿namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Windows.Forms.Layout;

    public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
    {
        internal TableLayoutColumnStyleCollection() : base(null)
        {
        }

        internal TableLayoutColumnStyleCollection(IArrangedElement Owner) : base(Owner)
        {
        }

        public int Add(ColumnStyle columnStyle) => 
            ((IList) this).Add(columnStyle);

        public bool Contains(ColumnStyle columnStyle) => 
            ((IList) this).Contains(columnStyle);

        public int IndexOf(ColumnStyle columnStyle) => 
            ((IList) this).IndexOf(columnStyle);

        public void Insert(int index, ColumnStyle columnStyle)
        {
            ((IList) this).Insert(index, columnStyle);
        }

        public void Remove(ColumnStyle columnStyle)
        {
            ((IList) this).Remove(columnStyle);
        }

        public ColumnStyle this[int index]
        {
            get => 
                ((ColumnStyle) this[index]);
            set
            {
                this[index] = value;
            }
        }

        internal override string PropertyName =>
            PropertyNames.ColumnStyles;
    }
}

