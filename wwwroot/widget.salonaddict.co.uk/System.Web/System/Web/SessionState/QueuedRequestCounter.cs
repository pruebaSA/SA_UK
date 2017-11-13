namespace System.Web.SessionState
{
    using System;
    using System.Threading;

    internal class QueuedRequestCounter
    {
        private int _count;

        public int Decrement() => 
            Interlocked.Decrement(ref this._count);

        public int Increment() => 
            Interlocked.Increment(ref this._count);

        public int Count =>
            Interlocked.CompareExchange(ref this._count, 0, 0);
    }
}

