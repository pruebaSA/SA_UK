namespace System.Web.UI
{
    using System;
    using System.Web.Configuration;

    internal sealed class CustomErrorsSectionWrapper : ICustomErrorsSection
    {
        private readonly CustomErrorsSection _customErrorsSection;

        public CustomErrorsSectionWrapper(CustomErrorsSection customErrorsSection)
        {
            this._customErrorsSection = customErrorsSection;
        }

        string ICustomErrorsSection.DefaultRedirect =>
            this._customErrorsSection.DefaultRedirect;

        CustomErrorCollection ICustomErrorsSection.Errors =>
            this._customErrorsSection.Errors;
    }
}

