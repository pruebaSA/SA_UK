namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Web.UI.WebControls;

    internal interface ILinqDataSourceWrapper
    {
        ParameterCollection CloneDeleteParameters();
        ParameterCollection CloneGroupByParameters();
        ParameterCollection CloneInsertParameters();
        ParameterCollection CloneOrderByParameters();
        ParameterCollection CloneOrderGroupsByParameters();
        ParameterCollection CloneParameters(ParameterCollection original);
        ParameterCollection CloneSelectParameters();
        ParameterCollection CloneUpdateParameters();
        ParameterCollection CloneWhereParameters();
        LinqDataSourceState GetState();
        void SetDeleteParameterContents(ParameterCollection newParams);
        void SetGroupByParameterContents(ParameterCollection newParams);
        void SetInsertParameterContents(ParameterCollection newParams);
        void SetOrderByParameterContents(ParameterCollection newParams);
        void SetOrderGroupsByParameterContents(ParameterCollection newParams);
        void SetSelectParameterContents(ParameterCollection newParams);
        void SetState(LinqDataSourceState state);
        void SetUpdateParameterContents(ParameterCollection newParams);
        void SetWhereParameterContents(ParameterCollection newParams);

        bool AutoGenerateOrderByClause { get; }

        bool AutoGenerateWhereClause { get; }

        bool CanPage { get; }

        bool CanSort { get; }

        string ContextTypeName { get; set; }

        bool EnableDelete { get; set; }

        bool EnableInsert { get; set; }

        bool EnableUpdate { get; set; }

        string GroupBy { get; set; }

        string OrderBy { get; set; }

        string OrderGroupsBy { get; set; }

        string Select { get; set; }

        IServiceProvider ServiceProvider { get; }

        string TableName { get; set; }

        string Where { get; set; }
    }
}

