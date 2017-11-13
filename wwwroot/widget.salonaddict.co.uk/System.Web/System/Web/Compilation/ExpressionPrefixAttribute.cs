namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ExpressionPrefixAttribute : Attribute
    {
        private string _expressionPrefix;

        public ExpressionPrefixAttribute(string expressionPrefix)
        {
            if (string.IsNullOrEmpty(expressionPrefix))
            {
                throw new ArgumentNullException("expressionPrefix");
            }
            this._expressionPrefix = expressionPrefix;
        }

        public string ExpressionPrefix =>
            this._expressionPrefix;
    }
}

