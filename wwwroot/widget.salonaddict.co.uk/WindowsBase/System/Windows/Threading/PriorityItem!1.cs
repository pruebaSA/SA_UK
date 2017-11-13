namespace System.Windows.Threading
{
    using System;

    internal class PriorityItem<T>
    {
        private PriorityChain<T> _chain;
        private T _data;
        private PriorityItem<T> _priorityNext;
        private PriorityItem<T> _priorityPrev;
        private PriorityItem<T> _sequentialNext;
        private PriorityItem<T> _sequentialPrev;

        public PriorityItem(T data)
        {
            this._data = data;
        }

        internal PriorityChain<T> Chain
        {
            get => 
                this._chain;
            set
            {
                this._chain = value;
            }
        }

        public T Data =>
            this._data;

        public bool IsQueued =>
            (this._chain != null);

        internal PriorityItem<T> PriorityNext
        {
            get => 
                this._priorityNext;
            set
            {
                this._priorityNext = value;
            }
        }

        internal PriorityItem<T> PriorityPrev
        {
            get => 
                this._priorityPrev;
            set
            {
                this._priorityPrev = value;
            }
        }

        internal PriorityItem<T> SequentialNext
        {
            get => 
                this._sequentialNext;
            set
            {
                this._sequentialNext = value;
            }
        }

        internal PriorityItem<T> SequentialPrev
        {
            get => 
                this._sequentialPrev;
            set
            {
                this._sequentialPrev = value;
            }
        }
    }
}

