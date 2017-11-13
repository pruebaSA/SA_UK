namespace System.Data.Services.Common
{
    using System;
    using System.Globalization;

    internal sealed class EpmAttributeNameBuilder
    {
        private int index;
        private string postFix = string.Empty;

        internal EpmAttributeNameBuilder()
        {
        }

        internal void MoveNext()
        {
            this.index++;
            this.postFix = "_" + this.index.ToString(CultureInfo.InvariantCulture);
        }

        internal string EpmContentKind =>
            ("FC_ContentKind" + this.postFix);

        internal string EpmKeepInContent =>
            ("FC_KeepInContent" + this.postFix);

        internal string EpmNsPrefix =>
            ("FC_NsPrefix" + this.postFix);

        internal string EpmNsUri =>
            ("FC_NsUri" + this.postFix);

        internal string EpmSourcePath =>
            ("FC_SourcePath" + this.postFix);

        internal string EpmTargetPath =>
            ("FC_TargetPath" + this.postFix);
    }
}

