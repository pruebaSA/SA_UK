namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections.Generic;

    public class RecoveryStack : IRecoveryStack
    {
        private object lockObj = new object();
        private Stack<IRequiresRecovery> recoveries = new Stack<IRequiresRecovery>();

        public void Add(IRequiresRecovery recovery)
        {
            Guard.ArgumentNotNull(recovery, "recovery");
            lock (this.lockObj)
            {
                this.recoveries.Push(recovery);
            }
        }

        public void ExecuteRecovery()
        {
            while (this.recoveries.Count > 0)
            {
                this.recoveries.Pop().Recover();
            }
        }

        public int Count
        {
            get
            {
                lock (this.lockObj)
                {
                    return this.recoveries.Count;
                }
            }
        }
    }
}

