namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PersonalizationEntry
    {
        private bool _isSensitive;
        private PersonalizationScope _scope;
        private object _value;

        public PersonalizationEntry(object value, PersonalizationScope scope) : this(value, scope, false)
        {
        }

        public PersonalizationEntry(object value, PersonalizationScope scope, bool isSensitive)
        {
            PersonalizationProviderHelper.CheckPersonalizationScope(scope);
            this._value = value;
            this._scope = scope;
            this._isSensitive = isSensitive;
        }

        public bool IsSensitive
        {
            get => 
                this._isSensitive;
            set
            {
                this._isSensitive = value;
            }
        }

        public PersonalizationScope Scope
        {
            get => 
                this._scope;
            set
            {
                if ((value < PersonalizationScope.User) || (value > PersonalizationScope.Shared))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._scope = value;
            }
        }

        public object Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }
    }
}

