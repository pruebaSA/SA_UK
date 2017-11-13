namespace System.Web.UI.Design
{
    using System;
    using System.Globalization;

    internal static class DesignUtil
    {
        private const string DesignTimeHTML = "<div {0}{1}=\"0\"></div>";

        internal static string GetContainerDesignTimeHtml(bool renderInline) => 
            string.Format(CultureInfo.InvariantCulture, "<div {0}{1}=\"0\"></div>", new object[] { renderInline ? "style=\"display:inline\" " : string.Empty, DesignerRegion.DesignerRegionAttributeName });
    }
}

