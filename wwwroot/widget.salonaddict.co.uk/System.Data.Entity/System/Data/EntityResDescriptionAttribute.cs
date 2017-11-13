namespace System.Data
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class EntityResDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public EntityResDescriptionAttribute(string description) : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    base.DescriptionValue = EntityRes.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }
}

