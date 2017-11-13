namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StackFrame
    {
        internal int basePtr;
        internal int endPtr;
        internal int Count =>
            ((this.endPtr - this.basePtr) + 1);
        internal int EndPtr
        {
            set
            {
                this.endPtr = value;
            }
        }
        internal int this[int offset] =>
            (this.basePtr + offset);
        internal bool IsValidPtr(int ptr) => 
            ((ptr >= this.basePtr) && (ptr <= this.endPtr));
    }
}

