namespace System.Data
{
    using System;
    using System.Data.Common;

    internal interface IEntityAdapter
    {
        int Update(IEntityStateManager cache);

        bool AcceptChangesDuringUpdate { get; set; }

        int? CommandTimeout { get; set; }

        DbConnection Connection { get; set; }
    }
}

