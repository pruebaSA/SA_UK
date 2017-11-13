namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class ListSortDescription
    {
        private System.ComponentModel.PropertyDescriptor property;
        private ListSortDirection sortDirection;

        public ListSortDescription(System.ComponentModel.PropertyDescriptor property, ListSortDirection direction)
        {
            this.property = property;
            this.sortDirection = direction;
        }

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor
        {
            get => 
                this.property;
            set
            {
                this.property = value;
            }
        }

        public ListSortDirection SortDirection
        {
            get => 
                this.sortDirection;
            set
            {
                this.sortDirection = value;
            }
        }
    }
}

