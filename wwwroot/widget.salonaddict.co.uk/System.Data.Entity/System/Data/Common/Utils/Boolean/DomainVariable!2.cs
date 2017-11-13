namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;

    internal class DomainVariable<T_Variable, T_Element>
    {
        private readonly Set<T_Element> _domain;
        private readonly int _hashCode;
        private readonly T_Variable _identifier;
        private readonly IEqualityComparer<T_Variable> _identifierComparer;

        internal DomainVariable(T_Variable identifier, Set<T_Element> domain) : this(identifier, domain, null)
        {
        }

        internal DomainVariable(T_Variable identifier, Set<T_Element> domain, IEqualityComparer<T_Variable> identifierComparer)
        {
            this._identifier = identifier;
            this._domain = domain.AsReadOnly();
            this._identifierComparer = identifierComparer ?? EqualityComparer<T_Variable>.Default;
            int elementsHashCode = this._domain.GetElementsHashCode();
            int hashCode = this._identifierComparer.GetHashCode(this._identifier);
            this._hashCode = elementsHashCode ^ hashCode;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            DomainVariable<T_Variable, T_Element> variable = obj as DomainVariable<T_Variable, T_Element>;
            if (variable == null)
            {
                return false;
            }
            if (this._hashCode != variable._hashCode)
            {
                return false;
            }
            return (this._identifierComparer.Equals(this._identifier, variable._identifier) && this._domain.SetEquals(variable._domain));
        }

        public override int GetHashCode() => 
            this._hashCode;

        public override string ToString() => 
            StringUtil.FormatInvariant("{0}{{{1}}}", new object[] { this._identifier.ToString(), this._domain });

        internal Set<T_Element> Domain =>
            this._domain;

        internal T_Variable Identifier =>
            this._identifier;
    }
}

