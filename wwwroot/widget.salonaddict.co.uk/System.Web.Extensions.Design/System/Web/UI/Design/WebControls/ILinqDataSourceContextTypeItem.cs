namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourceContextTypeItem : IComparable<ILinqDataSourceContextTypeItem>
    {
        string DisplayName { get; set; }

        System.Type Type { get; }
    }
}

