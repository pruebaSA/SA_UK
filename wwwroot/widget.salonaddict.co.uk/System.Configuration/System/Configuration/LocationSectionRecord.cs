namespace System.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("LocationSectionRecord {ConfigKey}")]
    internal class LocationSectionRecord
    {
        private List<ConfigurationException> _errors;
        private System.Configuration.SectionXmlInfo _sectionXmlInfo;

        internal LocationSectionRecord(System.Configuration.SectionXmlInfo sectionXmlInfo, List<ConfigurationException> errors)
        {
            this._sectionXmlInfo = sectionXmlInfo;
            this._errors = errors;
        }

        internal void AddError(ConfigurationException e)
        {
            ErrorsHelper.AddError(ref this._errors, e);
        }

        internal string ConfigKey =>
            this._sectionXmlInfo.ConfigKey;

        internal ICollection<ConfigurationException> Errors =>
            this._errors;

        internal List<ConfigurationException> ErrorsList =>
            this._errors;

        internal bool HasErrors =>
            ErrorsHelper.GetHasErrors(this._errors);

        internal System.Configuration.SectionXmlInfo SectionXmlInfo =>
            this._sectionXmlInfo;
    }
}

