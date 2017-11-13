namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class SiteMapPathAutoFormat : DesignerAutoFormat
    {
        private Color _currentNodeStyleForeColor;
        private string _fontName;
        private FontUnit _fontSize;
        private bool _nodeStyleFontBold;
        private Color _nodeStyleForeColor;
        private string _pathSeparator;
        private bool _pathSeparatorStyleFontBold;
        private Color _pathSeparatorStyleForeColor;
        private bool _rootNodeStyleFontBold;
        private Color _rootNodeStyleForeColor;

        public SiteMapPathAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.Load(schemeData);
            base.Style.Width = 400;
            base.Style.Height = 100;
        }

        public override void Apply(Control control)
        {
            if (control is SiteMapPath)
            {
                this.Apply(control as SiteMapPath);
            }
        }

        private void Apply(SiteMapPath siteMapPath)
        {
            siteMapPath.Font.Name = this._fontName;
            siteMapPath.Font.Size = this._fontSize;
            siteMapPath.Font.ClearDefaults();
            siteMapPath.NodeStyle.Font.Bold = this._nodeStyleFontBold;
            siteMapPath.NodeStyle.ForeColor = this._nodeStyleForeColor;
            siteMapPath.NodeStyle.Font.ClearDefaults();
            siteMapPath.RootNodeStyle.Font.Bold = this._rootNodeStyleFontBold;
            siteMapPath.RootNodeStyle.ForeColor = this._rootNodeStyleForeColor;
            siteMapPath.RootNodeStyle.Font.ClearDefaults();
            siteMapPath.CurrentNodeStyle.ForeColor = this._currentNodeStyleForeColor;
            siteMapPath.PathSeparatorStyle.Font.Bold = this._pathSeparatorStyleFontBold;
            siteMapPath.PathSeparatorStyle.ForeColor = this._pathSeparatorStyleForeColor;
            siteMapPath.PathSeparatorStyle.Font.ClearDefaults();
            if ((this._pathSeparator != null) && (this._pathSeparator.Length == 0))
            {
                this._pathSeparator = null;
            }
            siteMapPath.PathSeparator = this._pathSeparator;
        }

        private bool GetBoolProperty(string propertyTag, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            return (((obj2 != null) && !obj2.Equals(DBNull.Value)) && bool.Parse(obj2.ToString()));
        }

        private string GetStringProperty(string propertyTag, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            if ((obj2 != null) && !obj2.Equals(DBNull.Value))
            {
                return obj2.ToString();
            }
            return string.Empty;
        }

        private void Load(DataRow schemeData)
        {
            if (schemeData != null)
            {
                this._fontName = this.GetStringProperty("FontName", schemeData);
                this._fontSize = new FontUnit(this.GetStringProperty("FontSize", schemeData), CultureInfo.InvariantCulture);
                this._pathSeparator = this.GetStringProperty("PathSeparator", schemeData);
                this._nodeStyleFontBold = this.GetBoolProperty("NodeStyleFontBold", schemeData);
                this._nodeStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("NodeStyleForeColor", schemeData));
                this._rootNodeStyleFontBold = this.GetBoolProperty("RootNodeStyleFontBold", schemeData);
                this._rootNodeStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("RootNodeStyleForeColor", schemeData));
                this._currentNodeStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("CurrentNodeStyleForeColor", schemeData));
                this._pathSeparatorStyleFontBold = this.GetBoolProperty("PathSeparatorStyleFontBold", schemeData);
                this._pathSeparatorStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("PathSeparatorStyleForeColor", schemeData));
            }
        }
    }
}

