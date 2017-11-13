namespace System.Web.UI
{
    using System;
    using System.Collections.ObjectModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class UpdatePanelTriggerCollection : Collection<UpdatePanelTrigger>
    {
        private bool _initialized;
        private UpdatePanel _owner;

        public UpdatePanelTriggerCollection(UpdatePanel owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            this._owner = owner;
        }

        protected override void ClearItems()
        {
            foreach (UpdatePanelTrigger trigger in this)
            {
                trigger.SetOwner(null);
            }
            base.ClearItems();
        }

        internal bool HasTriggered()
        {
            foreach (UpdatePanelTrigger trigger in this)
            {
                if (trigger.HasTriggered())
                {
                    return true;
                }
            }
            return false;
        }

        internal void Initialize()
        {
            foreach (UpdatePanelTrigger trigger in this)
            {
                trigger.Initialize();
            }
            this._initialized = true;
        }

        protected override void InsertItem(int index, UpdatePanelTrigger item)
        {
            item.SetOwner(this.Owner);
            if (this._initialized)
            {
                item.Initialize();
            }
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base[index].SetOwner(null);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, UpdatePanelTrigger item)
        {
            base[index].SetOwner(null);
            item.SetOwner(this.Owner);
            if (this._initialized)
            {
                item.Initialize();
            }
            base.SetItem(index, item);
        }

        public UpdatePanel Owner =>
            this._owner;
    }
}

