namespace System.Xml.Xsl.XPath
{
    using System.Xml.Xsl.Qil;

    internal interface IFocus
    {
        QilNode GetCurrent();
        QilNode GetLast();
        QilNode GetPosition();
    }
}

