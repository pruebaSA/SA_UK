namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.UI.WebControls;

    internal interface ILinqDataSourceDesignerHelper
    {
        bool CanConfigure(IServiceProvider serviceProvider);
        bool CanInsertUpdateDelete(ILinqDataSourceDesignerHelper helperHelper);
        bool CanRefreshSchema(IServiceProvider serviceProvider);
        ParameterCollection CloneDeleteParameters();
        ParameterCollection CloneGroupByParameters();
        ParameterCollection CloneInsertParameters();
        ParameterCollection CloneOrderByParameters();
        ParameterCollection CloneOrderGroupsByParameters();
        ParameterCollection CloneParameters(ParameterCollection original);
        ParameterCollection CloneSelectParameters();
        ParameterCollection CloneUpdateParameters();
        ParameterCollection CloneWhereParameters();
        ILinqDataSourceContextTypeItem GetContextType(ILinqDataSourceDesignerHelper helperHelper);
        List<ILinqDataSourceContextTypeItem> GetContextTypes(Type baseType, IServiceProvider serviceProvider);
        LinqDataSourceState GetLinqDataSourceState();
        List<ILinqDataSourcePropertyItem> GetProperties(Type tableType, bool isSorted, bool includeEnumerables);
        List<ILinqDataSourcePropertyItem> GetProperties(ILinqDataSourcePropertyItem table, bool isSorted, bool includeEnumerables);
        ILinqDataSourcePropertyItem GetTable(ILinqDataSourceDesignerHelper helperHelper);
        List<ILinqDataSourcePropertyItem> GetTables(Type contextType, bool isSorted);
        List<ILinqDataSourcePropertyItem> GetTables(ILinqDataSourceContextTypeItem context, bool isSorted);
        Type GetType(string typeName, IServiceProvider serviceProvider);
        LinqDesignerDataSourceView GetView(string viewName);
        string[] GetViewNames();
        bool IsDataContext(ILinqDataSourceDesignerHelper helperHelper);
        bool IsTableTypeTable(ILinqDataSourceDesignerHelper helperHelper);
        bool IsTableTypeTable(ILinqDataSourceDesignerHelper helperHelper, string contextTypeName, string tableName, IServiceProvider serviceProvider);
        DataTable LoadSchema();
        DataTable MakeDataTable(ILinqDataSourceDesignerHelper helperHelper, ILinqDataSourcePropertyItem table);
        void PreFilterProperties(IDictionary properties, Type designerType);
        void RefreshSchema(ILinqDataSourceDesignerHelper helperHelper, bool preferSilent);
        void RegisterClone(object original, object clone);
        void SaveSchema(string contextTypeName, string tableName, DataTable schema);
        void SetDeleteParameterContents(ParameterCollection newParams);
        void SetGroupByParameterContents(ParameterCollection newParams);
        void SetInsertParameterContents(ParameterCollection newParams);
        void SetLinqDataSourceState(LinqDataSourceState state);
        void SetOrderByParameterContents(ParameterCollection newParams);
        void SetOrderGroupsByParameterContents(ParameterCollection newParams);
        void SetSelectParameterContents(ParameterCollection newParams);
        void SetUpdateParameterContents(ParameterCollection newParams);
        void SetWhereParameterContents(ParameterCollection newParams);
        void SetWrapper(ILinqDataSourceWrapper wrapper);

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

