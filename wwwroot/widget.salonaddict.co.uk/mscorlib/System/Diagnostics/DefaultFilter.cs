namespace System.Diagnostics
{
    using System;

    internal class DefaultFilter : AssertFilter
    {
        internal DefaultFilter()
        {
        }

        public override AssertFilters AssertFailure(string condition, string message, StackTrace location) => 
            ((AssertFilters) Assert.ShowDefaultAssertDialog(condition, message));
    }
}

