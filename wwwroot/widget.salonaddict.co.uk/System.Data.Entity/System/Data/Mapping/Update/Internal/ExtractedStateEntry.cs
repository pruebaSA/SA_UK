namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ExtractedStateEntry
    {
        internal readonly EntityState State;
        internal readonly PropagatorResult Original;
        internal readonly PropagatorResult Current;
        internal readonly IEntityStateEntry Source;
        internal ExtractedStateEntry(UpdateTranslator translator, IEntityStateEntry stateEntry)
        {
            this.State = stateEntry.State;
            this.Source = stateEntry;
            switch (stateEntry.State)
            {
                case EntityState.Unchanged:
                    this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, Enumerable.Empty<string>());
                    this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, Enumerable.Empty<string>());
                    return;

                case EntityState.Added:
                    this.Original = null;
                    this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, null);
                    return;

                case EntityState.Deleted:
                    this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, null);
                    this.Current = null;
                    return;

                case EntityState.Modified:
                    this.Original = translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, stateEntry.GetModifiedProperties());
                    this.Current = translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, stateEntry.GetModifiedProperties());
                    return;
            }
            this.Original = null;
            this.Current = null;
        }
    }
}

