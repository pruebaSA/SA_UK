namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TargetControlTypeAttribute : Attribute
    {
        private Type _targetControlType;

        public TargetControlTypeAttribute(Type targetControlType)
        {
            if (targetControlType == null)
            {
                throw new ArgumentNullException("targetControlType");
            }
            this._targetControlType = targetControlType;
        }

        public Type TargetControlType =>
            this._targetControlType;

        public override object TypeId =>
            this._targetControlType;
    }
}

