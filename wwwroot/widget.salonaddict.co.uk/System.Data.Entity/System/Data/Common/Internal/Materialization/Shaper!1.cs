namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Runtime.CompilerServices;

    internal sealed class Shaper<T> : Shaper
    {
        private bool _dataWaiting;
        private bool _isActive;
        private IEnumerator<T> _rootEnumerator;
        private readonly Action CheckPermissionsAction;
        private readonly bool IsObjectQuery;
        internal readonly Coordinator<T> RootCoordinator;

        internal event EventHandler OnDone;

        internal Shaper(DbDataReader reader, ObjectContext context, MetadataWorkspace workspace, MergeOption mergeOption, int stateCount, CoordinatorFactory<T> rootCoordinatorFactory, Action checkPermissions) : base(reader, context, workspace, mergeOption, stateCount)
        {
            this.RootCoordinator = new Coordinator<T>(rootCoordinatorFactory, null, null);
            this.CheckPermissionsAction = checkPermissions;
            this.IsObjectQuery = typeof(T) != typeof(RecordState);
            this._isActive = true;
            this.RootCoordinator.Initialize(this);
        }

        private void Finally()
        {
            if (this._isActive)
            {
                this._isActive = false;
                if (this.IsObjectQuery)
                {
                    base.Reader.Dispose();
                }
                if (base.Context != null)
                {
                    base.Context.ReleaseConnection();
                }
                if (this.OnDone != null)
                {
                    this.OnDone(this, new EventArgs());
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.RootCoordinator.CoordinatorFactory.IsSimple)
            {
                return new SimpleEnumerator<T>((Shaper<T>) this);
            }
            RowNestedResultEnumerator<T> rowEnumerator = new RowNestedResultEnumerator<T>((Shaper<T>) this);
            if (this.IsObjectQuery)
            {
                return new ObjectQueryNestedEnumerator<T>(rowEnumerator);
            }
            return (IEnumerator<T>) new RecordStateEnumerator<T>(rowEnumerator);
        }

        private void InitializeRecordStates(CoordinatorFactory coordinatorFactory)
        {
            foreach (RecordStateFactory factory in coordinatorFactory.RecordStateFactories)
            {
                base.State[factory.StateSlotNumber] = factory.Create(coordinatorFactory);
            }
            foreach (CoordinatorFactory factory2 in coordinatorFactory.NestedCoordinators)
            {
                this.InitializeRecordStates(factory2);
            }
        }

        private bool StoreRead()
        {
            bool flag;
            try
            {
                flag = base.Reader.Read();
            }
            catch (Exception exception)
            {
                if (base.Reader.IsClosed)
                {
                    throw EntityUtil.DataReaderClosed("Read");
                }
                if (EntityUtil.IsCatchableEntityExceptionType(exception))
                {
                    throw EntityUtil.CommandExecution(Strings.EntityClient_StoreReaderFailed, exception);
                }
                throw;
            }
            return flag;
        }

        internal bool DataWaiting
        {
            get => 
                this._dataWaiting;
            set
            {
                this._dataWaiting = value;
            }
        }

        internal IEnumerator<T> RootEnumerator
        {
            get
            {
                if (this._rootEnumerator == null)
                {
                    this.InitializeRecordStates(this.RootCoordinator.CoordinatorFactory);
                    this._rootEnumerator = this.GetEnumerator();
                }
                return this._rootEnumerator;
            }
        }

        private class ObjectQueryNestedEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private T _previousElement;
            private readonly Shaper<T>.RowNestedResultEnumerator _rowEnumerator;
            private State<T> _state;

            internal ObjectQueryNestedEnumerator(Shaper<T>.RowNestedResultEnumerator rowEnumerator)
            {
                this._rowEnumerator = rowEnumerator;
                this._previousElement = default(T);
                this._state = State<T>.Start;
            }

            public void Dispose()
            {
                this._rowEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                switch (this._state)
                {
                    case State<T>.Start:
                        if (!this.TryReadToNextElement())
                        {
                            this._state = State<T>.NoRows;
                            break;
                        }
                        this.ReadElement();
                        break;

                    case State<T>.Reading:
                        this.ReadElement();
                        break;

                    case State<T>.NoRowsLastElementPending:
                        this._state = State<T>.NoRows;
                        break;

                    case State<T>.NoRows:
                        this._previousElement = default(T);
                        return false;
                }
                return true;
            }

            private void ReadElement()
            {
                this._previousElement = this._rowEnumerator.RootCoordinator.Current;
                if (this.TryReadToNextElement())
                {
                    this._state = State<T>.Reading;
                }
                else
                {
                    this._state = State<T>.NoRowsLastElementPending;
                }
            }

            public void Reset()
            {
                this._rowEnumerator.Reset();
            }

            private bool TryReadToNextElement()
            {
                while (this._rowEnumerator.MoveNext())
                {
                    if (this._rowEnumerator.Current[0] != null)
                    {
                        return true;
                    }
                }
                return false;
            }

            public T Current =>
                this._previousElement;

            object IEnumerator.Current =>
                this.Current;

            private enum State
            {
                public const Shaper<T>.ObjectQueryNestedEnumerator.State NoRows = Shaper<T>.ObjectQueryNestedEnumerator.State.NoRows;,
                public const Shaper<T>.ObjectQueryNestedEnumerator.State NoRowsLastElementPending = Shaper<T>.ObjectQueryNestedEnumerator.State.NoRowsLastElementPending;,
                public const Shaper<T>.ObjectQueryNestedEnumerator.State Reading = Shaper<T>.ObjectQueryNestedEnumerator.State.Reading;,
                public const Shaper<T>.ObjectQueryNestedEnumerator.State Start = Shaper<T>.ObjectQueryNestedEnumerator.State.Start;
            }
        }

        private class RecordStateEnumerator : IEnumerator<RecordState>, IDisposable, IEnumerator
        {
            private RecordState _current;
            private int _depth;
            private bool _readerConsumed;
            private readonly Shaper<T>.RowNestedResultEnumerator _rowEnumerator;

            internal RecordStateEnumerator(Shaper<T>.RowNestedResultEnumerator rowEnumerator)
            {
                this._rowEnumerator = rowEnumerator;
                this._current = null;
                this._depth = -1;
                this._readerConsumed = false;
            }

            public void Dispose()
            {
                this._rowEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (this._readerConsumed)
                {
                    goto Label_0097;
                }
            Label_000B:
                if ((-1 == this._depth) || (this._rowEnumerator.Current.Length == this._depth))
                {
                    if (!this._rowEnumerator.MoveNext())
                    {
                        this._current = null;
                        this._readerConsumed = true;
                        goto Label_0097;
                    }
                    this._depth = 0;
                }
                Coordinator coordinator = this._rowEnumerator.Current[this._depth];
                if (coordinator != null)
                {
                    this._current = ((Coordinator<RecordState>) coordinator).Current;
                    this._depth++;
                }
                else
                {
                    this._depth++;
                    goto Label_000B;
                }
            Label_0097:
                return !this._readerConsumed;
            }

            public void Reset()
            {
                this._rowEnumerator.Reset();
            }

            public RecordState Current =>
                this._current;

            object IEnumerator.Current =>
                this._current;
        }

        private class RowNestedResultEnumerator : IEnumerator<Coordinator[]>, IDisposable, IEnumerator
        {
            private readonly Coordinator[] _current;
            private readonly Shaper<T> _shaper;

            internal RowNestedResultEnumerator(Shaper<T> shaper)
            {
                this._shaper = shaper;
                this._current = new Coordinator[this._shaper.RootCoordinator.MaxDistanceToLeaf() + 1];
            }

            public void Dispose()
            {
                this._shaper.Finally();
            }

            public bool MoveNext()
            {
                Coordinator rootCoordinator = this._shaper.RootCoordinator;
                if (!this._shaper.StoreRead())
                {
                    this.RootCoordinator.ResetCollection(this._shaper);
                    return false;
                }
                int index = 0;
                bool flag = false;
                while (index < this._current.Length)
                {
                    while ((rootCoordinator != null) && !rootCoordinator.CoordinatorFactory.HasData(this._shaper))
                    {
                        rootCoordinator = rootCoordinator.Next;
                    }
                    if (rootCoordinator == null)
                    {
                        break;
                    }
                    if (rootCoordinator.HasNextElement(this._shaper))
                    {
                        if (!flag && (rootCoordinator.Child != null))
                        {
                            rootCoordinator.Child.ResetCollection(this._shaper);
                        }
                        flag = true;
                        rootCoordinator.ReadNextElement(this._shaper);
                        this._current[index] = rootCoordinator;
                    }
                    else
                    {
                        this._current[index] = null;
                    }
                    rootCoordinator = rootCoordinator.Child;
                    index++;
                }
                while (index < this._current.Length)
                {
                    this._current[index] = null;
                    index++;
                }
                return true;
            }

            public void Reset()
            {
                throw EntityUtil.NotSupported();
            }

            public Coordinator[] Current =>
                this._current;

            internal Coordinator<T> RootCoordinator =>
                this._shaper.RootCoordinator;

            object IEnumerator.Current =>
                this._current;
        }

        private class SimpleEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly Shaper<T> _shaper;

            internal SimpleEnumerator(Shaper<T> shaper)
            {
                this._shaper = shaper;
            }

            public void Dispose()
            {
                this._shaper.RootCoordinator.SetCurrentToDefault();
                this._shaper.Finally();
            }

            public bool MoveNext()
            {
                if (this._shaper._isActive)
                {
                    if (this._shaper.StoreRead())
                    {
                        if (this._shaper.CheckPermissionsAction != null)
                        {
                            this._shaper.CheckPermissionsAction();
                        }
                        this._shaper.RootCoordinator.ReadNextElement(this._shaper);
                        return true;
                    }
                    this.Dispose();
                }
                return false;
            }

            public void Reset()
            {
                throw EntityUtil.NotSupported();
            }

            public T Current =>
                this._shaper.RootCoordinator.Current;

            object IEnumerator.Current =>
                this._shaper.RootCoordinator.Current;
        }
    }
}

