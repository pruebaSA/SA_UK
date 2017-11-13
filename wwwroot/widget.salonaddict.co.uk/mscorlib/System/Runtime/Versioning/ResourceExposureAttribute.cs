namespace System.Runtime.Versioning
{
    using System;
    using System.Diagnostics;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, Inherited=false), Conditional("RESOURCE_ANNOTATION_WORK")]
    public sealed class ResourceExposureAttribute : Attribute
    {
        private ResourceScope _resourceExposureLevel;

        public ResourceExposureAttribute(ResourceScope exposureLevel)
        {
            this._resourceExposureLevel = exposureLevel;
        }

        public ResourceScope ResourceExposureLevel =>
            this._resourceExposureLevel;
    }
}

