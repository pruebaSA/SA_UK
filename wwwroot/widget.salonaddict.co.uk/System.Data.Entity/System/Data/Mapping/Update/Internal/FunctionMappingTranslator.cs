namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;

    internal abstract class FunctionMappingTranslator
    {
        protected FunctionMappingTranslator()
        {
        }

        internal static FunctionMappingTranslator CreateAssociationSetFunctionMappingTranslator(StorageAssociationSetMapping setMapping) => 
            new AssociationSetFunctionMappingTranslator(setMapping);

        internal static FunctionMappingTranslator CreateEntitySetFunctionMappingTranslator(StorageEntitySetMapping setMapping) => 
            new EntitySetFunctionMappingTranslator(setMapping);

        internal abstract FunctionUpdateCommand Translate(UpdateTranslator translator, ExtractedStateEntry stateEntry);

        private sealed class AssociationSetFunctionMappingTranslator : FunctionMappingTranslator
        {
            private readonly StorageAssociationSetFunctionMapping m_mapping;

            internal AssociationSetFunctionMappingTranslator(StorageAssociationSetMapping setMapping)
            {
                if (setMapping != null)
                {
                    this.m_mapping = setMapping.FunctionMapping;
                }
            }

            internal override FunctionUpdateCommand Translate(UpdateTranslator translator, ExtractedStateEntry stateEntry)
            {
                PropagatorResult current;
                if (this.m_mapping == null)
                {
                    return null;
                }
                bool flag = EntityState.Added == stateEntry.State;
                StorageFunctionMapping functionMapping = flag ? this.m_mapping.InsertFunctionMapping : this.m_mapping.DeleteFunctionMapping;
                FunctionUpdateCommand command = new FunctionUpdateCommand(functionMapping, translator, stateEntry.Source);
                if (flag)
                {
                    current = stateEntry.Current;
                }
                else
                {
                    current = stateEntry.Original;
                }
                foreach (StorageFunctionParameterBinding binding in functionMapping.ParameterBindings)
                {
                    EdmProperty property = (EdmProperty) binding.MemberPath.Members[0];
                    AssociationEndMember member = (AssociationEndMember) binding.MemberPath.Members[1];
                    PropagatorResult memberValue = current.GetMemberValue(member).GetMemberValue(property);
                    command.SetParameterValue(memberValue, binding, translator);
                }
                command.RegisterRowsAffectedParameter(functionMapping.RowsAffectedParameter);
                return command;
            }
        }

        private sealed class EntitySetFunctionMappingTranslator : FunctionMappingTranslator
        {
            private readonly Dictionary<EntityType, StorageEntityTypeFunctionMapping> m_typeMappings = new Dictionary<EntityType, StorageEntityTypeFunctionMapping>();

            internal EntitySetFunctionMappingTranslator(StorageEntitySetMapping setMapping)
            {
                foreach (StorageEntityTypeFunctionMapping mapping in setMapping.FunctionMappings)
                {
                    this.m_typeMappings.Add(mapping.EntityType, mapping);
                }
            }

            private static void BindFunctionParameters(UpdateTranslator translator, IEntityStateEntry stateEntry, PropagatorResult original, PropagatorResult current, StorageFunctionMapping functionMapping, FunctionUpdateCommand command)
            {
                EntityKey entityKey = stateEntry.EntityKey;
                foreach (StorageFunctionParameterBinding binding in functionMapping.ParameterBindings)
                {
                    PropagatorResult memberValue;
                    if (binding.MemberPath.AssociationSetEnd != null)
                    {
                        memberValue = RetrieveNavigationResult(translator, command, entityKey, binding);
                    }
                    else
                    {
                        memberValue = binding.IsCurrent ? current : original;
                        int count = binding.MemberPath.Members.Count;
                        while (count > 0)
                        {
                            count--;
                            EdmMember member = binding.MemberPath.Members[count];
                            memberValue = memberValue.GetMemberValue(member);
                        }
                    }
                    command.SetParameterValue(memberValue, binding, translator);
                }
                command.RegisterRowsAffectedParameter(functionMapping.RowsAffectedParameter);
            }

            private StorageFunctionMapping GetFunctionMapping(ExtractedStateEntry stateEntry)
            {
                EntityType structuralType;
                if (stateEntry.Current != null)
                {
                    structuralType = (EntityType) stateEntry.Current.StructuralType;
                }
                else
                {
                    structuralType = (EntityType) stateEntry.Original.StructuralType;
                }
                switch (stateEntry.State)
                {
                    case EntityState.Unchanged:
                    case EntityState.Modified:
                        return this.m_typeMappings[structuralType].UpdateFunctionMapping;

                    case EntityState.Added:
                        return this.m_typeMappings[structuralType].InsertFunctionMapping;

                    case EntityState.Deleted:
                        return this.m_typeMappings[structuralType].DeleteFunctionMapping;
                }
                return null;
            }

            private static PropagatorResult RetrieveNavigationResult(UpdateTranslator translator, FunctionUpdateCommand command, EntityKey entityKey, StorageFunctionParameterBinding parameterBinding)
            {
                AssociationSet parentAssociationSet = parameterBinding.MemberPath.AssociationSetEnd.ParentAssociationSet;
                EdmProperty property = (EdmProperty) parameterBinding.MemberPath.Members[0];
                AssociationEndMember correspondingAssociationEndMember = parameterBinding.MemberPath.AssociationSetEnd.CorrespondingAssociationEndMember;
                IEntityStateEntry stateEntry = RetrieveNavigationStateEntry(translator, command, entityKey, parentAssociationSet, correspondingAssociationEndMember, parameterBinding.IsCurrent);
                if (stateEntry == null)
                {
                    if (correspondingAssociationEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                    {
                        string entitySetName = entityKey.EntitySetName;
                        string name = parentAssociationSet.Name;
                        throw EntityUtil.Update(Strings.Update_MissingRequiredRelationshipValue(entitySetName, name), null, command.GetStateEntries(translator));
                    }
                    return PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags, null);
                }
                command.RegisterStateEntry(stateEntry);
                PropagatorResult result = parameterBinding.IsCurrent ? translator.RecordConverter.ConvertCurrentValuesToPropagatorResult(stateEntry, null) : translator.RecordConverter.ConvertOriginalValuesToPropagatorResult(stateEntry, null);
                return result.GetMemberValue(correspondingAssociationEndMember).GetMemberValue(property);
            }

            private static IEntityStateEntry RetrieveNavigationStateEntry(UpdateTranslator translator, FunctionUpdateCommand command, EntityKey entityKey, AssociationSet associationSetNavigation, AssociationEndMember endMember, bool isCurrent)
            {
                foreach (IEntityStateEntry entry2 in translator.GetRelationships(entityKey))
                {
                    if (entry2.EntitySet.EdmEquals(associationSetNavigation))
                    {
                        DbDataRecord currentValues;
                        if (isCurrent)
                        {
                            if ((EntityState.Added != entry2.State) && (EntityState.Unchanged != entry2.State))
                            {
                                continue;
                            }
                            currentValues = entry2.CurrentValues;
                        }
                        else
                        {
                            if ((EntityState.Deleted != entry2.State) && (EntityState.Unchanged != entry2.State))
                            {
                                continue;
                            }
                            currentValues = entry2.OriginalValues;
                        }
                        int ordinal = currentValues.GetOrdinal(endMember.Name);
                        int num2 = (ordinal == 0) ? 1 : 0;
                        EntityKey key = (EntityKey) currentValues[num2];
                        EntityKey key1 = (EntityKey) currentValues[ordinal];
                        if (key == entityKey)
                        {
                            return entry2;
                        }
                    }
                }
                return null;
            }

            internal override FunctionUpdateCommand Translate(UpdateTranslator translator, ExtractedStateEntry stateEntry)
            {
                StorageFunctionMapping functionMapping = this.GetFunctionMapping(stateEntry);
                FunctionUpdateCommand command = new FunctionUpdateCommand(functionMapping, translator, stateEntry.Source);
                BindFunctionParameters(translator, stateEntry.Source, stateEntry.Original, stateEntry.Current, functionMapping, command);
                if (functionMapping.ResultBindings != null)
                {
                    foreach (StorageFunctionResultBinding binding in functionMapping.ResultBindings)
                    {
                        PropagatorResult memberValue = stateEntry.Current.GetMemberValue(binding.Property);
                        command.AddResultColumn(translator, binding.ColumnName, memberValue);
                    }
                }
                return command;
            }
        }
    }
}

