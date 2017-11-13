namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Runtime.InteropServices;

    internal class ComplexTypeMaterializer
    {
        private int _lastPlanIndex;
        private Plan[] _lastPlans;
        private readonly MetadataWorkspace _workspace;
        private const int MaxPlanCount = 4;

        internal ComplexTypeMaterializer(MetadataWorkspace workspace)
        {
            this._workspace = workspace;
        }

        private static object ConvertDBNull(object value)
        {
            if (DBNull.Value == value)
            {
                return null;
            }
            return value;
        }

        internal object CreateComplex(IExtendedDataRecord record, DataRecordInfo recordInfo, object result)
        {
            Plan plan = this.GetPlan(record, recordInfo);
            if (result == null)
            {
                result = ((Func<object>) plan.ClrType)();
            }
            this.SetProperties(record, result, plan.Properties);
            return result;
        }

        private object CreateComplexRecursive(IExtendedDataRecord record, object existing) => 
            this.CreateComplex(record, record.DataRecordInfo, existing);

        private object CreateComplexRecursive(object record, object existing)
        {
            if (DBNull.Value == record)
            {
                return existing;
            }
            return this.CreateComplexRecursive((IExtendedDataRecord) record, existing);
        }

        private Plan GetPlan(IExtendedDataRecord record, DataRecordInfo recordInfo)
        {
            Plan[] planArray = this._lastPlans ?? (this._lastPlans = new Plan[4]);
            int index = this._lastPlanIndex - 1;
            for (int i = 0; i < 4; i++)
            {
                index = (index + 1) % 4;
                if (planArray[index] == null)
                {
                    break;
                }
                if (planArray[index].Key == recordInfo.RecordType)
                {
                    this._lastPlanIndex = index;
                    return planArray[index];
                }
            }
            ObjectTypeMapping objectMapping = System.Data.Common.Internal.Materialization.Util.GetObjectMapping(recordInfo.RecordType.EdmType, this._workspace);
            this._lastPlanIndex = index;
            planArray[index] = new Plan(recordInfo.RecordType, objectMapping, recordInfo.FieldMetadata);
            return planArray[index];
        }

        private void SetProperties(IExtendedDataRecord record, object result, PlanEdmProperty[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetExistingComplex != null)
                {
                    object existing = properties[i].GetExistingComplex(result);
                    object obj3 = this.CreateComplexRecursive(record.GetValue(properties[i].Ordinal), existing);
                    if (existing == null)
                    {
                        properties[i].ClrProperty(result, obj3);
                    }
                }
                else
                {
                    properties[i].ClrProperty(result, ConvertDBNull(record.GetValue(properties[i].Ordinal)));
                }
            }
        }

        private sealed class Plan
        {
            internal readonly Delegate ClrType;
            internal readonly TypeUsage Key;
            internal readonly ComplexTypeMaterializer.PlanEdmProperty[] Properties;

            internal Plan(TypeUsage key, ObjectTypeMapping mapping, System.Collections.ObjectModel.ReadOnlyCollection<FieldMetadata> fields)
            {
                this.Key = key;
                this.ClrType = LightweightCodeGenerator.GetConstructorDelegateForType(mapping.ClrType);
                this.Properties = new ComplexTypeMaterializer.PlanEdmProperty[fields.Count];
                int ordinal = -1;
                for (int i = 0; i < this.Properties.Length; i++)
                {
                    FieldMetadata metadata = fields[i];
                    ordinal = metadata.Ordinal;
                    this.Properties[i] = new ComplexTypeMaterializer.PlanEdmProperty(ordinal, mapping.GetPropertyMap(metadata.FieldType.Name).ClrProperty);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PlanEdmProperty
        {
            internal readonly int Ordinal;
            internal readonly Func<object, object> GetExistingComplex;
            internal readonly Action<object, object> ClrProperty;
            internal PlanEdmProperty(int ordinal, EdmProperty property)
            {
                this.Ordinal = ordinal;
                this.GetExistingComplex = Helper.IsComplexType(property.TypeUsage.EdmType) ? LightweightCodeGenerator.GetGetterDelegateForProperty(property) : null;
                this.ClrProperty = LightweightCodeGenerator.GetSetterDelegateForProperty(property);
            }
        }
    }
}

