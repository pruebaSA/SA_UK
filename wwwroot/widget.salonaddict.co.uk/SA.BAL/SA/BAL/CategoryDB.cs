namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class CategoryDB : BaseEntity
    {
        public bool Active { get; set; }

        public Guid CategoryId { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string Name { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public string SEName { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }
    }
}

