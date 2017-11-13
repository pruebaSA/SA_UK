﻿namespace System.Data.Services.Client
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class MediaEntryAttribute : Attribute
    {
        private readonly string mediaMemberName;

        public MediaEntryAttribute(string mediaMemberName)
        {
            Util.CheckArgumentNull<string>(mediaMemberName, "mediaMemberName");
            this.mediaMemberName = mediaMemberName;
        }

        public string MediaMemberName =>
            this.mediaMemberName;
    }
}

