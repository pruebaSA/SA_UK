namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class OwnerDrawPropertyBag : MarshalByRefObject, ISerializable
    {
        private Color backColor;
        private System.Drawing.Font font;
        private Control.FontHandleWrapper fontWrapper;
        private Color foreColor;
        private static object internalSyncObject = new object();

        internal OwnerDrawPropertyBag()
        {
            this.foreColor = Color.Empty;
            this.backColor = Color.Empty;
        }

        protected OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
        {
            this.foreColor = Color.Empty;
            this.backColor = Color.Empty;
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializationEntry current = enumerator.Current;
                if (current.Name == "Font")
                {
                    this.font = (System.Drawing.Font) current.Value;
                }
                else
                {
                    if (current.Name == "ForeColor")
                    {
                        this.foreColor = (Color) current.Value;
                        continue;
                    }
                    if (current.Name == "BackColor")
                    {
                        this.backColor = (Color) current.Value;
                    }
                }
            }
        }

        public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
        {
            lock (internalSyncObject)
            {
                OwnerDrawPropertyBag bag = new OwnerDrawPropertyBag();
                if (value != null)
                {
                    bag.backColor = value.backColor;
                    bag.foreColor = value.foreColor;
                    bag.Font = value.font;
                }
                return bag;
            }
        }

        public virtual bool IsEmpty() => 
            (((this.Font == null) && this.foreColor.IsEmpty) && this.backColor.IsEmpty);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("BackColor", this.BackColor);
            si.AddValue("ForeColor", this.ForeColor);
            si.AddValue("Font", this.Font);
        }

        public Color BackColor
        {
            get => 
                this.backColor;
            set
            {
                this.backColor = value;
            }
        }

        public System.Drawing.Font Font
        {
            get => 
                this.font;
            set
            {
                this.font = value;
            }
        }

        internal IntPtr FontHandle =>
            this.fontWrapper?.Handle;

        public Color ForeColor
        {
            get => 
                this.foreColor;
            set
            {
                this.foreColor = value;
            }
        }
    }
}

