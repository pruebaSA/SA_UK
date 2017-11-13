namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    internal class ShaperFactory<T> : ShaperFactory
    {
        private readonly Action _checkPermissions;
        private readonly MergeOption _mergeOption;
        private readonly CoordinatorFactory<T> _rootCoordinatorFactory;
        private readonly int _stateCount;

        internal ShaperFactory(int stateCount, CoordinatorFactory<T> rootCoordinatorFactory, Action checkPermissions, MergeOption mergeOption)
        {
            this._stateCount = stateCount;
            this._rootCoordinatorFactory = rootCoordinatorFactory;
            this._checkPermissions = checkPermissions;
            this._mergeOption = mergeOption;
        }

        internal Shaper<T> Create(DbDataReader reader, MetadataWorkspace workspace) => 
            new Shaper<T>(reader, null, workspace, MergeOption.NoTracking, this._stateCount, this._rootCoordinatorFactory, this._checkPermissions);

        internal Shaper<T> Create(DbDataReader reader, ObjectContext context, MetadataWorkspace workspace, MergeOption mergeOption) => 
            new Shaper<T>(reader, context, workspace, mergeOption, this._stateCount, this._rootCoordinatorFactory, this._checkPermissions);
    }
}

