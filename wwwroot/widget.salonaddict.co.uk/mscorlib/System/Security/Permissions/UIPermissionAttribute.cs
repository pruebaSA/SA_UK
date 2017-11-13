﻿namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public sealed class UIPermissionAttribute : CodeAccessSecurityAttribute
    {
        private UIPermissionClipboard m_clipboardFlag;
        private UIPermissionWindow m_windowFlag;

        public UIPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (base.m_unrestricted)
            {
                return new UIPermission(PermissionState.Unrestricted);
            }
            return new UIPermission(this.m_windowFlag, this.m_clipboardFlag);
        }

        public UIPermissionClipboard Clipboard
        {
            get => 
                this.m_clipboardFlag;
            set
            {
                this.m_clipboardFlag = value;
            }
        }

        public UIPermissionWindow Window
        {
            get => 
                this.m_windowFlag;
            set
            {
                this.m_windowFlag = value;
            }
        }
    }
}

