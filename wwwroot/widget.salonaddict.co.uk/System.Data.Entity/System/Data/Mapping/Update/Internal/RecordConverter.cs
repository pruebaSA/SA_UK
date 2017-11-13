namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;

    internal class RecordConverter
    {
        private UpdateTranslator m_updateTranslator;

        internal RecordConverter(UpdateTranslator updateTranslator)
        {
            this.m_updateTranslator = updateTranslator;
        }

        internal PropagatorResult ConvertCurrentValuesToPropagatorResult(IEntityStateEntry stateEntry, IEnumerable<string> modifiedProperties) => 
            this.ConvertStateEntryToPropagatorResult(stateEntry, modifiedProperties, false);

        internal PropagatorResult ConvertOriginalValuesToPropagatorResult(IEntityStateEntry stateEntry, IEnumerable<string> modifiedProperties) => 
            this.ConvertStateEntryToPropagatorResult(stateEntry, modifiedProperties, true);

        private PropagatorResult ConvertStateEntryToPropagatorResult(IEntityStateEntry stateEntry, IEnumerable<string> modifiedProperties, bool useOriginalValues)
        {
            PropagatorResult result;
            try
            {
                EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
                IExtendedDataRecord record = useOriginalValues ? EntityUtil.CheckArgumentNull<IExtendedDataRecord>(stateEntry.OriginalValues as IExtendedDataRecord, "stateEntry.OriginalValues") : EntityUtil.CheckArgumentNull<IExtendedDataRecord>(stateEntry.CurrentValues, "stateEntry.CurrentValues");
                bool isModified = false;
                result = ExtractorMetadata.ExtractResultFromRecord(stateEntry, isModified, record, modifiedProperties, this.m_updateTranslator);
            }
            catch (Exception exception)
            {
                if (UpdateTranslator.RequiresContext(exception))
                {
                    throw EntityUtil.Update(Strings.Update_ErrorLoadingRecord, exception, new IEntityStateEntry[] { stateEntry });
                }
                throw;
            }
            return result;
        }
    }
}

