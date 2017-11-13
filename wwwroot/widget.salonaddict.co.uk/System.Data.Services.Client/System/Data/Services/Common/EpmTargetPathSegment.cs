namespace System.Data.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("EpmTargetPathSegment {SegmentName} HasContent={HasContent}")]
    internal class EpmTargetPathSegment
    {
        private EpmTargetPathSegment parentSegment;
        private string segmentName;
        private string segmentNamespacePrefix;
        private string segmentNamespaceUri;
        private List<EpmTargetPathSegment> subSegments;

        internal EpmTargetPathSegment()
        {
            this.subSegments = new List<EpmTargetPathSegment>();
        }

        internal EpmTargetPathSegment(string segmentName, string segmentNamespaceUri, string segmentNamespacePrefix, EpmTargetPathSegment parentSegment) : this()
        {
            this.segmentName = segmentName;
            this.segmentNamespaceUri = segmentNamespaceUri;
            this.segmentNamespacePrefix = segmentNamespacePrefix;
            this.parentSegment = parentSegment;
        }

        internal EntityPropertyMappingInfo EpmInfo { get; set; }

        internal bool HasContent =>
            (this.EpmInfo != null);

        internal bool IsAttribute =>
            (this.SegmentName[0] == '@');

        internal EpmTargetPathSegment ParentSegment =>
            this.parentSegment;

        internal string SegmentName =>
            this.segmentName;

        internal string SegmentNamespacePrefix =>
            this.segmentNamespacePrefix;

        internal string SegmentNamespaceUri =>
            this.segmentNamespaceUri;

        internal List<EpmTargetPathSegment> SubSegments =>
            this.subSegments;
    }
}

