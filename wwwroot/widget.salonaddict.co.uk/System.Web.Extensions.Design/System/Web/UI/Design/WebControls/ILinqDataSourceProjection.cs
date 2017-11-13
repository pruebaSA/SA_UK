namespace System.Web.UI.Design.WebControls
{
    using System;

    internal interface ILinqDataSourceProjection
    {
        string ToString();

        LinqDataSourceAggregateFunctions AggregateFunction { get; set; }

        string Alias { get; set; }

        ILinqDataSourcePropertyItem Column { get; set; }

        bool IsAliasMandatory { get; }
    }
}

