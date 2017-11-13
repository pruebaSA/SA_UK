namespace System.Diagnostics
{
    using System;
    using System.Configuration;

    internal class SwitchesDictionarySectionHandler : DictionarySectionHandler
    {
        protected override string KeyAttributeName =>
            "name";

        internal override bool ValueRequired =>
            true;
    }
}

