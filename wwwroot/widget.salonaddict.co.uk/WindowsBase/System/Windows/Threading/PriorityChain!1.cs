namespace System.Windows.Threading
{
    using System;

    internal class PriorityChain<T>
    {
        private int _count;
        private PriorityItem<T> _head;
        private DispatcherPriority _priority;
        private PriorityItem<T> _tail;

        public PriorityChain(DispatcherPriority priority)
        {
            this._priority = priority;
        }

        public int Count
        {
            get => 
                this._count;
            set
            {
                this._count = value;
            }
        }

        public PriorityItem<T> Head
        {
            get => 
                this._head;
            set
            {
                this._head = value;
            }
        }

        public DispatcherPriority Priority
        {
            get => 
                this._priority;
            set
            {
                this._priority = value;
            }
        }

        public PriorityItem<T> Tail
        {
            get => 
                this._tail;
            set
            {
                this._tail = value;
            }
        }
    }
}

