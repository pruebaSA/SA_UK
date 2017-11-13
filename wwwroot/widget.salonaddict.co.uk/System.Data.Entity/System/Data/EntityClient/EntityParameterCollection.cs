namespace System.Data.EntityClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Reflection;

    public sealed class EntityParameterCollection : DbParameterCollection
    {
        private bool _isDirty;
        private List<EntityParameter> _items;
        private static Type ItemType = typeof(EntityParameter);

        internal EntityParameterCollection()
        {
        }

        public EntityParameter Add(EntityParameter value)
        {
            this.Add(value);
            return value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int Add(object value)
        {
            this.OnChange();
            this.ValidateType(value);
            this.Validate(-1, value);
            this.InnerList.Add((EntityParameter) value);
            return (this.Count - 1);
        }

        public EntityParameter Add(string parameterName, DbType dbType) => 
            this.Add(new EntityParameter(parameterName, dbType));

        public EntityParameter Add(string parameterName, DbType dbType, int size) => 
            this.Add(new EntityParameter(parameterName, dbType, size));

        public void AddRange(EntityParameter[] values)
        {
            this.AddRange(values);
        }

        public override void AddRange(Array values)
        {
            this.OnChange();
            if (values == null)
            {
                throw ADP.ArgumentNull("values");
            }
            foreach (object obj2 in values)
            {
                this.ValidateType(obj2);
            }
            foreach (EntityParameter parameter in values)
            {
                this.Validate(-1, parameter);
                this.InnerList.Add(parameter);
            }
        }

        public EntityParameter AddWithValue(string parameterName, object value)
        {
            EntityParameter parameter = new EntityParameter {
                ParameterName = parameterName,
                Value = value
            };
            return this.Add(parameter);
        }

        private int CheckName(string parameterName)
        {
            int index = this.IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, ItemType);
            }
            return index;
        }

        public override void Clear()
        {
            this.OnChange();
            List<EntityParameter> innerList = this.InnerList;
            if (innerList != null)
            {
                foreach (EntityParameter parameter in innerList)
                {
                    parameter.ResetParent();
                }
                innerList.Clear();
            }
        }

        public override bool Contains(object value) => 
            (-1 != this.IndexOf(value));

        public override bool Contains(string parameterName) => 
            (this.IndexOf(parameterName) != -1);

        public void CopyTo(EntityParameter[] array, int index)
        {
            this.CopyTo(array, index);
        }

        public override void CopyTo(Array array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator() => 
            this.InnerList.GetEnumerator();

        protected override DbParameter GetParameter(int index)
        {
            this.RangeCheck(index);
            return this.InnerList[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            int index = this.IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, ItemType);
            }
            return this.InnerList[index];
        }

        public int IndexOf(EntityParameter value) => 
            this.IndexOf(value);

        public override int IndexOf(object value)
        {
            if (value != null)
            {
                this.ValidateType(value);
                List<EntityParameter> innerList = this.InnerList;
                if (innerList != null)
                {
                    int count = innerList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (value == innerList[i])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public override int IndexOf(string parameterName) => 
            IndexOf(this.InnerList, parameterName);

        private static int IndexOf(IEnumerable items, string parameterName)
        {
            if (items != null)
            {
                int num = 0;
                foreach (EntityParameter parameter in items)
                {
                    if (ADP.SrcCompare(parameterName, parameter.ParameterName) == 0)
                    {
                        return num;
                    }
                    num++;
                }
                num = 0;
                foreach (EntityParameter parameter2 in items)
                {
                    if (ADP.DstCompare(parameterName, parameter2.ParameterName) == 0)
                    {
                        return num;
                    }
                    num++;
                }
            }
            return -1;
        }

        public void Insert(int index, EntityParameter value)
        {
            this.Insert(index, value);
        }

        public override void Insert(int index, object value)
        {
            this.OnChange();
            this.ValidateType(value);
            this.Validate(-1, (EntityParameter) value);
            this.InnerList.Insert(index, (EntityParameter) value);
        }

        private void OnChange()
        {
            this._isDirty = true;
        }

        private void RangeCheck(int index)
        {
            if ((index < 0) || (this.Count <= index))
            {
                throw ADP.ParametersMappingIndex(index, this);
            }
        }

        public void Remove(EntityParameter value)
        {
            this.Remove(value);
        }

        public override void Remove(object value)
        {
            this.OnChange();
            this.ValidateType(value);
            int index = this.IndexOf(value);
            if (-1 != index)
            {
                this.RemoveIndex(index);
            }
            else if (this != ((EntityParameter) value).CompareExchangeParent(null, this))
            {
                throw ADP.CollectionRemoveInvalidObject(ItemType, this);
            }
        }

        public override void RemoveAt(int index)
        {
            this.OnChange();
            this.RangeCheck(index);
            this.RemoveIndex(index);
        }

        public override void RemoveAt(string parameterName)
        {
            this.OnChange();
            int index = this.CheckName(parameterName);
            this.RemoveIndex(index);
        }

        private void RemoveIndex(int index)
        {
            List<EntityParameter> innerList = this.InnerList;
            EntityParameter parameter = innerList[index];
            innerList.RemoveAt(index);
            parameter.ResetParent();
        }

        private void Replace(int index, object newValue)
        {
            List<EntityParameter> innerList = this.InnerList;
            this.ValidateType(newValue);
            this.Validate(index, newValue);
            EntityParameter parameter = innerList[index];
            innerList[index] = (EntityParameter) newValue;
            parameter.ResetParent();
        }

        internal void ResetIsDirty()
        {
            this._isDirty = false;
            foreach (EntityParameter parameter in this)
            {
                parameter.ResetIsDirty();
            }
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            this.OnChange();
            this.RangeCheck(index);
            this.Replace(index, value);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            this.OnChange();
            int index = this.IndexOf(parameterName);
            if (index < 0)
            {
                throw ADP.ParametersSourceIndex(parameterName, this, ItemType);
            }
            this.Replace(index, value);
        }

        private void Validate(int index, object value)
        {
            if (value == null)
            {
                throw ADP.ParameterNull("value", this, ItemType);
            }
            object obj2 = ((EntityParameter) value).CompareExchangeParent(this, null);
            if (obj2 != null)
            {
                if (this != obj2)
                {
                    throw ADP.ParametersIsNotParent(ItemType, this);
                }
                if (index != this.IndexOf(value))
                {
                    throw ADP.ParametersIsParent(ItemType, this);
                }
            }
            if (((EntityParameter) value).ParameterName.Length == 0)
            {
                string str;
                index = 1;
                do
                {
                    str = "Parameter" + index.ToString(CultureInfo.CurrentCulture);
                    index++;
                }
                while (-1 != this.IndexOf(str));
                ((EntityParameter) value).ParameterName = str;
            }
        }

        private void ValidateType(object value)
        {
            if (value == null)
            {
                throw ADP.ParameterNull("value", this, ItemType);
            }
            if (!ItemType.IsInstanceOfType(value))
            {
                throw ADP.InvalidParameterType(this, ItemType, value);
            }
        }

        public override int Count =>
            this._items?.Count;

        private List<EntityParameter> InnerList
        {
            get
            {
                List<EntityParameter> list = this._items;
                if (list == null)
                {
                    list = new List<EntityParameter>();
                    this._items = list;
                }
                return list;
            }
        }

        internal bool IsDirty
        {
            get
            {
                if (this._isDirty)
                {
                    return true;
                }
                foreach (EntityParameter parameter in this)
                {
                    if (parameter.IsDirty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool IsFixedSize =>
            ((IList) this.InnerList).IsFixedSize;

        public override bool IsReadOnly =>
            ((IList) this.InnerList).IsReadOnly;

        public override bool IsSynchronized =>
            ((ICollection) this.InnerList).IsSynchronized;

        public EntityParameter this[int index]
        {
            get => 
                ((EntityParameter) this.GetParameter(index));
            set
            {
                this.SetParameter(index, value);
            }
        }

        public EntityParameter this[string parameterName]
        {
            get => 
                ((EntityParameter) this.GetParameter(parameterName));
            set
            {
                this.SetParameter(parameterName, value);
            }
        }

        public override object SyncRoot =>
            ((ICollection) this.InnerList).SyncRoot;
    }
}

