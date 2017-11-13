namespace System.Data
{
    using System;
    using System.ComponentModel;

    internal sealed class DataRelationPropertyDescriptor : PropertyDescriptor
    {
        private DataRelation relation;

        internal DataRelationPropertyDescriptor(DataRelation dataRelation) : base(dataRelation.RelationName, null)
        {
            this.relation = dataRelation;
        }

        public override bool CanResetValue(object component) => 
            false;

        public override bool Equals(object other)
        {
            if (other is DataRelationPropertyDescriptor)
            {
                DataRelationPropertyDescriptor descriptor = (DataRelationPropertyDescriptor) other;
                return (descriptor.Relation == this.Relation);
            }
            return false;
        }

        public override int GetHashCode() => 
            this.Relation.GetHashCode();

        public override object GetValue(object component)
        {
            DataRowView view = (DataRowView) component;
            return view.CreateChildView(this.relation);
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component) => 
            false;

        public override Type ComponentType =>
            typeof(DataRowView);

        public override bool IsReadOnly =>
            false;

        public override Type PropertyType =>
            typeof(IBindingList);

        internal DataRelation Relation =>
            this.relation;
    }
}

