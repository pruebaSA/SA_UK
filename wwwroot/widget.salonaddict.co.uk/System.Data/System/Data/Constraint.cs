namespace System.Data
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Globalization;

    [TypeConverter(typeof(ConstraintConverter)), DefaultProperty("ConstraintName")]
    public abstract class Constraint
    {
        private string _schemaName = "";
        private DataSet dataSet;
        internal PropertyCollection extendedProperties;
        private bool inCollection;
        internal string name = "";

        protected Constraint()
        {
        }

        internal abstract bool CanBeRemovedFromCollection(ConstraintCollection constraint, bool fThrowException);
        internal abstract bool CanEnableConstraint();
        internal abstract void CheckCanAddToCollection(ConstraintCollection constraint);
        internal void CheckConstraint()
        {
            if (!this.CanEnableConstraint())
            {
                throw ExceptionBuilder.ConstraintViolation(this.ConstraintName);
            }
        }

        internal abstract void CheckConstraint(DataRow row, DataRowAction action);
        internal abstract void CheckState();
        protected void CheckStateForProperty()
        {
            try
            {
                this.CheckState();
            }
            catch (Exception exception)
            {
                if (!ADP.IsCatchableExceptionType(exception))
                {
                    throw;
                }
                throw ExceptionBuilder.BadObjectPropertyAccess(exception.Message);
            }
        }

        internal abstract Constraint Clone(DataSet destination);
        internal abstract Constraint Clone(DataSet destination, bool ignoreNSforTableLookup);
        internal abstract bool ContainsColumn(DataColumn column);
        internal abstract bool IsConstraintViolated();
        protected internal void SetDataSet(DataSet dataSet)
        {
            this.dataSet = dataSet;
        }

        public override string ToString() => 
            this.ConstraintName;

        [CLSCompliant(false)]
        protected virtual DataSet _DataSet =>
            this.dataSet;

        [ResDescription("ConstraintNameDescr"), ResCategory("DataCategory_Data"), DefaultValue("")]
        public virtual string ConstraintName
        {
            get => 
                this.name;
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if ((ADP.IsEmpty(value) && (this.Table != null)) && this.InCollection)
                {
                    throw ExceptionBuilder.NoConstraintName();
                }
                CultureInfo culture = (this.Table != null) ? this.Table.Locale : CultureInfo.CurrentCulture;
                if (string.Compare(this.name, value, true, culture) != 0)
                {
                    if ((this.Table != null) && this.InCollection)
                    {
                        this.Table.Constraints.RegisterName(value);
                        if (this.name.Length != 0)
                        {
                            this.Table.Constraints.UnregisterName(this.name);
                        }
                    }
                    this.name = value;
                }
                else if (string.Compare(this.name, value, false, culture) != 0)
                {
                    this.name = value;
                }
            }
        }

        [ResDescription("ExtendedPropertiesDescr"), ResCategory("DataCategory_Data"), Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get
            {
                if (this.extendedProperties == null)
                {
                    this.extendedProperties = new PropertyCollection();
                }
                return this.extendedProperties;
            }
        }

        internal virtual bool InCollection
        {
            get => 
                this.inCollection;
            set
            {
                this.inCollection = value;
                if (value)
                {
                    this.dataSet = this.Table.DataSet;
                }
                else
                {
                    this.dataSet = null;
                }
            }
        }

        internal string SchemaName
        {
            get
            {
                if (ADP.IsEmpty(this._schemaName))
                {
                    return this.ConstraintName;
                }
                return this._schemaName;
            }
            set
            {
                if (!ADP.IsEmpty(value))
                {
                    this._schemaName = value;
                }
            }
        }

        [ResDescription("ConstraintTableDescr")]
        public abstract DataTable Table { get; }
    }
}

