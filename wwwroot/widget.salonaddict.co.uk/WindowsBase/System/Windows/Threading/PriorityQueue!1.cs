namespace System.Windows.Threading
{
    using System;
    using System.Collections.Generic;

    internal class PriorityQueue<T>
    {
        private Stack<PriorityChain<T>> _cacheReusableChains;
        private int _count;
        private PriorityItem<T> _head;
        private SortedList<int, PriorityChain<T>> _priorityChains;
        private PriorityItem<T> _tail;

        public PriorityQueue()
        {
            this._priorityChains = new SortedList<int, PriorityChain<T>>();
            this._cacheReusableChains = new Stack<PriorityChain<T>>(10);
            this._head = (PriorityItem<T>) (this._tail = null);
            this._count = 0;
        }

        public void ChangeItemPriority(PriorityItem<T> item, DispatcherPriority priority)
        {
            this.RemoveItemFromPriorityChain(item);
            PriorityChain<T> chain = this.GetChain(priority);
            this.InsertItemInPriorityChain(item, chain);
        }

        public T Dequeue()
        {
            int count = this._priorityChains.Count;
            if (count <= 0)
            {
                throw new InvalidOperationException();
            }
            PriorityChain<T> chain = this._priorityChains.Values[count - 1];
            PriorityItem<T> head = chain.Head;
            this.RemoveItem(head);
            return head.Data;
        }

        public PriorityItem<T> Enqueue(DispatcherPriority priority, T data)
        {
            PriorityChain<T> chain = this.GetChain(priority);
            PriorityItem<T> item = new PriorityItem<T>(data);
            this.InsertItemInSequentialChain(item, this._tail);
            this.InsertItemInPriorityChain(item, chain, chain.Tail);
            return item;
        }

        private PriorityChain<T> GetChain(DispatcherPriority priority)
        {
            PriorityChain<T> chain = null;
            int count = this._priorityChains.Count;
            if (count > 0)
            {
                if (priority == this._priorityChains.Keys[0])
                {
                    chain = this._priorityChains.Values[0];
                }
                else if (priority == this._priorityChains.Keys[count - 1])
                {
                    chain = this._priorityChains.Values[count - 1];
                }
                else if ((priority > this._priorityChains.Keys[0]) && (priority < this._priorityChains.Keys[count - 1]))
                {
                    this._priorityChains.TryGetValue((int) priority, out chain);
                }
            }
            if (chain == null)
            {
                if (this._cacheReusableChains.Count > 0)
                {
                    chain = this._cacheReusableChains.Pop();
                    chain.Priority = priority;
                }
                else
                {
                    chain = new PriorityChain<T>(priority);
                }
                this._priorityChains.Add((int) priority, chain);
            }
            return chain;
        }

        private void InsertItemInPriorityChain(PriorityItem<T> item, PriorityChain<T> chain)
        {
            if (chain.Head == null)
            {
                this.InsertItemInPriorityChain(item, chain, null);
            }
            else
            {
                PriorityItem<T> after = null;
                after = item.SequentialPrev;
                while (after != null)
                {
                    if (after.Chain == chain)
                    {
                        break;
                    }
                    after = after.SequentialPrev;
                }
                this.InsertItemInPriorityChain(item, chain, after);
            }
        }

        internal void InsertItemInPriorityChain(PriorityItem<T> item, PriorityChain<T> chain, PriorityItem<T> after)
        {
            item.Chain = chain;
            if (after == null)
            {
                if (chain.Head != null)
                {
                    chain.Head.PriorityPrev = item;
                    item.PriorityNext = chain.Head;
                    chain.Head = item;
                }
                else
                {
                    chain.Head = chain.Tail = item;
                }
            }
            else
            {
                item.PriorityPrev = after;
                if (after.PriorityNext != null)
                {
                    item.PriorityNext = after.PriorityNext;
                    after.PriorityNext.PriorityPrev = item;
                    after.PriorityNext = item;
                }
                else
                {
                    after.PriorityNext = item;
                    chain.Tail = item;
                }
            }
            chain.Count++;
        }

        internal void InsertItemInSequentialChain(PriorityItem<T> item, PriorityItem<T> after)
        {
            if (after == null)
            {
                if (this._head != null)
                {
                    this._head.SequentialPrev = item;
                    item.SequentialNext = this._head;
                    this._head = item;
                }
                else
                {
                    this._head = this._tail = item;
                }
            }
            else
            {
                item.SequentialPrev = after;
                if (after.SequentialNext != null)
                {
                    item.SequentialNext = after.SequentialNext;
                    after.SequentialNext.SequentialPrev = item;
                    after.SequentialNext = item;
                }
                else
                {
                    after.SequentialNext = item;
                    this._tail = item;
                }
            }
            this._count++;
        }

        public T Peek()
        {
            T data = default(T);
            int count = this._priorityChains.Count;
            if (count > 0)
            {
                PriorityChain<T> chain = this._priorityChains.Values[count - 1];
                data = chain.Head.Data;
            }
            return data;
        }

        public void RemoveItem(PriorityItem<T> item)
        {
            PriorityChain<T> chain = item.Chain;
            this.RemoveItemFromPriorityChain(item);
            this.RemoveItemFromSequentialChain(item);
        }

        private void RemoveItemFromPriorityChain(PriorityItem<T> item)
        {
            if (item.PriorityPrev != null)
            {
                item.PriorityPrev.PriorityNext = item.PriorityNext;
            }
            else
            {
                item.Chain.Head = item.PriorityNext;
            }
            if (item.PriorityNext != null)
            {
                item.PriorityNext.PriorityPrev = item.PriorityPrev;
            }
            else
            {
                item.Chain.Tail = item.PriorityPrev;
            }
            item.PriorityPrev = (PriorityItem<T>) (item.PriorityNext = null);
            PriorityChain<T> chain = item.Chain;
            chain.Count--;
            if (item.Chain.Count == 0)
            {
                if (item.Chain.Priority == this._priorityChains.Keys[this._priorityChains.Count - 1])
                {
                    this._priorityChains.RemoveAt(this._priorityChains.Count - 1);
                }
                else
                {
                    this._priorityChains.Remove((int) item.Chain.Priority);
                }
                if (this._cacheReusableChains.Count < 10)
                {
                    item.Chain.Priority = DispatcherPriority.Invalid;
                    this._cacheReusableChains.Push(item.Chain);
                }
            }
            item.Chain = null;
        }

        private void RemoveItemFromSequentialChain(PriorityItem<T> item)
        {
            if (item.SequentialPrev != null)
            {
                item.SequentialPrev.SequentialNext = item.SequentialNext;
            }
            else
            {
                this._head = item.SequentialNext;
            }
            if (item.SequentialNext != null)
            {
                item.SequentialNext.SequentialPrev = item.SequentialPrev;
            }
            else
            {
                this._tail = item.SequentialPrev;
            }
            item.SequentialPrev = (PriorityItem<T>) (item.SequentialNext = null);
            this._count--;
        }

        public DispatcherPriority MaxPriority
        {
            get
            {
                int count = this._priorityChains.Count;
                if (count > 0)
                {
                    return this._priorityChains.Keys[count - 1];
                }
                return DispatcherPriority.Invalid;
            }
        }
    }
}

