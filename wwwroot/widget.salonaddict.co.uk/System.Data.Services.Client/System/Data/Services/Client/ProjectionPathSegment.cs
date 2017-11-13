namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("Segment {ProjectionType} {Member}")]
    internal class ProjectionPathSegment
    {
        internal ProjectionPathSegment(ProjectionPath startPath, string member, Type projectionType)
        {
            this.Member = member;
            this.StartPath = startPath;
            this.ProjectionType = projectionType;
        }

        internal string Member { get; private set; }

        internal Type ProjectionType { get; set; }

        internal ProjectionPath StartPath { get; private set; }
    }
}

