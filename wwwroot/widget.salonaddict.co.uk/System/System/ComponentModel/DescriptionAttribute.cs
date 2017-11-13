namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public static readonly DescriptionAttribute Default = new DescriptionAttribute();
        private string description;

        public DescriptionAttribute() : this(string.Empty)
        {
        }

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            DescriptionAttribute attribute = obj as DescriptionAttribute;
            return ((attribute != null) && (attribute.Description == this.Description));
        }

        public override int GetHashCode() => 
            this.Description.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public virtual string Description =>
            this.DescriptionValue;

        protected string DescriptionValue
        {
            get => 
                this.description;
            set
            {
                this.description = value;
            }
        }
    }
}

