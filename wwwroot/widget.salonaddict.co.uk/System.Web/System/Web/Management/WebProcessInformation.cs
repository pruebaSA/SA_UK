namespace System.Web.Management
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebProcessInformation
    {
        private string _accountName;
        private int _processId;
        private string _processName;

        internal WebProcessInformation()
        {
            StringBuilder filename = new StringBuilder(0x100);
            if (System.Web.UnsafeNativeMethods.GetModuleFileName(IntPtr.Zero, filename, 0x100) == 0)
            {
                this._processName = string.Empty;
            }
            else
            {
                this._processName = filename.ToString();
                int num = this._processName.LastIndexOf('\\');
                if (num != -1)
                {
                    this._processName = this._processName.Substring(num + 1);
                }
            }
            this._processId = SafeNativeMethods.GetCurrentProcessId();
            this._accountName = HttpRuntime.WpUserId;
        }

        public void FormatToString(WebEventFormatter formatter)
        {
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_process_id", this.ProcessID.ToString(CultureInfo.InstalledUICulture)));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_process_name", this.ProcessName));
            formatter.AppendLine(WebBaseEvent.FormatResourceStringWithCache("Webevent_event_account_name", this.AccountName));
        }

        public string AccountName
        {
            get
            {
                if (this._accountName == null)
                {
                    return string.Empty;
                }
                return this._accountName;
            }
        }

        public int ProcessID =>
            this._processId;

        public string ProcessName =>
            this._processName;
    }
}

