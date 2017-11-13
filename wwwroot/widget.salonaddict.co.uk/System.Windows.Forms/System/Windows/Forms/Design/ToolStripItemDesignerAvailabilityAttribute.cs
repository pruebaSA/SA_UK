namespace System.Windows.Forms.Design
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ToolStripItemDesignerAvailabilityAttribute : Attribute
    {
        public static readonly ToolStripItemDesignerAvailabilityAttribute Default = new ToolStripItemDesignerAvailabilityAttribute();
        private ToolStripItemDesignerAvailability visibility;

        public ToolStripItemDesignerAvailabilityAttribute()
        {
            this.visibility = ToolStripItemDesignerAvailability.None;
        }

        public ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability visibility)
        {
            this.visibility = visibility;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            ToolStripItemDesignerAvailabilityAttribute attribute = obj as ToolStripItemDesignerAvailabilityAttribute;
            return ((attribute != null) && (attribute.ItemAdditionVisibility == this.visibility));
        }

        public override int GetHashCode() => 
            this.visibility.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public ToolStripItemDesignerAvailability ItemAdditionVisibility =>
            this.visibility;
    }
}

