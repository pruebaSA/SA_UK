namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    internal class Coordinator<T> : Coordinator
    {
        private T _current;
        private List<T> _elements;
        private Action<Shaper, List<T>> _handleClose;
        private readonly bool IsUsingElementCollection;
        internal readonly CoordinatorFactory<T> TypedCoordinatorFactory;

        internal Coordinator(CoordinatorFactory<T> coordinator, Coordinator parent, Coordinator next) : base(coordinator, parent, next)
        {
            this.TypedCoordinatorFactory = coordinator;
            Coordinator child = null;
            foreach (CoordinatorFactory factory in coordinator.NestedCoordinators.Reverse<CoordinatorFactory>())
            {
                base.Child = factory.CreateCoordinator(this, child);
                child = base.Child;
            }
            this.IsUsingElementCollection = !base.IsRoot && (typeof(T) != typeof(RecordState));
        }

        private IEnumerable<T> GetElements() => 
            this._elements;

        internal override void ReadNextElement(Shaper shaper)
        {
            T local;
            try
            {
                local = this.TypedCoordinatorFactory.Element(shaper);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    local = this.TypedCoordinatorFactory.ElementWithErrorHandling(shaper);
                }
                throw;
            }
            if (this.IsUsingElementCollection)
            {
                this._elements.Add(local);
            }
            else
            {
                this._current = local;
            }
        }

        internal void RegisterCloseHandler(Action<Shaper, List<T>> closeHandler)
        {
            this._handleClose = closeHandler;
        }

        internal override void ResetCollection(Shaper shaper)
        {
            if (this._handleClose != null)
            {
                this._handleClose(shaper, this._elements);
                this._handleClose = null;
            }
            base.IsEntered = false;
            if (this.IsUsingElementCollection)
            {
                this._elements = new List<T>();
            }
            if (base.Child != null)
            {
                base.Child.ResetCollection(shaper);
            }
            if (base.Next != null)
            {
                base.Next.ResetCollection(shaper);
            }
        }

        internal void SetCurrentToDefault()
        {
            this._current = default(T);
        }

        internal T Current =>
            this._current;
    }
}

