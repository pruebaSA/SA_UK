namespace System.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable, DataContract(IsReference=true), DebuggerDisplay("{ConcatKeyValue()}")]
    public sealed class EntityKey : IEquatable<EntityKey>
    {
        private object[] _compositeKeyValues;
        [NonSerialized]
        private EntityKeyMember[] _deserializedMembers;
        private string _entityContainerName;
        private string _entitySetName;
        [NonSerialized]
        private int _hashCode;
        private bool _isLocked;
        private string[] _keyNames;
        private static Dictionary<string, string> _nameLookup = new Dictionary<string, string>();
        private object _singletonKeyValue;
        public static readonly EntityKey EntityNotValidKey = new EntityKey("EntityNotValidKey.EntityNotValidKey");
        public static readonly EntityKey NoEntitySetKey = new EntityKey("NoEntitySetKey.NoEntitySetKey");
        private const string s_EntityNotValidKey = "EntityNotValidKey.EntityNotValidKey";
        private const string s_NoEntitySetKey = "NoEntitySetKey.NoEntitySetKey";

        public EntityKey()
        {
            this._isLocked = false;
        }

        internal EntityKey(EntitySetBase entitySet)
        {
            EntityUtil.CheckArgumentNull<EntitySetBase>(entitySet, "entitySet");
            this._entitySetName = entitySet.Name;
            this._entityContainerName = entitySet.EntityContainer.Name;
            this._isLocked = true;
        }

        internal EntityKey(string qualifiedEntitySetName)
        {
            GetEntitySetName(qualifiedEntitySetName, out this._entitySetName, out this._entityContainerName);
            this._isLocked = true;
        }

        internal EntityKey(EntitySet entitySet, IExtendedDataRecord record)
        {
            EntityUtil.CheckArgumentNull<EntitySet>(entitySet, "entitySet");
            EntityUtil.CheckArgumentNull<string>(entitySet.Name, "entitySet.Name");
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EntityContainer>(entitySet.EntityContainer, "entitySet.Container");
            EntityUtil.CheckArgumentNull<string>(entitySet.EntityContainer.Name, "entitySet.Container.Name");
            EntityUtil.CheckArgumentNull<IExtendedDataRecord>(record, "record");
            this._entitySetName = entitySet.Name;
            this._entityContainerName = entitySet.EntityContainer.Name;
            CheckKeyValues(entitySet, record, out this._keyNames, out this._singletonKeyValue, out this._compositeKeyValues);
            this._isLocked = true;
        }

        internal EntityKey(EntitySetBase entitySet, object singletonKeyValue)
        {
            this._singletonKeyValue = singletonKeyValue;
            this._entitySetName = entitySet.Name;
            this._entityContainerName = entitySet.EntityContainer.Name;
            this._keyNames = entitySet.ElementType.KeyMemberNames;
            this._isLocked = true;
        }

        internal EntityKey(EntitySetBase entitySet, object[] compositeKeyValues)
        {
            this._compositeKeyValues = compositeKeyValues;
            this._entitySetName = entitySet.Name;
            this._entityContainerName = entitySet.EntityContainer.Name;
            this._keyNames = entitySet.ElementType.KeyMemberNames;
            this._isLocked = true;
        }

        public EntityKey(string qualifiedEntitySetName, IEnumerable<KeyValuePair<string, object>> entityKeyValues)
        {
            GetEntitySetName(qualifiedEntitySetName, out this._entitySetName, out this._entityContainerName);
            CheckKeyValues(entityKeyValues, out this._keyNames, out this._singletonKeyValue, out this._compositeKeyValues);
            this._isLocked = true;
        }

        public EntityKey(string qualifiedEntitySetName, IEnumerable<EntityKeyMember> entityKeyValues)
        {
            GetEntitySetName(qualifiedEntitySetName, out this._entitySetName, out this._entityContainerName);
            CheckKeyValues(new KeyValueReader(entityKeyValues), out this._keyNames, out this._singletonKeyValue, out this._compositeKeyValues);
            this._isLocked = true;
        }

        public EntityKey(string qualifiedEntitySetName, string keyName, object keyValue)
        {
            GetEntitySetName(qualifiedEntitySetName, out this._entitySetName, out this._entityContainerName);
            EntityUtil.CheckStringArgument(keyName, "keyName");
            EntityUtil.CheckArgumentNull<object>(keyValue, "keyValue");
            this._keyNames = new string[1];
            ValidateName(keyName);
            this._keyNames[0] = keyName;
            this._singletonKeyValue = keyValue;
            this._isLocked = true;
        }

        [Conditional("DEBUG")]
        private void AssertCorrectState(EntitySetBase entitySet, bool isTemporary)
        {
            if (this._singletonKeyValue != null)
            {
                if (entitySet == null)
                {
                }
            }
            else if (this._compositeKeyValues != null)
            {
                for (int i = 0; i < this._compositeKeyValues.Length; i++)
                {
                }
            }
            else
            {
                bool flag1 = this.IsTemporary;
            }
        }

        private static bool CheckKeyValues(IEnumerable<KeyValuePair<string, object>> entityKeyValues, out string[] keyNames, out object singletonKeyValue, out object[] compositeKeyValues) => 
            CheckKeyValues(entityKeyValues, false, false, out keyNames, out singletonKeyValue, out compositeKeyValues);

        private static void CheckKeyValues(EntitySet entitySet, IEnumerable<KeyValuePair<string, object>> entityKeyValues, out string[] keyNames, out object singletonKeyValue, out object[] compositeKeyValues)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<KeyValuePair<string, object>>>(entityKeyValues, "entityKeyValues");
            int expectedNumFields = 0;
            int actualNumFields = 0;
            ReadOnlyMetadataCollection<EdmMember> keyMembers = null;
            singletonKeyValue = null;
            compositeKeyValues = null;
            using (IEnumerator<KeyValuePair<string, object>> enumerator = entityKeyValues.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> current = enumerator.Current;
                    actualNumFields++;
                }
            }
            keyMembers = entitySet.ElementType.KeyMembers;
            keyNames = entitySet.ElementType.KeyMemberNames;
            expectedNumFields = keyMembers.Count;
            if (expectedNumFields != actualNumFields)
            {
                throw EntityUtil.IncorrectNumberOfKeyValuePairs("entityKeyValues", entitySet.ElementType.FullName, expectedNumFields, actualNumFields);
            }
            if (expectedNumFields == 1)
            {
                foreach (KeyValuePair<string, object> pair in entityKeyValues)
                {
                    if (EntityUtil.IsNull(pair.Value) || string.IsNullOrEmpty(pair.Key))
                    {
                        throw EntityUtil.NoNullsAllowedInKeyValuePairs("entityKeyValues");
                    }
                    if (keyNames[0] != pair.Key)
                    {
                        throw EntityUtil.MissingKeyValue("entityKeyValues", keyNames[0], entitySet.ElementType.FullName);
                    }
                    singletonKeyValue = pair.Value;
                    Type clrEquivalentType = ((PrimitiveType) keyMembers[0].TypeUsage.EdmType).ClrEquivalentType;
                    if (clrEquivalentType != singletonKeyValue.GetType())
                    {
                        throw EntityUtil.IncorrectValueType("entityKeyValues", keyMembers[0].Name, clrEquivalentType.FullName, singletonKeyValue.GetType().FullName);
                    }
                }
            }
            else
            {
                compositeKeyValues = new object[expectedNumFields];
                int index = 0;
                for (index = 0; index < expectedNumFields; index++)
                {
                    EdmMember member = entitySet.ElementType.KeyMembers[index];
                    bool flag = false;
                    foreach (KeyValuePair<string, object> pair2 in entityKeyValues)
                    {
                        if (member.Name == pair2.Key)
                        {
                            if (EntityUtil.IsNull(pair2.Value) || string.IsNullOrEmpty(pair2.Key))
                            {
                                throw EntityUtil.NoNullsAllowedInKeyValuePairs("entityKeyValues");
                            }
                            compositeKeyValues[index] = pair2.Value;
                            flag = true;
                            Type type2 = ((PrimitiveType) member.TypeUsage.EdmType).ClrEquivalentType;
                            if (type2 != compositeKeyValues[index].GetType())
                            {
                                throw EntityUtil.IncorrectValueType("entityKeyValues", member.Name, type2.FullName, compositeKeyValues[index].GetType().FullName);
                            }
                            break;
                        }
                    }
                    if (!flag)
                    {
                        throw EntityUtil.MissingKeyValue("entityKeyValues", member.Name, entitySet.ElementType.FullName);
                    }
                }
            }
        }

        private static void CheckKeyValues(EntitySet entitySet, IExtendedDataRecord record, out string[] keyNames, out object singletonKeyValue, out object[] compositeKeyValues)
        {
            singletonKeyValue = null;
            compositeKeyValues = null;
            int count = entitySet.ElementType.KeyMembers.Count;
            keyNames = entitySet.ElementType.KeyMemberNames;
            EntityType edmType = record.DataRecordInfo.RecordType.EdmType as EntityType;
            if (edmType == null)
            {
                throw EntityUtil.DataRecordMustBeEntity();
            }
            if ((entitySet != null) && !entitySet.ElementType.IsAssignableFrom(edmType))
            {
                throw EntityUtil.EntityTypesDoNotMatch(edmType.Name, entitySet.ElementType.FullName);
            }
            if (count == 1)
            {
                EdmMember member = edmType.KeyMembers[0];
                try
                {
                    singletonKeyValue = CheckValue("record", member.Name, record[member.Name], (PrimitiveType) member.TypeUsage.EdmType);
                    return;
                }
                catch (IndexOutOfRangeException exception)
                {
                    throw EntityUtil.MissingKeyValue("record", member.Name, edmType.FullName, exception);
                }
            }
            compositeKeyValues = new object[count];
            for (int i = 0; i < count; i++)
            {
                EdmMember member2 = edmType.KeyMembers[i];
                try
                {
                    compositeKeyValues[i] = CheckValue("record", member2.Name, record[member2.Name], (PrimitiveType) member2.TypeUsage.EdmType);
                }
                catch (IndexOutOfRangeException exception2)
                {
                    throw EntityUtil.MissingKeyValue("record", member2.Name, edmType.FullName, exception2);
                }
            }
        }

        private static bool CheckKeyValues(IEnumerable<KeyValuePair<string, object>> entityKeyValues, bool allowNullKeys, bool tokenizeStrings, out string[] keyNames, out object singletonKeyValue, out object[] compositeKeyValues)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<KeyValuePair<string, object>>>(entityKeyValues, "entityKeyValues");
            int num2 = 0;
            keyNames = null;
            singletonKeyValue = null;
            compositeKeyValues = null;
            using (IEnumerator<KeyValuePair<string, object>> enumerator = entityKeyValues.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, object> current = enumerator.Current;
                    num2++;
                }
            }
            int num = num2;
            if (num == 0)
            {
                if (!allowNullKeys)
                {
                    throw EntityUtil.EntityKeyMustHaveValues("entityKeyValues");
                }
            }
            else
            {
                keyNames = new string[num];
                if (num == 1)
                {
                    lock (_nameLookup)
                    {
                        foreach (KeyValuePair<string, object> pair in entityKeyValues)
                        {
                            if (EntityUtil.IsNull(pair.Value) || string.IsNullOrEmpty(pair.Key))
                            {
                                throw EntityUtil.NoNullsAllowedInKeyValuePairs("entityKeyValues");
                            }
                            ValidateName(pair.Key);
                            keyNames[0] = tokenizeStrings ? LookupSingletonName(pair.Key) : pair.Key;
                            singletonKeyValue = pair.Value;
                        }
                        goto Label_01B2;
                    }
                }
                compositeKeyValues = new object[num];
                int index = 0;
                lock (_nameLookup)
                {
                    foreach (KeyValuePair<string, object> pair2 in entityKeyValues)
                    {
                        if (EntityUtil.IsNull(pair2.Value) || string.IsNullOrEmpty(pair2.Key))
                        {
                            throw EntityUtil.NoNullsAllowedInKeyValuePairs("entityKeyValues");
                        }
                        ValidateName(pair2.Key);
                        keyNames[index] = tokenizeStrings ? LookupSingletonName(pair2.Key) : pair2.Key;
                        compositeKeyValues[index] = pair2.Value;
                        index++;
                    }
                }
            }
        Label_01B2:
            return (num > 0);
        }

        private static object CheckValue(string argumentName, string keyFieldName, object value, PrimitiveType expectedType)
        {
            if (EntityUtil.IsNull(value))
            {
                throw EntityUtil.NoNullsAllowedInKeyValuePairs(argumentName);
            }
            if (expectedType.ClrEquivalentType != value.GetType())
            {
                throw EntityUtil.IncorrectValueType(argumentName, keyFieldName, expectedType.ClrEquivalentType.FullName, value.GetType().FullName);
            }
            return value;
        }

        private static bool ComplexKeyValuesEqual(EntityKey key1, EntityKey key2)
        {
            for (int i = 0; i < key1._compositeKeyValues.Length; i++)
            {
                if (key1._keyNames[i].Equals(key2._keyNames[i]))
                {
                    if (!object.Equals(key1._compositeKeyValues[i], key2._compositeKeyValues[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    return KeyValuesEqual(key1, key2);
                }
            }
            return true;
        }

        internal string ConcatKeyValue()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("EntitySet=").Append(this._entitySetName);
            if (!this.IsTemporary)
            {
                foreach (EntityKeyMember member in this.EntityKeyValues)
                {
                    builder.Append(';');
                    builder.Append(member.Key).Append("=").Append(member.Value);
                }
            }
            return builder.ToString();
        }

        private void DeserializeMembers()
        {
            if (CheckKeyValues(new KeyValueReader(this._deserializedMembers), true, true, out this._keyNames, out this._singletonKeyValue, out this._compositeKeyValues))
            {
                this._deserializedMembers = null;
            }
        }

        public bool Equals(EntityKey other) => 
            InternalEquals(this, other);

        public override bool Equals(object obj) => 
            InternalEquals(this, obj as EntityKey);

        internal object FindValueByName(string keyName)
        {
            if (this.SingletonKeyValue != null)
            {
                return this._singletonKeyValue;
            }
            object[] compositeKeyValues = this.CompositeKeyValues;
            for (int i = 0; i < compositeKeyValues.Length; i++)
            {
                if (keyName == this._keyNames[i])
                {
                    return compositeKeyValues[i];
                }
            }
            throw EntityUtil.ArgumentOutOfRange("keyName");
        }

        public EntitySet GetEntitySet(MetadataWorkspace metadataWorkspace)
        {
            EntityUtil.CheckArgumentNull<MetadataWorkspace>(metadataWorkspace, "metadataWorkspace");
            if (string.IsNullOrEmpty(this._entityContainerName) || string.IsNullOrEmpty(this._entitySetName))
            {
                throw EntityUtil.MissingQualifiedEntitySetName();
            }
            return metadataWorkspace.GetEntityContainer(this._entityContainerName, DataSpace.CSpace).GetEntitySetByName(this._entitySetName, false);
        }

        internal static void GetEntitySetName(string qualifiedEntitySetName, out string entitySet, out string container)
        {
            entitySet = null;
            container = null;
            EntityUtil.CheckStringArgument(qualifiedEntitySetName, "qualifiedEntitySetName");
            string[] strArray = qualifiedEntitySetName.Split(new char[] { '.' });
            if (strArray.Length != 2)
            {
                throw EntityUtil.InvalidQualifiedEntitySetName();
            }
            container = strArray[0];
            entitySet = strArray[1];
            if (((container == null) || (container.Length == 0)) || ((entitySet == null) || (entitySet.Length == 0)))
            {
                throw EntityUtil.InvalidQualifiedEntitySetName();
            }
            ValidateName(container);
            ValidateName(entitySet);
        }

        public override int GetHashCode()
        {
            int hashCode = this._hashCode;
            if (hashCode == 0)
            {
                if (this.RequiresDeserialization)
                {
                    this.DeserializeMembers();
                }
                int num2 = 0;
                if (this._entitySetName != null)
                {
                    num2 += this._entitySetName.GetHashCode();
                }
                if (this._entityContainerName != null)
                {
                    num2 += this._entityContainerName.GetHashCode();
                }
                if (this._singletonKeyValue != null)
                {
                    hashCode = num2 ^ this._singletonKeyValue.GetHashCode();
                }
                else if (this._compositeKeyValues != null)
                {
                    hashCode = num2 ^ GetHashCode(this._compositeKeyValues);
                }
                else
                {
                    hashCode = base.GetHashCode();
                }
                if (!this._isLocked && ((string.IsNullOrEmpty(this._entitySetName) || string.IsNullOrEmpty(this._entityContainerName)) || ((this._singletonKeyValue == null) && (this._compositeKeyValues == null))))
                {
                    return hashCode;
                }
                this._hashCode = hashCode;
            }
            return hashCode;
        }

        private static int GetHashCode(object[] compositeKeyValues)
        {
            int num = 0;
            for (int i = 0; i < compositeKeyValues.Length; i++)
            {
                num += compositeKeyValues[i].GetHashCode();
            }
            return num;
        }

        internal KeyValuePair<string, DbExpression>[] GetKeyValueExpressions(DbCommandTree tree, EntitySet entitySet)
        {
            int length = 0;
            if (!this.IsTemporary)
            {
                if (this._singletonKeyValue != null)
                {
                    length = 1;
                }
                else
                {
                    length = this._compositeKeyValues.Length;
                }
            }
            if (entitySet.ElementType.KeyMembers.Count != length)
            {
                throw EntityUtil.EntitySetDoesNotMatch("metadataWorkspace", TypeHelpers.GetFullName(entitySet));
            }
            if (this._singletonKeyValue != null)
            {
                EdmMember member = entitySet.ElementType.KeyMembers[0];
                return new KeyValuePair<string, DbExpression>[] { new KeyValuePair<string, DbExpression>(member.Name, tree.CreateConstantExpression(this._singletonKeyValue, Helper.GetModelTypeUsage(member))) };
            }
            KeyValuePair<string, DbExpression>[] pairArray = new KeyValuePair<string, DbExpression>[this._compositeKeyValues.Length];
            for (int i = 0; i < this._compositeKeyValues.Length; i++)
            {
                EdmMember member2 = entitySet.ElementType.KeyMembers[i];
                pairArray[i] = new KeyValuePair<string, DbExpression>(member2.Name, tree.CreateConstantExpression(this._compositeKeyValues[i], Helper.GetModelTypeUsage(member2)));
            }
            return pairArray;
        }

        private static bool InternalEquals(EntityKey key1, EntityKey key2)
        {
            if (object.ReferenceEquals(key1, key2))
            {
                return true;
            }
            if (object.ReferenceEquals(key1, null) || object.ReferenceEquals(key2, null))
            {
                return false;
            }
            if (key1.RequiresDeserialization)
            {
                key1.DeserializeMembers();
            }
            else if (key2.RequiresDeserialization)
            {
                key2.DeserializeMembers();
            }
            if (key1._singletonKeyValue != null)
            {
                return (((key1._singletonKeyValue.Equals(key2._singletonKeyValue) && string.Equals(key1._keyNames[0], key2._keyNames[0])) && string.Equals(key1._entityContainerName, key2._entityContainerName)) && string.Equals(key1._entitySetName, key2._entitySetName));
            }
            return (((((key1._compositeKeyValues != null) && (key2._compositeKeyValues != null)) && (string.Equals(key1._entityContainerName, key2._entityContainerName) && string.Equals(key1._entitySetName, key2._entitySetName))) && (key1._compositeKeyValues.Length == key2._compositeKeyValues.Length)) && ComplexKeyValuesEqual(key1, key2));
        }

        private static bool KeyValuesEqual(EntityKey key1, EntityKey key2)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (EntityKeyMember member in key1.EntityKeyValues)
            {
                dictionary.Add(member.Key, member.Value);
            }
            foreach (EntityKeyMember member2 in key2.EntityKeyValues)
            {
                object obj2;
                if (!dictionary.TryGetValue(member2.Key, out obj2) || !object.Equals(member2.Value, obj2))
                {
                    return false;
                }
                dictionary.Remove(member2.Key);
            }
            return true;
        }

        private static string LookupSingletonName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (_nameLookup.ContainsKey(name))
            {
                return _nameLookup[name];
            }
            _nameLookup.Add(name, name);
            return name;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            lock (_nameLookup)
            {
                this._entitySetName = LookupSingletonName(this._entitySetName);
                this._entityContainerName = LookupSingletonName(this._entityContainerName);
                if (this._keyNames != null)
                {
                    for (int i = 0; i < this._keyNames.Length; i++)
                    {
                        this._keyNames[i] = LookupSingletonName(this._keyNames[i]);
                    }
                }
            }
        }

        [OnDeserializing]
        public void OnDeserializing(StreamingContext context)
        {
            if (this.RequiresDeserialization)
            {
                this.DeserializeMembers();
            }
        }

        public static bool operator ==(EntityKey key1, EntityKey key2) => 
            InternalEquals(key1, key2);

        public static bool operator !=(EntityKey key1, EntityKey key2) => 
            !InternalEquals(key1, key2);

        internal void ValidateEntityKey(EntitySet entitySet)
        {
            this.ValidateEntityKey(entitySet, false, null);
        }

        internal void ValidateEntityKey(EntitySet entitySet, bool isArgumentException, string argumentName)
        {
            if (entitySet != null)
            {
                ReadOnlyMetadataCollection<EdmMember> keyMembers = entitySet.ElementType.KeyMembers;
                if (this._singletonKeyValue != null)
                {
                    if (keyMembers.Count != 1)
                    {
                        if (isArgumentException)
                        {
                            throw EntityUtil.IncorrectNumberOfKeyValuePairs(argumentName, entitySet.ElementType.FullName, keyMembers.Count, 1);
                        }
                        throw EntityUtil.IncorrectNumberOfKeyValuePairsInvalidOperation(entitySet.ElementType.FullName, keyMembers.Count, 1);
                    }
                    Type clrEquivalentType = ((PrimitiveType) keyMembers[0].TypeUsage.EdmType).ClrEquivalentType;
                    if (clrEquivalentType != this._singletonKeyValue.GetType())
                    {
                        if (isArgumentException)
                        {
                            throw EntityUtil.IncorrectValueType(argumentName, keyMembers[0].Name, clrEquivalentType.FullName, this._singletonKeyValue.GetType().FullName);
                        }
                        throw EntityUtil.IncorrectValueTypeInvalidOperation(keyMembers[0].Name, clrEquivalentType.FullName, this._singletonKeyValue.GetType().FullName);
                    }
                    if (this._keyNames[0] != keyMembers[0].Name)
                    {
                        if (isArgumentException)
                        {
                            throw EntityUtil.MissingKeyValue(argumentName, keyMembers[0].Name, entitySet.ElementType.FullName);
                        }
                        throw EntityUtil.MissingKeyValueInvalidOperation(keyMembers[0].Name, entitySet.ElementType.FullName);
                    }
                }
                else if (this._compositeKeyValues != null)
                {
                    if (keyMembers.Count != this._compositeKeyValues.Length)
                    {
                        if (isArgumentException)
                        {
                            throw EntityUtil.IncorrectNumberOfKeyValuePairs(argumentName, entitySet.ElementType.FullName, keyMembers.Count, this._compositeKeyValues.Length);
                        }
                        throw EntityUtil.IncorrectNumberOfKeyValuePairsInvalidOperation(entitySet.ElementType.FullName, keyMembers.Count, this._compositeKeyValues.Length);
                    }
                    for (int i = 0; i < this._compositeKeyValues.Length; i++)
                    {
                        EdmMember member = entitySet.ElementType.KeyMembers[i];
                        bool flag = false;
                        for (int j = 0; j < this._compositeKeyValues.Length; j++)
                        {
                            if (member.Name == this._keyNames[j])
                            {
                                Type type2 = ((PrimitiveType) member.TypeUsage.EdmType).ClrEquivalentType;
                                if (type2 != this._compositeKeyValues[j].GetType())
                                {
                                    if (isArgumentException)
                                    {
                                        throw EntityUtil.IncorrectValueType(argumentName, member.Name, type2.FullName, this._compositeKeyValues[j].GetType().FullName);
                                    }
                                    throw EntityUtil.IncorrectValueTypeInvalidOperation(member.Name, type2.FullName, this._compositeKeyValues[j].GetType().FullName);
                                }
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            if (isArgumentException)
                            {
                                throw EntityUtil.MissingKeyValue(argumentName, member.Name, entitySet.ElementType.FullName);
                            }
                            throw EntityUtil.MissingKeyValueInvalidOperation(member.Name, entitySet.ElementType.FullName);
                        }
                    }
                }
            }
        }

        internal static void ValidateName(string name)
        {
            if (!Utils.ValidUndottedName(name))
            {
                throw EntityUtil.EntityKeyInvalidName(name);
            }
        }

        private void ValidateWritable(object instance)
        {
            if (this._isLocked || (instance != null))
            {
                throw EntityUtil.CannotChangeEntityKey();
            }
        }

        private object[] CompositeKeyValues
        {
            get
            {
                if (this.RequiresDeserialization)
                {
                    this.DeserializeMembers();
                }
                return this._compositeKeyValues;
            }
        }

        [DataMember]
        public string EntityContainerName
        {
            get => 
                this._entityContainerName;
            set
            {
                this.ValidateWritable(this._entityContainerName);
                lock (_nameLookup)
                {
                    this._entityContainerName = LookupSingletonName(value);
                }
            }
        }

        [DataMember]
        public EntityKeyMember[] EntityKeyValues
        {
            get
            {
                if (this.IsTemporary)
                {
                    return null;
                }
                if (this._singletonKeyValue != null)
                {
                    return new EntityKeyMember[] { new EntityKeyMember(this._keyNames[0], this._singletonKeyValue) };
                }
                EntityKeyMember[] memberArray = new EntityKeyMember[this._compositeKeyValues.Length];
                for (int i = 0; i < this._compositeKeyValues.Length; i++)
                {
                    memberArray[i] = new EntityKeyMember(this._keyNames[i], this._compositeKeyValues[i]);
                }
                return memberArray;
            }
            set
            {
                this.ValidateWritable(this._keyNames);
                if ((value != null) && !CheckKeyValues(new KeyValueReader(value), true, true, out this._keyNames, out this._singletonKeyValue, out this._compositeKeyValues))
                {
                    this._deserializedMembers = value;
                }
            }
        }

        [DataMember]
        public string EntitySetName
        {
            get => 
                this._entitySetName;
            set
            {
                this.ValidateWritable(this._entitySetName);
                lock (_nameLookup)
                {
                    this._entitySetName = LookupSingletonName(value);
                }
            }
        }

        public bool IsTemporary =>
            ((this.SingletonKeyValue == null) && (this.CompositeKeyValues == null));

        private bool RequiresDeserialization =>
            (this._deserializedMembers != null);

        private object SingletonKeyValue
        {
            get
            {
                if (this.RequiresDeserialization)
                {
                    this.DeserializeMembers();
                }
                return this._singletonKeyValue;
            }
        }

        private class KeyValueReader : IEnumerable<KeyValuePair<string, object>>, IEnumerable
        {
            private IEnumerable<EntityKeyMember> _enumerator;

            public KeyValueReader(IEnumerable<EntityKeyMember> enumerator)
            {
                this._enumerator = enumerator;
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                foreach (EntityKeyMember iteratorVariable0 in this._enumerator)
                {
                    if (iteratorVariable0 != null)
                    {
                        yield return new KeyValuePair<string, object>(iteratorVariable0.Key, iteratorVariable0.Value);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

        }
    }
}

