namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Reflection;

    internal class MappedFunction : MetaFunction
    {
        private static ReadOnlyCollection<MetaParameter> _emptyParameters = new List<MetaParameter>(0).AsReadOnly();
        private static ReadOnlyCollection<MetaType> _emptyTypes = new List<MetaType>(0).AsReadOnly();
        private FunctionMapping map;
        private MethodInfo method;
        private MetaModel model;
        private ReadOnlyCollection<MetaParameter> parameters;
        private MetaParameter returnParameter;
        private ReadOnlyCollection<MetaType> rowTypes;

        internal MappedFunction(MappedMetaModel model, FunctionMapping map, MethodInfo method)
        {
            this.model = model;
            this.map = map;
            this.method = method;
            this.rowTypes = _emptyTypes;
            if ((map.Types.Count == 0) && (this.method.ReturnType == typeof(IMultipleResults)))
            {
                throw System.Data.Linq.Mapping.Error.NoResultTypesDeclaredForFunction(method.Name);
            }
            if ((map.Types.Count > 1) && (this.method.ReturnType != typeof(IMultipleResults)))
            {
                throw System.Data.Linq.Mapping.Error.TooManyResultTypesDeclaredForFunction(method.Name);
            }
            if ((map.Types.Count == 1) && (this.method.ReturnType != typeof(IMultipleResults)))
            {
                Type elementType = TypeSystem.GetElementType(method.ReturnType);
                this.rowTypes = new List<MetaType>(1) { this.GetMetaType(map.Types[0], elementType) }.AsReadOnly();
            }
            else if (map.Types.Count > 0)
            {
                List<MetaType> list2 = new List<MetaType>();
                foreach (TypeMapping mapping in map.Types)
                {
                    Type type2 = model.FindType(mapping.Name);
                    if (type2 == null)
                    {
                        throw System.Data.Linq.Mapping.Error.CouldNotFindElementTypeInModel(mapping.Name);
                    }
                    MetaType metaType = this.GetMetaType(mapping, type2);
                    if (!list2.Contains(metaType))
                    {
                        list2.Add(metaType);
                    }
                }
                this.rowTypes = list2.AsReadOnly();
            }
            else if (map.FunReturn != null)
            {
                this.returnParameter = new MappedReturnParameter(method.ReturnParameter, map.FunReturn);
            }
            ParameterInfo[] parameters = this.method.GetParameters();
            if (parameters.Length > 0)
            {
                List<MetaParameter> list3 = new List<MetaParameter>(parameters.Length);
                if (this.map.Parameters.Count != parameters.Length)
                {
                    throw System.Data.Linq.Mapping.Error.IncorrectNumberOfParametersMappedForMethod(this.map.MethodName);
                }
                for (int i = 0; i < parameters.Length; i++)
                {
                    list3.Add(new MappedParameter(parameters[i], this.map.Parameters[i]));
                }
                this.parameters = list3.AsReadOnly();
            }
            else
            {
                this.parameters = _emptyParameters;
            }
        }

        private MetaType GetMetaType(TypeMapping tm, Type elementType)
        {
            MetaTable table = this.model.GetTable(elementType);
            if (table != null)
            {
                return table.RowType.GetInheritanceType(elementType);
            }
            return new MappedRootType((MappedMetaModel) this.model, null, tm, elementType);
        }

        public override bool HasMultipleResults =>
            (this.method.ReturnType == typeof(IMultipleResults));

        public override bool IsComposable =>
            this.map.IsComposable;

        public override string MappedName =>
            this.map.Name;

        public override MethodInfo Method =>
            this.method;

        public override MetaModel Model =>
            this.model;

        public override string Name =>
            this.method.Name;

        public override ReadOnlyCollection<MetaParameter> Parameters =>
            this.parameters;

        public override ReadOnlyCollection<MetaType> ResultRowTypes =>
            this.rowTypes;

        public override MetaParameter ReturnParameter =>
            this.returnParameter;
    }
}

