namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("State = {state}")]
    public sealed class LinkDescriptor : Descriptor
    {
        internal static readonly IEqualityComparer<LinkDescriptor> EquivalenceComparer = new Equivalent();
        private object source;
        private string sourceProperty;
        private object target;

        internal LinkDescriptor(object source, string sourceProperty, object target) : this(source, sourceProperty, target, EntityStates.Unchanged)
        {
        }

        internal LinkDescriptor(object source, string sourceProperty, object target, EntityStates state) : base(state)
        {
            this.source = source;
            this.sourceProperty = sourceProperty;
            this.target = target;
        }

        internal bool IsEquivalent(object src, string srcPropName, object targ) => 
            (((this.source == src) && (this.target == targ)) && (this.sourceProperty == srcPropName));

        internal override bool IsResource =>
            false;

        public object Source =>
            this.source;

        public string SourceProperty =>
            this.sourceProperty;

        public object Target =>
            this.target;

        private sealed class Equivalent : IEqualityComparer<LinkDescriptor>
        {
            public bool Equals(LinkDescriptor x, LinkDescriptor y) => 
                x.IsEquivalent(y.source, y.sourceProperty, y.target);

            public int GetHashCode(LinkDescriptor obj) => 
                ((obj.Source.GetHashCode() ^ ((obj.Target != null) ? obj.Target.GetHashCode() : 0)) ^ obj.SourceProperty.GetHashCode());
        }
    }
}

