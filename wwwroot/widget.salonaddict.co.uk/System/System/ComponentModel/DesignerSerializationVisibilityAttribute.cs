namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class DesignerSerializationVisibilityAttribute : Attribute
    {
        public static readonly DesignerSerializationVisibilityAttribute Content = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content);
        public static readonly DesignerSerializationVisibilityAttribute Default = Visible;
        public static readonly DesignerSerializationVisibilityAttribute Hidden = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);
        private DesignerSerializationVisibility visibility;
        public static readonly DesignerSerializationVisibilityAttribute Visible = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

        public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility visibility)
        {
            this.visibility = visibility;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            DesignerSerializationVisibilityAttribute attribute = obj as DesignerSerializationVisibilityAttribute;
            return ((attribute != null) && (attribute.Visibility == this.visibility));
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public DesignerSerializationVisibility Visibility =>
            this.visibility;
    }
}

