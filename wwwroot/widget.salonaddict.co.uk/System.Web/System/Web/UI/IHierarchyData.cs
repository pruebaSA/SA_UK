namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IHierarchyData
    {
        IHierarchicalEnumerable GetChildren();
        IHierarchyData GetParent();

        bool HasChildren { get; }

        object Item { get; }

        string Path { get; }

        string Type { get; }
    }
}

