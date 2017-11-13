namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StackRegion
    {
        internal QueryRange bounds;
        internal int stackPtr;
        internal StackRegion(QueryRange bounds)
        {
            this.bounds = bounds;
            this.stackPtr = bounds.start - 1;
        }

        internal int Count =>
            ((this.stackPtr - this.bounds.start) + 1);
        internal bool NeedsGrowth =>
            (this.stackPtr > this.bounds.end);
        internal void Clear()
        {
            this.stackPtr = this.bounds.start - 1;
        }

        internal void Grow(int growBy)
        {
            this.bounds.end += growBy;
        }

        internal bool IsValidStackPtr() => 
            this.bounds.IsInRange(this.stackPtr);

        internal bool IsValidStackPtr(int stackPtr) => 
            this.bounds.IsInRange(stackPtr);

        internal void Shift(int shiftBy)
        {
            this.bounds.Shift(shiftBy);
            this.stackPtr += shiftBy;
        }
    }
}

