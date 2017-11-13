namespace System.Data.Services.Client
{
    using System;

    internal enum BatchStreamState
    {
        EndBatch,
        StartBatch,
        BeginChangeSet,
        EndChangeSet,
        Post,
        Put,
        Delete,
        Get,
        Merge,
        GetResponse,
        ChangeResponse
    }
}

