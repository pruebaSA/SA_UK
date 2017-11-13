namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TreeNodeEventArgs : EventArgs
    {
        private TreeNode _node;

        public TreeNodeEventArgs(TreeNode node)
        {
            this._node = node;
        }

        public TreeNode Node =>
            this._node;
    }
}

