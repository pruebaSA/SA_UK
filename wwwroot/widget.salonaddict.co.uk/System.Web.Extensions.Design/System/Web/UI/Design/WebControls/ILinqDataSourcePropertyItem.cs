namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourcePropertyItem : IComparable<ILinqDataSourcePropertyItem>
    {
        string DisplayName { get; set; }

        bool IsIdentity { get; }

        bool IsNullable { get; }

        bool IsPrimaryKey { get; }

        bool IsProperty { get; }

        bool IsReadOnly { get; }

        bool IsUnique { get; }

        int Length { get; }

        string Name { get; }

        System.Type Type { get; }
    }
}

