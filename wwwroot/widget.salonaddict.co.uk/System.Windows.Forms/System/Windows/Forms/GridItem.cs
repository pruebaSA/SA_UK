namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public abstract class GridItem
    {
        private object userData;

        protected GridItem()
        {
        }

        public abstract bool Select();

        public virtual bool Expandable =>
            false;

        public virtual bool Expanded
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("GridItemNotExpandable"));
            }
        }

        public abstract GridItemCollection GridItems { get; }

        public abstract System.Windows.Forms.GridItemType GridItemType { get; }

        public abstract string Label { get; }

        public abstract GridItem Parent { get; }

        public abstract System.ComponentModel.PropertyDescriptor PropertyDescriptor { get; }

        [System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("ControlTagDescr"), Localizable(false), DefaultValue((string) null), TypeConverter(typeof(StringConverter)), Bindable(true)]
        public object Tag
        {
            get => 
                this.userData;
            set
            {
                this.userData = value;
            }
        }

        public abstract object Value { get; }
    }
}

