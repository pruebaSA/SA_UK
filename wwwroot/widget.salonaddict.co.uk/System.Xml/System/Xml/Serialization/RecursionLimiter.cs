namespace System.Xml.Serialization
{
    using System;

    internal class RecursionLimiter
    {
        private WorkItems deferredWorkItems;
        private int depth = 0;
        private int maxDepth = (DiagnosticsSwitches.NonRecursiveTypeLoading.Enabled ? 1 : 0x7fffffff);

        internal RecursionLimiter()
        {
        }

        internal WorkItems DeferredWorkItems
        {
            get
            {
                if (this.deferredWorkItems == null)
                {
                    this.deferredWorkItems = new WorkItems();
                }
                return this.deferredWorkItems;
            }
        }

        internal int Depth
        {
            get => 
                this.depth;
            set
            {
                this.depth = value;
            }
        }

        internal bool IsExceededLimit =>
            (this.depth > this.maxDepth);
    }
}

