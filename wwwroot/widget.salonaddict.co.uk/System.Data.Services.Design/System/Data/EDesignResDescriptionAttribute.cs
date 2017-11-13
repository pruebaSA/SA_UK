namespace System.Data
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class EDesignResDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public EDesignResDescriptionAttribute(string description) : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    base.DescriptionValue = EDesignRes.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }
}

