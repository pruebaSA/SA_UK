namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal class DomainConstraint<T_Variable, T_Element>
    {
        private readonly int _hashCode;
        private readonly Set<T_Element> _range;
        private readonly DomainVariable<T_Variable, T_Element> _variable;

        internal DomainConstraint(DomainVariable<T_Variable, T_Element> variable, Set<T_Element> range)
        {
            this._variable = variable;
            this._range = range.AsReadOnly();
            this._hashCode = this._variable.GetHashCode() ^ this._range.GetElementsHashCode();
        }

        internal DomainConstraint(DomainVariable<T_Variable, T_Element> variable, T_Element element) : this(variable, new Set<T_Element>(new T_Element[] { element }).MakeReadOnly())
        {
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            DomainConstraint<T_Variable, T_Element> constraint = obj as DomainConstraint<T_Variable, T_Element>;
            if (constraint == null)
            {
                return false;
            }
            if (this._hashCode != constraint._hashCode)
            {
                return false;
            }
            return (this._range.SetEquals(constraint._range) && this._variable.Equals(constraint._variable));
        }

        public override int GetHashCode() => 
            this._hashCode;

        internal DomainConstraint<T_Variable, T_Element> InvertDomainConstraint() => 
            new DomainConstraint<T_Variable, T_Element>(this._variable, this._variable.Domain.Difference(this._range).AsReadOnly());

        public override string ToString() => 
            StringUtil.FormatInvariant("{0} in [{1}]", new object[] { this._variable, this._range });

        internal Set<T_Element> Range =>
            this._range;

        internal DomainVariable<T_Variable, T_Element> Variable =>
            this._variable;
    }
}

