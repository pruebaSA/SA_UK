namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl.XsltOld.Debugger;

    internal class ActionFrame : IStackFrame
    {
        private Action action;
        private PrefixQName calulatedName;
        private ActionFrame container;
        private int counter;
        private int currentAction;
        private XPathNodeIterator newNodeSet;
        private XPathNodeIterator nodeSet;
        private int state;
        private string storedOutput;
        private object[] variables;
        private Hashtable withParams;

        internal void AllocateVariables(int count)
        {
            if (0 < count)
            {
                this.variables = new object[count];
            }
            else
            {
                this.variables = null;
            }
        }

        internal bool Execute(Processor processor)
        {
            if (this.action == null)
            {
                return true;
            }
            this.action.Execute(processor, this);
            if (this.State != -1)
            {
                return false;
            }
            if (this.container != null)
            {
                this.currentAction++;
                this.action = this.container.GetAction(this.currentAction);
                this.State = 0;
            }
            else
            {
                this.action = null;
            }
            return (this.action == null);
        }

        internal void Exit()
        {
            this.Finished();
            this.container = null;
        }

        internal void Finished()
        {
            this.State = -1;
        }

        private Action GetAction(int actionIndex) => 
            ((ContainerAction) this.action).GetAction(actionIndex);

        internal object GetParameter(XmlQualifiedName name)
        {
            if (this.withParams != null)
            {
                return this.withParams[name];
            }
            return null;
        }

        internal object GetVariable(int index) => 
            this.variables[index];

        internal int IncrementCounter() => 
            ++this.counter;

        internal void Inherit(ActionFrame parent)
        {
            this.variables = parent.variables;
        }

        internal void Init(Action action, XPathNodeIterator nodeSet)
        {
            this.Init(action, null, nodeSet);
        }

        internal void Init(ActionFrame containerFrame, XPathNodeIterator nodeSet)
        {
            this.Init(containerFrame.GetAction(0), containerFrame, nodeSet);
        }

        private void Init(Action action, ActionFrame container, XPathNodeIterator nodeSet)
        {
            this.state = 0;
            this.action = action;
            this.container = container;
            this.currentAction = 0;
            this.nodeSet = nodeSet;
            this.newNodeSet = null;
        }

        internal void InitNewNodeSet(XPathNodeIterator nodeSet)
        {
            this.newNodeSet = nodeSet;
        }

        internal void InitNodeSet(XPathNodeIterator nodeSet)
        {
            this.nodeSet = nodeSet;
        }

        internal bool NewNextNode(Processor proc)
        {
            bool flag = this.newNodeSet.MoveNext();
            if ((flag && proc.Stylesheet.Whitespace) && (this.newNodeSet.Current.NodeType == XPathNodeType.Whitespace))
            {
                XPathNodeType nodeType;
                bool flag2;
                XPathNavigator node = this.newNodeSet.Current.Clone();
                do
                {
                    node.MoveTo(this.newNodeSet.Current);
                    node.MoveToParent();
                    flag2 = !proc.Stylesheet.PreserveWhiteSpace(proc, node) && (flag = this.newNodeSet.MoveNext());
                    nodeType = this.newNodeSet.Current.NodeType;
                }
                while (flag2 && (nodeType == XPathNodeType.Whitespace));
            }
            return flag;
        }

        internal bool NextNode(Processor proc)
        {
            bool flag = this.nodeSet.MoveNext();
            if ((flag && proc.Stylesheet.Whitespace) && (this.nodeSet.Current.NodeType == XPathNodeType.Whitespace))
            {
                XPathNodeType nodeType;
                bool flag2;
                XPathNavigator node = this.nodeSet.Current.Clone();
                do
                {
                    node.MoveTo(this.nodeSet.Current);
                    node.MoveToParent();
                    flag2 = !proc.Stylesheet.PreserveWhiteSpace(proc, node) && (flag = this.nodeSet.MoveNext());
                    nodeType = this.nodeSet.Current.NodeType;
                }
                while (flag2 && (nodeType == XPathNodeType.Whitespace));
            }
            return flag;
        }

        internal void ResetParams()
        {
            if (this.withParams != null)
            {
                this.withParams.Clear();
            }
        }

        internal void SetAction(Action action)
        {
            this.SetAction(action, 0);
        }

        internal void SetAction(Action action, int state)
        {
            this.action = action;
            this.state = state;
        }

        internal void SetParameter(XmlQualifiedName name, object value)
        {
            if (this.withParams == null)
            {
                this.withParams = new Hashtable();
            }
            this.withParams[name] = value;
        }

        internal void SetVariable(int index, object value)
        {
            this.variables[index] = value;
        }

        internal void SortNewNodeSet(Processor proc, ArrayList sortarray)
        {
            int count = sortarray.Count;
            XPathSortComparer comparer = new XPathSortComparer(count);
            for (int i = 0; i < count; i++)
            {
                Sort sort = (Sort) sortarray[i];
                Query compiledQuery = proc.GetCompiledQuery(sort.select);
                comparer.AddSort(compiledQuery, new XPathComparerHelper(sort.order, sort.caseOrder, sort.lang, sort.dataType));
            }
            List<SortKey> list = new List<SortKey>();
            while (this.NewNextNode(proc))
            {
                XPathNodeIterator nodeSet = this.nodeSet;
                this.nodeSet = this.newNodeSet;
                SortKey item = new SortKey(count, list.Count, this.newNodeSet.Current.Clone());
                for (int j = 0; j < count; j++)
                {
                    item[j] = comparer.Expression(j).Evaluate(this.newNodeSet);
                }
                list.Add(item);
                this.nodeSet = nodeSet;
            }
            list.Sort(comparer);
            this.newNodeSet = new XPathSortArrayIterator(list);
        }

        XPathNavigator IStackFrame.GetVariable(int varIndex) => 
            this.action.GetDbgData(this).Variables[varIndex].GetDbgData(null).StyleSheet;

        int IStackFrame.GetVariablesCount() => 
            this.action?.GetDbgData(this).Variables.Length;

        object IStackFrame.GetVariableValue(int varIndex) => 
            this.GetVariable(this.action.GetDbgData(this).Variables[varIndex].VarKey);

        internal PrefixQName CalulatedName
        {
            get => 
                this.calulatedName;
            set
            {
                this.calulatedName = value;
            }
        }

        internal ActionFrame Container =>
            this.container;

        internal int Counter
        {
            get => 
                this.counter;
            set
            {
                this.counter = value;
            }
        }

        internal XPathNodeIterator NewNodeSet =>
            this.newNodeSet;

        internal XPathNavigator Node
        {
            get
            {
                if (this.nodeSet != null)
                {
                    return this.nodeSet.Current;
                }
                return null;
            }
        }

        internal XPathNodeIterator NodeSet =>
            this.nodeSet;

        internal int State
        {
            get => 
                this.state;
            set
            {
                this.state = value;
            }
        }

        internal string StoredOutput
        {
            get => 
                this.storedOutput;
            set
            {
                this.storedOutput = value;
            }
        }

        XPathNavigator IStackFrame.Instruction =>
            this.action?.GetDbgData(this).StyleSheet;

        XPathNodeIterator IStackFrame.NodeSet =>
            this.nodeSet.Clone();

        private class XPathSortArrayIterator : XPathArrayIterator
        {
            public XPathSortArrayIterator(List<SortKey> list) : base(list)
            {
            }

            public XPathSortArrayIterator(ActionFrame.XPathSortArrayIterator it) : base((XPathArrayIterator) it)
            {
            }

            public override XPathNodeIterator Clone() => 
                new ActionFrame.XPathSortArrayIterator(this);

            public override XPathNavigator Current =>
                ((SortKey) base.list[base.index - 1]).Node;
        }
    }
}

