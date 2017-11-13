namespace System.Xml.Xsl.Xslt
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Xsl.Qil;

    internal class Keys : KeyedCollection<QilName, List<Key>>
    {
        protected override QilName GetKeyForItem(List<Key> list) => 
            list[0].Name;
    }
}

