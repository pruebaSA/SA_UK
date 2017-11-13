namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataKey : IStateManager
    {
        private bool _isTracking;
        private string[] _keyNames;
        private IOrderedDictionary _keyTable;

        public DataKey(IOrderedDictionary keyTable)
        {
            this._keyTable = keyTable;
        }

        public DataKey(IOrderedDictionary keyTable, string[] keyNames) : this(keyTable)
        {
            this._keyNames = keyNames;
        }

        protected virtual void LoadViewState(object state)
        {
            if (state != null)
            {
                if (this._keyNames == null)
                {
                    if (state != null)
                    {
                        ArrayList list = state as ArrayList;
                        if (list == null)
                        {
                            throw new HttpException(System.Web.SR.GetString("ViewState_InvalidViewState"));
                        }
                        OrderedDictionaryStateHelper.LoadViewState(this._keyTable, list);
                    }
                }
                else
                {
                    object[] objArray = (object[]) state;
                    if (objArray[0] != null)
                    {
                        for (int i = 0; (i < objArray.Length) && (i < this._keyNames.Length); i++)
                        {
                            this._keyTable.Add(this._keyNames[i], objArray[i]);
                        }
                    }
                }
            }
        }

        protected virtual object SaveViewState()
        {
            int count = this._keyTable.Count;
            if (count <= 0)
            {
                return null;
            }
            if (this._keyNames != null)
            {
                object obj2 = new object[count];
                for (int i = 0; i < count; i++)
                {
                    ((object[]) obj2)[i] = this._keyTable[i];
                }
                return obj2;
            }
            return OrderedDictionaryStateHelper.SaveViewState(this._keyTable);
        }

        void IStateManager.LoadViewState(object state)
        {
            this.LoadViewState(state);
        }

        object IStateManager.SaveViewState() => 
            this.SaveViewState();

        void IStateManager.TrackViewState()
        {
            this.TrackViewState();
        }

        protected virtual void TrackViewState()
        {
            this._isTracking = true;
        }

        protected virtual bool IsTrackingViewState =>
            this._isTracking;

        public virtual object this[int index] =>
            this._keyTable[index];

        public virtual object this[string name] =>
            this._keyTable[name];

        bool IStateManager.IsTrackingViewState =>
            this.IsTrackingViewState;

        public virtual object Value
        {
            get
            {
                if (this._keyTable.Count > 0)
                {
                    return this._keyTable[0];
                }
                return null;
            }
        }

        public virtual IOrderedDictionary Values
        {
            get
            {
                if (this._keyTable is OrderedDictionary)
                {
                    return ((OrderedDictionary) this._keyTable).AsReadOnly();
                }
                if (this._keyTable is ICloneable)
                {
                    return (IOrderedDictionary) ((ICloneable) this._keyTable).Clone();
                }
                OrderedDictionary dictionary = new OrderedDictionary();
                foreach (DictionaryEntry entry in this._keyTable)
                {
                    dictionary.Add(entry.Key, entry.Value);
                }
                return dictionary.AsReadOnly();
            }
        }
    }
}

