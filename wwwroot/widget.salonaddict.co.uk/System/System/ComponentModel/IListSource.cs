namespace System.ComponentModel
{
    using System;
    using System.Collections;

    [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Editor("System.Windows.Forms.Design.DataSourceListEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), MergableProperty(false)]
    public interface IListSource
    {
        IList GetList();

        bool ContainsListCollection { get; }
    }
}

