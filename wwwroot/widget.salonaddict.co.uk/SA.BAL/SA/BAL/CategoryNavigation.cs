namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CategoryNavigation
    {
        public List<CategoryNavigation> Categories { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; }
    }
}

