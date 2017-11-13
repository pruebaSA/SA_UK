namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class RefreshPropertiesAttribute : Attribute
    {
        public static readonly RefreshPropertiesAttribute All = new RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties.All);
        public static readonly RefreshPropertiesAttribute Default = new RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties.None);
        private System.ComponentModel.RefreshProperties refresh;
        public static readonly RefreshPropertiesAttribute Repaint = new RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties.Repaint);

        public RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties refresh)
        {
            this.refresh = refresh;
        }

        public override bool Equals(object value) => 
            ((value is RefreshPropertiesAttribute) && (((RefreshPropertiesAttribute) value).RefreshProperties == this.refresh));

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public System.ComponentModel.RefreshProperties RefreshProperties =>
            this.refresh;
    }
}

