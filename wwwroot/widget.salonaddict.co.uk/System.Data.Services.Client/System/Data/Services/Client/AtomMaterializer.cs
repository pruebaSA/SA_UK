namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;

    [DebuggerDisplay("AtomMaterializer {parser}")]
    internal class AtomMaterializer
    {
        private readonly DataServiceContext context;
        private object currentValue;
        private readonly Type expectedType;
        private bool ignoreMissingProperties;
        private readonly AtomMaterializerLog log;
        private readonly Action<object, object> materializedObjectCallback;
        private readonly ProjectionPlan materializeEntryPlan;
        private readonly MergeOption mergeOption;
        private readonly Dictionary<IEnumerable, DataServiceQueryContinuation> nextLinkTable;
        private readonly AtomParser parser;
        private object targetInstance;

        internal AtomMaterializer(AtomParser parser, DataServiceContext context, Type expectedType, bool ignoreMissingProperties, MergeOption mergeOption, AtomMaterializerLog log, Action<object, object> materializedObjectCallback, QueryComponents queryComponents, ProjectionPlan plan)
        {
            this.context = context;
            this.parser = parser;
            this.expectedType = expectedType;
            this.ignoreMissingProperties = ignoreMissingProperties;
            this.mergeOption = mergeOption;
            this.log = log;
            this.materializedObjectCallback = materializedObjectCallback;
            this.nextLinkTable = new Dictionary<IEnumerable, DataServiceQueryContinuation>(ReferenceEqualityComparer<IEnumerable>.Instance);
            this.materializeEntryPlan = plan ?? CreatePlan(queryComponents);
        }

        private static void ApplyDataValue(ClientType type, AtomContentProperty property, bool ignoreMissingProperties, DataServiceContext context, object instance)
        {
            ClientType.ClientProperty property2 = type.GetProperty(property.Name, ignoreMissingProperties);
            if (property2 != null)
            {
                if (property.Properties != null)
                {
                    if (property2.IsKnownType || ClientConvert.IsKnownType(MaterializeAtom.GetEntryClientType(property.TypeName, context, property2.PropertyType, true).ElementType))
                    {
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_ExpectingSimpleValue);
                    }
                    bool flag = false;
                    ClientType actualType = ClientType.Create(property2.PropertyType);
                    object obj2 = property2.GetValue(instance);
                    if (obj2 == null)
                    {
                        obj2 = actualType.CreateInstance();
                        flag = true;
                    }
                    MaterializeDataValues(actualType, property.Properties, ignoreMissingProperties, context);
                    ApplyDataValues(actualType, property.Properties, ignoreMissingProperties, context, obj2);
                    if (flag)
                    {
                        property2.SetValue(instance, obj2, property.Name, true);
                    }
                }
                else
                {
                    property2.SetValue(instance, property.MaterializedValue, property.Name, true);
                }
            }
        }

        private static void ApplyDataValues(ClientType type, IEnumerable<AtomContentProperty> properties, bool ignoreMissingProperties, DataServiceContext context, object instance)
        {
            foreach (AtomContentProperty property in properties)
            {
                ApplyDataValue(type, property, ignoreMissingProperties, context, instance);
            }
        }

        private static void ApplyEntityPropertyMappings(AtomEntry entry, ClientType entryType)
        {
            if (entryType.HasEntityPropertyMappings)
            {
                XElement tag = entry.Tag as XElement;
                ApplyEntityPropertyMappings(entry, tag, entryType.EpmTargetTree.SyndicationRoot);
                ApplyEntityPropertyMappings(entry, tag, entryType.EpmTargetTree.NonSyndicationRoot);
            }
            entry.EntityPropertyMappingsApplied = true;
        }

        private static void ApplyEntityPropertyMappings(AtomEntry entry, XElement entryElement, EpmTargetPathSegment target)
        {
            Stack<EpmTargetPathSegment> stack = new Stack<EpmTargetPathSegment>();
            Stack<XElement> stack2 = new Stack<XElement>();
            stack.Push(target);
            stack2.Push(entryElement);
            while (stack.Count > 0)
            {
                EpmTargetPathSegment segment = stack.Pop();
                XElement element = stack2.Pop();
                if (segment.HasContent)
                {
                    XNode node = element.Nodes().Where<XNode>(delegate (XNode n) {
                        if (n.NodeType != XmlNodeType.Text)
                        {
                            return (n.NodeType == XmlNodeType.SignificantWhitespace);
                        }
                        return true;
                    }).FirstOrDefault<XNode>();
                    string str = (node == null) ? null : ((XText) node).Value;
                    string sourcePath = segment.EpmInfo.Attribute.SourcePath;
                    string typeName = (string) element.Attribute(XName.Get("type", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"));
                    SetValueOnPath(entry.DataValues, sourcePath, str, typeName);
                }
                foreach (EpmTargetPathSegment segment2 in segment.SubSegments)
                {
                    if (segment2.IsAttribute)
                    {
                        string localName = segment2.SegmentName.Substring(1);
                        XAttribute attribute = element.Attribute(XName.Get(localName, segment2.SegmentNamespaceUri));
                        if (attribute != null)
                        {
                            SetValueOnPath(entry.DataValues, segment2.EpmInfo.Attribute.SourcePath, attribute.Value, null);
                        }
                    }
                    else
                    {
                        XElement item = element.Element(XName.Get(segment2.SegmentName, segment2.SegmentNamespaceUri));
                        if (item != null)
                        {
                            stack.Push(segment2);
                            stack2.Push(item);
                        }
                    }
                }
            }
        }

        private void ApplyFeedToCollection(AtomEntry entry, ClientType.ClientProperty property, AtomFeed feed, bool includeLinks)
        {
            ClientType type = ClientType.Create(property.CollectionType);
            foreach (AtomEntry entry2 in feed.Entries)
            {
                this.Materialize(entry2, type.ElementType, includeLinks);
            }
            ProjectionPlan continuationPlan = includeLinks ? CreatePlanForDirectMaterialization(property.CollectionType) : CreatePlanForShallowMaterialization(property.CollectionType);
            this.ApplyItemsToCollection(entry, property, from e in feed.Entries select e.ResolvedObject, feed.NextLink, continuationPlan);
        }

        private void ApplyItemsToCollection(AtomEntry entry, ClientType.ClientProperty property, IEnumerable items, Uri nextLink, ProjectionPlan continuationPlan)
        {
            Func<LinkDescriptor, bool> predicate = null;
            object instance = entry.ShouldUpdateFromPayload ? GetOrCreateCollectionProperty(entry.ResolvedObject, property, null) : null;
            ClientType type = ClientType.Create(property.CollectionType);
            foreach (object obj3 in items)
            {
                if (!type.ElementType.IsAssignableFrom(obj3.GetType()))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_EntryIntoCollectionMismatch(obj3.GetType().FullName, type.ElementType.FullName));
                }
                if (entry.ShouldUpdateFromPayload)
                {
                    property.SetValue(instance, obj3, property.PropertyName, true);
                    this.log.AddedLink(entry, property.PropertyName, obj3);
                }
            }
            if (entry.ShouldUpdateFromPayload)
            {
                this.FoundNextLinkForCollection(instance as IEnumerable, nextLink, continuationPlan);
            }
            else
            {
                this.FoundNextLinkForUnmodifiedCollection(property.GetValue(entry.ResolvedObject) as IEnumerable);
            }
            if ((this.mergeOption == MergeOption.OverwriteChanges) || (this.mergeOption == MergeOption.PreserveChanges))
            {
                if (predicate == null)
                {
                    predicate = delegate (LinkDescriptor x) {
                        if (MergeOption.OverwriteChanges != this.mergeOption)
                        {
                            return EntityStates.Added != x.State;
                        }
                        return true;
                    };
                }
                foreach (object obj4 in (from x in this.context.GetLinks(entry.ResolvedObject, property.PropertyName).Where<LinkDescriptor>(predicate) select x.Target).Except<object>(EnumerateAsElementType<object>(items)))
                {
                    if (instance != null)
                    {
                        property.RemoveValue(instance, obj4);
                    }
                    this.log.RemovedLink(entry, property.PropertyName, obj4);
                }
            }
        }

        private static void CheckEntryToAccessNotNull(AtomEntry entry, string name)
        {
            if (entry.IsNull)
            {
                throw new NullReferenceException(System.Data.Services.Client.Strings.AtomMaterializer_EntryToAccessIsNull(name));
            }
        }

        private static ProjectionPlan CreatePlan(QueryComponents queryComponents)
        {
            LambdaExpression projection = queryComponents.Projection;
            if (projection == null)
            {
                return CreatePlanForDirectMaterialization(queryComponents.LastSegmentType);
            }
            ProjectionPlan plan = ProjectionPlanCompiler.CompilePlan(projection, queryComponents.NormalizerRewrites);
            plan.LastSegmentType = queryComponents.LastSegmentType;
            return plan;
        }

        private static ProjectionPlan CreatePlanForDirectMaterialization(Type lastSegmentType) => 
            new ProjectionPlan { 
                Plan = new Func<object, object, Type, object>(AtomMaterializerInvoker.DirectMaterializePlan),
                ProjectedType = lastSegmentType,
                LastSegmentType = lastSegmentType
            };

        private static ProjectionPlan CreatePlanForShallowMaterialization(Type lastSegmentType) => 
            new ProjectionPlan { 
                Plan = new Func<object, object, Type, object>(AtomMaterializerInvoker.ShallowMaterializePlan),
                ProjectedType = lastSegmentType,
                LastSegmentType = lastSegmentType
            };

        internal static object DirectMaterializePlan(AtomMaterializer materializer, AtomEntry entry, Type expectedEntryType)
        {
            materializer.Materialize(entry, expectedEntryType, true);
            return entry.ResolvedObject;
        }

        internal static IEnumerable<T> EnumerateAsElementType<T>(IEnumerable source)
        {
            IEnumerable<T> enumerable = source as IEnumerable<T>;
            if (enumerable != null)
            {
                return enumerable;
            }
            return EnumerateAsElementTypeInternal<T>(source);
        }

        internal static IEnumerable<T> EnumerateAsElementTypeInternal<T>(IEnumerable source)
        {
            IEnumerator enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                yield return (T) current;
            }
        }

        private void FoundNextLinkForCollection(IEnumerable collection, Uri link, ProjectionPlan plan)
        {
            if ((collection != null) && !this.nextLinkTable.ContainsKey(collection))
            {
                DataServiceQueryContinuation continuation = DataServiceQueryContinuation.Create(link, plan);
                this.nextLinkTable.Add(collection, continuation);
                Util.SetNextLinkForCollection(collection, continuation);
            }
        }

        private void FoundNextLinkForUnmodifiedCollection(IEnumerable collection)
        {
            if ((collection != null) && !this.nextLinkTable.ContainsKey(collection))
            {
                this.nextLinkTable.Add(collection, null);
            }
        }

        private static Action<object, object> GetAddToCollectionDelegate(Type listType)
        {
            Type type;
            ParameterExpression expression;
            ParameterExpression expression2;
            MethodInfo addToCollectionMethod = ClientType.GetAddToCollectionMethod(listType, out type);
            return (Action<object, object>) Expression.Lambda(Expression.Call(Expression.Convert(expression = Expression.Parameter(typeof(object), "list"), listType), addToCollectionMethod, new Expression[] { Expression.Convert(expression2 = Expression.Parameter(typeof(object), "element"), type) }), new ParameterExpression[] { expression, expression2 }).Compile();
        }

        private static object GetOrCreateCollectionProperty(object instance, ClientType.ClientProperty property, Type collectionType)
        {
            object obj2 = property.GetValue(instance);
            if (obj2 == null)
            {
                if (collectionType == null)
                {
                    collectionType = property.PropertyType;
                    if (collectionType.IsInterface)
                    {
                        collectionType = typeof(Collection<>).MakeGenericType(new Type[] { property.CollectionType });
                    }
                }
                obj2 = Activator.CreateInstance(collectionType);
                property.SetValue(instance, obj2, property.PropertyName, false);
            }
            return obj2;
        }

        private static AtomContentProperty GetPropertyOrThrow(List<AtomContentProperty> properties, string propertyName)
        {
            Func<AtomContentProperty, bool> predicate = null;
            AtomContentProperty property = null;
            if (properties != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.Name == propertyName;
                }
                property = properties.Where<AtomContentProperty>(predicate).FirstOrDefault<AtomContentProperty>();
            }
            if (property == null)
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_PropertyMissing(propertyName));
            }
            return property;
        }

        private static AtomContentProperty GetPropertyOrThrow(AtomEntry entry, string propertyName)
        {
            Func<AtomContentProperty, bool> predicate = null;
            AtomContentProperty property = null;
            List<AtomContentProperty> dataValues = entry.DataValues;
            if (dataValues != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.Name == propertyName;
                }
                property = dataValues.Where<AtomContentProperty>(predicate).FirstOrDefault<AtomContentProperty>();
            }
            if (property == null)
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_PropertyMissingFromEntry(propertyName, entry.Identity));
            }
            return property;
        }

        internal static bool IsDataServiceCollection(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && WebUtil.IsDataServiceCollectionType(type.GetGenericTypeDefinition()))
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        internal static List<TTarget> ListAsElementType<T, TTarget>(AtomMaterializer materializer, IEnumerable<T> source) where T: TTarget
        {
            List<TTarget> list2;
            DataServiceQueryContinuation continuation;
            List<TTarget> list = source as List<TTarget>;
            if (list != null)
            {
                return list;
            }
            IList list3 = source as IList;
            if (list3 != null)
            {
                list2 = new List<TTarget>(list3.Count);
            }
            else
            {
                list2 = new List<TTarget>();
            }
            foreach (T local in source)
            {
                list2.Add((TTarget) local);
            }
            if (materializer.nextLinkTable.TryGetValue(source, out continuation))
            {
                materializer.nextLinkTable[list2] = continuation;
            }
            return list2;
        }

        private void Materialize(AtomEntry entry, Type expectedEntryType, bool includeLinks)
        {
            this.ResolveOrCreateInstance(entry, expectedEntryType);
            this.MaterializeResolvedEntry(entry, includeLinks);
        }

        private static bool MaterializeDataValue(Type type, AtomContentProperty atomProperty, DataServiceContext context)
        {
            string typeName = atomProperty.TypeName;
            string text = atomProperty.Text;
            ClientType type2 = null;
            Type type3 = Nullable.GetUnderlyingType(type) ?? type;
            bool flag = ClientConvert.IsKnownType(type3);
            if (!flag)
            {
                type2 = MaterializeAtom.GetEntryClientType(typeName, context, type, true);
                flag = ClientConvert.IsKnownType(type2.ElementType);
            }
            if (!flag)
            {
                return false;
            }
            if (atomProperty.IsNull)
            {
                if (!ClientType.CanAssignNull(type))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_CannotAssignNull(atomProperty.Name, type.FullName));
                }
                atomProperty.MaterializedValue = null;
                return true;
            }
            object obj2 = text;
            if (text != null)
            {
                obj2 = ClientConvert.ChangeType(text, (type2 != null) ? type2.ElementType : type3);
            }
            atomProperty.MaterializedValue = obj2;
            return true;
        }

        private static void MaterializeDataValues(ClientType actualType, List<AtomContentProperty> values, bool ignoreMissingProperties, DataServiceContext context)
        {
            foreach (AtomContentProperty property in values)
            {
                string name = property.Name;
                ClientType.ClientProperty property2 = actualType.GetProperty(name, ignoreMissingProperties);
                if ((((property2 != null) && (property.Feed == null)) && ((property.Entry == null) && !MaterializeDataValue(property2.NullablePropertyType, property, context))) && (property2.CollectionType != null))
                {
                    throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.ClientType_CollectionOfNonEntities);
                }
            }
        }

        private void MaterializeResolvedEntry(AtomEntry entry, bool includeLinks)
        {
            ClientType actualType = entry.ActualType;
            if (!entry.EntityPropertyMappingsApplied)
            {
                ApplyEntityPropertyMappings(entry, entry.ActualType);
            }
            MaterializeDataValues(actualType, entry.DataValues, this.ignoreMissingProperties, this.context);
            foreach (AtomContentProperty property in entry.DataValues)
            {
                ClientType.ClientProperty property2 = actualType.GetProperty(property.Name, this.ignoreMissingProperties);
                if (((property2 != null) && ((entry.ShouldUpdateFromPayload || (property.Entry != null)) || (property.Feed != null))) && (includeLinks || ((property.Entry == null) && (property.Feed == null))))
                {
                    ValidatePropertyMatch(property2, property);
                    AtomFeed feed = property.Feed;
                    if (feed != null)
                    {
                        this.ApplyFeedToCollection(entry, property2, feed, includeLinks);
                    }
                    else if (property.Entry != null)
                    {
                        if (!property.IsNull)
                        {
                            this.Materialize(property.Entry, property2.PropertyType, includeLinks);
                        }
                        if (entry.ShouldUpdateFromPayload)
                        {
                            property2.SetValue(entry.ResolvedObject, property.Entry.ResolvedObject, property.Name, true);
                            this.log.SetLink(entry, property2.PropertyName, property.Entry.ResolvedObject);
                        }
                    }
                    else
                    {
                        ApplyDataValue(actualType, property, this.ignoreMissingProperties, this.context, entry.ResolvedObject);
                    }
                }
            }
            if (this.materializedObjectCallback != null)
            {
                this.materializedObjectCallback(entry.Tag, entry.ResolvedObject);
            }
        }

        private static void MaterializeToList(AtomMaterializer materializer, IEnumerable list, Type nestedExpectedType, IEnumerable<AtomEntry> entries)
        {
            Action<object, object> addToCollectionDelegate = GetAddToCollectionDelegate(list.GetType());
            foreach (AtomEntry entry in entries)
            {
                if (!entry.EntityHasBeenResolved)
                {
                    materializer.Materialize(entry, nestedExpectedType, false);
                }
                addToCollectionDelegate(list, entry.ResolvedObject);
            }
        }

        private void MergeLists(AtomEntry entry, ClientType.ClientProperty property, IEnumerable list, Uri nextLink, ProjectionPlan plan)
        {
            if ((entry.ShouldUpdateFromPayload && (property.NullablePropertyType == list.GetType())) && (property.GetValue(entry.ResolvedObject) == null))
            {
                property.SetValue(entry.ResolvedObject, list, property.PropertyName, false);
                this.FoundNextLinkForCollection(list, nextLink, plan);
                foreach (object obj2 in list)
                {
                    this.log.AddedLink(entry, property.PropertyName, obj2);
                }
            }
            else
            {
                this.ApplyItemsToCollection(entry, property, list, nextLink, plan);
            }
        }

        internal static bool ProjectionCheckValueForPathIsNull(AtomEntry entry, Type expectedType, ProjectionPath path)
        {
            if ((path.Count == 0) || ((path.Count == 1) && (path[0].Member == null)))
            {
                return entry.IsNull;
            }
            bool isNull = false;
            AtomContentProperty atomProperty = null;
            List<AtomContentProperty> dataValues = entry.DataValues;
            for (int i = 0; i < path.Count; i++)
            {
                ProjectionPathSegment segment = path[i];
                if (segment.Member != null)
                {
                    bool flag2 = i == (path.Count - 1);
                    string member = segment.Member;
                    ClientType.ClientProperty property = ClientType.Create(expectedType).GetProperty(member, false);
                    atomProperty = GetPropertyOrThrow(dataValues, member);
                    ValidatePropertyMatch(property, atomProperty);
                    if (atomProperty.Feed != null)
                    {
                        isNull = false;
                    }
                    else
                    {
                        if (flag2)
                        {
                            isNull = atomProperty.Entry.IsNull;
                        }
                        dataValues = atomProperty.Entry.DataValues;
                        entry = atomProperty.Entry;
                    }
                    expectedType = property.PropertyType;
                }
            }
            return isNull;
        }

        internal static void ProjectionEnsureEntryAvailableOfType(AtomMaterializer materializer, AtomEntry entry, Type requiredType)
        {
            if (entry.EntityHasBeenResolved)
            {
                if (!requiredType.IsAssignableFrom(entry.ResolvedObject.GetType()))
                {
                    throw new InvalidOperationException(string.Concat(new object[] { "Expecting type '", requiredType, "' for '", entry.Identity, "', but found a previously created instance of type '", entry.ResolvedObject.GetType() }));
                }
            }
            else
            {
                if (entry.Identity == null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MissingIdElement);
                }
                if (!materializer.TryResolveAsCreated(entry) && !materializer.TryResolveFromContext(entry, requiredType))
                {
                    materializer.ResolveByCreatingWithType(entry, requiredType);
                }
                else if (!requiredType.IsAssignableFrom(entry.ResolvedObject.GetType()))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_Current(requiredType, entry.ResolvedObject.GetType()));
                }
            }
        }

        internal static AtomEntry ProjectionGetEntry(AtomEntry entry, string name)
        {
            AtomContentProperty propertyOrThrow = GetPropertyOrThrow(entry, name);
            if (propertyOrThrow.Entry == null)
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_PropertyNotExpectedEntry(name, entry.Identity));
            }
            CheckEntryToAccessNotNull(propertyOrThrow.Entry, name);
            return propertyOrThrow.Entry;
        }

        internal static object ProjectionInitializeEntity(AtomMaterializer materializer, AtomEntry entry, Type expectedType, Type resultType, string[] properties, Func<object, object, Type, object>[] propertyValues)
        {
            if ((entry == null) || entry.IsNull)
            {
                throw new NullReferenceException(System.Data.Services.Client.Strings.AtomMaterializer_EntryToInitializeIsNull(resultType.FullName));
            }
            if (!entry.EntityHasBeenResolved)
            {
                ProjectionEnsureEntryAvailableOfType(materializer, entry, resultType);
            }
            else if (!resultType.IsAssignableFrom(entry.ActualType.ElementType))
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.AtomMaterializer_ProjectEntityTypeMismatch(resultType.FullName, entry.ActualType.ElementType.FullName, entry.Identity));
            }
            object resolvedObject = entry.ResolvedObject;
            for (int i = 0; i < properties.Length; i++)
            {
                ClientType.ClientProperty property = entry.ActualType.GetProperty(properties[i], materializer.ignoreMissingProperties);
                object target = propertyValues[i](materializer, entry, expectedType);
                if (entry.ShouldUpdateFromPayload && ClientType.Create(property.NullablePropertyType, false).IsEntityType)
                {
                    materializer.Log.SetLink(entry, property.PropertyName, target);
                }
                bool flag = (property.CollectionType == null) || !ClientType.CheckElementTypeIsEntity(property.CollectionType);
                if (entry.ShouldUpdateFromPayload)
                {
                    if (flag)
                    {
                        property.SetValue(resolvedObject, target, property.PropertyName, false);
                    }
                    else
                    {
                        IEnumerable list = (IEnumerable) target;
                        DataServiceQueryContinuation continuation = materializer.nextLinkTable[list];
                        Uri nextLinkUri = continuation?.NextLinkUri;
                        ProjectionPlan plan = continuation?.Plan;
                        materializer.MergeLists(entry, property, list, nextLinkUri, plan);
                    }
                }
                else if (!flag)
                {
                    materializer.FoundNextLinkForUnmodifiedCollection(property.GetValue(entry.ResolvedObject) as IEnumerable);
                }
            }
            return resolvedObject;
        }

        internal static IEnumerable ProjectionSelect(AtomMaterializer materializer, AtomEntry entry, Type expectedType, Type resultType, ProjectionPath path, Func<object, object, Type, object> selector)
        {
            ClientType type = entry.ActualType ?? ClientType.Create(expectedType);
            IEnumerable enumerable = (IEnumerable) Util.ActivatorCreateInstance(typeof(List<>).MakeGenericType(new Type[] { resultType }), new object[0]);
            AtomContentProperty atomProperty = null;
            ClientType.ClientProperty property = null;
            for (int i = 0; i < path.Count; i++)
            {
                ProjectionPathSegment segment = path[i];
                if (segment.Member != null)
                {
                    string member = segment.Member;
                    property = type.GetProperty(member, false);
                    atomProperty = GetPropertyOrThrow(entry, member);
                    if (atomProperty.Entry != null)
                    {
                        entry = atomProperty.Entry;
                        type = ClientType.Create(property.NullablePropertyType, false);
                    }
                }
            }
            ValidatePropertyMatch(property, atomProperty);
            AtomFeed feed = atomProperty.Feed;
            Action<object, object> addToCollectionDelegate = GetAddToCollectionDelegate(enumerable.GetType());
            foreach (AtomEntry entry2 in feed.Entries)
            {
                object obj2 = selector(materializer, entry2, property.CollectionType);
                addToCollectionDelegate(enumerable, obj2);
            }
            ProjectionPlan plan = new ProjectionPlan {
                LastSegmentType = property.CollectionType,
                Plan = selector,
                ProjectedType = resultType
            };
            materializer.FoundNextLinkForCollection(enumerable, feed.NextLink, plan);
            return enumerable;
        }

        internal static object ProjectionValueForPath(AtomMaterializer materializer, AtomEntry entry, Type expectedType, ProjectionPath path)
        {
            if ((path.Count == 0) || ((path.Count == 1) && (path[0].Member == null)))
            {
                if (!entry.EntityHasBeenResolved)
                {
                    materializer.Materialize(entry, expectedType, false);
                }
                return entry.ResolvedObject;
            }
            object resolvedObject = null;
            AtomContentProperty atomProperty = null;
            List<AtomContentProperty> dataValues = entry.DataValues;
            for (int i = 0; i < path.Count; i++)
            {
                ProjectionPathSegment segment = path[i];
                if (segment.Member != null)
                {
                    bool flag = i == (path.Count - 1);
                    string member = segment.Member;
                    if (flag)
                    {
                        CheckEntryToAccessNotNull(entry, member);
                        if (!entry.EntityPropertyMappingsApplied)
                        {
                            ClientType entryType = MaterializeAtom.GetEntryClientType(entry.TypeName, materializer.context, expectedType, false);
                            ApplyEntityPropertyMappings(entry, entryType);
                        }
                    }
                    ClientType.ClientProperty property = ClientType.Create(expectedType).GetProperty(member, false);
                    atomProperty = GetPropertyOrThrow(dataValues, member);
                    ValidatePropertyMatch(property, atomProperty);
                    AtomFeed feed = atomProperty.Feed;
                    if (feed != null)
                    {
                        Type implementationType = ClientType.GetImplementationType(segment.ProjectionType, typeof(ICollection<>));
                        if (implementationType == null)
                        {
                            implementationType = ClientType.GetImplementationType(segment.ProjectionType, typeof(IEnumerable<>));
                        }
                        Type nestedExpectedType = implementationType.GetGenericArguments()[0];
                        Type projectionType = segment.ProjectionType;
                        if (projectionType.IsInterface || IsDataServiceCollection(projectionType))
                        {
                            projectionType = typeof(Collection<>).MakeGenericType(new Type[] { nestedExpectedType });
                        }
                        IEnumerable list = (IEnumerable) Util.ActivatorCreateInstance(projectionType, new object[0]);
                        MaterializeToList(materializer, list, nestedExpectedType, feed.Entries);
                        if (IsDataServiceCollection(segment.ProjectionType))
                        {
                            list = (IEnumerable) Util.ActivatorCreateInstance(WebUtil.GetDataServiceCollectionOfT(new Type[] { nestedExpectedType }), new object[] { list, TrackingMode.None });
                        }
                        ProjectionPlan plan = CreatePlanForShallowMaterialization(nestedExpectedType);
                        materializer.FoundNextLinkForCollection(list, feed.NextLink, plan);
                        resolvedObject = list;
                    }
                    else if (atomProperty.Entry != null)
                    {
                        if ((flag && !atomProperty.Entry.EntityHasBeenResolved) && !atomProperty.IsNull)
                        {
                            materializer.Materialize(atomProperty.Entry, property.PropertyType, false);
                        }
                        dataValues = atomProperty.Entry.DataValues;
                        resolvedObject = atomProperty.Entry.ResolvedObject;
                        entry = atomProperty.Entry;
                    }
                    else
                    {
                        if (atomProperty.Properties != null)
                        {
                            if ((atomProperty.MaterializedValue == null) && !atomProperty.IsNull)
                            {
                                ClientType actualType = ClientType.Create(property.PropertyType);
                                object instance = Util.ActivatorCreateInstance(property.PropertyType, new object[0]);
                                MaterializeDataValues(actualType, atomProperty.Properties, materializer.ignoreMissingProperties, materializer.context);
                                ApplyDataValues(actualType, atomProperty.Properties, materializer.ignoreMissingProperties, materializer.context, instance);
                                atomProperty.MaterializedValue = instance;
                            }
                        }
                        else
                        {
                            MaterializeDataValue(property.NullablePropertyType, atomProperty, materializer.context);
                        }
                        dataValues = atomProperty.Properties;
                        resolvedObject = atomProperty.MaterializedValue;
                    }
                    expectedType = property.PropertyType;
                }
            }
            return resolvedObject;
        }

        internal void PropagateContinuation<T>(IEnumerable<T> from, DataServiceCollection<T> to)
        {
            DataServiceQueryContinuation continuation;
            if (this.nextLinkTable.TryGetValue(from, out continuation))
            {
                this.nextLinkTable.Add(to, continuation);
                Util.SetNextLinkForCollection(to, continuation);
            }
        }

        internal bool Read()
        {
            this.currentValue = null;
            this.nextLinkTable.Clear();
            while (this.parser.Read())
            {
                switch (this.parser.DataKind)
                {
                    case AtomDataKind.Entry:
                        this.CurrentEntry.ResolvedObject = this.TargetInstance;
                        this.currentValue = this.materializeEntryPlan.Run(this, this.CurrentEntry, this.expectedType);
                        return true;

                    case AtomDataKind.Feed:
                    case AtomDataKind.FeedCount:
                    case AtomDataKind.PagingLinks:
                    {
                        continue;
                    }
                }
                Type type = Nullable.GetUnderlyingType(this.expectedType) ?? this.expectedType;
                ClientType actualType = ClientType.Create(type);
                if (ClientConvert.IsKnownType(type))
                {
                    string propertyValue = this.parser.ReadCustomElementString();
                    if (propertyValue != null)
                    {
                        this.currentValue = ClientConvert.ChangeType(propertyValue, type);
                    }
                    return true;
                }
                if (!actualType.IsEntityType && this.parser.IsDataWebElement)
                {
                    AtomContentProperty property = this.parser.ReadCurrentPropertyValue();
                    if ((property == null) || property.IsNull)
                    {
                        this.currentValue = null;
                    }
                    else
                    {
                        this.currentValue = actualType.CreateInstance();
                        MaterializeDataValues(actualType, property.Properties, this.ignoreMissingProperties, this.context);
                        ApplyDataValues(actualType, property.Properties, this.ignoreMissingProperties, this.context, this.currentValue);
                    }
                    return true;
                }
            }
            return false;
        }

        private void ResolveByCreating(AtomEntry entry, Type expectedEntryType)
        {
            ClientType type = MaterializeAtom.GetEntryClientType(entry.TypeName, this.context, expectedEntryType, true);
            this.ResolveByCreatingWithType(entry, type.ElementType);
        }

        private void ResolveByCreatingWithType(AtomEntry entry, Type type)
        {
            entry.ActualType = ClientType.Create(type);
            entry.ResolvedObject = Activator.CreateInstance(type);
            entry.CreatedByMaterializer = true;
            entry.ShouldUpdateFromPayload = true;
            entry.EntityHasBeenResolved = true;
            this.log.CreatedInstance(entry);
        }

        private void ResolveOrCreateInstance(AtomEntry entry, Type expectedEntryType)
        {
            if (!this.TryResolveAsTarget(entry))
            {
                if (entry.Identity == null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MissingIdElement);
                }
                if (!this.TryResolveAsCreated(entry) && !this.TryResolveFromContext(entry, expectedEntryType))
                {
                    this.ResolveByCreating(entry, expectedEntryType);
                }
            }
        }

        private static void SetValueOnPath(List<AtomContentProperty> values, string path, string value, string typeName)
        {
            bool flag = true;
            AtomContentProperty property = null;
            Func<AtomContentProperty, bool> predicate = null;
            foreach (string step in path.Split(new char[] { '/' }))
            {
                if (values == null)
                {
                    property.EnsureProperties();
                    values = property.Properties;
                }
                if (predicate == null)
                {
                    predicate = v => v.Name == step;
                }
                property = values.Where<AtomContentProperty>(predicate).FirstOrDefault<AtomContentProperty>();
                if (property == null)
                {
                    AtomContentProperty item = new AtomContentProperty();
                    flag = false;
                    item.Name = step;
                    values.Add(item);
                    property = item;
                }
                else if (property.IsNull)
                {
                    return;
                }
                values = property.Properties;
            }
            if (!flag)
            {
                property.TypeName = typeName;
                property.Text = value;
            }
        }

        internal static object ShallowMaterializePlan(AtomMaterializer materializer, AtomEntry entry, Type expectedEntryType)
        {
            materializer.Materialize(entry, expectedEntryType, false);
            return entry.ResolvedObject;
        }

        private bool TryResolveAsCreated(AtomEntry entry)
        {
            AtomEntry entry2;
            if (!this.log.TryResolve(entry, out entry2))
            {
                return false;
            }
            entry.ActualType = entry2.ActualType;
            entry.ResolvedObject = entry2.ResolvedObject;
            entry.CreatedByMaterializer = entry2.CreatedByMaterializer;
            entry.ShouldUpdateFromPayload = entry2.ShouldUpdateFromPayload;
            entry.EntityHasBeenResolved = true;
            return true;
        }

        private bool TryResolveAsTarget(AtomEntry entry)
        {
            if (entry.ResolvedObject == null)
            {
                return false;
            }
            entry.ActualType = ClientType.Create(entry.ResolvedObject.GetType());
            this.log.FoundTargetInstance(entry);
            entry.ShouldUpdateFromPayload = this.mergeOption != MergeOption.PreserveChanges;
            entry.EntityHasBeenResolved = true;
            return true;
        }

        private bool TryResolveFromContext(AtomEntry entry, Type expectedEntryType)
        {
            if (this.mergeOption != MergeOption.NoTracking)
            {
                EntityStates states;
                entry.ResolvedObject = this.context.TryGetEntity(entry.Identity, entry.ETagText, this.mergeOption, out states);
                if (entry.ResolvedObject != null)
                {
                    if (!expectedEntryType.IsInstanceOfType(entry.ResolvedObject))
                    {
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_Current(expectedEntryType, entry.ResolvedObject.GetType()));
                    }
                    entry.ActualType = ClientType.Create(entry.ResolvedObject.GetType());
                    entry.EntityHasBeenResolved = true;
                    entry.ShouldUpdateFromPayload = ((this.mergeOption == MergeOption.OverwriteChanges) || ((this.mergeOption == MergeOption.PreserveChanges) && (states == EntityStates.Unchanged))) || ((this.mergeOption == MergeOption.PreserveChanges) && (states == EntityStates.Deleted));
                    this.log.FoundExistingInstance(entry);
                    return true;
                }
            }
            return false;
        }

        internal static void ValidatePropertyMatch(ClientType.ClientProperty property, AtomContentProperty atomProperty)
        {
            if (property.IsKnownType && ((atomProperty.Feed != null) || (atomProperty.Entry != null)))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MismatchAtomLinkLocalSimple);
            }
            if ((atomProperty.Feed != null) && (property.CollectionType == null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MismatchAtomLinkFeedPropertyNotCollection(property.PropertyName));
            }
            if ((atomProperty.Entry != null) && (property.CollectionType != null))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.Deserialize_MismatchAtomLinkEntryPropertyIsCollection(property.PropertyName));
            }
        }

        internal DataServiceContext Context =>
            this.context;

        internal AtomEntry CurrentEntry =>
            this.parser.CurrentEntry;

        internal AtomFeed CurrentFeed =>
            this.parser.CurrentFeed;

        internal object CurrentValue =>
            this.currentValue;

        internal bool IsEndOfStream =>
            (this.parser.DataKind == AtomDataKind.Finished);

        internal AtomMaterializerLog Log =>
            this.log;

        internal ProjectionPlan MaterializeEntryPlan =>
            this.materializeEntryPlan;

        internal Dictionary<IEnumerable, DataServiceQueryContinuation> NextLinkTable =>
            this.nextLinkTable;

        internal object TargetInstance
        {
            get => 
                this.targetInstance;
            set
            {
                this.targetInstance = value;
            }
        }

        [CompilerGenerated]
        private sealed class <EnumerateAsElementTypeInternal>d__0<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public IEnumerable <>3__source;
            public IEnumerator <>7__wrap2;
            public IDisposable <>7__wrap3;
            private int <>l__initialThreadId;
            public object <item>5__1;
            public IEnumerable source;

            [DebuggerHidden]
            public <EnumerateAsElementTypeInternal>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally4()
            {
                this.<>1__state = -1;
                this.<>7__wrap3 = this.<>7__wrap2 as IDisposable;
                if (this.<>7__wrap3 != null)
                {
                    this.<>7__wrap3.Dispose();
                }
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap2 = this.source.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_0070;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_0070;

                        default:
                            goto Label_0083;
                    }
                Label_003C:
                    this.<item>5__1 = this.<>7__wrap2.Current;
                    this.<>2__current = (T) this.<item>5__1;
                    this.<>1__state = 2;
                    return true;
                Label_0070:
                    if (this.<>7__wrap2.MoveNext())
                    {
                        goto Label_003C;
                    }
                    this.<>m__Finally4();
                Label_0083:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                AtomMaterializer.<EnumerateAsElementTypeInternal>d__0<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (AtomMaterializer.<EnumerateAsElementTypeInternal>d__0<T>) this;
                }
                else
                {
                    d__ = new AtomMaterializer.<EnumerateAsElementTypeInternal>d__0<T>(0);
                }
                d__.source = this.<>3__source;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally4();
                        }
                        return;
                }
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

