namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Client.Xml;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Linq;

    internal class MaterializeAtom : IDisposable, IEnumerable, IEnumerator
    {
        private bool calledGetEnumerator;
        private readonly DataServiceContext context;
        private const long CountStateFailure = -2L;
        private const long CountStateInitial = -1L;
        private long countValue;
        private object current;
        private readonly Type elementType;
        private readonly bool expectingSingleValue;
        private readonly bool ignoreMissingProperties;
        private readonly AtomMaterializer materializer;
        internal readonly MergeOption MergeOptionValue;
        private bool moved;
        private readonly AtomParser parser;
        private XmlReader reader;
        private TextWriter writer;

        private MaterializeAtom()
        {
        }

        private MaterializeAtom(DataServiceContext context, XmlReader reader, Type type, MergeOption mergeOption) : this(context, reader, new QueryComponents(null, Util.DataServiceVersionEmpty, type, null, null), null, mergeOption)
        {
        }

        internal MaterializeAtom(DataServiceContext context, XmlReader reader, QueryComponents queryComponents, ProjectionPlan plan, MergeOption mergeOption)
        {
            Type type;
            this.context = context;
            this.elementType = queryComponents.LastSegmentType;
            this.MergeOptionValue = mergeOption;
            this.ignoreMissingProperties = context.IgnoreMissingProperties;
            this.reader = (reader == null) ? null : new XmlAtomErrorReader(reader);
            this.countValue = -1L;
            this.expectingSingleValue = ClientConvert.IsKnownNullableType(this.elementType);
            reader.Settings.NameTable.Add(context.DataNamespace);
            string originalString = this.context.TypeScheme.OriginalString;
            this.parser = new AtomParser(this.reader, new Func<XmlReader, KeyValuePair<XmlReader, object>>(AtomParser.XElementBuilderCallback), originalString, context.DataNamespace);
            AtomMaterializerLog log = new AtomMaterializerLog(this.context, mergeOption);
            Type expectedType = GetTypeForMaterializer(this.expectingSingleValue, this.elementType, out type);
            this.materializer = new AtomMaterializer(this.parser, context, expectedType, this.ignoreMissingProperties, mergeOption, log, new Action<object, object>(this.MaterializedObjectCallback), queryComponents, plan);
        }

        private void CheckGetEnumerator()
        {
            if (this.calledGetEnumerator)
            {
                throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.Deserialize_GetEnumerator);
            }
            this.calledGetEnumerator = true;
        }

        internal long CountValue()
        {
            if (this.countValue == -1L)
            {
                this.ReadCountValue();
            }
            else if (this.countValue == -2L)
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.MaterializeFromAtom_CountNotPresent);
            }
            return this.countValue;
        }

        internal static MaterializeAtom CreateWrapper(IEnumerable results) => 
            new ResultsWrapper(results, null);

        internal static MaterializeAtom CreateWrapper(IEnumerable results, DataServiceQueryContinuation continuation) => 
            new ResultsWrapper(results, continuation);

        public void Dispose()
        {
            this.current = null;
            if (this.reader != null)
            {
                ((IDisposable) this.reader).Dispose();
            }
            if (this.writer != null)
            {
                this.writer.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        internal virtual DataServiceQueryContinuation GetContinuation(IEnumerable key)
        {
            DataServiceQueryContinuation continuation;
            if (key == null)
            {
                if ((this.expectingSingleValue && !this.moved) || (!this.expectingSingleValue && !this.materializer.IsEndOfStream))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.MaterializeFromAtom_TopLevelLinkNotAvailable);
                }
                if (this.expectingSingleValue || (this.materializer.CurrentFeed == null))
                {
                    return null;
                }
                return DataServiceQueryContinuation.Create(this.materializer.CurrentFeed.NextLink, this.materializer.MaterializeEntryPlan);
            }
            if (!this.materializer.NextLinkTable.TryGetValue(key, out continuation))
            {
                throw new ArgumentException(System.Data.Services.Client.Strings.MaterializeFromAtom_CollectionKeyNotPresentInLinkTable);
            }
            return continuation;
        }

        internal static ClientType GetEntryClientType(string typeName, DataServiceContext context, Type expectedType, bool checkAssignable) => 
            ClientType.Create(context.ResolveTypeFromName(typeName, expectedType, checkAssignable));

        public virtual IEnumerator GetEnumerator()
        {
            this.CheckGetEnumerator();
            return this;
        }

        private static Type GetTypeForMaterializer(bool expectingSingleValue, Type elementType, out Type implementationType)
        {
            if (!expectingSingleValue && typeof(IEnumerable).IsAssignableFrom(elementType))
            {
                implementationType = ClientType.GetImplementationType(elementType, typeof(ICollection<>));
                if (implementationType != null)
                {
                    return implementationType.GetGenericArguments()[0];
                }
            }
            implementationType = null;
            return elementType;
        }

        private void MaterializedObjectCallback(object tag, object entity)
        {
            XElement element = (XElement) tag;
            if (this.context.HasReadingEntityHandlers)
            {
                XmlUtil.RemoveDuplicateNamespaceAttributes(element);
                this.context.FireReadingEntityEvent(entity, element);
            }
        }

        public bool MoveNext()
        {
            bool flag2;
            bool applyingChanges = this.context.ApplyingChanges;
            try
            {
                this.context.ApplyingChanges = true;
                flag2 = this.MoveNextInternal();
            }
            finally
            {
                this.context.ApplyingChanges = applyingChanges;
            }
            return flag2;
        }

        private bool MoveNextInternal()
        {
            Type elementType;
            if (this.reader == null)
            {
                return false;
            }
            this.current = null;
            this.materializer.Log.Clear();
            bool flag = false;
            GetTypeForMaterializer(this.expectingSingleValue, this.elementType, out elementType);
            if (elementType != null)
            {
                if (this.moved)
                {
                    return false;
                }
                Type type2 = elementType.GetGenericArguments()[0];
                elementType = this.elementType;
                if (elementType.IsInterface)
                {
                    elementType = typeof(Collection<>).MakeGenericType(new Type[] { type2 });
                }
                IList list = (IList) Activator.CreateInstance(elementType);
                while (this.materializer.Read())
                {
                    this.moved = true;
                    list.Add(this.materializer.CurrentValue);
                }
                this.current = list;
                flag = true;
            }
            if (this.current == null)
            {
                if (this.expectingSingleValue && this.moved)
                {
                    flag = false;
                }
                else
                {
                    flag = this.materializer.Read();
                    if (flag)
                    {
                        this.current = this.materializer.CurrentValue;
                    }
                    this.moved = true;
                }
            }
            this.materializer.Log.ApplyToContext();
            return flag;
        }

        private void ReadCountValue()
        {
            if ((this.materializer.CurrentFeed != null) && this.materializer.CurrentFeed.Count.HasValue)
            {
                this.countValue = this.materializer.CurrentFeed.Count.Value;
            }
            else
            {
                while ((this.reader.NodeType != XmlNodeType.Element) && this.reader.Read())
                {
                }
                if (this.reader.EOF)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.MaterializeFromAtom_CountNotPresent);
                }
                XElement element = XElement.Load(this.reader);
                this.reader.Close();
                XElement element2 = element.Descendants((XName) (XNamespace.Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata") + "count")).FirstOrDefault<XElement>();
                if (element2 == null)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.MaterializeFromAtom_CountNotPresent);
                }
                if (!long.TryParse(element2.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.countValue))
                {
                    throw new FormatException(System.Data.Services.Client.Strings.MaterializeFromAtom_CountFormatError);
                }
                if (this.countValue < 0L)
                {
                    throw new FormatException(System.Data.Services.Client.Strings.MaterializeFromAtom_CountFormatError);
                }
                this.reader = new XmlAtomErrorReader(element.CreateReader());
                this.parser.ReplaceReader(this.reader);
            }
        }

        internal static string ReadElementString(XmlReader reader, bool checkNullAttribute)
        {
            // This item is obfuscated and can not be translated.
            int expressionStack_11_0;
            string str = null;
            if (checkNullAttribute)
            {
                expressionStack_11_0 = (int) !Util.DoesNullAttributeSayTrue(reader);
            }
            else
            {
                expressionStack_11_0 = 0;
            }
            bool flag = (bool) expressionStack_11_0;
            if (reader.IsEmptyElement)
            {
                if (!flag)
                {
                    return null;
                }
                return string.Empty;
            }
        Label_0091:
            if (!reader.Read())
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_ExpectingSimpleValue);
            }
            switch (reader.NodeType)
            {
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                    if (str != null)
                    {
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MixedTextWithComment);
                    }
                    str = reader.Value;
                    goto Label_0091;

                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                    goto Label_0091;

                case XmlNodeType.EndElement:
                    string expressionStack_63_0;
                    if (str != null)
                    {
                        return str;
                    }
                    else
                    {
                        expressionStack_63_0 = str;
                    }
                    expressionStack_63_0 = string.Empty;
                    if (flag)
                    {
                        return string.Empty;
                    }
                    return null;
            }
            throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_ExpectingSimpleValue);
        }

        internal void SetInsertingObject(object addedObject)
        {
            this.materializer.TargetInstance = addedObject;
        }

        internal static void SkipToEnd(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                int depth = reader.Depth;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Depth == depth))
                    {
                        return;
                    }
                }
            }
        }

        void IEnumerator.Reset()
        {
            throw System.Data.Services.Client.Error.NotSupported();
        }

        internal DataServiceContext Context =>
            this.context;

        public object Current =>
            this.current;

        internal static MaterializeAtom EmptyResults =>
            new ResultsWrapper(null, null);

        internal bool IsEmptyResults =>
            (this.reader == null);

        private class ResultsWrapper : MaterializeAtom
        {
            private readonly DataServiceQueryContinuation continuation;
            private readonly IEnumerable results;

            internal ResultsWrapper(IEnumerable results, DataServiceQueryContinuation continuation)
            {
                this.results = results ?? new object[0];
                this.continuation = continuation;
            }

            internal override DataServiceQueryContinuation GetContinuation(IEnumerable key)
            {
                if (key != null)
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.MaterializeFromAtom_GetNestLinkForFlatCollection);
                }
                return this.continuation;
            }

            public override IEnumerator GetEnumerator() => 
                this.results.GetEnumerator();
        }
    }
}

