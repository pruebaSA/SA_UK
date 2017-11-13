namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceSelectEventArgs : CancelEventArgs
    {
        private DataSourceSelectArguments _arguments;
        private IDictionary<string, object> _groupByParameters;
        private IOrderedDictionary _orderByParameters;
        private IDictionary<string, object> _orderGroupsByParameters;
        private object _result;
        private IDictionary<string, object> _selectParameters;
        private IDictionary<string, object> _whereParameters;

        public LinqDataSourceSelectEventArgs(DataSourceSelectArguments arguments, IDictionary<string, object> whereParameters, IOrderedDictionary orderByParameters, IDictionary<string, object> groupByParameters, IDictionary<string, object> orderGroupsByParameters, IDictionary<string, object> selectParameters)
        {
            this._arguments = arguments;
            this._groupByParameters = groupByParameters;
            this._orderByParameters = orderByParameters;
            this._orderGroupsByParameters = orderGroupsByParameters;
            this._selectParameters = selectParameters;
            this._whereParameters = whereParameters;
        }

        public DataSourceSelectArguments Arguments =>
            this._arguments;

        public IDictionary<string, object> GroupByParameters =>
            this._groupByParameters;

        public IOrderedDictionary OrderByParameters =>
            this._orderByParameters;

        public IDictionary<string, object> OrderGroupsByParameters =>
            this._orderGroupsByParameters;

        public object Result
        {
            get => 
                this._result;
            set
            {
                this._result = value;
            }
        }

        public IDictionary<string, object> SelectParameters =>
            this._selectParameters;

        public IDictionary<string, object> WhereParameters =>
            this._whereParameters;
    }
}

