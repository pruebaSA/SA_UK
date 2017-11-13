namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceState
    {
        private bool _autoGenerateOrderByClause;
        private bool _autoGenerateWhereClause;
        private string _contextTypeName;
        private ParameterCollection _deleteParameters;
        private bool _enableDelete;
        private bool _enableInsert;
        private bool _enableUpdate;
        private string _groupBy;
        private ParameterCollection _groupByParameters;
        private ParameterCollection _insertParameters;
        private string _orderBy;
        private ParameterCollection _orderByParameters;
        private string _orderGroupsBy;
        private ParameterCollection _orderGroupsByParameters;
        private string _select;
        private ParameterCollection _selectParameters;
        private string _tableName;
        private ParameterCollection _updateParameters;
        private string _where;
        private ParameterCollection _whereParameters;

        public LinqDataSourceState()
        {
        }

        public LinqDataSourceState(ParameterCollection whereParameters, ParameterCollection orderByParameters, ParameterCollection groupByParameters, ParameterCollection orderGroupsByParameters, ParameterCollection selectParameters, ParameterCollection insertParameters, ParameterCollection updateParameters, ParameterCollection deleteParameters)
        {
            this._whereParameters = whereParameters;
            this._orderByParameters = orderByParameters;
            this._groupByParameters = groupByParameters;
            this._orderGroupsByParameters = orderGroupsByParameters;
            this._selectParameters = selectParameters;
            this._insertParameters = insertParameters;
            this._updateParameters = updateParameters;
            this._deleteParameters = deleteParameters;
        }

        public bool AutoGenerateOrderByClause
        {
            get => 
                this._autoGenerateOrderByClause;
            set
            {
                this._autoGenerateOrderByClause = value;
            }
        }

        public bool AutoGenerateWhereClause
        {
            get => 
                this._autoGenerateWhereClause;
            set
            {
                this._autoGenerateWhereClause = value;
            }
        }

        public string ContextTypeName
        {
            get
            {
                if (this._contextTypeName == null)
                {
                    return string.Empty;
                }
                return this._contextTypeName;
            }
            set
            {
                this._contextTypeName = value;
            }
        }

        public ParameterCollection DeleteParameters
        {
            get
            {
                if (this._deleteParameters == null)
                {
                    this._deleteParameters = new ParameterCollection();
                }
                return this._deleteParameters;
            }
        }

        public bool EnableDelete
        {
            get => 
                this._enableDelete;
            set
            {
                this._enableDelete = value;
            }
        }

        public bool EnableInsert
        {
            get => 
                this._enableInsert;
            set
            {
                this._enableInsert = value;
            }
        }

        public bool EnableUpdate
        {
            get => 
                this._enableUpdate;
            set
            {
                this._enableUpdate = value;
            }
        }

        public string GroupBy
        {
            get
            {
                if (this._groupBy == null)
                {
                    return string.Empty;
                }
                return this._groupBy;
            }
            set
            {
                this._groupBy = value;
            }
        }

        public ParameterCollection GroupByParameters
        {
            get
            {
                if (this._groupByParameters == null)
                {
                    this._groupByParameters = new ParameterCollection();
                }
                return this._groupByParameters;
            }
        }

        public ParameterCollection InsertParameters
        {
            get
            {
                if (this._insertParameters == null)
                {
                    this._insertParameters = new ParameterCollection();
                }
                return this._insertParameters;
            }
        }

        public string OrderBy
        {
            get
            {
                if (this._orderBy == null)
                {
                    return string.Empty;
                }
                return this._orderBy;
            }
            set
            {
                this._orderBy = value;
            }
        }

        public ParameterCollection OrderByParameters
        {
            get
            {
                if (this._orderByParameters == null)
                {
                    this._orderByParameters = new ParameterCollection();
                }
                return this._orderByParameters;
            }
        }

        public string OrderGroupsBy
        {
            get
            {
                if (this._orderGroupsBy == null)
                {
                    return string.Empty;
                }
                return this._orderGroupsBy;
            }
            set
            {
                this._orderGroupsBy = value;
            }
        }

        public ParameterCollection OrderGroupsByParameters
        {
            get
            {
                if (this._orderGroupsByParameters == null)
                {
                    this._orderGroupsByParameters = new ParameterCollection();
                }
                return this._orderGroupsByParameters;
            }
        }

        public string Select
        {
            get
            {
                if (this._select == null)
                {
                    return string.Empty;
                }
                return this._select;
            }
            set
            {
                this._select = value;
            }
        }

        public ParameterCollection SelectParameters
        {
            get
            {
                if (this._selectParameters == null)
                {
                    this._selectParameters = new ParameterCollection();
                }
                return this._selectParameters;
            }
        }

        public string TableName
        {
            get
            {
                if (this._tableName == null)
                {
                    return string.Empty;
                }
                return this._tableName;
            }
            set
            {
                this._tableName = value;
            }
        }

        public ParameterCollection UpdateParameters
        {
            get
            {
                if (this._updateParameters == null)
                {
                    this._updateParameters = new ParameterCollection();
                }
                return this._updateParameters;
            }
        }

        public string Where
        {
            get
            {
                if (this._where == null)
                {
                    return string.Empty;
                }
                return this._where;
            }
            set
            {
                this._where = value;
            }
        }

        public ParameterCollection WhereParameters
        {
            get
            {
                if (this._whereParameters == null)
                {
                    this._whereParameters = new ParameterCollection();
                }
                return this._whereParameters;
            }
        }
    }
}

