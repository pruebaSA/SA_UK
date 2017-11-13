namespace System.ServiceModel.Syndication
{
    using System;
    using System.Runtime.Serialization;

    public class ReferencedCategoriesDocument : CategoriesDocument
    {
        private Uri link;

        public ReferencedCategoriesDocument()
        {
        }

        public ReferencedCategoriesDocument(Uri link)
        {
            if (link == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("link");
            }
            this.link = link;
        }

        internal override bool IsInline =>
            false;

        public Uri Link
        {
            get => 
                this.link;
            set
            {
                this.link = value;
            }
        }
    }
}

