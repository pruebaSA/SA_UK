namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data;

    public interface IEntityWithKey
    {
        System.Data.EntityKey EntityKey { get; set; }
    }
}

