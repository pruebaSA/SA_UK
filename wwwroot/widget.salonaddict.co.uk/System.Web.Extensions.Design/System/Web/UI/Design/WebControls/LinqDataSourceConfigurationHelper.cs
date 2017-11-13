namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Web.UI;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal class LinqDataSourceConfigurationHelper
    {
        private ILinqDataSourceDesignerHelper _designerHelper;
        private System.Web.UI.Control _linqDataSource;
        private LinqDataSourceState _linqDataSourceState;
        private IServiceProvider _serviceProvider;

        internal LinqDataSourceConfigurationHelper()
        {
        }

        public LinqDataSourceConfigurationHelper(ILinqDataSourceDesignerHelper designerHelper, System.Web.UI.Control linqDataSource, LinqDataSourceState linqDataSourceState, IServiceProvider serviceProvider)
        {
            this._designerHelper = designerHelper;
            this._linqDataSource = linqDataSource;
            this._serviceProvider = serviceProvider;
            this._linqDataSourceState = linqDataSourceState;
        }

        public virtual ILinqDataSourceConfigureAdvanced MakeAdvanced(ILinqDataSourceConfigureAdvancedForm advancedForm) => 
            new LinqDataSourceConfigureAdvanced(advancedForm, this._linqDataSourceState);

        public virtual ILinqDataSourceConfigureAdvancedForm MakeAdvancedForm() => 
            new LinqDataSourceConfigureAdvancedForm(this._serviceProvider);

        public virtual ILinqDataSourceConfigureOrderBy MakeOrderBy(ILinqDataSourceConfigureOrderByForm orderByForm) => 
            new LinqDataSourceConfigureOrderBy(orderByForm, this._designerHelper, this._linqDataSourceState);

        public virtual ILinqDataSourceConfigureOrderByForm MakeOrderByForm() => 
            new LinqDataSourceConfigureOrderByForm(this._serviceProvider);

        public virtual ILinqDataSourceStatementEditorForm MakeStatementEditorForm(bool autoGen, string statement, ParameterCollection originalParameters, string operation) => 
            new LinqDataSourceStatementEditorForm(this._linqDataSource, this._serviceProvider, true, false, autoGen, statement, this._designerHelper.CloneParameters(originalParameters), operation);

        public virtual ILinqDataSourceConfigureWhere MakeWhere(ILinqDataSourceConfigureWhereForm whereForm) => 
            new LinqDataSourceConfigureWhere(whereForm, this._designerHelper, this._linqDataSource, this._serviceProvider, this._linqDataSourceState);

        public virtual ILinqDataSourceConfigureWhereForm MakeWhereForm() => 
            new LinqDataSourceConfigureWhereForm(this._serviceProvider);

        public virtual DialogResult ShowDialog(object form) => 
            UIServiceHelper.ShowDialog(this._serviceProvider, (Form) form);
    }
}

