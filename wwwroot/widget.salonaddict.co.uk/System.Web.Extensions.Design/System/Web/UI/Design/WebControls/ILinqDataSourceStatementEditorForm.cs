namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal interface ILinqDataSourceStatementEditorForm
    {
        DialogResult ShowDialog();

        bool AutoGen { get; }

        ParameterCollection Parameters { get; }

        string Statement { get; }
    }
}

