namespace System.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable]
    internal class TreeSet<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISerializable, IDeserializationCallback
    {
        private object _syncRoot;
        private IComparer<T> comparer;
        private const string ComparerName = "Comparer";
        private int count;
        private const string CountName = "Count";
        private const string ItemsName = "Items";
        private Node<T> root;
        private SerializationInfo siInfo;
        private int version;
        private const string VersionName = "Version";

        public TreeSet(IComparer<T> comparer)
        {
            if (comparer == null)
            {
                this.comparer = Comparer<T>.Default;
            }
            else
            {
                this.comparer = comparer;
            }
        }

        protected TreeSet(SerializationInfo info, StreamingContext context)
        {
            this.siInfo = info;
        }

        public void Add(T item)
        {
            if (this.root == null)
            {
                this.root = new Node<T>(item, false);
                this.count = 1;
            }
            else
            {
                Node<T> root = this.root;
                Node<T> node = null;
                Node<T> grandParent = null;
                Node<T> greatGrandParent = null;
                int num = 0;
                while (root != null)
                {
                    num = this.comparer.Compare(item, root.Item);
                    if (num == 0)
                    {
                        this.root.IsRed = false;
                        System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_AddingDuplicate);
                    }
                    if (TreeSet<T>.Is4Node(root))
                    {
                        TreeSet<T>.Split4Node(root);
                        if (TreeSet<T>.IsRed(node))
                        {
                            this.InsertionBalance(root, ref node, grandParent, greatGrandParent);
                        }
                    }
                    greatGrandParent = grandParent;
                    grandParent = node;
                    node = root;
                    root = (num < 0) ? root.Left : root.Right;
                }
                Node<T> current = new Node<T>(item);
                if (num > 0)
                {
                    node.Right = current;
                }
                else
                {
                    node.Left = current;
                }
                if (node.IsRed)
                {
                    this.InsertionBalance(current, ref node, grandParent, greatGrandParent);
                }
                this.root.IsRed = false;
                this.count++;
                this.version++;
            }
        }

        public void Clear()
        {
            this.root = null;
            this.count = 0;
            this.version++;
        }

        public bool Contains(T item) => 
            (this.FindNode(item) != null);

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
            }
            if (index < 0)
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.index);
            }
            if ((array.Length - index) < this.Count)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            this.InOrderTreeWalk(delegate (Node<T> node) {
                array[index++] = node.Item;
                return true;
            });
        }

        internal Node<T> FindNode(T item)
        {
            int num;
            for (Node<T> node = this.root; node != null; node = (num < 0) ? node.Left : node.Right)
            {
                num = this.comparer.Compare(item, node.Item);
                if (num == 0)
                {
                    return node;
                }
            }
            return null;
        }

        public Enumerator<T> GetEnumerator() => 
            new Enumerator<T>((TreeSet<T>) this);

        protected void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.info);
            }
            info.AddValue("Count", this.count);
            info.AddValue("Comparer", this.comparer, typeof(IComparer<T>));
            info.AddValue("Version", this.version);
            if (this.root != null)
            {
                T[] array = new T[this.Count];
                this.CopyTo(array, 0);
                info.AddValue("Items", array, typeof(T[]));
            }
        }

        private static Node<T> GetSibling(Node<T> node, Node<T> parent)
        {
            if (parent.Left == node)
            {
                return parent.Right;
            }
            return parent.Left;
        }

        internal bool InOrderTreeWalk(TreeWalkAction<T> action)
        {
            if (this.root != null)
            {
                Stack<Node<T>> stack = new Stack<Node<T>>(2 * ((int) Math.Log((double) (this.Count + 1))));
                Node<T> root = this.root;
                while (root != null)
                {
                    stack.Push(root);
                    root = root.Left;
                }
                while (stack.Count != 0)
                {
                    root = stack.Pop();
                    if (!action(root))
                    {
                        return false;
                    }
                    for (Node<T> node2 = root.Right; node2 != null; node2 = node2.Left)
                    {
                        stack.Push(node2);
                    }
                }
            }
            return true;
        }

        private void InsertionBalance(Node<T> current, ref Node<T> parent, Node<T> grandParent, Node<T> greatGrandParent)
        {
            Node<T> node;
            bool flag = grandParent.Right == parent;
            bool flag2 = parent.Right == current;
            if (flag == flag2)
            {
                node = flag2 ? TreeSet<T>.RotateLeft(grandParent) : TreeSet<T>.RotateRight(grandParent);
            }
            else
            {
                node = flag2 ? TreeSet<T>.RotateLeftRight(grandParent) : TreeSet<T>.RotateRightLeft(grandParent);
                parent = greatGrandParent;
            }
            grandParent.IsRed = true;
            node.IsRed = false;
            this.ReplaceChildOfNodeOrRoot(greatGrandParent, grandParent, node);
        }

        private static bool Is2Node(Node<T> node) => 
            ((TreeSet<T>.IsBlack(node) && TreeSet<T>.IsNullOrBlack(node.Left)) && TreeSet<T>.IsNullOrBlack(node.Right));

        private static bool Is4Node(Node<T> node) => 
            (TreeSet<T>.IsRed(node.Left) && TreeSet<T>.IsRed(node.Right));

        private static bool IsBlack(Node<T> node) => 
            ((node != null) && !node.IsRed);

        private static bool IsNullOrBlack(Node<T> node)
        {
            if (node != null)
            {
                return !node.IsRed;
            }
            return true;
        }

        private static bool IsRed(Node<T> node) => 
            ((node != null) && node.IsRed);

        private static void Merge2Nodes(Node<T> parent, Node<T> child1, Node<T> child2)
        {
            parent.IsRed = false;
            child1.IsRed = true;
            child2.IsRed = true;
        }

        protected void OnDeserialization(object sender)
        {
            if (this.comparer == null)
            {
                if (this.siInfo == null)
                {
                    System.ThrowHelper.ThrowSerializationException(System.ExceptionResource.Serialization_InvalidOnDeser);
                }
                this.comparer = (IComparer<T>) this.siInfo.GetValue("Comparer", typeof(IComparer<T>));
                int num = this.siInfo.GetInt32("Count");
                if (num != 0)
                {
                    T[] localArray = (T[]) this.siInfo.GetValue("Items", typeof(T[]));
                    if (localArray == null)
                    {
                        System.ThrowHelper.ThrowSerializationException(System.ExceptionResource.Serialization_MissingValues);
                    }
                    for (int i = 0; i < localArray.Length; i++)
                    {
                        this.Add(localArray[i]);
                    }
                }
                this.version = this.siInfo.GetInt32("Version");
                if (this.count != num)
                {
                    System.ThrowHelper.ThrowSerializationException(System.ExceptionResource.Serialization_MismatchedCount);
                }
                this.siInfo = null;
            }
        }

        public bool Remove(T item)
        {
            if (this.root == null)
            {
                return false;
            }
            Node<T> root = this.root;
            Node<T> parent = null;
            Node<T> node3 = null;
            Node<T> match = null;
            Node<T> parentOfMatch = null;
            bool flag = false;
            while (root != null)
            {
                if (TreeSet<T>.Is2Node(root))
                {
                    if (parent == null)
                    {
                        root.IsRed = true;
                    }
                    else
                    {
                        Node<T> sibling = TreeSet<T>.GetSibling(root, parent);
                        if (sibling.IsRed)
                        {
                            if (parent.Right == sibling)
                            {
                                TreeSet<T>.RotateLeft(parent);
                            }
                            else
                            {
                                TreeSet<T>.RotateRight(parent);
                            }
                            parent.IsRed = true;
                            sibling.IsRed = false;
                            this.ReplaceChildOfNodeOrRoot(node3, parent, sibling);
                            node3 = sibling;
                            if (parent == match)
                            {
                                parentOfMatch = sibling;
                            }
                            sibling = (parent.Left == root) ? parent.Right : parent.Left;
                        }
                        if (TreeSet<T>.Is2Node(sibling))
                        {
                            TreeSet<T>.Merge2Nodes(parent, root, sibling);
                        }
                        else
                        {
                            TreeRotation rotation = TreeSet<T>.RotationNeeded(parent, root, sibling);
                            Node<T> newChild = null;
                            switch (rotation)
                            {
                                case TreeRotation.LeftRotation:
                                    sibling.Right.IsRed = false;
                                    newChild = TreeSet<T>.RotateLeft(parent);
                                    break;

                                case TreeRotation.RightRotation:
                                    sibling.Left.IsRed = false;
                                    newChild = TreeSet<T>.RotateRight(parent);
                                    break;

                                case TreeRotation.RightLeftRotation:
                                    newChild = TreeSet<T>.RotateRightLeft(parent);
                                    break;

                                case TreeRotation.LeftRightRotation:
                                    newChild = TreeSet<T>.RotateLeftRight(parent);
                                    break;
                            }
                            newChild.IsRed = parent.IsRed;
                            parent.IsRed = false;
                            root.IsRed = true;
                            this.ReplaceChildOfNodeOrRoot(node3, parent, newChild);
                            if (parent == match)
                            {
                                parentOfMatch = newChild;
                            }
                            node3 = newChild;
                        }
                    }
                }
                int num = flag ? -1 : this.comparer.Compare(item, root.Item);
                if (num == 0)
                {
                    flag = true;
                    match = root;
                    parentOfMatch = parent;
                }
                node3 = parent;
                parent = root;
                if (num < 0)
                {
                    root = root.Left;
                }
                else
                {
                    root = root.Right;
                }
            }
            if (match != null)
            {
                this.ReplaceNode(match, parentOfMatch, parent, node3);
                this.count--;
            }
            if (this.root != null)
            {
                this.root.IsRed = false;
            }
            this.version++;
            return flag;
        }

        private void ReplaceChildOfNodeOrRoot(Node<T> parent, Node<T> child, Node<T> newChild)
        {
            if (parent != null)
            {
                if (parent.Left == child)
                {
                    parent.Left = newChild;
                }
                else
                {
                    parent.Right = newChild;
                }
            }
            else
            {
                this.root = newChild;
            }
        }

        private void ReplaceNode(Node<T> match, Node<T> parentOfMatch, Node<T> succesor, Node<T> parentOfSuccesor)
        {
            if (succesor == match)
            {
                succesor = match.Left;
            }
            else
            {
                if (succesor.Right != null)
                {
                    succesor.Right.IsRed = false;
                }
                if (parentOfSuccesor != match)
                {
                    parentOfSuccesor.Left = succesor.Right;
                    succesor.Right = match.Right;
                }
                succesor.Left = match.Left;
            }
            if (succesor != null)
            {
                succesor.IsRed = match.IsRed;
            }
            this.ReplaceChildOfNodeOrRoot(parentOfMatch, match, succesor);
        }

        private static Node<T> RotateLeft(Node<T> node)
        {
            Node<T> right = node.Right;
            node.Right = right.Left;
            right.Left = node;
            return right;
        }

        private static Node<T> RotateLeftRight(Node<T> node)
        {
            Node<T> left = node.Left;
            Node<T> right = left.Right;
            node.Left = right.Right;
            right.Right = node;
            left.Right = right.Left;
            right.Left = left;
            return right;
        }

        private static Node<T> RotateRight(Node<T> node)
        {
            Node<T> left = node.Left;
            node.Left = left.Right;
            left.Right = node;
            return left;
        }

        private static Node<T> RotateRightLeft(Node<T> node)
        {
            Node<T> right = node.Right;
            Node<T> left = right.Left;
            node.Right = left.Left;
            left.Left = node;
            right.Left = left.Right;
            left.Right = right;
            return left;
        }

        private static TreeRotation RotationNeeded(Node<T> parent, Node<T> current, Node<T> sibling)
        {
            if (TreeSet<T>.IsRed(sibling.Left))
            {
                if (parent.Left == current)
                {
                    return TreeRotation.RightLeftRotation;
                }
                return TreeRotation.RightRotation;
            }
            if (parent.Left == current)
            {
                return TreeRotation.LeftRotation;
            }
            return TreeRotation.LeftRightRotation;
        }

        private static void Split4Node(Node<T> node)
        {
            node.IsRed = true;
            node.Left.IsRed = false;
            node.Right.IsRed = false;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            new Enumerator<T>((TreeSet<T>) this);

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                System.ThrowHelper.ThrowArgumentNullException(System.ExceptionArgument.array);
            }
            if (array.Rank != 1)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_NonZeroLowerBound);
            }
            if (index < 0)
            {
                System.ThrowHelper.ThrowArgumentOutOfRangeException(System.ExceptionArgument.arrayIndex, System.ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if ((array.Length - index) < this.Count)
            {
                System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            T[] localArray = array as T[];
            if (localArray != null)
            {
                this.CopyTo(localArray, index);
            }
            else
            {
                TreeWalkAction<T> action = null;
                object[] objects = array as object[];
                if (objects == null)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
                try
                {
                    if (action == null)
                    {
                        action = delegate (Node<T> node) {
                            objects[index++] = node.Item;
                            return true;
                        };
                    }
                    this.InOrderTreeWalk(action);
                }
                catch (ArrayTypeMismatchException)
                {
                    System.ThrowHelper.ThrowArgumentException(System.ExceptionResource.Argument_InvalidArrayType);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            new Enumerator<T>((TreeSet<T>) this);

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this.OnDeserialization(sender);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        internal void UpdateVersion()
        {
            this.version++;
        }

        public IComparer<T> Comparer =>
            this.comparer;

        public int Count =>
            this.count;

        bool ICollection<T>.IsReadOnly =>
            false;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private const string TreeName = "Tree";
            private const string NodeValueName = "Item";
            private const string EnumStartName = "EnumStarted";
            private const string VersionName = "Version";
            private TreeSet<T> tree;
            private int version;
            private Stack<TreeSet<T>.Node> stack;
            private TreeSet<T>.Node current;
            private static TreeSet<T>.Node dummyNode;
            internal Enumerator(TreeSet<T> set)
            {
                this.tree = set;
                this.version = this.tree.version;
                this.stack = new Stack<TreeSet<T>.Node>(2 * ((int) Math.Log((double) (set.Count + 1))));
                this.current = null;
                this.Intialize();
            }

            private void Intialize()
            {
                this.current = null;
                for (TreeSet<T>.Node node = this.tree.root; node != null; node = node.Left)
                {
                    this.stack.Push(node);
                }
            }

            public bool MoveNext()
            {
                if (this.version != this.tree.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                if (this.stack.Count == 0)
                {
                    this.current = null;
                    return false;
                }
                this.current = this.stack.Pop();
                for (TreeSet<T>.Node node = this.current.Right; node != null; node = node.Left)
                {
                    this.stack.Push(node);
                }
                return true;
            }

            public void Dispose()
            {
            }

            public T Current
            {
                get
                {
                    if (this.current != null)
                    {
                        return this.current.Item;
                    }
                    return default(T);
                }
            }
            object IEnumerator.Current =>
                this.current?.Item;
            internal bool NotStartedOrEnded =>
                (this.current == null);
            internal void Reset()
            {
                if (this.version != this.tree.version)
                {
                    System.ThrowHelper.ThrowInvalidOperationException(System.ExceptionResource.InvalidOperation_EnumFailedVersion);
                }
                this.stack.Clear();
                this.Intialize();
            }

            void IEnumerator.Reset()
            {
                this.Reset();
            }

            static Enumerator()
            {
                TreeSet<T>.Enumerator.dummyNode = new TreeSet<T>.Node(default(T));
            }
        }

        internal class Node
        {
            private bool isRed;
            private T item;
            private TreeSet<T>.Node left;
            private TreeSet<T>.Node right;

            public Node(T item)
            {
                this.item = item;
                this.isRed = true;
            }

            public Node(T item, bool isRed)
            {
                this.item = item;
                this.isRed = isRed;
            }

            public bool IsRed
            {
                get => 
                    this.isRed;
                set
                {
                    this.isRed = value;
                }
            }

            public T Item
            {
                get => 
                    this.item;
                set
                {
                    this.item = value;
                }
            }

            public TreeSet<T>.Node Left
            {
                get => 
                    this.left;
                set
                {
                    this.left = value;
                }
            }

            public TreeSet<T>.Node Right
            {
                get => 
                    this.right;
                set
                {
                    this.right = value;
                }
            }
        }
    }
}

