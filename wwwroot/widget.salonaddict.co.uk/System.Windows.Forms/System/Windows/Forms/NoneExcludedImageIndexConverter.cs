namespace System.Windows.Forms
{
    using System;

    internal sealed class NoneExcludedImageIndexConverter : ImageIndexConverter
    {
        protected override bool IncludeNoneAsStandardValue =>
            false;
    }
}

