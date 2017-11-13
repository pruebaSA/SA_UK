namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.XPath;

    internal class NavigatorInput
    {
        private Keywords _Atoms;
        private string _Href;
        private System.Xml.Xsl.XsltOld.InputScopeManager _Manager;
        private XPathNavigator _Navigator;
        private NavigatorInput _Next;
        private PositionInfo _PositionInfo;

        internal NavigatorInput(XPathNavigator navigator) : this(navigator, navigator.BaseURI, null)
        {
        }

        internal NavigatorInput(XPathNavigator navigator, string baseUri, InputScope rootScope)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException("navigator");
            }
            if (baseUri == null)
            {
                throw new ArgumentNullException("baseUri");
            }
            this._Next = null;
            this._Href = baseUri;
            this._Atoms = new Keywords(navigator.NameTable);
            this._Atoms.LookupKeywords();
            this._Navigator = navigator;
            this._Manager = new System.Xml.Xsl.XsltOld.InputScopeManager(this._Navigator, rootScope);
            this._PositionInfo = PositionInfo.GetPositionInfo(this._Navigator);
            if (this.NodeType == XPathNodeType.Root)
            {
                this._Navigator.MoveToFirstChild();
            }
        }

        internal bool Advance() => 
            this._Navigator.MoveToNext();

        [Conditional("DEBUG")]
        internal void AssertInput()
        {
        }

        internal void Close()
        {
            this._Navigator = null;
            this._PositionInfo = null;
        }

        internal bool MoveToFirstAttribute() => 
            this._Navigator.MoveToFirstAttribute();

        internal bool MoveToFirstNamespace() => 
            this._Navigator.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml);

        internal bool MoveToNextAttribute() => 
            this._Navigator.MoveToNextAttribute();

        internal bool MoveToNextNamespace() => 
            this._Navigator.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml);

        internal bool Recurse() => 
            this._Navigator.MoveToFirstChild();

        internal bool ToParent() => 
            this._Navigator.MoveToParent();

        internal Keywords Atoms =>
            this._Atoms;

        internal string BaseURI =>
            this._Navigator.BaseURI;

        internal string Href =>
            this._Href;

        internal System.Xml.Xsl.XsltOld.InputScopeManager InputScopeManager =>
            this._Manager;

        internal bool IsEmptyTag =>
            this._Navigator.IsEmptyElement;

        internal int LineNumber =>
            this._PositionInfo.LineNumber;

        internal int LinePosition =>
            this._PositionInfo.LinePosition;

        internal string LocalName =>
            this._Navigator.LocalName;

        internal string Name =>
            this._Navigator.Name;

        internal string NamespaceURI =>
            this._Navigator.NamespaceURI;

        internal XPathNavigator Navigator =>
            this._Navigator;

        internal NavigatorInput Next
        {
            get => 
                this._Next;
            set
            {
                this._Next = value;
            }
        }

        internal XPathNodeType NodeType =>
            this._Navigator.NodeType;

        internal string Prefix =>
            this._Navigator.Prefix;

        internal string Value =>
            this._Navigator.Value;
    }
}

