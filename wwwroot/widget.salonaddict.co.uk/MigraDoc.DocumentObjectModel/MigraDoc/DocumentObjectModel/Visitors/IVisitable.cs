namespace MigraDoc.DocumentObjectModel.Visitors
{
    using System;

    internal interface IVisitable
    {
        void AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren);
    }
}

