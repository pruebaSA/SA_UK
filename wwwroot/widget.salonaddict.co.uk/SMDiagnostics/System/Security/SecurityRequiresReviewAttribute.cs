namespace System.Security
{
    using System;
    using System.Diagnostics;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=false), Conditional("DEBUG")]
    internal sealed class SecurityRequiresReviewAttribute : Attribute
    {
        private SecurityCriticalScope scope;

        public SecurityRequiresReviewAttribute()
        {
        }

        public SecurityRequiresReviewAttribute(SecurityCriticalScope scope)
        {
        }

        public SecurityCriticalScope Scope
        {
            get => 
                this.scope;
            set
            {
                this.scope = value;
            }
        }
    }
}

