namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class InputQueue<T> : IDisposable where T: class
    {
        private static WaitCallback completeOutstandingReadersCallback;
        private static WaitCallback completeWaitersFalseCallback;
        private static WaitCallback completeWaitersTrueCallback;
        private ItemQueue<T> itemQueue;
        private static WaitCallback onDispatchCallback;
        private static WaitCallback onInvokeDequeuedCallback;
        private QueueState<T> queueState;
        private Queue<IQueueReader<T>> readerQueue;
        private List<IQueueWaiter<T>> waiterList;

        public InputQueue()
        {
            this.itemQueue = new ItemQueue<T>();
            this.readerQueue = new Queue<IQueueReader<T>>();
            this.waiterList = new List<IQueueWaiter<T>>();
            this.queueState = QueueState<T>.Open;
        }

        public IAsyncResult BeginDequeue(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Item<T> item = new Item<T>();
            lock (this.ThisLock)
            {
                if (this.queueState == QueueState<T>.Open)
                {
                    if (!this.itemQueue.HasAvailableItem)
                    {
                        AsyncQueueReader<T> reader = new AsyncQueueReader<T>((InputQueue<T>) this, timeout, callback, state);
                        this.readerQueue.Enqueue(reader);
                        return reader;
                    }
                    item = this.itemQueue.DequeueAvailableItem();
                }
                else if (this.queueState == QueueState<T>.Shutdown)
                {
                    if (this.itemQueue.HasAvailableItem)
                    {
                        item = this.itemQueue.DequeueAvailableItem();
                    }
                    else if (this.itemQueue.HasAnyItem)
                    {
                        AsyncQueueReader<T> reader2 = new AsyncQueueReader<T>((InputQueue<T>) this, timeout, callback, state);
                        this.readerQueue.Enqueue(reader2);
                        return reader2;
                    }
                }
            }
            InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
            return new TypedCompletedAsyncResult<T>(item.GetValue(), callback, state);
        }

        public IAsyncResult BeginWaitForItem(TimeSpan timeout, AsyncCallback callback, object state)
        {
            lock (this.ThisLock)
            {
                if (this.queueState == QueueState<T>.Open)
                {
                    if (!this.itemQueue.HasAvailableItem)
                    {
                        AsyncQueueWaiter<T> item = new AsyncQueueWaiter<T>(timeout, callback, state);
                        this.waiterList.Add(item);
                        return item;
                    }
                }
                else if (((this.queueState == QueueState<T>.Shutdown) && !this.itemQueue.HasAvailableItem) && this.itemQueue.HasAnyItem)
                {
                    AsyncQueueWaiter<T> waiter2 = new AsyncQueueWaiter<T>(timeout, callback, state);
                    this.waiterList.Add(waiter2);
                    return waiter2;
                }
            }
            return new TypedCompletedAsyncResult<bool>(true, callback, state);
        }

        public void Close()
        {
            this.Dispose();
        }

        private static void CompleteOutstandingReadersCallback(object state)
        {
            IQueueReader<T>[] readerArray = (IQueueReader<T>[]) state;
            for (int i = 0; i < readerArray.Length; i++)
            {
                Item<T> item = new Item<T>();
                readerArray[i].Set(item);
            }
        }

        private static void CompleteWaiters(bool itemAvailable, IQueueWaiter<T>[] waiters)
        {
            for (int i = 0; i < waiters.Length; i++)
            {
                waiters[i].Set(itemAvailable);
            }
        }

        private static void CompleteWaitersFalseCallback(object state)
        {
            InputQueue<T>.CompleteWaiters(false, (IQueueWaiter<T>[]) state);
        }

        private static void CompleteWaitersLater(bool itemAvailable, IQueueWaiter<T>[] waiters)
        {
            if (itemAvailable)
            {
                if (InputQueue<T>.completeWaitersTrueCallback == null)
                {
                    InputQueue<T>.completeWaitersTrueCallback = new WaitCallback(InputQueue<T>.CompleteWaitersTrueCallback);
                }
                IOThreadScheduler.ScheduleCallback(InputQueue<T>.completeWaitersTrueCallback, waiters);
            }
            else
            {
                if (InputQueue<T>.completeWaitersFalseCallback == null)
                {
                    InputQueue<T>.completeWaitersFalseCallback = new WaitCallback(InputQueue<T>.CompleteWaitersFalseCallback);
                }
                IOThreadScheduler.ScheduleCallback(InputQueue<T>.completeWaitersFalseCallback, waiters);
            }
        }

        private static void CompleteWaitersTrueCallback(object state)
        {
            InputQueue<T>.CompleteWaiters(true, (IQueueWaiter<T>[]) state);
        }

        public T Dequeue(TimeSpan timeout)
        {
            T local;
            if (!this.Dequeue(timeout, out local))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(System.ServiceModel.SR.GetString("TimeoutInputQueueDequeue1", new object[] { timeout })));
            }
            return local;
        }

        public bool Dequeue(TimeSpan timeout, out T value)
        {
            WaitQueueReader<T> reader = null;
            Item<T> item = new Item<T>();
            lock (this.ThisLock)
            {
                if (this.queueState == QueueState<T>.Open)
                {
                    if (this.itemQueue.HasAvailableItem)
                    {
                        item = this.itemQueue.DequeueAvailableItem();
                    }
                    else
                    {
                        reader = new WaitQueueReader<T>((InputQueue<T>) this);
                        this.readerQueue.Enqueue(reader);
                    }
                }
                else if (this.queueState == QueueState<T>.Shutdown)
                {
                    if (!this.itemQueue.HasAvailableItem)
                    {
                        if (!this.itemQueue.HasAnyItem)
                        {
                            value = default(T);
                            return true;
                        }
                        reader = new WaitQueueReader<T>((InputQueue<T>) this);
                        this.readerQueue.Enqueue(reader);
                    }
                    else
                    {
                        item = this.itemQueue.DequeueAvailableItem();
                    }
                }
                else
                {
                    value = default(T);
                    return true;
                }
            }
            if (reader != null)
            {
                return reader.Wait(timeout, out value);
            }
            InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
            value = item.GetValue();
            return true;
        }

        public void Dispatch()
        {
            IQueueReader<T> reader = null;
            Item<T> item = new Item<T>();
            IQueueReader<T>[] array = null;
            IQueueWaiter<T>[] waiters = null;
            bool itemAvailable = true;
            lock (this.ThisLock)
            {
                itemAvailable = (this.queueState != QueueState<T>.Closed) && (this.queueState != QueueState<T>.Shutdown);
                this.GetWaiters(out waiters);
                if (this.queueState != QueueState<T>.Closed)
                {
                    this.itemQueue.MakePendingItemAvailable();
                    if (this.readerQueue.Count > 0)
                    {
                        item = this.itemQueue.DequeueAvailableItem();
                        reader = this.readerQueue.Dequeue();
                        if (((this.queueState == QueueState<T>.Shutdown) && (this.readerQueue.Count > 0)) && (this.itemQueue.ItemCount == 0))
                        {
                            array = new IQueueReader<T>[this.readerQueue.Count];
                            this.readerQueue.CopyTo(array, 0);
                            this.readerQueue.Clear();
                            itemAvailable = false;
                        }
                    }
                }
            }
            if (array != null)
            {
                if (InputQueue<T>.completeOutstandingReadersCallback == null)
                {
                    InputQueue<T>.completeOutstandingReadersCallback = new WaitCallback(InputQueue<T>.CompleteOutstandingReadersCallback);
                }
                IOThreadScheduler.ScheduleCallback(InputQueue<T>.completeOutstandingReadersCallback, array);
            }
            if (waiters != null)
            {
                InputQueue<T>.CompleteWaitersLater(itemAvailable, waiters);
            }
            if (reader != null)
            {
                InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
                reader.Set(item);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                bool flag = false;
                lock (this.ThisLock)
                {
                    if (this.queueState != QueueState<T>.Closed)
                    {
                        this.queueState = QueueState<T>.Closed;
                        flag = true;
                    }
                }
                if (flag)
                {
                    while (this.readerQueue.Count > 0)
                    {
                        Item<T> item2 = new Item<T>();
                        this.readerQueue.Dequeue().Set(item2);
                    }
                    while (this.itemQueue.HasAnyItem)
                    {
                        Item<T> item = this.itemQueue.DequeueAnyItem();
                        item.Dispose();
                        InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
                    }
                }
            }
        }

        public T EndDequeue(IAsyncResult result)
        {
            T local;
            if (!this.EndDequeue(result, out local))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException());
            }
            return local;
        }

        public bool EndDequeue(IAsyncResult result, out T value)
        {
            if (result is TypedCompletedAsyncResult<T>)
            {
                value = TypedCompletedAsyncResult<T>.End(result);
                return true;
            }
            return AsyncQueueReader<T>.End(result, out value);
        }

        public bool EndWaitForItem(IAsyncResult result)
        {
            if (result is TypedCompletedAsyncResult<bool>)
            {
                return TypedCompletedAsyncResult<bool>.End(result);
            }
            return AsyncQueueWaiter<T>.End(result);
        }

        public void EnqueueAndDispatch(T item)
        {
            this.EnqueueAndDispatch(item, null);
        }

        private void EnqueueAndDispatch(Item<T> item, bool canDispatchOnThisThread)
        {
            bool flag = false;
            IQueueReader<T> reader = null;
            bool flag2 = false;
            IQueueWaiter<T>[] waiters = null;
            bool itemAvailable = true;
            lock (this.ThisLock)
            {
                itemAvailable = (this.queueState != QueueState<T>.Closed) && (this.queueState != QueueState<T>.Shutdown);
                this.GetWaiters(out waiters);
                if (this.queueState == QueueState<T>.Open)
                {
                    if (canDispatchOnThisThread)
                    {
                        if (this.readerQueue.Count == 0)
                        {
                            this.itemQueue.EnqueueAvailableItem(item);
                        }
                        else
                        {
                            reader = this.readerQueue.Dequeue();
                        }
                    }
                    else if (this.readerQueue.Count == 0)
                    {
                        this.itemQueue.EnqueueAvailableItem(item);
                    }
                    else
                    {
                        this.itemQueue.EnqueuePendingItem(item);
                        flag2 = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
            if (waiters != null)
            {
                if (canDispatchOnThisThread)
                {
                    InputQueue<T>.CompleteWaiters(itemAvailable, waiters);
                }
                else
                {
                    InputQueue<T>.CompleteWaitersLater(itemAvailable, waiters);
                }
            }
            if (reader != null)
            {
                InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
                reader.Set(item);
            }
            if (flag2)
            {
                if (InputQueue<T>.onDispatchCallback == null)
                {
                    InputQueue<T>.onDispatchCallback = new WaitCallback(InputQueue<T>.OnDispatchCallback);
                }
                IOThreadScheduler.ScheduleCallback(InputQueue<T>.onDispatchCallback, this);
            }
            else if (flag)
            {
                InputQueue<T>.InvokeDequeuedCallback(item.DequeuedCallback);
                item.Dispose();
            }
        }

        public void EnqueueAndDispatch(T item, ItemDequeuedCallback dequeuedCallback)
        {
            this.EnqueueAndDispatch(item, dequeuedCallback, true);
        }

        public void EnqueueAndDispatch(Exception exception, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            this.EnqueueAndDispatch(new Item<T>(exception, dequeuedCallback), canDispatchOnThisThread);
        }

        public void EnqueueAndDispatch(T item, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            this.EnqueueAndDispatch(new Item<T>(item, dequeuedCallback), canDispatchOnThisThread);
        }

        private bool EnqueueWithoutDispatch(Item<T> item)
        {
            lock (this.ThisLock)
            {
                if ((this.queueState != QueueState<T>.Closed) && (this.queueState != QueueState<T>.Shutdown))
                {
                    if (this.readerQueue.Count == 0)
                    {
                        this.itemQueue.EnqueueAvailableItem(item);
                        return false;
                    }
                    this.itemQueue.EnqueuePendingItem(item);
                    return true;
                }
            }
            item.Dispose();
            InputQueue<T>.InvokeDequeuedCallbackLater(item.DequeuedCallback);
            return false;
        }

        public bool EnqueueWithoutDispatch(T item, ItemDequeuedCallback dequeuedCallback) => 
            this.EnqueueWithoutDispatch(new Item<T>(item, dequeuedCallback));

        public bool EnqueueWithoutDispatch(Exception exception, ItemDequeuedCallback dequeuedCallback) => 
            this.EnqueueWithoutDispatch(new Item<T>(exception, dequeuedCallback));

        private void GetWaiters(out IQueueWaiter<T>[] waiters)
        {
            if (this.waiterList.Count > 0)
            {
                waiters = this.waiterList.ToArray();
                this.waiterList.Clear();
            }
            else
            {
                waiters = null;
            }
        }

        private static void InvokeDequeuedCallback(ItemDequeuedCallback dequeuedCallback)
        {
            if (dequeuedCallback != null)
            {
                dequeuedCallback();
            }
        }

        private static void InvokeDequeuedCallbackLater(ItemDequeuedCallback dequeuedCallback)
        {
            if (dequeuedCallback != null)
            {
                if (InputQueue<T>.onInvokeDequeuedCallback == null)
                {
                    InputQueue<T>.onInvokeDequeuedCallback = new WaitCallback(InputQueue<T>.OnInvokeDequeuedCallback);
                }
                IOThreadScheduler.ScheduleCallback(InputQueue<T>.onInvokeDequeuedCallback, dequeuedCallback);
            }
        }

        private static void OnDispatchCallback(object state)
        {
            ((InputQueue<T>) state).Dispatch();
        }

        private static void OnInvokeDequeuedCallback(object state)
        {
            ItemDequeuedCallback callback = (ItemDequeuedCallback) state;
            callback();
        }

        private bool RemoveReader(IQueueReader<T> reader)
        {
            lock (this.ThisLock)
            {
                if ((this.queueState == QueueState<T>.Open) || (this.queueState == QueueState<T>.Shutdown))
                {
                    bool flag = false;
                    for (int i = this.readerQueue.Count; i > 0; i--)
                    {
                        IQueueReader<T> objA = this.readerQueue.Dequeue();
                        if (object.ReferenceEquals(objA, reader))
                        {
                            flag = true;
                        }
                        else
                        {
                            this.readerQueue.Enqueue(objA);
                        }
                    }
                    return flag;
                }
            }
            return false;
        }

        public void Shutdown()
        {
            this.Shutdown(null);
        }

        public void Shutdown(CommunicationObject communicationObject)
        {
            IQueueReader<T>[] array = null;
            lock (this.ThisLock)
            {
                if ((this.queueState == QueueState<T>.Shutdown) || (this.queueState == QueueState<T>.Closed))
                {
                    return;
                }
                this.queueState = QueueState<T>.Shutdown;
                if ((this.readerQueue.Count > 0) && (this.itemQueue.ItemCount == 0))
                {
                    array = new IQueueReader<T>[this.readerQueue.Count];
                    this.readerQueue.CopyTo(array, 0);
                    this.readerQueue.Clear();
                }
            }
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Exception pendingException = communicationObject?.GetPendingException();
                    array[i].Set(new Item<T>(pendingException, null));
                }
            }
        }

        public bool WaitForItem(TimeSpan timeout)
        {
            WaitQueueWaiter<T> item = null;
            bool flag = false;
            lock (this.ThisLock)
            {
                if (this.queueState == QueueState<T>.Open)
                {
                    if (this.itemQueue.HasAvailableItem)
                    {
                        flag = true;
                    }
                    else
                    {
                        item = new WaitQueueWaiter<T>();
                        this.waiterList.Add(item);
                    }
                }
                else if (this.queueState == QueueState<T>.Shutdown)
                {
                    if (!this.itemQueue.HasAvailableItem)
                    {
                        if (!this.itemQueue.HasAnyItem)
                        {
                            return true;
                        }
                        item = new WaitQueueWaiter<T>();
                        this.waiterList.Add(item);
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    return true;
                }
            }
            if (item != null)
            {
                return item.Wait(timeout);
            }
            return flag;
        }

        public int PendingCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.itemQueue.ItemCount;
                }
            }
        }

        private object ThisLock =>
            this.itemQueue;

        private class AsyncQueueReader : TraceAsyncResult, InputQueue<T>.IQueueReader
        {
            private bool expired;
            private InputQueue<T> inputQueue;
            private T item;
            private IOThreadTimer timer;
            private static WaitCallback timerCallback;

            static AsyncQueueReader()
            {
                InputQueue<T>.AsyncQueueReader.timerCallback = new WaitCallback(InputQueue<T>.AsyncQueueReader.TimerCallback);
            }

            public AsyncQueueReader(InputQueue<T> inputQueue, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.inputQueue = inputQueue;
                if (timeout != TimeSpan.MaxValue)
                {
                    this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueReader.timerCallback, this, false);
                    this.timer.Set(timeout);
                }
            }

            public static bool End(IAsyncResult result, out T value)
            {
                InputQueue<T>.AsyncQueueReader reader = AsyncResult.End<InputQueue<T>.AsyncQueueReader>(result);
                if (reader.expired)
                {
                    value = default(T);
                    return false;
                }
                value = reader.item;
                return true;
            }

            public void Set(InputQueue<T>.Item item)
            {
                this.item = item.Value;
                if (this.timer != null)
                {
                    this.timer.Cancel();
                }
                base.Complete(false, item.Exception);
            }

            private static void TimerCallback(object state)
            {
                InputQueue<T>.AsyncQueueReader reader = (InputQueue<T>.AsyncQueueReader) state;
                if (reader.inputQueue.RemoveReader(reader))
                {
                    reader.expired = true;
                    reader.Complete(false);
                }
            }
        }

        private class AsyncQueueWaiter : AsyncResult, InputQueue<T>.IQueueWaiter
        {
            private bool itemAvailable;
            private object thisLock;
            private IOThreadTimer timer;
            private static WaitCallback timerCallback;

            static AsyncQueueWaiter()
            {
                InputQueue<T>.AsyncQueueWaiter.timerCallback = new WaitCallback(InputQueue<T>.AsyncQueueWaiter.TimerCallback);
            }

            public AsyncQueueWaiter(TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.thisLock = new object();
                if (timeout != TimeSpan.MaxValue)
                {
                    this.timer = new IOThreadTimer(InputQueue<T>.AsyncQueueWaiter.timerCallback, this, false);
                    this.timer.Set(timeout);
                }
            }

            public static bool End(IAsyncResult result) => 
                AsyncResult.End<InputQueue<T>.AsyncQueueWaiter>(result).itemAvailable;

            public void Set(bool itemAvailable)
            {
                bool flag;
                lock (this.ThisLock)
                {
                    flag = (this.timer == null) || this.timer.Cancel();
                    this.itemAvailable = itemAvailable;
                }
                if (flag)
                {
                    base.Complete(false);
                }
            }

            private static void TimerCallback(object state)
            {
                ((InputQueue<T>.AsyncQueueWaiter) state).Complete(false);
            }

            private object ThisLock =>
                this.thisLock;
        }

        private interface IQueueReader
        {
            void Set(InputQueue<T>.Item item);
        }

        private interface IQueueWaiter
        {
            void Set(bool itemAvailable);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Item
        {
            private T value;
            private System.Exception exception;
            private ItemDequeuedCallback dequeuedCallback;
            public Item(T value, ItemDequeuedCallback dequeuedCallback) : this(value, null, dequeuedCallback)
            {
            }

            public Item(System.Exception exception, ItemDequeuedCallback dequeuedCallback) : this(default(T), exception, dequeuedCallback)
            {
            }

            private Item(T value, System.Exception exception, ItemDequeuedCallback dequeuedCallback)
            {
                this.value = value;
                this.exception = exception;
                this.dequeuedCallback = dequeuedCallback;
            }

            public System.Exception Exception =>
                this.exception;
            public T Value =>
                this.value;
            public ItemDequeuedCallback DequeuedCallback =>
                this.dequeuedCallback;
            public void Dispose()
            {
                if (this.value != null)
                {
                    if (this.value is IDisposable)
                    {
                        ((IDisposable) this.value).Dispose();
                    }
                    else if (this.value is ICommunicationObject)
                    {
                        ((ICommunicationObject) this.value).Abort();
                    }
                }
            }

            public T GetValue()
            {
                if (this.exception != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.exception);
                }
                return this.value;
            }
        }

        private class ItemQueue
        {
            private int head;
            private InputQueue<T>.Item[] items;
            private int pendingCount;
            private int totalCount;

            public ItemQueue()
            {
                this.items = new InputQueue<T>.Item[1];
            }

            public InputQueue<T>.Item DequeueAnyItem()
            {
                if (this.pendingCount == this.totalCount)
                {
                    this.pendingCount--;
                }
                return this.DequeueItemCore();
            }

            public InputQueue<T>.Item DequeueAvailableItem()
            {
                if (this.totalCount == this.pendingCount)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                return this.DequeueItemCore();
            }

            private InputQueue<T>.Item DequeueItemCore()
            {
                if (this.totalCount == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                InputQueue<T>.Item item = this.items[this.head];
                this.items[this.head] = new InputQueue<T>.Item();
                this.totalCount--;
                this.head = (this.head + 1) % this.items.Length;
                return item;
            }

            public void EnqueueAvailableItem(InputQueue<T>.Item item)
            {
                this.EnqueueItemCore(item);
            }

            private void EnqueueItemCore(InputQueue<T>.Item item)
            {
                if (this.totalCount == this.items.Length)
                {
                    InputQueue<T>.Item[] itemArray = new InputQueue<T>.Item[this.items.Length * 2];
                    for (int i = 0; i < this.totalCount; i++)
                    {
                        itemArray[i] = this.items[(this.head + i) % this.items.Length];
                    }
                    this.head = 0;
                    this.items = itemArray;
                }
                int index = (this.head + this.totalCount) % this.items.Length;
                this.items[index] = item;
                this.totalCount++;
            }

            public void EnqueuePendingItem(InputQueue<T>.Item item)
            {
                this.EnqueueItemCore(item);
                this.pendingCount++;
            }

            public void MakePendingItemAvailable()
            {
                if (this.pendingCount == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                this.pendingCount--;
            }

            public bool HasAnyItem =>
                (this.totalCount > 0);

            public bool HasAvailableItem =>
                (this.totalCount > this.pendingCount);

            public int ItemCount =>
                this.totalCount;
        }

        private enum QueueState
        {
            public const InputQueue<T>.QueueState Closed = InputQueue<T>.QueueState.Closed;,
            public const InputQueue<T>.QueueState Open = InputQueue<T>.QueueState.Open;,
            public const InputQueue<T>.QueueState Shutdown = InputQueue<T>.QueueState.Shutdown;
        }

        private class WaitQueueReader : InputQueue<T>.IQueueReader
        {
            private Exception exception;
            private InputQueue<T> inputQueue;
            private T item;
            private ManualResetEvent waitEvent;

            public WaitQueueReader(InputQueue<T> inputQueue)
            {
                this.inputQueue = inputQueue;
                this.waitEvent = new ManualResetEvent(false);
            }

            public void Set(InputQueue<T>.Item item)
            {
                lock (((InputQueue<T>.WaitQueueReader) this))
                {
                    this.exception = item.Exception;
                    this.item = item.Value;
                    this.waitEvent.Set();
                }
            }

            public bool Wait(TimeSpan timeout, out T value)
            {
                bool flag = false;
                try
                {
                    if (!TimeoutHelper.WaitOne(this.waitEvent, timeout, false))
                    {
                        if (this.inputQueue.RemoveReader(this))
                        {
                            value = default(T);
                            flag = true;
                            return false;
                        }
                        this.waitEvent.WaitOne();
                    }
                    flag = true;
                }
                finally
                {
                    if (flag)
                    {
                        this.waitEvent.Close();
                    }
                }
                if (this.exception != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.exception);
                }
                value = this.item;
                return true;
            }
        }

        private class WaitQueueWaiter : InputQueue<T>.IQueueWaiter
        {
            private bool itemAvailable;
            private ManualResetEvent waitEvent;

            public WaitQueueWaiter()
            {
                this.waitEvent = new ManualResetEvent(false);
            }

            public void Set(bool itemAvailable)
            {
                lock (((InputQueue<T>.WaitQueueWaiter) this))
                {
                    this.itemAvailable = itemAvailable;
                    this.waitEvent.Set();
                }
            }

            public bool Wait(TimeSpan timeout)
            {
                if (!TimeoutHelper.WaitOne(this.waitEvent, timeout, false))
                {
                    return false;
                }
                return this.itemAvailable;
            }
        }
    }
}

