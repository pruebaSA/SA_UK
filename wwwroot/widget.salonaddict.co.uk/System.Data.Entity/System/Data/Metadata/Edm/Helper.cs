namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.XPath;

    internal static class Helper
    {
        internal const char CommaSymbol = ',';
        internal static readonly EdmMember[] EmptyArrayEdmProperty = new EdmMember[0];
        internal static readonly ReadOnlyCollection<EdmFunction> EmptyEdmFunctionReadOnlyCollection = new ReadOnlyCollection<EdmFunction>(new EdmFunction[0]);
        internal static readonly ReadOnlyCollection<FacetDescription> EmptyFacetDescriptionEnumerable = new ReadOnlyCollection<FacetDescription>(new FacetDescription[0]);
        internal static readonly KeyValuePair<string, object>[] EmptyKeyValueStringObjectArray = new KeyValuePair<string, object>[0];
        internal static readonly ReadOnlyCollection<KeyValuePair<string, object>> EmptyKeyValueStringObjectList = new ReadOnlyCollection<KeyValuePair<string, object>>(new KeyValuePair<string, object>[0]);
        internal static readonly ReadOnlyCollection<PrimitiveType> EmptyPrimitiveTypeReadOnlyCollection = new ReadOnlyCollection<PrimitiveType>(new PrimitiveType[0]);
        internal static readonly ReadOnlyCollection<string> EmptyStringList = new ReadOnlyCollection<string>(new string[0]);
        internal const char PeriodSymbol = '.';

        internal static string CombineErrorMessage(IEnumerable<EdmItemError> errors)
        {
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            int num = 0;
            foreach (EdmItemError error in errors)
            {
                if (num++ != 0)
                {
                    builder.Append(Environment.NewLine);
                }
                builder.Append(error.Message);
            }
            return builder.ToString();
        }

        internal static string CombineErrorMessage(IEnumerable<EdmSchemaError> errors)
        {
            StringBuilder builder = new StringBuilder(Environment.NewLine);
            int num = 0;
            foreach (EdmSchemaError error in errors)
            {
                if (num++ != 0)
                {
                    builder.Append(Environment.NewLine);
                }
                builder.Append(error.ToString());
            }
            return builder.ToString();
        }

        internal static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sources)
        {
            foreach (IEnumerable<T> iteratorVariable0 in sources)
            {
                if (iteratorVariable0 != null)
                {
                    foreach (T iteratorVariable1 in iteratorVariable0)
                    {
                        yield return iteratorVariable1;
                    }
                }
            }
        }

        internal static void DisposeXmlReaders(IEnumerable<XmlReader> xmlReaders)
        {
            foreach (XmlReader reader in xmlReaders)
            {
                ((IDisposable) reader).Dispose();
            }
        }

        internal static IList GetAllStructuralMembers(EdmType edmType)
        {
            switch (edmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.AssociationType:
                    return ((AssociationType) edmType).AssociationEndMembers;

                case BuiltInTypeKind.ComplexType:
                    return ((ComplexType) edmType).Properties;

                case BuiltInTypeKind.EntityType:
                    return ((System.Data.Metadata.Edm.EntityType) edmType).Properties;

                case BuiltInTypeKind.RowType:
                    return ((RowType) edmType).Properties;
            }
            return EmptyArrayEdmProperty;
        }

        internal static string GetAttributeValue(XPathNavigator nav, string attributeName)
        {
            nav = nav.Clone();
            string str = null;
            if (nav.MoveToAttribute(attributeName, string.Empty))
            {
                str = nav.Value;
            }
            return str;
        }

        internal static string GetCommaDelimitedString(List<string> stringList)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (string str in stringList)
            {
                builder.Append(str);
                if (++num != stringList.Count)
                {
                    builder.Append(", ");
                }
            }
            return builder.ToString();
        }

        internal static FacetDescription GetFacet(IEnumerable<FacetDescription> facetCollection, string facetName)
        {
            foreach (FacetDescription description in facetCollection)
            {
                if (description.FacetName == facetName)
                {
                    return description;
                }
            }
            return null;
        }

        internal static string GetFileNameFromUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            if (uri.IsFile)
            {
                return uri.LocalPath;
            }
            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.UnacceptableUri(uri), "uri");
            }
            return uri.AbsolutePath;
        }

        internal static TypeUsage GetModelTypeUsage(EdmMember member) => 
            GetModelTypeUsage(member.TypeUsage);

        internal static TypeUsage GetModelTypeUsage(TypeUsage typeUsage) => 
            typeUsage.GetModelTypeUsage();

        internal static object GetTypedAttributeValue(XPathNavigator nav, string attributeName, Type clrType)
        {
            nav = nav.Clone();
            object obj2 = null;
            if (nav.MoveToAttribute(attributeName, string.Empty))
            {
                obj2 = nav.ValueAs(clrType);
            }
            return obj2;
        }

        internal static bool IsAssignableFrom(EdmType firstType, EdmType secondType)
        {
            if (secondType == null)
            {
                return false;
            }
            if (!firstType.Equals(secondType))
            {
                return IsSubtypeOf(secondType, firstType);
            }
            return true;
        }

        internal static bool IsAssociationEndMember(EdmMember member) => 
            (BuiltInTypeKind.AssociationEndMember == member.BuiltInTypeKind);

        internal static bool IsAssociationType(EdmType type) => 
            (BuiltInTypeKind.AssociationType == type.BuiltInTypeKind);

        internal static bool IsCollectionType(GlobalItem item) => 
            (BuiltInTypeKind.CollectionType == item.BuiltInTypeKind);

        internal static bool IsComplexType(EdmType type) => 
            (BuiltInTypeKind.ComplexType == type.BuiltInTypeKind);

        internal static bool IsEdmFunction(GlobalItem item) => 
            (BuiltInTypeKind.EdmFunction == item.BuiltInTypeKind);

        internal static bool IsEdmProperty(EdmMember member) => 
            (BuiltInTypeKind.EdmProperty == member.BuiltInTypeKind);

        internal static bool IsEntityContainer(GlobalItem item) => 
            (BuiltInTypeKind.EntityContainer == item.BuiltInTypeKind);

        internal static bool IsEntitySet(EntitySetBase entitySetBase) => 
            (BuiltInTypeKind.EntitySet == entitySetBase.BuiltInTypeKind);

        internal static bool IsEntityType(EdmType type) => 
            (BuiltInTypeKind.EntityType == type.BuiltInTypeKind);

        internal static bool IsEntityTypeBase(EdmType edmType)
        {
            if (!IsEntityType(edmType))
            {
                return IsRelationshipType(edmType);
            }
            return true;
        }

        internal static bool IsEnumType(EdmType edmType) => 
            (BuiltInTypeKind.EnumType == edmType.BuiltInTypeKind);

        internal static bool IsNavigationProperty(EdmMember member) => 
            (BuiltInTypeKind.NavigationProperty == member.BuiltInTypeKind);

        internal static bool IsPrimitiveType(EdmType type) => 
            (BuiltInTypeKind.PrimitiveType == type.BuiltInTypeKind);

        internal static int IsPromotableTo(IList<PrimitiveType> promotionTypes, PrimitiveType primitiveType)
        {
            for (int i = 0; i < promotionTypes.Count; i++)
            {
                if (object.ReferenceEquals(promotionTypes[i], primitiveType))
                {
                    return i;
                }
            }
            return -1;
        }

        internal static bool IsRefType(GlobalItem item) => 
            (BuiltInTypeKind.RefType == item.BuiltInTypeKind);

        internal static bool IsRelationshipEndMember(EdmMember member) => 
            (BuiltInTypeKind.AssociationEndMember == member.BuiltInTypeKind);

        internal static bool IsRelationshipSet(EntitySetBase entitySetBase) => 
            (BuiltInTypeKind.AssociationSet == entitySetBase.BuiltInTypeKind);

        internal static bool IsRelationshipType(EdmType type) => 
            (BuiltInTypeKind.AssociationType == type.BuiltInTypeKind);

        internal static bool IsRowType(GlobalItem item) => 
            (BuiltInTypeKind.RowType == item.BuiltInTypeKind);

        internal static bool IsStructuralType(EdmType type)
        {
            if ((!IsComplexType(type) && !IsEntityType(type)) && !IsRelationshipType(type))
            {
                return IsRowType(type);
            }
            return true;
        }

        internal static bool IsSubtypeOf(EdmType firstType, EdmType secondType)
        {
            if (secondType != null)
            {
                for (EdmType type = firstType.BaseType; type != null; type = type.BaseType)
                {
                    if (type == secondType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsTransientType(EdmType edmType)
        {
            if (!IsCollectionType(edmType) && !IsRefType(edmType))
            {
                return IsRowType(edmType);
            }
            return true;
        }

        internal static bool IsUnboundedFacetValue(Facet facet) => 
            object.ReferenceEquals(facet.Value, EdmConstants.UnboundedValue);

        internal static bool IsValidKeyType(PrimitiveTypeKind kind) => 
            (kind != PrimitiveTypeKind.Binary);

        internal static IEnumerable<KeyValuePair<T, S>> PairEnumerations<T, S>(IBaseList<T> left, IEnumerable<S> right)
        {
            IEnumerator enumerator = left.GetEnumerator();
            IEnumerator<S> iteratorVariable1 = right.GetEnumerator();
            while (true)
            {
                if (!enumerator.MoveNext() || !iteratorVariable1.MoveNext())
                {
                    yield break;
                }
                yield return new KeyValuePair<T, S>((T) enumerator.Current, iteratorVariable1.Current);
            }
        }

        internal static string ToString(ParameterMode value)
        {
            switch (value)
            {
                case ParameterMode.In:
                    return "In";

                case ParameterMode.Out:
                    return "Out";

                case ParameterMode.InOut:
                    return "InOut";

                case ParameterMode.ReturnValue:
                    return "ReturnValue";
            }
            return value.ToString();
        }

        internal static string ToString(ParameterDirection value)
        {
            switch (value)
            {
                case ParameterDirection.Input:
                    return "Input";

                case ParameterDirection.Output:
                    return "Output";

                case ParameterDirection.InputOutput:
                    return "InputOutput";

                case ParameterDirection.ReturnValue:
                    return "ReturnValue";
            }
            return value.ToString();
        }

        internal static TypeUsage ValidateAndConvertTypeUsage(EdmProperty edmProperty, EdmProperty columnProperty, IXmlLineInfo lineInfo, string sourceLocation, List<EdmSchemaError> parsingErrors, StoreItemCollection storeItemCollection) => 
            ValidateAndConvertTypeUsage(edmProperty, lineInfo, sourceLocation, edmProperty.TypeUsage, columnProperty.TypeUsage, parsingErrors, storeItemCollection);

        internal static TypeUsage ValidateAndConvertTypeUsage(EdmMember edmMember, IXmlLineInfo lineInfo, string sourceLocation, TypeUsage cspaceType, TypeUsage sspaceType, List<EdmSchemaError> parsingErrors, StoreItemCollection storeItemCollection)
        {
            TypeUsage superType = sspaceType;
            if (sspaceType.EdmType.DataSpace == DataSpace.SSpace)
            {
                superType = sspaceType.GetModelTypeUsage();
            }
            if (TypeSemantics.IsSubTypeOf(cspaceType, superType))
            {
                return superType;
            }
            return null;
        }

        [CompilerGenerated]
        private sealed class <Concat>d__5<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private T <>2__current;
            public IEnumerable<T>[] <>3__sources;
            public IEnumerable<T>[] <>7__wrap9;
            public int <>7__wrapa;
            public IEnumerator<T> <>7__wrapb;
            private int <>l__initialThreadId;
            public T <element>5__7;
            public IEnumerable<T> <source>5__6;
            public IEnumerable<T>[] sources;

            [DebuggerHidden]
            public <Concat>d__5(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally8()
            {
                this.<>1__state = -1;
            }

            private void <>m__Finallyc()
            {
                this.<>1__state = 1;
                if (this.<>7__wrapb != null)
                {
                    this.<>7__wrapb.Dispose();
                }
            }

            private bool MoveNext()
            {
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>1__state = 1;
                            this.<>7__wrap9 = this.sources;
                            this.<>7__wrapa = 0;
                            while (this.<>7__wrapa < this.<>7__wrap9.Length)
                            {
                                this.<source>5__6 = this.<>7__wrap9[this.<>7__wrapa];
                                if (this.<source>5__6 != null)
                                {
                                    this.<>7__wrapb = this.<source>5__6.GetEnumerator();
                                    this.<>1__state = 2;
                                    while (this.<>7__wrapb.MoveNext())
                                    {
                                        this.<element>5__7 = this.<>7__wrapb.Current;
                                        this.<>2__current = this.<element>5__7;
                                        this.<>1__state = 3;
                                        return true;
                                    Label_009A:
                                        this.<>1__state = 2;
                                    }
                                    this.<>m__Finallyc();
                                }
                                this.<>7__wrapa++;
                            }
                            this.<>m__Finally8();
                            break;

                        case 3:
                            goto Label_009A;
                    }
                    return false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                Helper.<Concat>d__5<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (Helper.<Concat>d__5<T>) this;
                }
                else
                {
                    d__ = new Helper.<Concat>d__5<T>(0);
                }
                d__.sources = this.<>3__sources;
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
                    case 3:
                        switch (this.<>1__state)
                        {
                            case 2:
                            case 3:
                                try
                                {
                                }
                                finally
                                {
                                    this.<>m__Finallyc();
                                }
                                goto Label_003E;
                        }
                        break;

                    default:
                        return;
                }
            Label_003E:
                this.<>m__Finally8();
            }

            T IEnumerator<T>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }

        [CompilerGenerated]
        private sealed class <PairEnumerations>d__0<T, S> : IEnumerable<KeyValuePair<T, S>>, IEnumerable, IEnumerator<KeyValuePair<T, S>>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private KeyValuePair<T, S> <>2__current;
            public IBaseList<T> <>3__left;
            public IEnumerable<S> <>3__right;
            private int <>l__initialThreadId;
            public IEnumerator <leftEnumerator>5__1;
            public IEnumerator<S> <rightEnumerator>5__2;
            public IBaseList<T> left;
            public IEnumerable<S> right;

            [DebuggerHidden]
            public <PairEnumerations>d__0(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        this.<leftEnumerator>5__1 = this.left.GetEnumerator();
                        this.<rightEnumerator>5__2 = this.right.GetEnumerator();
                        break;

                    case 1:
                        this.<>1__state = -1;
                        break;

                    default:
                        goto Label_0092;
                }
                if (this.<leftEnumerator>5__1.MoveNext() && this.<rightEnumerator>5__2.MoveNext())
                {
                    this.<>2__current = new KeyValuePair<T, S>((T) this.<leftEnumerator>5__1.Current, this.<rightEnumerator>5__2.Current);
                    this.<>1__state = 1;
                    return true;
                }
            Label_0092:
                return false;
            }

            [DebuggerHidden]
            IEnumerator<KeyValuePair<T, S>> IEnumerable<KeyValuePair<T, S>>.GetEnumerator()
            {
                Helper.<PairEnumerations>d__0<T, S> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (Helper.<PairEnumerations>d__0<T, S>) this;
                }
                else
                {
                    d__ = new Helper.<PairEnumerations>d__0<T, S>(0);
                }
                d__.left = this.<>3__left;
                d__.right = this.<>3__right;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<T,S>>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            KeyValuePair<T, S> IEnumerator<KeyValuePair<T, S>>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

