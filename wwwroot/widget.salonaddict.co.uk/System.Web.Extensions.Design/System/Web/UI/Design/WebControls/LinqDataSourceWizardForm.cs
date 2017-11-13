namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceWizardForm : WizardForm, ILinqDataSourceWizardForm
    {
        private ILinqDataSourceChooseContextType _chooseContext;
        private ILinqDataSourceConfiguration _configure;
        private System.Web.UI.Design.WebControls.LinqDataSourceState _linqDataSourceState;

        public LinqDataSourceWizardForm(IServiceProvider serviceProvider, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState, LinqDataSourceDesigner linqDataSourceDesigner) : base(serviceProvider)
        {
            this._linqDataSourceState = linqDataSourceState;
            base.Glyph = GetBitmap("System.Web.Resources.Design.LinqDataSourceWizard.bmp");
            this.Text = string.Format(CultureInfo.InvariantCulture, AtlasWebDesign.LinqDataSourceWizardForm_Caption, new object[] { ((LinqDataSource) linqDataSourceDesigner.Component).ID });
            LinqDataSourceChooseContextTypePanel panel = new LinqDataSourceChooseContextTypePanel();
            this._chooseContext = new LinqDataSourceChooseContextType(panel, this, linqDataSourceDesigner.Helper, serviceProvider, this._linqDataSourceState);
            LinqDataSourceConfigurationPanel panel2 = new LinqDataSourceConfigurationPanel();
            this._configure = new LinqDataSourceConfiguration(panel2, this, linqDataSourceDesigner.Helper, (Control) linqDataSourceDesigner.Component, this._linqDataSourceState, serviceProvider);
            this._chooseContext.ContextChanged += new LinqDataSourceContextChangedEventHandler(this._configure.ContextChangedHandler);
            this._chooseContext.LoadState();
            this._configure.LoadState();
            base.SetPanels(new WizardPanel[] { panel, panel2 });
            base.FinishButton.Enabled = true;
        }

        internal static Bitmap GetBitmap(string resourceName)
        {
            Bitmap bitmap = new Bitmap(typeof(LinqDataSourceWizardForm).Assembly.GetManifestResourceStream(resourceName));
            bitmap.MakeTransparent();
            return bitmap;
        }

        protected override void OnFinishButtonClick(object sender, EventArgs e)
        {
            this._chooseContext.SaveState();
            this._configure.SaveState();
            base.OnFinishButtonClick(sender, e);
        }

        public void SetCanFinish(bool enabled)
        {
            base.FinishButton.Enabled = enabled;
        }

        public void SetCanNext(bool enabled)
        {
            base.NextButton.Enabled = enabled;
        }

        protected override string HelpTopic =>
            "net.Asp.LinqDataSource.WizardDialog";

        public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState =>
            this._linqDataSourceState;
    }
}

