namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("AtomEntry {ResolvedObject} @ {Identity}")]
    internal class AtomEntry
    {
        private EntryFlags flags;

        private bool GetFlagValue(EntryFlags mask) => 
            ((this.flags & mask) != 0);

        private void SetFlagValue(EntryFlags mask, bool value)
        {
            if (value)
            {
                this.flags |= mask;
            }
            else
            {
                this.flags &= ~mask;
            }
        }

        public ClientType ActualType { get; set; }

        public bool CreatedByMaterializer
        {
            get => 
                this.GetFlagValue(EntryFlags.CreatedByMaterializer);
            set
            {
                this.SetFlagValue(EntryFlags.CreatedByMaterializer, value);
            }
        }

        public List<AtomContentProperty> DataValues { get; set; }

        public Uri EditLink { get; set; }

        public bool EntityHasBeenResolved
        {
            get => 
                this.GetFlagValue(EntryFlags.EntityHasBeenResolved);
            set
            {
                this.SetFlagValue(EntryFlags.EntityHasBeenResolved, value);
            }
        }

        public bool EntityPropertyMappingsApplied
        {
            get => 
                this.GetFlagValue(EntryFlags.EntityPropertyMappingsApplied);
            set
            {
                this.SetFlagValue(EntryFlags.EntityPropertyMappingsApplied, value);
            }
        }

        public string ETagText { get; set; }

        public string Identity { get; set; }

        public bool IsNull
        {
            get => 
                this.GetFlagValue(EntryFlags.IsNull);
            set
            {
                this.SetFlagValue(EntryFlags.IsNull, value);
            }
        }

        public Uri MediaContentUri { get; set; }

        public Uri MediaEditUri { get; set; }

        public bool? MediaLinkEntry
        {
            get
            {
                if (!this.GetFlagValue(EntryFlags.MediaLinkEntryAssigned))
                {
                    return null;
                }
                return new bool?(this.GetFlagValue(EntryFlags.MediaLinkEntryValue));
            }
            set
            {
                this.SetFlagValue(EntryFlags.MediaLinkEntryAssigned, true);
                this.SetFlagValue(EntryFlags.MediaLinkEntryValue, value.Value);
            }
        }

        public Uri QueryLink { get; set; }

        public object ResolvedObject { get; set; }

        public bool ShouldUpdateFromPayload
        {
            get => 
                this.GetFlagValue(EntryFlags.ShouldUpdateFromPayload);
            set
            {
                this.SetFlagValue(EntryFlags.ShouldUpdateFromPayload, value);
            }
        }

        public string StreamETagText { get; set; }

        public object Tag { get; set; }

        public string TypeName { get; set; }

        [Flags]
        private enum EntryFlags
        {
            CreatedByMaterializer = 2,
            EntityHasBeenResolved = 4,
            EntityPropertyMappingsApplied = 0x20,
            IsNull = 0x40,
            MediaLinkEntryAssigned = 0x10,
            MediaLinkEntryValue = 8,
            ShouldUpdateFromPayload = 1
        }
    }
}

