using System;

[AttributeUsage(AttributeTargets.Module, AllowMultiple=true)]
internal sealed class BidMetaTextAttribute : Attribute
{
    private string _metaText;

    internal BidMetaTextAttribute(string str)
    {
        this._metaText = str;
    }

    internal string MetaText =>
        this._metaText;
}

