namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    [Serializable, DataContract(IsReference=true)]
    public abstract class ComplexObject : StructuralObject
    {
        private StructuralObject _parent;
        private string _parentPropertyName;

        protected ComplexObject()
        {
        }

        internal void AttachToParent(StructuralObject parent, string parentPropertyName)
        {
            if (this._parent != null)
            {
                throw EntityUtil.ComplexObjectAlreadyAttachedToParent();
            }
            this._parent = parent;
            this._parentPropertyName = parentPropertyName;
        }

        internal void DetachFromParent()
        {
            this._parent = null;
            this._parentPropertyName = null;
        }

        internal sealed override void ReportComplexPropertyChanged(string entityMemberName, ComplexObject complexObject, string complexMemberName)
        {
            if (this._parent != null)
            {
                this._parent.ReportComplexPropertyChanged(this._parentPropertyName, complexObject, complexMemberName);
            }
        }

        internal sealed override void ReportComplexPropertyChanging(string entityMemberName, ComplexObject complexObject, string complexMemberName)
        {
            if (this._parent != null)
            {
                this._parent.ReportComplexPropertyChanging(this._parentPropertyName, complexObject, complexMemberName);
            }
        }

        protected sealed override void ReportPropertyChanged(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            this.ReportComplexPropertyChanged(null, this, property);
            base.ReportPropertyChanged(property);
        }

        protected sealed override void ReportPropertyChanging(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            base.ReportPropertyChanging(property);
            this.ReportComplexPropertyChanging(null, this, property);
        }

        internal sealed override bool IsChangeTracked =>
            ((this._parent != null) && this._parent.IsChangeTracked);
    }
}

