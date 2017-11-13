namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Linq;

    internal class ExtractorMetadata
    {
        private readonly Dictionary<string, MemberInformation> m_memberMap;
        private readonly UpdateTranslator m_translator;
        private readonly StructuralType m_type;

        internal ExtractorMetadata(StructuralType type, UpdateTranslator translator)
        {
            System.Data.Common.Utils.Set<EdmMember> empty;
            this.m_type = EntityUtil.CheckArgumentNull<StructuralType>(type, "type");
            this.m_translator = EntityUtil.CheckArgumentNull<UpdateTranslator>(translator, "translator");
            BuiltInTypeKind builtInTypeKind = type.BuiltInTypeKind;
            if (builtInTypeKind != BuiltInTypeKind.EntityType)
            {
                if (builtInTypeKind != BuiltInTypeKind.RowType)
                {
                    empty = System.Data.Common.Utils.Set<EdmMember>.Empty;
                }
                else
                {
                    empty = new System.Data.Common.Utils.Set<EdmMember>(type.Members).MakeReadOnly();
                }
            }
            else
            {
                empty = new System.Data.Common.Utils.Set<EdmMember>(((EntityType) type).KeyMembers).MakeReadOnly();
            }
            this.m_memberMap = new Dictionary<string, MemberInformation>();
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(type);
            for (int i = 0; i < allStructuralMembers.Count; i++)
            {
                EdmMember element = allStructuralMembers[i];
                PropagatorFlags noFlags = PropagatorFlags.NoFlags;
                if (empty.Contains(element))
                {
                    noFlags = (PropagatorFlags) ((byte) (noFlags | PropagatorFlags.Key));
                }
                if (MetadataHelper.GetConcurrencyMode(element) == ConcurrencyMode.Fixed)
                {
                    noFlags = (PropagatorFlags) ((byte) (noFlags | PropagatorFlags.ConcurrencyValue));
                }
                bool isServerGenerated = this.m_translator.ViewLoader.IsServerGen(element);
                bool isNullConditionMember = this.m_translator.ViewLoader.IsNullConditionMember(element);
                this.m_memberMap.Add(element.Name, new MemberInformation(i, noFlags, element, isServerGenerated, isNullConditionMember));
            }
        }

        private PropagatorResult CreateEntityKeyResult(IEntityStateEntry stateEntry, EntityKey entityKey)
        {
            RowType keyRowType = entityKey.GetEntitySet(this.m_translator.MetadataWorkspace).ElementType.GetKeyRowType(this.m_translator.MetadataWorkspace);
            ExtractorMetadata extractorMetadata = this.m_translator.GetExtractorMetadata(keyRowType);
            PropagatorResult[] values = new PropagatorResult[keyRowType.Members.Count];
            for (int i = 0; i < keyRowType.Members.Count; i++)
            {
                EdmMember member = keyRowType.Members[i];
                MemberInformation information = extractorMetadata.m_memberMap[member.Name];
                long identifier = this.m_translator.KeyManager.GetKeyIdentifierForMemberOffset(entityKey, i, keyRowType.Members.Count);
                object obj2 = null;
                if (entityKey.IsTemporary)
                {
                    obj2 = stateEntry.StateManager.GetEntityStateEntry(entityKey).CurrentValues[member.Name];
                }
                else
                {
                    obj2 = entityKey.FindValueByName(member.Name);
                }
                values[i] = PropagatorResult.CreateKeyValue(information.Flags, obj2, stateEntry, identifier);
            }
            return PropagatorResult.CreateStructuralValue(values, extractorMetadata.m_type, false);
        }

        private PropagatorResult CreateSimpleResult(IEntityStateEntry stateEntry, IExtendedDataRecord record, MemberInformation memberInformation, long identifier, bool isModified, int recordOrdinal, object value)
        {
            CurrentValueRecord record2 = record as CurrentValueRecord;
            PropagatorFlags flags = memberInformation.Flags;
            if (!isModified)
            {
                flags = (PropagatorFlags) ((byte) (flags | (PropagatorFlags.NoFlags | PropagatorFlags.Preserve)));
            }
            if (-1L != identifier)
            {
                PropagatorResult result;
                if (memberInformation.IsServerGenerated && (record2 != null))
                {
                    result = PropagatorResult.CreateServerGenKeyValue(flags, value, stateEntry, identifier, recordOrdinal);
                }
                else
                {
                    result = PropagatorResult.CreateKeyValue(flags, value, stateEntry, identifier);
                }
                this.m_translator.KeyManager.RegisterIdentifierOwner(result);
                return result;
            }
            if (memberInformation.IsServerGenerated && (record2 != null))
            {
                return PropagatorResult.CreateServerGenSimpleValue(flags, value, record2, recordOrdinal);
            }
            return PropagatorResult.CreateSimpleValue(flags, value);
        }

        internal static PropagatorResult ExtractResultFromRecord(IEntityStateEntry stateEntry, bool isModified, IExtendedDataRecord record, IEnumerable<string> modifiedPropertyNames, UpdateTranslator translator)
        {
            StructuralType edmType = (StructuralType) record.DataRecordInfo.RecordType.EdmType;
            ExtractorMetadata extractorMetadata = translator.GetExtractorMetadata(edmType);
            EntityKey entityKey = stateEntry.EntityKey;
            BitArray modifiedPropertiesBitArray = extractorMetadata.GetModifiedPropertiesBitArray(modifiedPropertyNames, stateEntry);
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(edmType);
            PropagatorResult[] values = new PropagatorResult[allStructuralMembers.Count];
            for (int i = 0; i < values.Length; i++)
            {
                EdmMember member = allStructuralMembers[i];
                values[i] = extractorMetadata.RetrieveMember(stateEntry, record, entityKey, member, modifiedPropertiesBitArray);
            }
            return PropagatorResult.CreateStructuralValue(values, edmType, isModified);
        }

        internal BitArray GetModifiedPropertiesBitArray(IEnumerable<string> modifiedProperties, IEntityStateEntry stateEntry)
        {
            int count = TypeHelpers.GetAllStructuralMembers(this.m_type).Count;
            if (modifiedProperties == null)
            {
                return null;
            }
            BitArray array = new BitArray(count, false);
            foreach (string str in modifiedProperties)
            {
                MemberInformation information;
                if (!this.m_memberMap.TryGetValue(str, out information))
                {
                    throw EntityUtil.Update(System.Data.Entity.Strings.Update_MissingModifiedProperty(str, this.m_type.Name), null, new IEntityStateEntry[] { stateEntry });
                }
                array[information.Ordinal] = true;
            }
            return array;
        }

        internal PropagatorResult RetrieveMember(IEntityStateEntry stateEntry, IExtendedDataRecord record, EntityKey key, EdmMember member, BitArray modifiedProperties)
        {
            long num;
            MemberInformation memberInformation = this.m_memberMap[member.Name];
            if (memberInformation.IsKeyMember)
            {
                IList<EdmMember> keyMembers = ((EntitySet) stateEntry.EntitySet).ElementType.KeyMembers;
                int index = keyMembers.IndexOf(member);
                num = this.m_translator.KeyManager.GetKeyIdentifierForMemberOffset(key, index, keyMembers.Count);
            }
            else
            {
                num = -1L;
            }
            bool isModified = (modifiedProperties == null) || modifiedProperties[memberInformation.Ordinal];
            int ordinal = record.GetOrdinal(member.Name);
            if (memberInformation.CheckIsNotNull && record.IsDBNull(ordinal))
            {
                throw EntityUtil.Update(System.Data.Entity.Strings.Update_NullValue(member.Name), null, new IEntityStateEntry[] { stateEntry });
            }
            object obj2 = record.GetValue(ordinal);
            EntityKey entityKey = obj2 as EntityKey;
            if (entityKey != null)
            {
                return this.CreateEntityKeyResult(stateEntry, entityKey);
            }
            IExtendedDataRecord record2 = obj2 as IExtendedDataRecord;
            if (record2 != null)
            {
                IEnumerable<string> modifiedPropertyNames = isModified ? null : Enumerable.Empty<string>();
                UpdateTranslator translator = this.m_translator;
                return ExtractResultFromRecord(stateEntry, isModified, record2, modifiedPropertyNames, translator);
            }
            return this.CreateSimpleResult(stateEntry, record, memberInformation, num, isModified, ordinal, obj2);
        }

        private class MemberInformation
        {
            internal readonly bool CheckIsNotNull;
            internal readonly PropagatorFlags Flags;
            internal readonly bool IsServerGenerated;
            internal readonly EdmMember Member;
            internal readonly int Ordinal;

            internal MemberInformation(int ordinal, PropagatorFlags flags, EdmMember member, bool isServerGenerated, bool isNullConditionMember)
            {
                this.Ordinal = ordinal;
                this.Flags = flags;
                this.Member = member;
                this.IsServerGenerated = isServerGenerated;
                this.CheckIsNotNull = !TypeSemantics.IsNullable(member) && (isNullConditionMember || (member.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType));
            }

            internal bool IsKeyMember =>
                (0x10 == ((byte) (this.Flags & PropagatorFlags.Key)));
        }
    }
}

