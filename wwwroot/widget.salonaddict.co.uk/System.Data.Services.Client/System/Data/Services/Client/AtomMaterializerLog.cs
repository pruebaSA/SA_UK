namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class AtomMaterializerLog
    {
        private readonly Dictionary<string, AtomEntry> appendOnlyEntries = new Dictionary<string, AtomEntry>(EqualityComparer<string>.Default);
        private readonly DataServiceContext context;
        private readonly Dictionary<string, AtomEntry> foundEntriesWithMedia;
        private readonly Dictionary<string, AtomEntry> identityStack;
        private object insertRefreshObject;
        private readonly List<LinkDescriptor> links;
        private readonly MergeOption mergeOption;

        internal AtomMaterializerLog(DataServiceContext context, MergeOption mergeOption)
        {
            this.context = context;
            this.mergeOption = mergeOption;
            this.foundEntriesWithMedia = new Dictionary<string, AtomEntry>(EqualityComparer<string>.Default);
            this.identityStack = new Dictionary<string, AtomEntry>(EqualityComparer<string>.Default);
            this.links = new List<LinkDescriptor>();
        }

        internal void AddedLink(AtomEntry source, string propertyName, object target)
        {
            if (this.Tracking && (ShouldTrackWithContext(source) && ShouldTrackWithContext(target)))
            {
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Added);
                this.links.Add(item);
            }
        }

        private void ApplyMediaEntryInformation(AtomEntry entry, EntityDescriptor descriptor)
        {
            if ((entry.MediaEditUri != null) || (entry.MediaContentUri != null))
            {
                if (entry.MediaEditUri != null)
                {
                    descriptor.EditStreamUri = new Uri(this.context.BaseUriWithSlash, entry.MediaEditUri);
                }
                if (entry.MediaContentUri != null)
                {
                    descriptor.ReadStreamUri = new Uri(this.context.BaseUriWithSlash, entry.MediaContentUri);
                }
                descriptor.StreamETag = entry.StreamETagText;
            }
        }

        internal void ApplyToContext()
        {
            if (this.Tracking)
            {
                foreach (KeyValuePair<string, AtomEntry> pair in this.identityStack)
                {
                    AtomEntry entry = pair.Value;
                    if ((entry.CreatedByMaterializer || (entry.ResolvedObject == this.insertRefreshObject)) || entry.ShouldUpdateFromPayload)
                    {
                        EntityDescriptor descriptor = new EntityDescriptor(pair.Key, entry.QueryLink, entry.EditLink, entry.ResolvedObject, null, null, null, entry.ETagText, EntityStates.Unchanged);
                        descriptor = this.context.InternalAttachEntityDescriptor(descriptor, false);
                        descriptor.State = EntityStates.Unchanged;
                        this.ApplyMediaEntryInformation(entry, descriptor);
                        descriptor.ServerTypeName = entry.TypeName;
                    }
                    else
                    {
                        EntityStates states;
                        this.context.TryGetEntity(pair.Key, entry.ETagText, this.mergeOption, out states);
                    }
                }
                foreach (AtomEntry entry2 in this.foundEntriesWithMedia.Values)
                {
                    EntityDescriptor entityDescriptor = this.context.GetEntityDescriptor(entry2.ResolvedObject);
                    this.ApplyMediaEntryInformation(entry2, entityDescriptor);
                }
                foreach (LinkDescriptor descriptor3 in this.links)
                {
                    if (EntityStates.Added == descriptor3.State)
                    {
                        if ((EntityStates.Deleted == this.context.GetEntityDescriptor(descriptor3.Target).State) || (EntityStates.Deleted == this.context.GetEntityDescriptor(descriptor3.Source).State))
                        {
                            this.context.DeleteLink(descriptor3.Source, descriptor3.SourceProperty, descriptor3.Target);
                        }
                        else
                        {
                            this.context.AttachLink(descriptor3.Source, descriptor3.SourceProperty, descriptor3.Target, this.mergeOption);
                        }
                    }
                    else
                    {
                        if (EntityStates.Modified == descriptor3.State)
                        {
                            object target = descriptor3.Target;
                            if (MergeOption.PreserveChanges == this.mergeOption)
                            {
                                LinkDescriptor descriptor4 = this.context.GetLinks(descriptor3.Source, descriptor3.SourceProperty).FirstOrDefault<LinkDescriptor>();
                                if ((descriptor4 != null) && (descriptor4.Target == null))
                                {
                                    goto Label_02A1;
                                }
                                if (((target != null) && (EntityStates.Deleted == this.context.GetEntityDescriptor(target).State)) || (EntityStates.Deleted == this.context.GetEntityDescriptor(descriptor3.Source).State))
                                {
                                    target = null;
                                }
                            }
                            this.context.AttachLink(descriptor3.Source, descriptor3.SourceProperty, target, this.mergeOption);
                        }
                        else
                        {
                            this.context.DetachLink(descriptor3.Source, descriptor3.SourceProperty, descriptor3.Target);
                        }
                    Label_02A1:;
                    }
                }
            }
        }

        internal void Clear()
        {
            this.foundEntriesWithMedia.Clear();
            this.identityStack.Clear();
            this.links.Clear();
            this.insertRefreshObject = null;
        }

        internal void CreatedInstance(AtomEntry entry)
        {
            if (ShouldTrackWithContext(entry))
            {
                this.identityStack.Add(entry.Identity, entry);
                if (this.mergeOption == MergeOption.AppendOnly)
                {
                    this.appendOnlyEntries.Add(entry.Identity, entry);
                }
            }
        }

        internal void FoundExistingInstance(AtomEntry entry)
        {
            if (this.mergeOption == MergeOption.OverwriteChanges)
            {
                this.identityStack[entry.Identity] = entry;
            }
            else if (this.Tracking && (entry.MediaLinkEntry == true))
            {
                this.foundEntriesWithMedia[entry.Identity] = entry;
            }
        }

        internal void FoundTargetInstance(AtomEntry entry)
        {
            if (ShouldTrackWithContext(entry))
            {
                this.context.AttachIdentity(entry.Identity, entry.QueryLink, entry.EditLink, entry.ResolvedObject, entry.ETagText);
                this.identityStack.Add(entry.Identity, entry);
                this.insertRefreshObject = entry.ResolvedObject;
            }
        }

        internal void RemovedLink(AtomEntry source, string propertyName, object target)
        {
            if (ShouldTrackWithContext(source) && ShouldTrackWithContext(target))
            {
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Detached);
                this.links.Add(item);
            }
        }

        internal void SetLink(AtomEntry source, string propertyName, object target)
        {
            if (this.Tracking && (ShouldTrackWithContext(source) && ShouldTrackWithContext(target)))
            {
                LinkDescriptor item = new LinkDescriptor(source.ResolvedObject, propertyName, target, EntityStates.Modified);
                this.links.Add(item);
            }
        }

        private static bool ShouldTrackWithContext(AtomEntry entry) => 
            entry.ActualType.IsEntityType;

        private static bool ShouldTrackWithContext(object entity) => 
            ((entity == null) || ClientType.Create(entity.GetType()).IsEntityType);

        internal bool TryResolve(AtomEntry entry, out AtomEntry existingEntry)
        {
            if (this.identityStack.TryGetValue(entry.Identity, out existingEntry))
            {
                return true;
            }
            if (this.appendOnlyEntries.TryGetValue(entry.Identity, out existingEntry))
            {
                EntityStates states;
                this.context.TryGetEntity(entry.Identity, entry.ETagText, this.mergeOption, out states);
                if (states == EntityStates.Unchanged)
                {
                    return true;
                }
                this.appendOnlyEntries.Remove(entry.Identity);
            }
            existingEntry = null;
            return false;
        }

        internal bool Tracking =>
            (this.mergeOption != MergeOption.NoTracking);
    }
}

