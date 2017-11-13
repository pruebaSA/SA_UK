﻿namespace System.Security.AccessControl
{
    using System;
    using System.Security.Principal;

    public abstract class ObjectAuditRule : AuditRule
    {
        private readonly Guid _inheritedObjectType;
        private readonly ObjectAceFlags _objectFlags;
        private readonly Guid _objectType;

        protected ObjectAuditRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, Guid objectType, Guid inheritedObjectType, AuditFlags auditFlags) : base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags, auditFlags)
        {
            if (!objectType.Equals(Guid.Empty) && ((accessMask & ObjectAce.AccessMaskWithObjectType) != 0))
            {
                this._objectType = objectType;
                this._objectFlags |= ObjectAceFlags.ObjectAceTypePresent;
            }
            else
            {
                this._objectType = Guid.Empty;
            }
            if (!inheritedObjectType.Equals(Guid.Empty) && ((inheritanceFlags & InheritanceFlags.ContainerInherit) != InheritanceFlags.None))
            {
                this._inheritedObjectType = inheritedObjectType;
                this._objectFlags |= ObjectAceFlags.InheritedObjectAceTypePresent;
            }
            else
            {
                this._inheritedObjectType = Guid.Empty;
            }
        }

        public Guid InheritedObjectType =>
            this._inheritedObjectType;

        public ObjectAceFlags ObjectFlags =>
            this._objectFlags;

        public Guid ObjectType =>
            this._objectType;
    }
}

