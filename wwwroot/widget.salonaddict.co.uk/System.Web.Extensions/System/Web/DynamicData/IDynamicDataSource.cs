namespace System.Web.DynamicData
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IDynamicDataSource : IDataSource
    {
        event EventHandler<DynamicValidatorEventArgs> Exception;

        bool AutoGenerateWhereClause { get; set; }

        Type ContextType { get; set; }

        bool EnableDelete { get; set; }

        bool EnableInsert { get; set; }

        bool EnableUpdate { get; set; }

        string EntitySetName { get; set; }

        string Where { get; set; }

        ParameterCollection WhereParameters { get; }
    }
}

