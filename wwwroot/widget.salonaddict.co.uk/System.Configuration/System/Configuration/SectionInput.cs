﻿namespace System.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("SectionInput {_sectionXmlInfo.ConfigKey}")]
    internal class SectionInput
    {
        private List<ConfigurationException> _errors;
        private bool _isProtectionProviderDetermined;
        private ProtectedConfigurationProvider _protectionProvider;
        private object _result;
        private object _resultRuntimeObject;
        private System.Configuration.SectionXmlInfo _sectionXmlInfo;
        private static object s_unevaluated = new object();

        internal SectionInput(System.Configuration.SectionXmlInfo sectionXmlInfo, List<ConfigurationException> errors)
        {
            this._sectionXmlInfo = sectionXmlInfo;
            this._errors = errors;
            this._result = s_unevaluated;
            this._resultRuntimeObject = s_unevaluated;
        }

        internal void ClearResult()
        {
            this._result = s_unevaluated;
            this._resultRuntimeObject = s_unevaluated;
        }

        internal void ThrowOnErrors()
        {
            ErrorsHelper.ThrowOnErrors(this._errors);
        }

        internal ICollection<ConfigurationException> Errors =>
            this._errors;

        internal bool HasErrors =>
            ErrorsHelper.GetHasErrors(this._errors);

        internal bool HasResult =>
            (this._result != s_unevaluated);

        internal bool HasResultRuntimeObject =>
            (this._resultRuntimeObject != s_unevaluated);

        internal bool IsProtectionProviderDetermined =>
            this._isProtectionProviderDetermined;

        internal ProtectedConfigurationProvider ProtectionProvider
        {
            get => 
                this._protectionProvider;
            set
            {
                this._protectionProvider = value;
                this._isProtectionProviderDetermined = true;
            }
        }

        internal object Result
        {
            get => 
                this._result;
            set
            {
                this._result = value;
            }
        }

        internal object ResultRuntimeObject
        {
            get => 
                this._resultRuntimeObject;
            set
            {
                this._resultRuntimeObject = value;
            }
        }

        internal System.Configuration.SectionXmlInfo SectionXmlInfo =>
            this._sectionXmlInfo;
    }
}

