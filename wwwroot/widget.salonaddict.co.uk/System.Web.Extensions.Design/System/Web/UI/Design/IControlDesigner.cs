namespace System.Web.UI.Design
{
    using System;

    internal interface IControlDesigner
    {
        string CreatePlaceHolderDesignTimeHtml();
        void UpdateDesignTimeHtml();

        bool Visible { get; }
    }
}

