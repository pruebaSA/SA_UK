namespace System.Data.Design
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;

    internal class DesignColumn : DataSourceComponent, IDataSourceNamedObject, INamedObject, ICloneable
    {
        private System.Data.DataColumn dataColumn;
        private System.Data.Design.DesignTable designTable;
        internal static string EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINROW = "Generator_ColumnPropNameInRow";
        internal static string EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINTABLE = "Generator_ColumnPropNameInTable";
        internal static string EXTPROPNAME_GENERATOR_COLUMNVARNAMEINTABLE = "Generator_ColumnVarNameInTable";
        internal static string EXTPROPNAME_USER_COLUMNNAME = "Generator_UserColumnName";
        private StringCollection namingPropNames;
        private const string NullValuePropertyName = "nullValue";
        private const string NullValueThrow = "_throw";
        private const string ROPNAME_EXPRESSION = "Expression";

        public DesignColumn()
        {
            this.namingPropNames = new StringCollection();
            this.dataColumn = new System.Data.DataColumn();
            this.designTable = null;
            this.namingPropNames.Add("typedName");
        }

        public DesignColumn(System.Data.DataColumn dataColumn)
        {
            this.namingPropNames = new StringCollection();
            if (dataColumn == null)
            {
                throw new InternalException("DesignColumn object needs a valid DataColumn", 0x4e29);
            }
            this.dataColumn = dataColumn;
            this.namingPropNames.Add("typedName");
        }

        public object Clone() => 
            new DesignColumn(DataDesignUtil.CloneColumn(this.dataColumn));

        internal bool IsKeyColumn()
        {
            if (this.DesignTable == null)
            {
                return false;
            }
            ArrayList relatedDataConstraints = this.DesignTable.GetRelatedDataConstraints(new DesignColumn[] { this }, true);
            return ((relatedDataConstraints != null) && (relatedDataConstraints.Count > 0));
        }

        private void OnDataTypeChanged()
        {
        }

        public override string ToString() => 
            (this.PublicTypeName + " " + this.Name);

        [RefreshProperties(RefreshProperties.All), DefaultValue(false)]
        public bool AutoIncrement
        {
            get => 
                this.dataColumn.AutoIncrement;
            set
            {
                if (this.dataColumn.AutoIncrement != value)
                {
                    Type dataType = this.DataType;
                    this.dataColumn.AutoIncrement = value;
                    Type type1 = this.DataType;
                }
            }
        }

        public System.Data.DataColumn DataColumn =>
            this.dataColumn;

        [DefaultValue(typeof(string)), RefreshProperties(RefreshProperties.All)]
        public Type DataType
        {
            get => 
                this.dataColumn.DataType;
            set
            {
                if (this.dataColumn.DataType != value)
                {
                    bool autoIncrement = this.AutoIncrement;
                    this.dataColumn.DataType = value;
                    this.OnDataTypeChanged();
                    bool flag1 = this.AutoIncrement;
                }
            }
        }

        internal System.Data.Design.DesignTable DesignTable
        {
            get => 
                this.designTable;
            set
            {
                this.designTable = value;
            }
        }

        [DefaultValue(""), RefreshProperties(RefreshProperties.All)]
        public string Expression
        {
            get => 
                this.dataColumn.Expression;
            set
            {
                bool readOnly = this.dataColumn.ReadOnly;
                this.dataColumn.Expression = value;
            }
        }

        protected override object ExternalPropertyHost =>
            this.dataColumn;

        internal string GeneratorColumnPropNameInRow
        {
            get => 
                (this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINROW] as string);
            set
            {
                this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINROW] = value;
            }
        }

        internal string GeneratorColumnPropNameInTable
        {
            get => 
                (this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINTABLE] as string);
            set
            {
                this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNPROPNAMEINTABLE] = value;
            }
        }

        internal string GeneratorColumnVarNameInTable
        {
            get => 
                (this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNVARNAMEINTABLE] as string);
            set
            {
                this.dataColumn.ExtendedProperties[EXTPROPNAME_GENERATOR_COLUMNVARNAMEINTABLE] = value;
            }
        }

        [Browsable(false)]
        public override string GeneratorName =>
            this.GeneratorColumnPropNameInRow;

        [DefaultValue(-1)]
        public int MaxLength
        {
            get => 
                this.dataColumn.MaxLength;
            set
            {
                if ((this.MaxLength >= 0) && (value > this.MaxLength))
                {
                    this.dataColumn.MaxLength = -1;
                }
                this.dataColumn.MaxLength = value;
            }
        }

        [MergableProperty(false), DefaultValue("")]
        public string Name
        {
            get => 
                this.dataColumn.ColumnName;
            set
            {
                string columnName = this.dataColumn.ColumnName;
                if (!StringUtil.EqualValue(value, columnName))
                {
                    if (this.CollectionParent != null)
                    {
                        this.CollectionParent.ValidateUniqueName(this, value);
                    }
                    this.dataColumn.ColumnName = value;
                    if ((columnName.Length > 0) && (value.Length > 0))
                    {
                        System.Data.Design.DesignTable designTable = this.DesignTable;
                        if (designTable != null)
                        {
                            designTable.UpdateColumnMappingDataSetColumnName(columnName, value);
                        }
                    }
                }
            }
        }

        internal override StringCollection NamingPropertyNames =>
            this.namingPropNames;

        [DefaultValue("_throw")]
        public string NullValue
        {
            get
            {
                if (this.dataColumn.ExtendedProperties.Contains("nullValue"))
                {
                    return (this.dataColumn.ExtendedProperties["nullValue"] as string);
                }
                return "_throw";
            }
            set
            {
                if (value != this.NullValue)
                {
                    this.dataColumn.ExtendedProperties["nullValue"] = value;
                }
            }
        }

        [Browsable(false)]
        public string PublicTypeName =>
            "Column";

        [DefaultValue("")]
        public string Source
        {
            get
            {
                if ((this.DesignTable != null) && (this.DesignTable.Mappings != null))
                {
                    int num = this.DesignTable.Mappings.IndexOfDataSetColumn(this.DataColumn.ColumnName);
                    DataColumnMapping byDataSetColumn = null;
                    if (num >= 0)
                    {
                        byDataSetColumn = this.DesignTable.Mappings.GetByDataSetColumn(this.DataColumn.ColumnName);
                    }
                    if (byDataSetColumn != null)
                    {
                        return byDataSetColumn.SourceColumn;
                    }
                }
                return string.Empty;
            }
            set
            {
                if (this.DesignTable != null)
                {
                    this.DesignTable.UpdateColumnMappingSourceColumnName(this.DataColumn.ColumnName, value);
                }
            }
        }

        [DefaultValue(false)]
        public bool Unique
        {
            get => 
                this.dataColumn.Unique;
            set
            {
            }
        }

        internal string UserColumnName
        {
            get => 
                (this.dataColumn.ExtendedProperties[EXTPROPNAME_USER_COLUMNNAME] as string);
            set
            {
                this.dataColumn.ExtendedProperties[EXTPROPNAME_USER_COLUMNNAME] = value;
            }
        }
    }
}

