namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal class ExpressionList : IList<DbExpression>, ICollection<DbExpression>, IEnumerable<DbExpression>, IEnumerable
    {
        private DbCommandTree _commandTree;
        private List<ExpressionLink> _list;
        private string _name;

        private ExpressionList(string name, DbCommandTree owner)
        {
            this._name = name;
            this._commandTree = owner;
        }

        internal ExpressionList(string name, DbCommandTree owner, IList<DbExpression> args) : this(name, owner)
        {
            int count = args.Count;
            List<ExpressionLink> list = new List<ExpressionLink>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new ExpressionLink(CommandTreeUtils.FormatIndex(this._name, i), this._commandTree, args[i]));
            }
            this._list = list;
        }

        internal ExpressionList(string name, DbCommandTree owner, int capacity) : this(name, owner)
        {
            this._list = new List<ExpressionLink>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this._list.Add(new ExpressionLink(CommandTreeUtils.FormatIndex(this._name, i), owner));
            }
        }

        internal ExpressionList(string name, DbCommandTree owner, IBaseList<EdmMember> members, IList<DbExpression> args) : this(name, owner)
        {
            this._list = new List<ExpressionLink>();
            int num = 0;
            foreach (EdmMember member in members)
            {
                string varName = CommandTreeUtils.FormatIndex(this._name, num++);
                this._commandTree.TypeHelper.CheckMember(member, varName);
                this._list.Add(new ExpressionLink(varName, this._commandTree, Helper.GetModelTypeUsage(member)));
            }
            this.SetElements(args);
        }

        internal ExpressionList(string name, DbCommandTree owner, PrimitiveTypeKind expectedElementPrimitiveType, IList<DbExpression> args) : this(name, owner)
        {
            int count = args.Count;
            List<ExpressionLink> list = new List<ExpressionLink>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new ExpressionLink(CommandTreeUtils.FormatIndex(this._name, i), this._commandTree, expectedElementPrimitiveType, args[i]));
            }
            this._list = list;
        }

        internal ExpressionList(string name, DbCommandTree owner, ReadOnlyMetadataCollection<FunctionParameter> paramInfo, IList<DbExpression> args) : this(name, owner)
        {
            int count = paramInfo.Count;
            List<ExpressionLink> list = new List<ExpressionLink>(count);
            for (int i = 0; i < count; i++)
            {
                string varName = CommandTreeUtils.FormatIndex(this._name, i);
                FunctionParameter paramMeta = paramInfo[i];
                this._commandTree.TypeHelper.CheckParameter(paramMeta, varName);
                list.Add(new ExpressionLink(varName, this._commandTree, paramMeta.TypeUsage));
            }
            this._list = list;
            this.SetElements(args);
        }

        internal ExpressionList(string name, DbCommandTree owner, TypeUsage expectedElementType, IList<DbExpression> args) : this(name, owner)
        {
            int count = args.Count;
            List<ExpressionLink> list = new List<ExpressionLink>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new ExpressionLink(CommandTreeUtils.FormatIndex(this._name, i), this._commandTree, expectedElementType, args[i]));
            }
            this._list = list;
        }

        internal TypeUsage GetCommonElementType()
        {
            TypeUsage resultType = null;
            foreach (ExpressionLink link in this._list)
            {
                if (resultType == null)
                {
                    resultType = link.Expression.ResultType;
                }
                else
                {
                    resultType = TypeHelpers.GetCommonTypeUsage(resultType, link.Expression.ResultType);
                }
                if (resultType == null)
                {
                    return resultType;
                }
            }
            return resultType;
        }

        internal void SetAt(int index, DbExpression value)
        {
            this._list[index].Expression = value;
        }

        internal void SetElements(IList<DbExpression> args)
        {
            int num = 0;
            for (int i = 0; i < args.Count; i++)
            {
                if (i > (this._list.Count - 1))
                {
                    throw EntityUtil.Argument(Strings.Cqt_ExpressionList_IncorrectElementCount, this._name);
                }
                this._list[i].InitializeValue(args[i]);
                num++;
            }
            if (num != this._list.Count)
            {
                throw EntityUtil.Argument(Strings.Cqt_ExpressionList_IncorrectElementCount, this._name);
            }
        }

        internal void SetExpectedElementType(TypeUsage type)
        {
            foreach (ExpressionLink link in this._list)
            {
                link.SetExpectedType(type);
            }
        }

        void ICollection<DbExpression>.Add(DbExpression item)
        {
            throw EntityUtil.NotSupported();
        }

        void ICollection<DbExpression>.Clear()
        {
            throw EntityUtil.NotSupported();
        }

        bool ICollection<DbExpression>.Contains(DbExpression item) => 
            (((IList<DbExpression>) this).IndexOf(item) > -1);

        void ICollection<DbExpression>.CopyTo(DbExpression[] array, int arrayIndex)
        {
            List<DbExpression> list = new List<DbExpression>();
            foreach (ExpressionLink link in this._list)
            {
                list.Add(link.Expression);
            }
            list.CopyTo(array, arrayIndex);
        }

        bool ICollection<DbExpression>.Remove(DbExpression item)
        {
            throw EntityUtil.NotSupported();
        }

        IEnumerator<DbExpression> IEnumerable<DbExpression>.GetEnumerator() => 
            new ExpressionListEnumerator(this._list.GetEnumerator());

        int IList<DbExpression>.IndexOf(DbExpression item)
        {
            for (int i = 0; i < this._list.Count; i++)
            {
                if (this._list[i].Expression == item)
                {
                    return i;
                }
            }
            return -1;
        }

        void IList<DbExpression>.Insert(int index, DbExpression item)
        {
            throw EntityUtil.NotSupported();
        }

        void IList<DbExpression>.RemoveAt(int index)
        {
            throw EntityUtil.NotSupported();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable<DbExpression>) this).GetEnumerator();

        internal List<ExpressionLink> ExpressionLinks =>
            this._list;

        int ICollection<DbExpression>.Count =>
            this._list.Count;

        bool ICollection<DbExpression>.IsReadOnly =>
            true;

        DbExpression IList<DbExpression>.this[int index]
        {
            get => 
                this._list[index].Expression;
            set
            {
                throw EntityUtil.NotSupported();
            }
        }

        private class ExpressionListEnumerator : IEnumerator<DbExpression>, IDisposable, IEnumerator
        {
            private IEnumerator<ExpressionLink> m_Enumerator;

            internal ExpressionListEnumerator(IEnumerator<ExpressionLink> innerEnum)
            {
                this.m_Enumerator = innerEnum;
            }

            bool IEnumerator.MoveNext() => 
                this.m_Enumerator.MoveNext();

            void IEnumerator.Reset()
            {
                this.m_Enumerator.Reset();
            }

            void IDisposable.Dispose()
            {
                this.m_Enumerator.Dispose();
            }

            DbExpression IEnumerator<DbExpression>.Current =>
                this.m_Enumerator.Current.Expression;

            object IEnumerator.Current =>
                this.m_Enumerator.Current.Expression;
        }
    }
}

