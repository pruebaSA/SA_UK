﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, DefaultProperty("Header"), DesignTimeVisible(false), TypeConverter(typeof(ListViewGroupConverter)), ToolboxItem(false)]
    public sealed class ListViewGroup : ISerializable
    {
        private string header;
        private HorizontalAlignment headerAlignment;
        private int id;
        private System.Windows.Forms.ListView.ListViewItemCollection items;
        private System.Windows.Forms.ListView listView;
        private string name;
        private static int nextHeader = 1;
        private static int nextID;
        private object userData;

        public ListViewGroup() : this(System.Windows.Forms.SR.GetString("ListViewGroupDefaultHeader", new object[] { nextHeader++ }))
        {
        }

        public ListViewGroup(string header)
        {
            this.header = header;
            this.id = nextID++;
        }

        private ListViewGroup(SerializationInfo info, StreamingContext context) : this()
        {
            this.Deserialize(info, context);
        }

        public ListViewGroup(string key, string headerText) : this()
        {
            this.name = key;
            this.header = headerText;
        }

        public ListViewGroup(string header, HorizontalAlignment headerAlignment) : this(header)
        {
            this.headerAlignment = headerAlignment;
        }

        private void Deserialize(SerializationInfo info, StreamingContext context)
        {
            int num = 0;
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializationEntry current = enumerator.Current;
                if (current.Name == "Header")
                {
                    this.Header = (string) current.Value;
                }
                else
                {
                    if (current.Name == "HeaderAlignment")
                    {
                        this.HeaderAlignment = (HorizontalAlignment) current.Value;
                        continue;
                    }
                    if (current.Name == "Tag")
                    {
                        this.Tag = current.Value;
                        continue;
                    }
                    if (current.Name == "ItemsCount")
                    {
                        num = (int) current.Value;
                        continue;
                    }
                    if (current.Name == "Name")
                    {
                        this.Name = (string) current.Value;
                    }
                }
            }
            if (num > 0)
            {
                ListViewItem[] items = new ListViewItem[num];
                for (int i = 0; i < num; i++)
                {
                    items[i] = (ListViewItem) info.GetValue("Item" + i, typeof(ListViewItem));
                }
                this.Items.AddRange(items);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Header", this.Header);
            info.AddValue("HeaderAlignment", this.HeaderAlignment);
            info.AddValue("Tag", this.Tag);
            if (!string.IsNullOrEmpty(this.Name))
            {
                info.AddValue("Name", this.Name);
            }
            if ((this.items != null) && (this.items.Count > 0))
            {
                info.AddValue("ItemsCount", this.Items.Count);
                for (int i = 0; i < this.Items.Count; i++)
                {
                    info.AddValue("Item" + i.ToString(CultureInfo.InvariantCulture), this.Items[i], typeof(ListViewItem));
                }
            }
        }

        public override string ToString() => 
            this.Header;

        private void UpdateListView()
        {
            if ((this.listView != null) && this.listView.IsHandleCreated)
            {
                this.listView.UpdateGroupNative(this);
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance")]
        public string Header
        {
            get
            {
                if (this.header != null)
                {
                    return this.header;
                }
                return "";
            }
            set
            {
                if (this.header != value)
                {
                    this.header = value;
                    if (this.listView != null)
                    {
                        this.listView.RecreateHandleInternal();
                    }
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(0)]
        public HorizontalAlignment HeaderAlignment
        {
            get => 
                this.headerAlignment;
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 2))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(HorizontalAlignment));
                }
                if (this.headerAlignment != value)
                {
                    this.headerAlignment = value;
                    this.UpdateListView();
                }
            }
        }

        internal int ID =>
            this.id;

        [Browsable(false)]
        public System.Windows.Forms.ListView.ListViewItemCollection Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new System.Windows.Forms.ListView.ListViewItemCollection(new ListViewGroupItemCollection(this));
                }
                return this.items;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public System.Windows.Forms.ListView ListView =>
            this.listView;

        internal System.Windows.Forms.ListView ListViewInternal
        {
            get => 
                this.listView;
            set
            {
                if (this.listView != value)
                {
                    this.listView = value;
                }
            }
        }

        [System.Windows.Forms.SRDescription("ListViewGroupNameDescr"), Browsable(true), DefaultValue(""), System.Windows.Forms.SRCategory("CatBehavior")]
        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), DefaultValue((string) null), Bindable(true), Localizable(false), System.Windows.Forms.SRDescription("ControlTagDescr"), TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get => 
                this.userData;
            set
            {
                this.userData = value;
            }
        }
    }
}

