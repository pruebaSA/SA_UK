namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TraceSRDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced;

        public TraceSRDescriptionAttribute(string description) : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    base.DescriptionValue = TraceSR.GetString(base.Description);
                }
                return base.Description;
            }
        }
    }
}

