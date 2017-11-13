namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    internal abstract class CoordinatorFactory
    {
        private static readonly Func<Shaper, bool> AlwaysFalse = s => false;
        private static readonly Func<Shaper, bool> AlwaysTrue = s => true;
        internal readonly Func<Shaper, bool> CheckKeys;
        internal readonly int Depth;
        internal readonly Func<Shaper, bool> HasData;
        internal readonly bool IsLeafResult;
        internal readonly bool IsSimple;
        internal readonly ReadOnlyCollection<CoordinatorFactory> NestedCoordinators;
        internal readonly ReadOnlyCollection<RecordStateFactory> RecordStateFactories;
        internal readonly Func<Shaper, bool> SetKeys;
        internal readonly int StateSlot;

        protected CoordinatorFactory(int depth, int stateSlot, Func<Shaper, bool> hasData, Func<Shaper, bool> setKeys, Func<Shaper, bool> checkKeys, CoordinatorFactory[] nestedCoordinators, RecordStateFactory[] recordStateFactories)
        {
            this.Depth = depth;
            this.StateSlot = stateSlot;
            this.IsLeafResult = 0 == nestedCoordinators.Length;
            if (hasData == null)
            {
                this.HasData = AlwaysTrue;
            }
            else
            {
                this.HasData = hasData;
            }
            if (setKeys == null)
            {
                this.SetKeys = AlwaysTrue;
            }
            else
            {
                this.SetKeys = setKeys;
            }
            if (checkKeys == null)
            {
                if (this.IsLeafResult)
                {
                    this.CheckKeys = AlwaysFalse;
                }
                else
                {
                    this.CheckKeys = AlwaysTrue;
                }
            }
            else
            {
                this.CheckKeys = checkKeys;
            }
            this.NestedCoordinators = new ReadOnlyCollection<CoordinatorFactory>(nestedCoordinators);
            this.RecordStateFactories = new ReadOnlyCollection<RecordStateFactory>(recordStateFactories);
            this.IsSimple = (this.IsLeafResult && (checkKeys == null)) && (null == hasData);
        }

        [CompilerGenerated]
        private static bool <.cctor>b__0(Shaper s) => 
            true;

        [CompilerGenerated]
        private static bool <.cctor>b__1(Shaper s) => 
            false;

        internal abstract Coordinator CreateCoordinator(Coordinator parent, Coordinator next);
    }
}

