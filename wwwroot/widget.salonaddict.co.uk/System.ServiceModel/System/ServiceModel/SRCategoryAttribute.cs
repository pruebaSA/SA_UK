﻿namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute
    {
        public SRCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value) => 
            System.ServiceModel.SR.GetString(value);
    }
}

