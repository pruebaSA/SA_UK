namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class InlineCategoriesDocument : CategoriesDocument
    {
        private Collection<SyndicationCategory> categories;
        private bool isFixed;
        private string scheme;

        public InlineCategoriesDocument()
        {
        }

        public InlineCategoriesDocument(IEnumerable<SyndicationCategory> categories) : this(categories, false, null)
        {
        }

        public InlineCategoriesDocument(IEnumerable<SyndicationCategory> categories, bool isFixed, string scheme)
        {
            if (categories != null)
            {
                this.categories = new NullNotAllowedCollection<SyndicationCategory>();
                foreach (SyndicationCategory category in categories)
                {
                    this.categories.Add(category);
                }
            }
            this.isFixed = isFixed;
            this.scheme = scheme;
        }

        protected internal virtual SyndicationCategory CreateCategory() => 
            new SyndicationCategory();

        public Collection<SyndicationCategory> Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = new NullNotAllowedCollection<SyndicationCategory>();
                }
                return this.categories;
            }
        }

        public bool IsFixed
        {
            get => 
                this.isFixed;
            set
            {
                this.isFixed = value;
            }
        }

        internal override bool IsInline =>
            true;

        public string Scheme
        {
            get => 
                this.scheme;
            set
            {
                this.scheme = value;
            }
        }
    }
}

