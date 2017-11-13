namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal sealed class BindingGraph
    {
        private Graph graph;
        private BindingObserver observer;

        public BindingGraph(BindingObserver observer)
        {
            this.observer = observer;
            this.graph = new Graph();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool AddCollection(object source, string sourceProperty, object collection, string collectionEntitySet)
        {
            if (this.graph.ExistsVertex(collection))
            {
                return false;
            }
            Vertex vertex = this.graph.AddVertex(collection);
            vertex.IsCollection = true;
            vertex.EntitySet = collectionEntitySet;
            ICollection is2 = collection as ICollection;
            if (source != null)
            {
                vertex.Parent = this.graph.LookupVertex(source);
                vertex.ParentProperty = sourceProperty;
                this.graph.AddEdge(source, collection, sourceProperty);
                Type collectionEntityType = BindingUtils.GetCollectionEntityType(collection.GetType());
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(collectionEntityType))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.DataBinding_NotifyPropertyChangedNotImpl(collectionEntityType));
                }
                typeof(BindingGraph).GetMethod("SetObserver", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(new Type[] { collectionEntityType }).Invoke(this, new object[] { is2 });
            }
            else
            {
                this.graph.Root = vertex;
            }
            this.AttachCollectionNotification(collection);
            foreach (object obj2 in is2)
            {
                this.AddEntity(source, sourceProperty, obj2, collectionEntitySet, collection);
            }
            return true;
        }

        public void AddComplexProperty(object source, string sourceProperty, object target)
        {
            Vertex vertex = this.graph.LookupVertex(source);
            if (this.graph.LookupVertex(target) != null)
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.DataBinding_ComplexObjectAssociatedWithMultipleEntities(target.GetType()));
            }
            Vertex vertex2 = this.graph.AddVertex(target);
            vertex2.Parent = vertex;
            vertex2.IsComplex = true;
            if (!this.AttachEntityOrComplexObjectNotification(target))
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.DataBinding_NotifyPropertyChangedNotImpl(target.GetType()));
            }
            this.graph.AddEdge(source, target, sourceProperty);
            this.AddFromProperties(target);
        }

        public bool AddEntity(object source, string sourceProperty, object target, string targetEntitySet, object edgeSource)
        {
            Vertex vertex = this.graph.LookupVertex(edgeSource);
            Vertex vertex2 = null;
            bool flag = false;
            if (target != null)
            {
                vertex2 = this.graph.LookupVertex(target);
                if (vertex2 == null)
                {
                    vertex2 = this.graph.AddVertex(target);
                    vertex2.EntitySet = BindingEntityInfo.GetEntitySet(target, targetEntitySet);
                    if (!this.AttachEntityOrComplexObjectNotification(target))
                    {
                        throw new InvalidOperationException(System.Data.Services.Client.Strings.DataBinding_NotifyPropertyChangedNotImpl(target.GetType()));
                    }
                    flag = true;
                }
                if (this.graph.ExistsEdge(edgeSource, target, vertex.IsCollection ? null : sourceProperty))
                {
                    throw new InvalidOperationException(System.Data.Services.Client.Strings.DataBinding_EntityAlreadyInCollection(target.GetType()));
                }
                this.graph.AddEdge(edgeSource, target, vertex.IsCollection ? null : sourceProperty);
            }
            if (!vertex.IsCollection)
            {
                this.observer.HandleUpdateEntityReference(source, sourceProperty, vertex.EntitySet, target, vertex2?.EntitySet);
            }
            else
            {
                this.observer.HandleAddEntity(source, sourceProperty, vertex.Parent?.EntitySet, edgeSource as ICollection, target, vertex2.EntitySet);
            }
            if (flag)
            {
                this.AddFromProperties(target);
            }
            return flag;
        }

        private void AddFromProperties(object entity)
        {
            foreach (BindingEntityInfo.BindingPropertyInfo info in BindingEntityInfo.GetObservableProperties(entity.GetType()))
            {
                object target = info.PropertyInfo.GetValue(entity);
                if (target != null)
                {
                    switch (info.PropertyKind)
                    {
                        case BindingPropertyKind.BindingPropertyKindEntity:
                        {
                            this.AddEntity(entity, info.PropertyInfo.PropertyName, target, null, entity);
                            continue;
                        }
                        case BindingPropertyKind.BindingPropertyKindCollection:
                        {
                            this.AddCollection(entity, info.PropertyInfo.PropertyName, target, null);
                            continue;
                        }
                    }
                    this.AddComplexProperty(entity, info.PropertyInfo.PropertyName, target);
                }
            }
        }

        private void AttachCollectionNotification(object target)
        {
            INotifyCollectionChanged changed = target as INotifyCollectionChanged;
            changed.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.observer.OnCollectionChanged);
            changed.CollectionChanged += new NotifyCollectionChangedEventHandler(this.observer.OnCollectionChanged);
        }

        private bool AttachEntityOrComplexObjectNotification(object target)
        {
            INotifyPropertyChanged changed = target as INotifyPropertyChanged;
            if (changed != null)
            {
                changed.PropertyChanged -= new PropertyChangedEventHandler(this.observer.OnPropertyChanged);
                changed.PropertyChanged += new PropertyChangedEventHandler(this.observer.OnPropertyChanged);
                return true;
            }
            return false;
        }

        private void DetachCollectionNotifications(object target)
        {
            INotifyCollectionChanged changed = target as INotifyCollectionChanged;
            if (changed != null)
            {
                changed.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.observer.OnCollectionChanged);
            }
        }

        private void DetachNotifications(object target)
        {
            this.DetachCollectionNotifications(target);
            INotifyPropertyChanged changed = target as INotifyPropertyChanged;
            if (changed != null)
            {
                changed.PropertyChanged -= new PropertyChangedEventHandler(this.observer.OnPropertyChanged);
            }
        }

        public void GetAncestorEntityForComplexProperty(ref object entity, ref string propertyName, ref object propertyValue)
        {
            for (Vertex vertex = this.graph.LookupVertex(entity); vertex.IsComplex; vertex = vertex.Parent)
            {
                propertyName = vertex.IncomingEdges[0].Label;
                propertyValue = vertex.Item;
                entity = vertex.Parent.Item;
            }
        }

        public IEnumerable<object> GetCollectionItems(object collection)
        {
            Vertex vertex = this.graph.LookupVertex(collection);
            foreach (Edge iteratorVariable1 in vertex.OutgoingEdges.ToList<Edge>())
            {
                yield return iteratorVariable1.Target.Item;
            }
        }

        public void GetEntityCollectionInfo(object collection, out object source, out string sourceProperty, out string sourceEntitySet, out string targetEntitySet)
        {
            this.graph.LookupVertex(collection).GetEntityCollectionInfo(out source, out sourceProperty, out sourceEntitySet, out targetEntitySet);
        }

        public void Remove(object item, object parent, string parentProperty)
        {
            Func<BindingEntityInfo.BindingPropertyInfo, bool> predicate = null;
            if (this.graph.LookupVertex(item) != null)
            {
                if (parentProperty != null)
                {
                    if (predicate == null)
                    {
                        predicate = p => p.PropertyInfo.PropertyName == parentProperty;
                    }
                    parent = BindingEntityInfo.GetObservableProperties(parent.GetType()).Single<BindingEntityInfo.BindingPropertyInfo>(predicate).PropertyInfo.GetValue(parent);
                }
                object source = null;
                string sourceProperty = null;
                string sourceEntitySet = null;
                string targetEntitySet = null;
                this.GetEntityCollectionInfo(parent, out source, out sourceProperty, out sourceEntitySet, out targetEntitySet);
                targetEntitySet = BindingEntityInfo.GetEntitySet(item, targetEntitySet);
                this.observer.HandleDeleteEntity(source, sourceProperty, sourceEntitySet, parent as ICollection, item, targetEntitySet);
                this.graph.RemoveEdge(parent, item, null);
            }
        }

        public void RemoveCollection(object collection)
        {
            foreach (Edge edge in this.graph.LookupVertex(collection).OutgoingEdges.ToList<Edge>())
            {
                this.graph.RemoveEdge(collection, edge.Target.Item, null);
            }
            this.RemoveUnreachableVertices();
        }

        public void RemoveNonTrackedEntities()
        {
            foreach (object obj2 in this.graph.Select(o => BindingEntityInfo.IsEntityType(o.GetType()) && !this.observer.IsContextTrackingEntity(o)))
            {
                this.graph.ClearEdgesForVertex(this.graph.LookupVertex(obj2));
            }
            this.RemoveUnreachableVertices();
        }

        public void RemoveRelation(object source, string relation)
        {
            Edge edge = this.graph.LookupVertex(source).OutgoingEdges.SingleOrDefault<Edge>(e => (e.Source.Item == source) && (e.Label == relation));
            if (edge != null)
            {
                this.graph.RemoveEdge(edge.Source.Item, edge.Target.Item, edge.Label);
            }
            this.RemoveUnreachableVertices();
        }

        public void RemoveUnreachableVertices()
        {
            this.graph.RemoveUnreachableVertices(new Action<object>(this.DetachNotifications));
        }

        public void Reset()
        {
            this.graph.Reset(new Action<object>(this.DetachNotifications));
        }

        private void SetObserver<T>(ICollection collection)
        {
            DataServiceCollection<T> services = collection as DataServiceCollection<T>;
            services.Observer = this.observer;
        }


        internal sealed class Edge : IEquatable<BindingGraph.Edge>
        {
            public bool Equals(BindingGraph.Edge other) => 
                ((((other != null) && object.ReferenceEquals(this.Source, other.Source)) && object.ReferenceEquals(this.Target, other.Target)) && (this.Label == other.Label));

            public string Label { get; set; }

            public BindingGraph.Vertex Source { get; set; }

            public BindingGraph.Vertex Target { get; set; }
        }

        internal sealed class Graph
        {
            private BindingGraph.Vertex root;
            private Dictionary<object, BindingGraph.Vertex> vertices = new Dictionary<object, BindingGraph.Vertex>(ReferenceEqualityComparer<object>.Instance);

            public BindingGraph.Edge AddEdge(object source, object target, string label)
            {
                BindingGraph.Vertex vertex = this.vertices[source];
                BindingGraph.Vertex vertex2 = this.vertices[target];
                BindingGraph.Edge item = new BindingGraph.Edge {
                    Source = vertex,
                    Target = vertex2,
                    Label = label
                };
                vertex.OutgoingEdges.Add(item);
                vertex2.IncomingEdges.Add(item);
                return item;
            }

            public BindingGraph.Vertex AddVertex(object item)
            {
                BindingGraph.Vertex vertex = new BindingGraph.Vertex(item);
                this.vertices.Add(item, vertex);
                return vertex;
            }

            public void ClearEdgesForVertex(BindingGraph.Vertex v)
            {
                foreach (BindingGraph.Edge edge in v.OutgoingEdges.Concat<BindingGraph.Edge>(v.IncomingEdges).ToList<BindingGraph.Edge>())
                {
                    this.RemoveEdge(edge.Source.Item, edge.Target.Item, edge.Label);
                }
            }

            public bool ExistsEdge(object source, object target, string label)
            {
                BindingGraph.Edge e = new BindingGraph.Edge {
                    Source = this.vertices[source],
                    Target = this.vertices[target],
                    Label = label
                };
                return this.vertices[source].OutgoingEdges.Any<BindingGraph.Edge>(r => r.Equals(e));
            }

            public bool ExistsVertex(object item)
            {
                BindingGraph.Vertex vertex;
                return this.vertices.TryGetValue(item, out vertex);
            }

            public BindingGraph.Vertex LookupVertex(object item)
            {
                BindingGraph.Vertex vertex;
                this.vertices.TryGetValue(item, out vertex);
                return vertex;
            }

            public void RemoveEdge(object source, object target, string label)
            {
                BindingGraph.Vertex vertex = this.vertices[source];
                BindingGraph.Vertex vertex2 = this.vertices[target];
                BindingGraph.Edge item = new BindingGraph.Edge {
                    Source = vertex,
                    Target = vertex2,
                    Label = label
                };
                vertex.OutgoingEdges.Remove(item);
                vertex2.IncomingEdges.Remove(item);
            }

            public void RemoveUnreachableVertices(Action<object> detachAction)
            {
                try
                {
                    foreach (BindingGraph.Vertex vertex in this.UnreachableVertices())
                    {
                        this.ClearEdgesForVertex(vertex);
                        detachAction(vertex.Item);
                        this.vertices.Remove(vertex.Item);
                    }
                }
                finally
                {
                    foreach (BindingGraph.Vertex vertex2 in this.vertices.Values)
                    {
                        vertex2.Color = VertexColor.White;
                    }
                }
            }

            public void Reset(Action<object> action)
            {
                foreach (object obj2 in this.vertices.Keys)
                {
                    action(obj2);
                }
                this.vertices.Clear();
            }

            public IList<object> Select(Func<object, bool> filter) => 
                this.vertices.Keys.Where<object>(filter).ToList<object>();

            private IEnumerable<BindingGraph.Vertex> UnreachableVertices()
            {
                Queue<BindingGraph.Vertex> queue = new Queue<BindingGraph.Vertex>();
                this.Root.Color = VertexColor.Gray;
                queue.Enqueue(this.Root);
                while (queue.Count != 0)
                {
                    BindingGraph.Vertex vertex = queue.Dequeue();
                    foreach (BindingGraph.Edge edge in vertex.OutgoingEdges)
                    {
                        if (edge.Target.Color == VertexColor.White)
                        {
                            edge.Target.Color = VertexColor.Gray;
                            queue.Enqueue(edge.Target);
                        }
                    }
                    vertex.Color = VertexColor.Black;
                }
                return (from v in this.vertices.Values
                    where v.Color == VertexColor.White
                    select v).ToList<BindingGraph.Vertex>();
            }

            public BindingGraph.Vertex Root
            {
                get => 
                    this.root;
                set
                {
                    this.root = value;
                }
            }
        }

        internal sealed class Vertex
        {
            private List<BindingGraph.Edge> incomingEdges;
            private List<BindingGraph.Edge> outgoingEdges;

            public Vertex(object item)
            {
                this.Item = item;
                this.Color = VertexColor.White;
            }

            public void GetEntityCollectionInfo(out object source, out string sourceProperty, out string sourceEntitySet, out string targetEntitySet)
            {
                if (!this.IsRootCollection)
                {
                    source = this.Parent.Item;
                    sourceProperty = this.ParentProperty;
                    sourceEntitySet = this.Parent.EntitySet;
                }
                else
                {
                    source = null;
                    sourceProperty = null;
                    sourceEntitySet = null;
                }
                targetEntitySet = this.EntitySet;
            }

            public VertexColor Color { get; set; }

            public string EntitySet { get; set; }

            public IList<BindingGraph.Edge> IncomingEdges
            {
                get
                {
                    if (this.incomingEdges == null)
                    {
                        this.incomingEdges = new List<BindingGraph.Edge>();
                    }
                    return this.incomingEdges;
                }
            }

            public bool IsCollection { get; set; }

            public bool IsComplex { get; set; }

            public bool IsRootCollection =>
                (this.IsCollection && (this.Parent == null));

            public object Item { get; private set; }

            public IList<BindingGraph.Edge> OutgoingEdges
            {
                get
                {
                    if (this.outgoingEdges == null)
                    {
                        this.outgoingEdges = new List<BindingGraph.Edge>();
                    }
                    return this.outgoingEdges;
                }
            }

            public BindingGraph.Vertex Parent { get; set; }

            public string ParentProperty { get; set; }
        }
    }
}

