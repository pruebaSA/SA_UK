namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TagPrefixAttribute : Attribute
    {
        private string namespaceName;
        private string tagPrefix;

        public TagPrefixAttribute(string namespaceName, string tagPrefix)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("namespaceName");
            }
            if (string.IsNullOrEmpty(tagPrefix))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("tagPrefix");
            }
            this.namespaceName = namespaceName;
            this.tagPrefix = tagPrefix;
        }

        public string NamespaceName =>
            this.namespaceName;

        public string TagPrefix =>
            this.tagPrefix;
    }
}

