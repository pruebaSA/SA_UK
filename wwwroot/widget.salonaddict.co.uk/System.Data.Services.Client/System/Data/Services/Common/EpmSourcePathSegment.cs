namespace System.Data.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class EpmSourcePathSegment
    {
        private string propertyName;
        private List<EpmSourcePathSegment> subProperties;

        internal EpmSourcePathSegment(string propertyName)
        {
            this.propertyName = propertyName;
            this.subProperties = new List<EpmSourcePathSegment>();
        }

        internal EntityPropertyMappingInfo EpmInfo { get; set; }

        internal string PropertyName =>
            this.propertyName;

        internal List<EpmSourcePathSegment> SubProperties =>
            this.subProperties;
    }
}

