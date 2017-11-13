namespace MigraDoc.DocumentObjectModel
{
    using System;

    public class DocumentRelations
    {
        public static DocumentObject GetParent(DocumentObject documentObject) => 
            documentObject?.Parent;

        public static DocumentObject GetParentOfType(DocumentObject documentObject, Type type)
        {
            if (documentObject == null)
            {
                throw new ArgumentNullException("documentObject");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (documentObject.parent == null)
            {
                return null;
            }
            if (documentObject.parent.GetType() == type)
            {
                return documentObject.parent;
            }
            return GetParentOfType(documentObject.parent, type);
        }

        public static bool HasParentOfType(DocumentObject documentObject, Type type)
        {
            if (documentObject == null)
            {
                throw new ArgumentNullException("documentObject");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (GetParentOfType(documentObject, type) != null);
        }
    }
}

