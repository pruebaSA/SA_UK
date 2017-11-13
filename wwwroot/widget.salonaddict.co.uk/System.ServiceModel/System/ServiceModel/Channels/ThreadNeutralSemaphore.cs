namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;

    internal class ThreadNeutralSemaphore
    {
        private bool aborted;
        private int count;
        private int maxCount;
        private object ThisLock = new object();
        private Queue<Waiter> waiters;

        public ThreadNeutralSemaphore(int maxCount)
        {
            if (maxCount < 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxCount", maxCount, System.ServiceModel.SR.GetString("ValueMustBePositive")));
            }
            this.maxCount = maxCount;
        }

        public void Abort()
        {
            lock (this.ThisLock)
            {
                if (!this.aborted)
                {
                    this.aborted = true;
                    if (this.waiters != null)
                    {
                        while (this.waiters.Count > 0)
                        {
                            this.waiters.Dequeue().Abort();
                        }
                    }
                }
            }
        }

        internal static TimeoutException CreateEnterTimedOutException(TimeSpan timeout) => 
            new TimeoutException(System.ServiceModel.SR.GetString("ThreadAcquisitionTimedOut", new object[] { timeout }));

        private static CommunicationObjectAbortedException CreateObjectAbortedException() => 
            new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("ThreadNeutralSemaphoreAborted"));

        public void Enter()
        {
            SyncWaiter waiter = this.EnterCore();
            if (waiter != null)
            {
                waiter.Wait();
            }
        }

        public void Enter(TimeSpan timeout)
        {
            if (!this.TryEnter(timeout))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateEnterTimedOutException(timeout));
            }
        }

        public bool Enter(WaitCallback callback, object state)
        {
            if (callback == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("callback");
            }
            lock (this.ThisLock)
            {
                if (this.count < this.maxCount)
                {
                    this.count++;
                    return true;
                }
                this.Waiters.Enqueue(new AsyncWaiter(callback, state));
                return false;
            }
        }

        private SyncWaiter EnterCore()
        {
            SyncWaiter waiter;
            lock (this.ThisLock)
            {
                if (this.aborted)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateObjectAbortedException());
                }
                if (this.count < this.maxCount)
                {
                    this.count++;
                    return null;
                }
                waiter = new SyncWaiter(this);
                this.Waiters.Enqueue(waiter);
            }
            return waiter;
        }

        public void Exit()
        {
            Waiter waiter;
            lock (this.ThisLock)
            {
                if (this.count == 0)
                {
                    string message = System.ServiceModel.SR.GetString("InvalidLockOperation");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SynchronizationLockException(message));
                }
                if ((this.waiters == null) || (this.waiters.Count == 0))
                {
                    this.count--;
                    return;
                }
                waiter = this.waiters.Dequeue();
            }
            waiter.Signal();
        }

        private bool RemoveWaiter(Waiter waiter)
        {
            bool flag = false;
            lock (this.ThisLock)
            {
                for (int i = this.Waiters.Count; i > 0; i--)
                {
                    Waiter objA = this.Waiters.Dequeue();
                    if (object.ReferenceEquals(objA, waiter))
                    {
                        flag = true;
                    }
                    else
                    {
                        this.Waiters.Enqueue(objA);
                    }
                }
            }
            return flag;
        }

        public bool TryEnter()
        {
            lock (this.ThisLock)
            {
                if (this.count < this.maxCount)
                {
                    this.count++;
                    return true;
                }
                return false;
            }
        }

        public bool TryEnter(TimeSpan timeout)
        {
            SyncWaiter waiter = this.EnterCore();
            if (waiter != null)
            {
                return waiter.Wait(timeout);
            }
            return true;
        }

        private Queue<Waiter> Waiters
        {
            get
            {
                if (this.waiters == null)
                {
                    this.waiters = new Queue<Waiter>();
                }
                return this.waiters;
            }
        }

        private class AsyncWaiter : ThreadNeutralSemaphore.Waiter
        {
            private WaitCallback callback;
            private object state;

            public AsyncWaiter(WaitCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
            }

            public override void Abort()
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ThreadNeutralSemaphoreAsyncAbort")));
            }

            public override void Signal()
            {
                IOThreadScheduler.ScheduleCallback(this.callback, this.state);
            }
        }

        private class SyncWaiter : ThreadNeutralSemaphore.Waiter
        {
            private bool aborted;
            private ThreadNeutralSemaphore parent;
            private AutoResetEvent waitHandle = new AutoResetEvent(false);

            public SyncWaiter(ThreadNeutralSemaphore parent)
            {
                this.parent = parent;
            }

            public override void Abort()
            {
                this.aborted = true;
                this.waitHandle.Set();
            }

            public override void Signal()
            {
                this.waitHandle.Set();
            }

            public void Wait()
            {
                this.waitHandle.WaitOne();
                this.waitHandle.Close();
                if (this.aborted)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ThreadNeutralSemaphore.CreateObjectAbortedException());
                }
            }

            public bool Wait(TimeSpan timeout)
            {
                bool flag = true;
                if (!TimeoutHelper.WaitOne(this.waitHandle, timeout, false))
                {
                    if (!this.parent.RemoveWaiter(this))
                    {
                        this.waitHandle.WaitOne();
                    }
                    else
                    {
                        flag = false;
                    }
                }
                this.waitHandle.Close();
                if (this.aborted)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ThreadNeutralSemaphore.CreateObjectAbortedException());
                }
                return flag;
            }
        }

        private abstract class Waiter
        {
            protected Waiter()
            {
            }

            public abstract void Abort();
            public abstract void Signal();
        }
    }
}

