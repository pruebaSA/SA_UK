namespace System.Windows.Forms.Design
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Design;
    using System.Windows.Forms;

    internal class ListBoxDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
                if (service != null)
                {
                    service.ComponentRename -= new ComponentRenameEventHandler(this.OnComponentRename);
                    service.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                }
            }
            base.Dispose(disposing);
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            base.AutoResizeHandles = true;
            IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
            if (service != null)
            {
                service.ComponentRename += new ComponentRenameEventHandler(this.OnComponentRename);
                service.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
            }
        }

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            ((ListBox) base.Component).FormattingEnabled = true;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)["Name"];
            if (descriptor != null)
            {
                this.UpdateControlName(descriptor.GetValue(base.Component).ToString());
            }
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (((e.Component == base.Component) && (e.Member != null)) && (e.Member.Name == "Items"))
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)["Name"];
                if (descriptor != null)
                {
                    this.UpdateControlName(descriptor.GetValue(base.Component).ToString());
                }
            }
        }

        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (e.Component == base.Component)
            {
                this.UpdateControlName(e.NewName);
            }
        }

        protected override void OnCreateHandle()
        {
            base.OnCreateHandle();
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)["Name"];
            if (descriptor != null)
            {
                this.UpdateControlName(descriptor.GetValue(base.Component).ToString());
            }
        }

        private void UpdateControlName(string name)
        {
            ListBox control = (ListBox) this.Control;
            if (control.IsHandleCreated && (control.Items.Count == 0))
            {
                System.Design.NativeMethods.SendMessage(control.Handle, 0x184, 0, 0);
                System.Design.NativeMethods.SendMessage(control.Handle, 0x180, 0, name);
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this._actionLists == null)
                {
                    this._actionLists = new DesignerActionListCollection();
                    if (base.Component is CheckedListBox)
                    {
                        this._actionLists.Add(new ListControlUnboundActionList(this));
                    }
                    else
                    {
                        this._actionLists.Add(new ListControlBoundActionList(this));
                    }
                }
                return this._actionLists;
            }
        }
    }
}

