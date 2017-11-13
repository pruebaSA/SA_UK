namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Threading;
    using System.Xml;

    internal class EdmProviderManifest : DbProviderManifest
    {
        private static TypeUsage[] _canonicalModelTypes;
        private Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>> _facetDescriptions;
        private System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> _functions;
        private static EdmProviderManifest _instance = new EdmProviderManifest();
        private System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> _primitiveTypes;
        private System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[] _promotionTypes;
        internal const string ConcurrencyModeFacetName = "ConcurrencyMode";
        internal const byte MaximumDateTimePrecision = 0xff;
        internal const byte MaximumDecimalPrecision = 0xff;
        internal const string StoreGeneratedPatternFacetName = "StoreGeneratedPattern";

        private EdmProviderManifest()
        {
        }

        private static EdmFunction CreateAggregateCannonicalFunction(string name, FunctionParameter returnParameter, FunctionParameter parameter) => 
            new EdmFunction(name, "Edm", DataSpace.CSpace, new EdmFunctionPayload { 
                IsAggregate = true,
                IsBuiltIn = true,
                ReturnParameter = returnParameter,
                Parameters = new FunctionParameter[] { parameter }
            });

        private static EdmFunction CreateCannonicalFunction(string name, FunctionParameter returnParameter, params FunctionParameter[] parameters) => 
            new EdmFunction(name, "Edm", DataSpace.CSpace, new EdmFunctionPayload { 
                IsBuiltIn = true,
                ReturnParameter = returnParameter,
                Parameters = parameters
            });

        internal TypeUsage ForgetScalarConstraints(TypeUsage type)
        {
            PrimitiveType edmType = type.EdmType as PrimitiveType;
            if (edmType != null)
            {
                return this.GetCanonicalModelTypeUsage(edmType.PrimitiveTypeKind);
            }
            return type;
        }

        internal TypeUsage GetCanonicalModelTypeUsage(PrimitiveTypeKind primitiveTypeKind)
        {
            if (_canonicalModelTypes == null)
            {
                this.InitializeCanonicalModelTypes();
            }
            return _canonicalModelTypes[(int) primitiveTypeKind];
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            throw new NotImplementedException();
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> GetFacetDescriptions(EdmType type)
        {
            this.InitializeFacetDescriptions();
            System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> onlys = null;
            if (this._facetDescriptions.TryGetValue(type as PrimitiveType, out onlys))
            {
                return onlys;
            }
            return Helper.EmptyFacetDescriptionEnumerable;
        }

        internal static FacetDescription[] GetInitialFacetDescriptions(PrimitiveTypeKind primitiveTypeKind)
        {
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    return new FacetDescription[] { new FacetDescription("MaxLength", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), 0, 0x7fffffff, null), new FacetDescription("FixedLength", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), null, null, null) };

                case PrimitiveTypeKind.DateTime:
                    return new FacetDescription[] { new FacetDescription("Precision", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), 0, 0xff, null) };

                case PrimitiveTypeKind.Decimal:
                    return new FacetDescription[] { new FacetDescription("Precision", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), 1, 0xff, null), new FacetDescription("Scale", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), 0, 0xff, null) };

                case PrimitiveTypeKind.String:
                    return new FacetDescription[] { new FacetDescription("MaxLength", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Int32), 0, 0x7fffffff, null), new FacetDescription("Unicode", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), null, null, null), new FacetDescription("FixedLength", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), null, null, null) };

                case PrimitiveTypeKind.Time:
                    return new FacetDescription[] { new FacetDescription("Precision", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), 0, 0xff, TypeUsage.DefaultDateTimePrecisionFacetValue) };

                case PrimitiveTypeKind.DateTimeOffset:
                    return new FacetDescription[] { new FacetDescription("Precision", MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte), 0, 0xff, TypeUsage.DefaultDateTimePrecisionFacetValue) };
            }
            return null;
        }

        public PrimitiveType GetPrimitiveType(PrimitiveTypeKind primitiveTypeKind)
        {
            this.InitializePrimitiveTypes();
            return this._primitiveTypes[(int) primitiveTypeKind];
        }

        internal System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetPromotionTypes(PrimitiveType primitiveType)
        {
            this.InitializePromotableTypes();
            return this._promotionTypes[(int) primitiveType.PrimitiveTypeKind];
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetStoreFunctions()
        {
            this.InitializeCanonicalFunctions();
            return this._functions;
        }

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetStoreTypes()
        {
            this.InitializePrimitiveTypes();
            return this._primitiveTypes;
        }

        private void InitializeCanonicalFunctions()
        {
            TypeUsage[] parameterTypeUsages;
            if (this._functions == null)
            {
                List<EdmFunction> list = new List<EdmFunction>();
                parameterTypeUsages = new TypeUsage[15];
                Func<PrimitiveTypeKind, FunctionParameter> func = ptk => new FunctionParameter("arg1", parameterTypeUsages[(int) ptk], ParameterMode.In);
                Func<PrimitiveTypeKind, FunctionParameter> func2 = ptk => new FunctionParameter("arg2", parameterTypeUsages[(int) ptk], ParameterMode.In);
                Func<PrimitiveTypeKind, FunctionParameter> func3 = ptk => new FunctionParameter("arg3", parameterTypeUsages[(int) ptk], ParameterMode.In);
                Func<PrimitiveTypeKind, FunctionParameter> func4 = ptk => new FunctionParameter("ReturnType", parameterTypeUsages[(int) ptk], ParameterMode.ReturnValue);
                Func<PrimitiveTypeKind, FunctionParameter> func5 = ptk => new FunctionParameter("arg1", TypeUsage.Create(parameterTypeUsages[(int) ptk].EdmType.GetCollectionType()), ParameterMode.In);
                for (int i = 0; i < 15; i++)
                {
                    parameterTypeUsages[i] = TypeUsage.Create(this._primitiveTypes[i]);
                }
                string[] strArray = new string[] { "Max", "Min" };
                PrimitiveTypeKind[] kindArray10 = new PrimitiveTypeKind[13];
                kindArray10[0] = PrimitiveTypeKind.Byte;
                kindArray10[1] = PrimitiveTypeKind.DateTime;
                kindArray10[2] = PrimitiveTypeKind.Decimal;
                kindArray10[3] = PrimitiveTypeKind.Double;
                kindArray10[4] = PrimitiveTypeKind.Int16;
                kindArray10[5] = PrimitiveTypeKind.Int32;
                kindArray10[6] = PrimitiveTypeKind.Int64;
                kindArray10[7] = PrimitiveTypeKind.SByte;
                kindArray10[8] = PrimitiveTypeKind.Single;
                kindArray10[9] = PrimitiveTypeKind.String;
                kindArray10[11] = PrimitiveTypeKind.Time;
                kindArray10[12] = PrimitiveTypeKind.DateTimeOffset;
                PrimitiveTypeKind[] kindArray = kindArray10;
                foreach (string str in strArray)
                {
                    foreach (PrimitiveTypeKind kind in kindArray)
                    {
                        EdmFunction function = CreateAggregateCannonicalFunction(str, func4(kind), func5(kind));
                        list.Add(function);
                    }
                }
                string[] strArray2 = new string[] { "Avg", "Sum" };
                PrimitiveTypeKind[] kindArray2 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Double, PrimitiveTypeKind.Int32, PrimitiveTypeKind.Int64 };
                foreach (string str2 in strArray2)
                {
                    foreach (PrimitiveTypeKind kind2 in kindArray2)
                    {
                        EdmFunction function2 = CreateAggregateCannonicalFunction(str2, func4(kind2), func5(kind2));
                        list.Add(function2);
                    }
                }
                string name = "StDev";
                PrimitiveTypeKind[] kindArray3 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Double, PrimitiveTypeKind.Int32, PrimitiveTypeKind.Int64 };
                foreach (PrimitiveTypeKind kind3 in kindArray3)
                {
                    EdmFunction function3 = CreateAggregateCannonicalFunction(name, func4(PrimitiveTypeKind.Double), func5(kind3));
                    list.Add(function3);
                }
                for (int j = 0; j < 15; j++)
                {
                    PrimitiveTypeKind arg = (PrimitiveTypeKind) j;
                    EdmFunction function4 = CreateAggregateCannonicalFunction("Count", func4(PrimitiveTypeKind.Int32), func5(arg));
                    list.Add(function4);
                }
                for (int k = 0; k < 15; k++)
                {
                    PrimitiveTypeKind kind5 = (PrimitiveTypeKind) k;
                    EdmFunction function5 = CreateAggregateCannonicalFunction("BigCount", func4(PrimitiveTypeKind.Int64), func5(kind5));
                    list.Add(function5);
                }
                list.Add(CreateCannonicalFunction("Trim", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("RTrim", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("LTrim", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("Concat", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("Length", func4(PrimitiveTypeKind.Int32), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                PrimitiveTypeKind[] kindArray4 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Byte, PrimitiveTypeKind.Int16, PrimitiveTypeKind.Int32, PrimitiveTypeKind.Int64, PrimitiveTypeKind.SByte };
                foreach (PrimitiveTypeKind kind6 in kindArray4)
                {
                    list.Add(CreateCannonicalFunction("Substring", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(kind6), func3(kind6) }));
                }
                foreach (PrimitiveTypeKind kind7 in kindArray4)
                {
                    list.Add(CreateCannonicalFunction("Left", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(kind7) }));
                }
                foreach (PrimitiveTypeKind kind8 in kindArray4)
                {
                    list.Add(CreateCannonicalFunction("Right", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(kind8) }));
                }
                list.Add(CreateCannonicalFunction("Replace", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(PrimitiveTypeKind.String), func3(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("IndexOf", func4(PrimitiveTypeKind.Int32), new FunctionParameter[] { func(PrimitiveTypeKind.String), func2(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("ToUpper", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("ToLower", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                list.Add(CreateCannonicalFunction("Reverse", func4(PrimitiveTypeKind.String), new FunctionParameter[] { func(PrimitiveTypeKind.String) }));
                string[] strArray3 = new string[] { "Year", "Month", "Day" };
                PrimitiveTypeKind[] kindArray5 = new PrimitiveTypeKind[] { PrimitiveTypeKind.DateTimeOffset, PrimitiveTypeKind.DateTime };
                foreach (string str4 in strArray3)
                {
                    foreach (PrimitiveTypeKind kind9 in kindArray5)
                    {
                        list.Add(CreateCannonicalFunction(str4, func4(PrimitiveTypeKind.Int32), new FunctionParameter[] { func(kind9) }));
                    }
                }
                string[] strArray4 = new string[] { "Hour", "Minute", "Second", "Millisecond" };
                PrimitiveTypeKind[] kindArray6 = new PrimitiveTypeKind[] { PrimitiveTypeKind.DateTimeOffset, PrimitiveTypeKind.DateTime, PrimitiveTypeKind.Time };
                foreach (string str5 in strArray4)
                {
                    foreach (PrimitiveTypeKind kind10 in kindArray6)
                    {
                        list.Add(CreateCannonicalFunction(str5, func4(PrimitiveTypeKind.Int32), new FunctionParameter[] { func(kind10) }));
                    }
                }
                EdmFunction item = CreateCannonicalFunction("CurrentDateTime", func4(PrimitiveTypeKind.DateTime), new FunctionParameter[0]);
                list.Add(item);
                EdmFunction function7 = CreateCannonicalFunction("CurrentDateTimeOffset", func4(PrimitiveTypeKind.DateTimeOffset), new FunctionParameter[0]);
                list.Add(function7);
                list.Add(CreateCannonicalFunction("GetTotalOffsetMinutes", func4(PrimitiveTypeKind.Int32), new FunctionParameter[] { func(PrimitiveTypeKind.DateTimeOffset) }));
                item = CreateCannonicalFunction("CurrentUtcDateTime", func4(PrimitiveTypeKind.DateTime), new FunctionParameter[0]);
                list.Add(item);
                string[] strArray5 = new string[] { "Round", "Floor", "Ceiling" };
                PrimitiveTypeKind[] kindArray7 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Single, PrimitiveTypeKind.Double, PrimitiveTypeKind.Decimal };
                foreach (string str6 in strArray5)
                {
                    foreach (PrimitiveTypeKind kind11 in kindArray7)
                    {
                        list.Add(CreateCannonicalFunction(str6, func4(kind11), new FunctionParameter[] { func(kind11) }));
                    }
                }
                PrimitiveTypeKind[] kindArray8 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Decimal, PrimitiveTypeKind.Double, PrimitiveTypeKind.Int16, PrimitiveTypeKind.Int32, PrimitiveTypeKind.Int64, PrimitiveTypeKind.Byte, PrimitiveTypeKind.Single };
                foreach (PrimitiveTypeKind kind12 in kindArray8)
                {
                    list.Add(CreateCannonicalFunction("Abs", func4(kind12), new FunctionParameter[] { func(kind12) }));
                }
                string[] strArray6 = new string[] { "BitwiseAnd", "BitwiseOr", "BitwiseXor" };
                PrimitiveTypeKind[] kindArray9 = new PrimitiveTypeKind[] { PrimitiveTypeKind.Int16, PrimitiveTypeKind.Int32, PrimitiveTypeKind.Int64, PrimitiveTypeKind.Byte };
                foreach (string str7 in strArray6)
                {
                    foreach (PrimitiveTypeKind kind13 in kindArray9)
                    {
                        list.Add(CreateCannonicalFunction(str7, func4(kind13), new FunctionParameter[] { func(kind13), func2(kind13) }));
                    }
                }
                foreach (PrimitiveTypeKind kind14 in kindArray9)
                {
                    list.Add(CreateCannonicalFunction("BitwiseNot", func4(kind14), new FunctionParameter[] { func(kind14) }));
                }
                list.Add(CreateCannonicalFunction("NewGuid", func4(PrimitiveTypeKind.Guid), new FunctionParameter[0]));
                foreach (EdmFunction function8 in list)
                {
                    function8.SetReadOnly();
                }
                System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> onlys = new System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction>(list);
                Interlocked.CompareExchange<System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction>>(ref this._functions, onlys, null);
            }
        }

        private void InitializeCanonicalModelTypes()
        {
            TypeUsage[] usageArray = new TypeUsage[15];
            for (int i = 0; i < 15; i++)
            {
                PrimitiveType edmType = this._primitiveTypes[i];
                usageArray[i] = TypeUsage.CreateDefaultTypeUsage(edmType);
            }
            Interlocked.CompareExchange<TypeUsage[]>(ref _canonicalModelTypes, usageArray, null);
        }

        private void InitializeFacetDescriptions()
        {
            if (this._facetDescriptions == null)
            {
                this.InitializePrimitiveTypes();
                Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>> dictionary = new Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>>();
                FacetDescription[] initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.String);
                PrimitiveType key = this._primitiveTypes[12];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.Binary);
                key = this._primitiveTypes[0];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.DateTime);
                key = this._primitiveTypes[3];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.Time);
                key = this._primitiveTypes[13];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.DateTimeOffset);
                key = this._primitiveTypes[14];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                initialFacetDescriptions = GetInitialFacetDescriptions(PrimitiveTypeKind.Decimal);
                key = this._primitiveTypes[4];
                dictionary.Add(key, Array.AsReadOnly<FacetDescription>(initialFacetDescriptions));
                Interlocked.CompareExchange<Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>>>(ref this._facetDescriptions, dictionary, null);
            }
        }

        private void InitializePrimitiveType(PrimitiveType primitiveType, PrimitiveTypeKind primitiveTypeKind, string name, Type clrType)
        {
            EdmType.Initialize(primitiveType, name, "Edm", DataSpace.CSpace, true, null);
            PrimitiveType.Initialize(primitiveType, primitiveTypeKind, true, this);
        }

        private void InitializePrimitiveTypes()
        {
            if (this._primitiveTypes == null)
            {
                PrimitiveType[] list = new PrimitiveType[15];
                list[0] = new PrimitiveType();
                list[1] = new PrimitiveType();
                list[2] = new PrimitiveType();
                list[3] = new PrimitiveType();
                list[4] = new PrimitiveType();
                list[5] = new PrimitiveType();
                list[7] = new PrimitiveType();
                list[6] = new PrimitiveType();
                list[9] = new PrimitiveType();
                list[10] = new PrimitiveType();
                list[11] = new PrimitiveType();
                list[8] = new PrimitiveType();
                list[12] = new PrimitiveType();
                list[13] = new PrimitiveType();
                list[14] = new PrimitiveType();
                this.InitializePrimitiveType(list[0], PrimitiveTypeKind.Binary, "Binary", typeof(byte[]));
                this.InitializePrimitiveType(list[1], PrimitiveTypeKind.Boolean, "Boolean", typeof(bool));
                this.InitializePrimitiveType(list[2], PrimitiveTypeKind.Byte, "Byte", typeof(byte));
                this.InitializePrimitiveType(list[3], PrimitiveTypeKind.DateTime, "DateTime", typeof(DateTime));
                this.InitializePrimitiveType(list[4], PrimitiveTypeKind.Decimal, "Decimal", typeof(decimal));
                this.InitializePrimitiveType(list[5], PrimitiveTypeKind.Double, "Double", typeof(double));
                this.InitializePrimitiveType(list[7], PrimitiveTypeKind.Single, "Single", typeof(float));
                this.InitializePrimitiveType(list[6], PrimitiveTypeKind.Guid, "Guid", typeof(Guid));
                this.InitializePrimitiveType(list[9], PrimitiveTypeKind.Int16, "Int16", typeof(short));
                this.InitializePrimitiveType(list[10], PrimitiveTypeKind.Int32, "Int32", typeof(int));
                this.InitializePrimitiveType(list[11], PrimitiveTypeKind.Int64, "Int64", typeof(long));
                this.InitializePrimitiveType(list[8], PrimitiveTypeKind.SByte, "SByte", typeof(sbyte));
                this.InitializePrimitiveType(list[12], PrimitiveTypeKind.String, "String", typeof(string));
                this.InitializePrimitiveType(list[13], PrimitiveTypeKind.Time, "Time", typeof(TimeSpan));
                this.InitializePrimitiveType(list[14], PrimitiveTypeKind.DateTimeOffset, "DateTimeOffset", typeof(DateTimeOffset));
                foreach (PrimitiveType type in list)
                {
                    type.ProviderManifest = this;
                    type.SetReadOnly();
                }
                System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> onlys = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(list);
                Interlocked.CompareExchange<System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>>(ref this._primitiveTypes, onlys, null);
            }
        }

        private void InitializePromotableTypes()
        {
            if (this._promotionTypes == null)
            {
                System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[] onlysArray = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[15];
                for (int i = 0; i < 15; i++)
                {
                    onlysArray[i] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[i] });
                }
                onlysArray[2] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[2], this._primitiveTypes[9], this._primitiveTypes[10], this._primitiveTypes[11], this._primitiveTypes[4], this._primitiveTypes[7], this._primitiveTypes[5] });
                onlysArray[9] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[9], this._primitiveTypes[10], this._primitiveTypes[11], this._primitiveTypes[4], this._primitiveTypes[7], this._primitiveTypes[5] });
                onlysArray[10] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[10], this._primitiveTypes[11], this._primitiveTypes[4], this._primitiveTypes[7], this._primitiveTypes[5] });
                onlysArray[11] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[11], this._primitiveTypes[4], this._primitiveTypes[7], this._primitiveTypes[5] });
                onlysArray[4] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[4] });
                onlysArray[7] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[5] });
                onlysArray[3] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[3] });
                onlysArray[13] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[13] });
                onlysArray[14] = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(new PrimitiveType[] { this._primitiveTypes[14] });
                Interlocked.CompareExchange<System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>[]>(ref this._promotionTypes, onlysArray, null);
            }
        }

        internal static EdmProviderManifest Instance =>
            _instance;

        public override string NamespaceName =>
            "Edm";

        internal string Token =>
            string.Empty;
    }
}

