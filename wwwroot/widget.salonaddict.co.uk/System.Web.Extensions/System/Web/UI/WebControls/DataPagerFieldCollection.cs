namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataPagerFieldCollection : StateManagedCollection
    {
        private DataPager _dataPager;
        private static readonly Type[] knownTypes = new Type[] { typeof(NextPreviousPagerField), typeof(NumericPagerField), typeof(TemplatePagerField) };

        public event EventHandler FieldsChanged;

        public DataPagerFieldCollection(DataPager dataPager)
        {
            this._dataPager = dataPager;
        }

        public void Add(DataPagerField field)
        {
            ((IList) this).Add(field);
        }

        public DataPagerFieldCollection CloneFields(DataPager pager)
        {
            DataPagerFieldCollection fields = new DataPagerFieldCollection(pager);
            foreach (DataPagerField field in this)
            {
                fields.Add(field.CloneField());
            }
            return fields;
        }

        public bool Contains(DataPagerField field) => 
            ((IList) this).Contains(field);

        public void CopyTo(DataPagerField[] array, int index)
        {
            this.CopyTo(array, index);
        }

        protected override object CreateKnownType(int index)
        {
            switch (index)
            {
                case 0:
                    return new NextPreviousPagerField();

                case 1:
                    return new NumericPagerField();

                case 2:
                    return new TemplatePagerField();
            }
            throw new ArgumentOutOfRangeException(AtlasWeb.PagerFieldCollection_InvalidTypeIndex);
        }

        protected override Type[] GetKnownTypes() => 
            knownTypes;

        public int IndexOf(DataPagerField field) => 
            ((IList) this).IndexOf(field);

        public void Insert(int index, DataPagerField field)
        {
            ((IList) this).Insert(index, field);
        }

        protected override void OnClearComplete()
        {
            this.OnFieldsChanged();
        }

        private void OnFieldChanged(object sender, EventArgs e)
        {
            this.OnFieldsChanged();
        }

        private void OnFieldsChanged()
        {
            if (this.FieldsChanged != null)
            {
                this.FieldsChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            DataPagerField field = value as DataPagerField;
            if (field != null)
            {
                field.FieldChanged += new EventHandler(this.OnFieldChanged);
            }
            field.SetDataPager(this._dataPager);
            this.OnFieldsChanged();
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            DataPagerField field = value as DataPagerField;
            if (field != null)
            {
                field.FieldChanged -= new EventHandler(this.OnFieldChanged);
            }
            this.OnFieldsChanged();
        }

        protected override void OnValidate(object o)
        {
            base.OnValidate(o);
            if (!(o is DataPagerField))
            {
                throw new ArgumentException(AtlasWeb.PagerFieldCollection_InvalidType);
            }
        }

        public void Remove(DataPagerField field)
        {
            ((IList) this).Remove(field);
        }

        public void RemoveAt(int index)
        {
            ((IList) this).RemoveAt(index);
        }

        protected override void SetDirtyObject(object o)
        {
            ((DataPagerField) o).SetDirty();
        }

        [Browsable(false)]
        public DataPagerField this[int index] =>
            (this[index] as DataPagerField);
    }
}

