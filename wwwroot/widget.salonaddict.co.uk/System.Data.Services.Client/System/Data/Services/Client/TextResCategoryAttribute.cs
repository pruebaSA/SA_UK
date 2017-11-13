﻿namespace System.Data.Services.Client
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TextResCategoryAttribute : CategoryAttribute
    {
        public TextResCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value) => 
            TextRes.GetString(value);
    }
}

