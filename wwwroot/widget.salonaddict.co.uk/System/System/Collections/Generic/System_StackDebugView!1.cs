namespace System.Collections.Generic
{
    using System;
    using System.Diagnostics;

    internal sealed class System_StackDebugView<T>
    {
        private Stack<T> stack;

        public System_StackDebugView(Stack<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }
            this.stack = stack;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items =>
            this.stack.ToArray();
    }
}

