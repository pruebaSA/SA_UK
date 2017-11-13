﻿namespace System.Messaging
{
    using System;
    using System.ComponentModel;

    public class AccessControlEntry
    {
        internal int accessFlags;
        private AccessControlEntryType entryType;
        private const System.Messaging.GenericAccessRights genericRightsMask = (System.Messaging.GenericAccessRights.Write | System.Messaging.GenericAccessRights.Execute | System.Messaging.GenericAccessRights.All | System.Messaging.GenericAccessRights.Read);
        private const System.Messaging.StandardAccessRights standardRightsMask = System.Messaging.StandardAccessRights.All;
        private System.Messaging.Trustee trustee;

        public AccessControlEntry()
        {
            this.entryType = AccessControlEntryType.Allow;
        }

        public AccessControlEntry(System.Messaging.Trustee trustee)
        {
            this.entryType = AccessControlEntryType.Allow;
            this.Trustee = trustee;
        }

        public AccessControlEntry(System.Messaging.Trustee trustee, System.Messaging.GenericAccessRights genericAccessRights, System.Messaging.StandardAccessRights standardAccessRights, AccessControlEntryType entryType)
        {
            this.entryType = AccessControlEntryType.Allow;
            this.GenericAccessRights = genericAccessRights;
            this.StandardAccessRights = standardAccessRights;
            this.Trustee = trustee;
            this.EntryType = entryType;
        }

        protected int CustomAccessRights
        {
            get => 
                this.accessFlags;
            set
            {
                this.accessFlags = value;
            }
        }

        public AccessControlEntryType EntryType
        {
            get => 
                this.entryType;
            set
            {
                if (!ValidationUtility.ValidateAccessControlEntryType(value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(AccessControlEntryType));
                }
                this.entryType = value;
            }
        }

        public System.Messaging.GenericAccessRights GenericAccessRights
        {
            get => 
                (((System.Messaging.GenericAccessRights) this.accessFlags) & (System.Messaging.GenericAccessRights.Write | System.Messaging.GenericAccessRights.Execute | System.Messaging.GenericAccessRights.All | System.Messaging.GenericAccessRights.Read));
            set
            {
                if ((value & (System.Messaging.GenericAccessRights.Write | System.Messaging.GenericAccessRights.Execute | System.Messaging.GenericAccessRights.All | System.Messaging.GenericAccessRights.Read)) != value)
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Messaging.GenericAccessRights));
                }
                this.accessFlags = (this.accessFlags & 0xfffffff) | value;
            }
        }

        public System.Messaging.StandardAccessRights StandardAccessRights
        {
            get => 
                (((System.Messaging.StandardAccessRights) this.accessFlags) & System.Messaging.StandardAccessRights.All);
            set
            {
                if ((value & System.Messaging.StandardAccessRights.All) != value)
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Messaging.StandardAccessRights));
                }
                this.accessFlags = (this.accessFlags & -2031617) | value;
            }
        }

        public System.Messaging.Trustee Trustee
        {
            get => 
                this.trustee;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.trustee = value;
            }
        }
    }
}

