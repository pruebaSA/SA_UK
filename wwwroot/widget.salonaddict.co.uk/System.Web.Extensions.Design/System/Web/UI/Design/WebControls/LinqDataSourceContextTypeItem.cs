namespace System.Web.UI.Design.WebControls
{
    using System;

    internal class LinqDataSourceContextTypeItem : ILinqDataSourceContextTypeItem, IComparable<ILinqDataSourceContextTypeItem>
    {
        private string _displayName;
        private Type _type;

        public LinqDataSourceContextTypeItem(Type type)
        {
            this._type = type;
            if (type != null)
            {
                this._displayName = type.FullName;
            }
        }

        int IComparable<ILinqDataSourceContextTypeItem>.CompareTo(ILinqDataSourceContextTypeItem other) => 
            string.Compare(this.ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);

        public override string ToString() => 
            this._displayName;

        string ILinqDataSourceContextTypeItem.DisplayName
        {
            get => 
                this._displayName;
            set
            {
                this._displayName = value;
            }
        }

        Type ILinqDataSourceContextTypeItem.Type =>
            this._type;
    }
}

