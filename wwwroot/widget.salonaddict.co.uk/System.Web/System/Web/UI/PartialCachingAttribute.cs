namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PartialCachingAttribute : Attribute
    {
        private int _duration;
        private bool _shared;
        private string _sqlDependency;
        private string _varyByControls;
        private string _varyByCustom;
        private string _varyByParams;

        public PartialCachingAttribute(int duration)
        {
            this._duration = duration;
        }

        public PartialCachingAttribute(int duration, string varyByParams, string varyByControls, string varyByCustom)
        {
            this._duration = duration;
            this._varyByParams = varyByParams;
            this._varyByControls = varyByControls;
            this._varyByCustom = varyByCustom;
        }

        public PartialCachingAttribute(int duration, string varyByParams, string varyByControls, string varyByCustom, bool shared)
        {
            this._duration = duration;
            this._varyByParams = varyByParams;
            this._varyByControls = varyByControls;
            this._varyByCustom = varyByCustom;
            this._shared = shared;
        }

        public PartialCachingAttribute(int duration, string varyByParams, string varyByControls, string varyByCustom, string sqlDependency, bool shared)
        {
            this._duration = duration;
            this._varyByParams = varyByParams;
            this._varyByControls = varyByControls;
            this._varyByCustom = varyByCustom;
            this._shared = shared;
            this._sqlDependency = sqlDependency;
        }

        public int Duration =>
            this._duration;

        public bool Shared =>
            this._shared;

        public string SqlDependency =>
            this._sqlDependency;

        public string VaryByControls =>
            this._varyByControls;

        public string VaryByCustom =>
            this._varyByCustom;

        public string VaryByParams =>
            this._varyByParams;
    }
}

