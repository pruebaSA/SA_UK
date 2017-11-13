namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Reflection;

    internal sealed class AttributedMetaFunction : MetaFunction
    {
        private static ReadOnlyCollection<MetaParameter> _emptyParameters = new List<MetaParameter>(0).AsReadOnly();
        private static ReadOnlyCollection<MetaType> _emptyTypes = new List<MetaType>(0).AsReadOnly();
        private FunctionAttribute functionAttrib;
        private MethodInfo methodInfo;
        private AttributedMetaModel model;
        private ReadOnlyCollection<MetaParameter> parameters;
        private MetaParameter returnParameter;
        private ReadOnlyCollection<MetaType> rowTypes;

        public AttributedMetaFunction(AttributedMetaModel model, MethodInfo mi)
        {
            this.model = model;
            this.methodInfo = mi;
            this.rowTypes = _emptyTypes;
            this.functionAttrib = Attribute.GetCustomAttribute(mi, typeof(FunctionAttribute), false) as FunctionAttribute;
            ResultTypeAttribute[] customAttributes = (ResultTypeAttribute[]) Attribute.GetCustomAttributes(mi, typeof(ResultTypeAttribute));
            if ((customAttributes.Length == 0) && (mi.ReturnType == typeof(IMultipleResults)))
            {
                throw System.Data.Linq.Mapping.Error.NoResultTypesDeclaredForFunction(mi.Name);
            }
            if ((customAttributes.Length > 1) && (mi.ReturnType != typeof(IMultipleResults)))
            {
                throw System.Data.Linq.Mapping.Error.TooManyResultTypesDeclaredForFunction(mi.Name);
            }
            if (((customAttributes.Length <= 1) && mi.ReturnType.IsGenericType) && (((mi.ReturnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || (mi.ReturnType.GetGenericTypeDefinition() == typeof(ISingleResult<>))) || (mi.ReturnType.GetGenericTypeDefinition() == typeof(IQueryable<>))))
            {
                Type elementType = TypeSystem.GetElementType(mi.ReturnType);
                this.rowTypes = new List<MetaType>(1) { this.GetMetaType(elementType) }.AsReadOnly();
            }
            else if (customAttributes.Length > 0)
            {
                List<MetaType> list2 = new List<MetaType>();
                foreach (ResultTypeAttribute attribute in customAttributes)
                {
                    Type type = attribute.Type;
                    MetaType metaType = this.GetMetaType(type);
                    if (!list2.Contains(metaType))
                    {
                        list2.Add(metaType);
                    }
                }
                this.rowTypes = list2.AsReadOnly();
            }
            else
            {
                this.returnParameter = new AttributedMetaParameter(this.methodInfo.ReturnParameter);
            }
            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length > 0)
            {
                List<MetaParameter> list3 = new List<MetaParameter>(parameters.Length);
                int index = 0;
                int length = parameters.Length;
                while (index < length)
                {
                    AttributedMetaParameter item = new AttributedMetaParameter(parameters[index]);
                    list3.Add(item);
                    index++;
                }
                this.parameters = list3.AsReadOnly();
            }
            else
            {
                this.parameters = _emptyParameters;
            }
        }

        private MetaType GetMetaType(Type type)
        {
            MetaTable tableNoLocks = this.model.GetTableNoLocks(type);
            if (tableNoLocks != null)
            {
                return tableNoLocks.RowType.GetInheritanceType(type);
            }
            return new AttributedRootType(this.model, null, type);
        }

        public override bool HasMultipleResults =>
            (this.methodInfo.ReturnType == typeof(IMultipleResults));

        public override bool IsComposable =>
            this.functionAttrib.IsComposable;

        public override string MappedName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.functionAttrib.Name))
                {
                    return this.functionAttrib.Name;
                }
                return this.methodInfo.Name;
            }
        }

        public override MethodInfo Method =>
            this.methodInfo;

        public override MetaModel Model =>
            this.model;

        public override string Name =>
            this.methodInfo.Name;

        public override ReadOnlyCollection<MetaParameter> Parameters =>
            this.parameters;

        public override ReadOnlyCollection<MetaType> ResultRowTypes =>
            this.rowTypes;

        public override MetaParameter ReturnParameter =>
            this.returnParameter;
    }
}

