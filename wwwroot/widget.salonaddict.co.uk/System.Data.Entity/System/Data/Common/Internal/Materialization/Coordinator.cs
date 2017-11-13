namespace System.Data.Common.Internal.Materialization
{
    using System;

    internal abstract class Coordinator
    {
        private Coordinator _child;
        private bool _isEntered;
        internal readonly System.Data.Common.Internal.Materialization.CoordinatorFactory CoordinatorFactory;
        internal readonly Coordinator Next;
        internal readonly Coordinator Parent;

        protected Coordinator(System.Data.Common.Internal.Materialization.CoordinatorFactory coordinatorFactory, Coordinator parent, Coordinator next)
        {
            this.CoordinatorFactory = coordinatorFactory;
            this.Parent = parent;
            this.Next = next;
        }

        internal bool HasNextElement(Shaper shaper)
        {
            bool flag = false;
            if (this.IsEntered && this.CoordinatorFactory.CheckKeys(shaper))
            {
                return flag;
            }
            this.CoordinatorFactory.SetKeys(shaper);
            this.IsEntered = true;
            return true;
        }

        internal void Initialize(Shaper shaper)
        {
            this.ResetCollection(shaper);
            shaper.State[this.CoordinatorFactory.StateSlot] = this;
            if (this.Child != null)
            {
                this.Child.Initialize(shaper);
            }
            if (this.Next != null)
            {
                this.Next.Initialize(shaper);
            }
        }

        internal int MaxDistanceToLeaf()
        {
            int num = 0;
            for (Coordinator coordinator = this.Child; coordinator != null; coordinator = coordinator.Next)
            {
                num = Math.Max(num, coordinator.MaxDistanceToLeaf() + 1);
            }
            return num;
        }

        internal abstract void ReadNextElement(Shaper shaper);
        internal abstract void ResetCollection(Shaper shaper);

        public Coordinator Child
        {
            get => 
                this._child;
            protected set
            {
                this._child = value;
            }
        }

        public bool IsEntered
        {
            get => 
                this._isEntered;
            protected set
            {
                this._isEntered = value;
            }
        }

        internal bool IsRoot =>
            (null == this.Parent);
    }
}

