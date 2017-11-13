namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Link<T>
    {
        private T underlyingValue;
        private IEnumerable<T> source;
        public Link(T value)
        {
            this.underlyingValue = value;
            this.source = null;
        }

        public Link(IEnumerable<T> source)
        {
            this.source = source;
            this.underlyingValue = default(T);
        }

        public Link(Link<T> link)
        {
            this.underlyingValue = link.underlyingValue;
            this.source = link.source;
        }

        public bool HasValue
        {
            get
            {
                if ((this.source != null) && !this.HasLoadedValue)
                {
                    return this.HasAssignedValue;
                }
                return true;
            }
        }
        public bool HasLoadedOrAssignedValue
        {
            get
            {
                if (!this.HasLoadedValue)
                {
                    return this.HasAssignedValue;
                }
                return true;
            }
        }
        internal bool HasLoadedValue =>
            (this.source == SourceState<T>.Loaded);
        internal bool HasAssignedValue =>
            (this.source == SourceState<T>.Assigned);
        internal T UnderlyingValue =>
            this.underlyingValue;
        internal IEnumerable<T> Source =>
            this.source;
        internal bool HasSource =>
            (((this.source != null) && !this.HasAssignedValue) && !this.HasLoadedValue);
        public T Value
        {
            get
            {
                if (this.HasSource)
                {
                    this.underlyingValue = this.source.SingleOrDefault<T>();
                    this.source = SourceState<T>.Loaded;
                }
                return this.underlyingValue;
            }
            set
            {
                this.underlyingValue = value;
                this.source = SourceState<T>.Assigned;
            }
        }
    }
}

