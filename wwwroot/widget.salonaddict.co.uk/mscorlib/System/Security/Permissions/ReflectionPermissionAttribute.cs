﻿namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false)]
    public sealed class ReflectionPermissionAttribute : CodeAccessSecurityAttribute
    {
        private ReflectionPermissionFlag m_flag;

        public ReflectionPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (base.m_unrestricted)
            {
                return new ReflectionPermission(PermissionState.Unrestricted);
            }
            return new ReflectionPermission(this.m_flag);
        }

        public ReflectionPermissionFlag Flags
        {
            get => 
                this.m_flag;
            set
            {
                this.m_flag = value;
            }
        }

        public bool MemberAccess
        {
            get => 
                ((this.m_flag & ReflectionPermissionFlag.MemberAccess) != ReflectionPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | ReflectionPermissionFlag.MemberAccess) : (this.m_flag & ~ReflectionPermissionFlag.MemberAccess);
            }
        }

        public bool ReflectionEmit
        {
            get => 
                ((this.m_flag & ReflectionPermissionFlag.ReflectionEmit) != ReflectionPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | ReflectionPermissionFlag.ReflectionEmit) : (this.m_flag & ~ReflectionPermissionFlag.ReflectionEmit);
            }
        }

        public bool RestrictedMemberAccess
        {
            get => 
                ((this.m_flag & ReflectionPermissionFlag.RestrictedMemberAccess) != ReflectionPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | ReflectionPermissionFlag.RestrictedMemberAccess) : (this.m_flag & ~ReflectionPermissionFlag.RestrictedMemberAccess);
            }
        }

        [Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
        public bool TypeInformation
        {
            get => 
                ((this.m_flag & ReflectionPermissionFlag.TypeInformation) != ReflectionPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | ReflectionPermissionFlag.TypeInformation) : (this.m_flag & ~ReflectionPermissionFlag.TypeInformation);
            }
        }
    }
}

