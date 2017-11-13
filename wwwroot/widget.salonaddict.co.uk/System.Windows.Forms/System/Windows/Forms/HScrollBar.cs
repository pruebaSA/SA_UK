namespace System.Windows.Forms
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [System.Windows.Forms.SRDescription("DescriptionHScrollBar"), ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class HScrollBar : ScrollBar
    {
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style = createParams.Style;
                return createParams;
            }
        }

        protected override Size DefaultSize =>
            new Size(80, SystemInformation.HorizontalScrollBarHeight);
    }
}

