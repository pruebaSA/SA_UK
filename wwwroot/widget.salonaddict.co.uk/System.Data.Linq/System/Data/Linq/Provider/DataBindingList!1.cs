namespace System.Data.Linq.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;

    internal class DataBindingList<TEntity> : SortableBindingList<TEntity> where TEntity: class
    {
        private bool addingNewInstance;
        private TEntity addNewInstance;
        private TEntity cancelNewInstance;
        private Table<TEntity> data;

        internal DataBindingList(IList<TEntity> sequence, Table<TEntity> data) : base((sequence != null) ? sequence : new List<TEntity>())
        {
            if (sequence == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("sequence");
            }
            if (data == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("data");
            }
            this.data = data;
        }

        protected override object AddNewCore()
        {
            this.addingNewInstance = true;
            this.addNewInstance = (TEntity) base.AddNewCore();
            return this.addNewInstance;
        }

        public override void CancelNew(int itemIndex)
        {
            if (((itemIndex >= 0) && (itemIndex < base.Count)) && (base[itemIndex] == this.addNewInstance))
            {
                this.cancelNewInstance = this.addNewInstance;
                this.addNewInstance = default(TEntity);
                this.addingNewInstance = false;
            }
            base.CancelNew(itemIndex);
        }

        protected override void ClearItems()
        {
            this.data.DeleteAllOnSubmit<TEntity>(this.data.ToList<TEntity>());
            base.ClearItems();
        }

        public override void EndNew(int itemIndex)
        {
            if (((itemIndex >= 0) && (itemIndex < base.Count)) && (base[itemIndex] == this.addNewInstance))
            {
                this.data.InsertOnSubmit(this.addNewInstance);
                this.addNewInstance = default(TEntity);
                this.addingNewInstance = false;
            }
            base.EndNew(itemIndex);
        }

        protected override void InsertItem(int index, TEntity item)
        {
            base.InsertItem(index, item);
            if ((!this.addingNewInstance && (index >= 0)) && (index <= base.Count))
            {
                this.data.InsertOnSubmit(item);
            }
        }

        protected override void RemoveItem(int index)
        {
            if (((index >= 0) && (index < base.Count)) && (base[index] == this.cancelNewInstance))
            {
                this.cancelNewInstance = default(TEntity);
            }
            else
            {
                this.data.DeleteOnSubmit(base[index]);
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TEntity item)
        {
            TEntity entity = base[index];
            base.SetItem(index, item);
            if ((index >= 0) && (index < base.Count))
            {
                if (entity == this.addNewInstance)
                {
                    this.addNewInstance = default(TEntity);
                    this.addingNewInstance = false;
                }
                else
                {
                    this.data.DeleteOnSubmit(entity);
                }
                this.data.InsertOnSubmit(item);
            }
        }
    }
}

