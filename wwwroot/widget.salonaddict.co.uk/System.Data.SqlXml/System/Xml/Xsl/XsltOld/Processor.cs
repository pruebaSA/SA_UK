namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.XsltOld.Debugger;

    internal sealed class Processor : IXsltProcessor
    {
        private HWStack actionStack;
        private XsltArgumentList args;
        private RecordBuilder builder;
        private IXsltDebugger debugger;
        private HWStack debuggerStack;
        private XPathNavigator document;
        private Hashtable documentCache;
        private ExecResult execResult;
        private int ignoreLevel;
        private Key[] keyList;
        private XsltCompileContext matchesContext;
        private XmlNameTable nameTable = new System.Xml.NameTable();
        private ArrayList numberList;
        private XsltOutput output;
        public PermissionSet permissions;
        private Query[] queryList;
        private List<TheQuery> queryStore;
        private XmlResolver resolver;
        private System.Xml.Xsl.XsltOld.RootAction rootAction;
        private Hashtable scriptExtensions;
        private StringBuilder sharedStringBuilder;
        private ArrayList sortArray;
        private const int StackIncrement = 10;
        private System.Xml.Xsl.XsltOld.Stylesheet stylesheet;
        private TemplateLookupAction templateLookup = new TemplateLookupAction();
        private XsltCompileContext valueOfContext;
        private StateMachine xsm;

        public Processor(XPathNavigator doc, XsltArgumentList args, XmlResolver resolver, System.Xml.Xsl.XsltOld.Stylesheet stylesheet, List<TheQuery> queryStore, System.Xml.Xsl.XsltOld.RootAction rootAction, IXsltDebugger debugger)
        {
            this.stylesheet = stylesheet;
            this.queryStore = queryStore;
            this.rootAction = rootAction;
            this.queryList = new Query[queryStore.Count];
            for (int i = 0; i < queryStore.Count; i++)
            {
                this.queryList[i] = Query.Clone(queryStore[i].CompiledQuery.QueryTree);
            }
            this.xsm = new StateMachine();
            this.document = doc;
            this.builder = null;
            this.actionStack = new HWStack(10);
            this.output = this.rootAction.Output;
            this.permissions = this.rootAction.permissions;
            this.resolver = (resolver != null) ? resolver : new XmlNullResolver();
            this.args = (args != null) ? args : new XsltArgumentList();
            this.debugger = debugger;
            if (this.debugger != null)
            {
                this.debuggerStack = new HWStack(10, 0x3e8);
                this.templateLookup = new TemplateLookupActionDbg();
            }
            if (this.rootAction.KeyList != null)
            {
                this.keyList = new Key[this.rootAction.KeyList.Count];
                for (int j = 0; j < this.keyList.Length; j++)
                {
                    this.keyList[j] = this.rootAction.KeyList[j].Clone();
                }
            }
            this.scriptExtensions = new Hashtable(this.stylesheet.ScriptObjectTypes.Count);
            foreach (DictionaryEntry entry in this.stylesheet.ScriptObjectTypes)
            {
                string key = (string) entry.Key;
                if (this.GetExtensionObject(key) != null)
                {
                    throw XsltException.Create("Xslt_ScriptDub", new string[] { key });
                }
                this.scriptExtensions.Add(key, Activator.CreateInstance((Type) entry.Value, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null));
            }
            this.PushActionFrame(this.rootAction, null);
        }

        internal void AddSort(Sort sortinfo)
        {
            this.sortArray.Add(sortinfo);
        }

        internal bool BeginEvent(XPathNodeType nodeType, string prefix, string name, string nspace, bool empty) => 
            this.BeginEvent(nodeType, prefix, name, nspace, empty, null, true);

        internal bool BeginEvent(XPathNodeType nodeType, string prefix, string name, string nspace, bool empty, object htmlProps, bool search)
        {
            int state = this.xsm.BeginOutlook(nodeType);
            if ((this.ignoreLevel > 0) || (state == 0x10))
            {
                this.ignoreLevel++;
                return true;
            }
            switch (this.builder.BeginEvent(state, nodeType, prefix, name, nspace, empty, htmlProps, search))
            {
                case OutputResult.Continue:
                    this.xsm.Begin(nodeType);
                    return true;

                case OutputResult.Interrupt:
                    this.xsm.Begin(nodeType);
                    this.ExecutionResult = ExecResult.Interrupt;
                    return true;

                case OutputResult.Overflow:
                    this.ExecutionResult = ExecResult.Interrupt;
                    return false;

                case OutputResult.Error:
                    this.ignoreLevel++;
                    return true;

                case OutputResult.Ignore:
                    return true;
            }
            return true;
        }

        internal bool CopyBeginEvent(XPathNavigator node, bool emptyflag)
        {
            switch (node.NodeType)
            {
                case XPathNodeType.Element:
                case XPathNodeType.Attribute:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    return this.BeginEvent(node.NodeType, node.Prefix, node.LocalName, node.NamespaceURI, emptyflag);

                case XPathNodeType.Namespace:
                    return this.BeginEvent(XPathNodeType.Namespace, null, node.LocalName, node.Value, false);
            }
            return true;
        }

        internal bool CopyEndEvent(XPathNavigator node)
        {
            switch (node.NodeType)
            {
                case XPathNodeType.Element:
                case XPathNodeType.Attribute:
                case XPathNodeType.Namespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    return this.EndEvent(node.NodeType);
            }
            return true;
        }

        internal bool CopyTextEvent(XPathNavigator node)
        {
            switch (node.NodeType)
            {
                case XPathNodeType.Attribute:
                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                {
                    string text = node.Value;
                    return this.TextEvent(text);
                }
            }
            return true;
        }

        private void ElementValueWithoutWS(XPathNavigator nav, StringBuilder builder)
        {
            bool flag = this.Stylesheet.PreserveWhiteSpace(this, nav);
            if (nav.MoveToFirstChild())
            {
                do
                {
                    switch (nav.NodeType)
                    {
                        case XPathNodeType.Element:
                            this.ElementValueWithoutWS(nav, builder);
                            break;

                        case XPathNodeType.Text:
                        case XPathNodeType.SignificantWhitespace:
                            builder.Append(nav.Value);
                            break;

                        case XPathNodeType.Whitespace:
                            if (flag)
                            {
                                builder.Append(nav.Value);
                            }
                            break;
                    }
                }
                while (nav.MoveToNext());
                nav.MoveToParent();
            }
        }

        internal bool EndEvent(XPathNodeType nodeType)
        {
            if (this.ignoreLevel > 0)
            {
                this.ignoreLevel--;
                return true;
            }
            int state = this.xsm.EndOutlook(nodeType);
            switch (this.builder.EndEvent(state, nodeType))
            {
                case OutputResult.Continue:
                    this.xsm.End(nodeType);
                    return true;

                case OutputResult.Interrupt:
                    this.xsm.End(nodeType);
                    this.ExecutionResult = ExecResult.Interrupt;
                    return true;

                case OutputResult.Overflow:
                    this.ExecutionResult = ExecResult.Interrupt;
                    return false;
            }
            return true;
        }

        internal object Evaluate(ActionFrame context, int key) => 
            this.GetValueQuery(key).Evaluate(context.NodeSet);

        internal bool EvaluateBoolean(ActionFrame context, int key)
        {
            object obj2 = this.Evaluate(context, key);
            if (obj2 == null)
            {
                return false;
            }
            XPathNavigator navigator = obj2 as XPathNavigator;
            if (navigator == null)
            {
                return Convert.ToBoolean(obj2, CultureInfo.InvariantCulture);
            }
            return Convert.ToBoolean(navigator.Value, CultureInfo.InvariantCulture);
        }

        internal string EvaluateString(ActionFrame context, int key)
        {
            object obj2 = this.Evaluate(context, key);
            string str = null;
            if (obj2 != null)
            {
                str = XmlConvert.ToXPathString(obj2);
            }
            if (str == null)
            {
                str = string.Empty;
            }
            return str;
        }

        internal void Execute()
        {
            while (this.execResult == ExecResult.Continue)
            {
                ActionFrame frame = (ActionFrame) this.actionStack.Peek();
                if (frame == null)
                {
                    this.builder.TheEnd();
                    this.ExecutionResult = ExecResult.Done;
                    break;
                }
                if (frame.Execute(this))
                {
                    this.actionStack.Pop();
                }
            }
            if (this.execResult == ExecResult.Interrupt)
            {
                this.execResult = ExecResult.Continue;
            }
        }

        public void Execute(Stream stream)
        {
            RecordOutput output = null;
            switch (this.output.Method)
            {
                case XsltOutput.OutputMethod.Xml:
                case XsltOutput.OutputMethod.Html:
                case XsltOutput.OutputMethod.Other:
                case XsltOutput.OutputMethod.Unknown:
                    output = new TextOutput(this, stream);
                    break;

                case XsltOutput.OutputMethod.Text:
                    output = new TextOnlyOutput(this, stream);
                    break;
            }
            this.builder = new RecordBuilder(output, this.nameTable);
            this.Execute();
        }

        public void Execute(TextWriter writer)
        {
            RecordOutput output = null;
            switch (this.output.Method)
            {
                case XsltOutput.OutputMethod.Xml:
                case XsltOutput.OutputMethod.Html:
                case XsltOutput.OutputMethod.Other:
                case XsltOutput.OutputMethod.Unknown:
                    output = new TextOutput(this, writer);
                    break;

                case XsltOutput.OutputMethod.Text:
                    output = new TextOnlyOutput(this, writer);
                    break;
            }
            this.builder = new RecordBuilder(output, this.nameTable);
            this.Execute();
        }

        public void Execute(XmlWriter writer)
        {
            this.builder = new RecordBuilder(new WriterOutput(this, writer), this.nameTable);
            this.Execute();
        }

        internal Query GetCompiledQuery(int key)
        {
            TheQuery query = this.queryStore[key];
            query.CompiledQuery.CheckErrors();
            Query query2 = Query.Clone(this.queryList[key]);
            query2.SetXsltContext(new XsltCompileContext(query._ScopeManager, this));
            return query2;
        }

        internal object GetExtensionObject(string nsUri) => 
            this.args.GetExtensionObject(nsUri);

        internal object GetGlobalParameter(XmlQualifiedName qname)
        {
            object param = this.args.GetParam(qname.Name, qname.Namespace);
            if (param == null)
            {
                return null;
            }
            if ((((param is XPathNodeIterator) || (param is XPathNavigator)) || ((param is bool) || (param is double))) || (param is string))
            {
                return param;
            }
            if ((((param is short) || (param is ushort)) || ((param is int) || (param is uint))) || (((param is long) || (param is ulong)) || ((param is float) || (param is decimal))))
            {
                return XmlConvert.ToXPathDouble(param);
            }
            return param.ToString();
        }

        private XsltCompileContext GetMatchesContext()
        {
            if (this.matchesContext == null)
            {
                this.matchesContext = new XsltCompileContext();
            }
            return this.matchesContext;
        }

        internal XPathNavigator GetNavigator(Uri ruri)
        {
            XPathNavigator navigator = null;
            if (this.documentCache != null)
            {
                navigator = this.documentCache[ruri] as XPathNavigator;
                if (navigator != null)
                {
                    return navigator.Clone();
                }
            }
            else
            {
                this.documentCache = new Hashtable();
            }
            object obj2 = this.resolver.GetEntity(ruri, null, null);
            if (obj2 is Stream)
            {
                XmlTextReaderImpl reader = new XmlTextReaderImpl(ruri.ToString(), (Stream) obj2) {
                    XmlResolver = this.resolver
                };
                navigator = Compiler.LoadDocument(reader).CreateNavigator();
            }
            else
            {
                if (!(obj2 is XPathNavigator))
                {
                    throw XsltException.Create("Xslt_CantResolve", new string[] { ruri.ToString() });
                }
                navigator = (XPathNavigator) obj2;
            }
            this.documentCache[ruri] = navigator.Clone();
            return navigator;
        }

        internal object GetParameter(XmlQualifiedName name)
        {
            ActionFrame frame = (ActionFrame) this.actionStack[this.actionStack.Length - 3];
            return frame.GetParameter(name);
        }

        internal XmlQualifiedName GetPrevioseMode() => 
            ((DebuggerFrame) this.debuggerStack[this.debuggerStack.Length - 2]).currentMode;

        internal string GetQueryExpression(int key) => 
            this.queryStore[key].CompiledQuery.Expression;

        internal object GetScriptObject(string nsUri) => 
            this.scriptExtensions[nsUri];

        internal StringBuilder GetSharedStringBuilder()
        {
            if (this.sharedStringBuilder == null)
            {
                this.sharedStringBuilder = new StringBuilder();
            }
            else
            {
                this.sharedStringBuilder.Length = 0;
            }
            return this.sharedStringBuilder;
        }

        private XsltCompileContext GetValueOfContext()
        {
            if (this.valueOfContext == null)
            {
                this.valueOfContext = new XsltCompileContext();
            }
            return this.valueOfContext;
        }

        internal Query GetValueQuery(int key) => 
            this.GetValueQuery(key, null);

        internal Query GetValueQuery(int key, XsltCompileContext context)
        {
            TheQuery query = this.queryStore[key];
            query.CompiledQuery.CheckErrors();
            Query query2 = this.queryList[key];
            if (context == null)
            {
                context = new XsltCompileContext(query._ScopeManager, this);
            }
            else
            {
                context.Reinitialize(query._ScopeManager, this);
            }
            query2.SetXsltContext(context);
            return query2;
        }

        internal object GetVariableValue(VariableAction variable)
        {
            int varKey = variable.VarKey;
            if (!variable.IsGlobal)
            {
                return ((ActionFrame) this.actionStack.Peek()).GetVariable(varKey);
            }
            ActionFrame parent = (ActionFrame) this.actionStack[0];
            object obj2 = parent.GetVariable(varKey);
            if (obj2 == VariableAction.BeingComputedMark)
            {
                throw XsltException.Create("Xslt_CircularReference", new string[] { variable.NameStr });
            }
            if (obj2 != null)
            {
                return obj2;
            }
            int length = this.actionStack.Length;
            ActionFrame frame2 = this.PushNewFrame();
            frame2.Inherit(parent);
            frame2.Init(variable, parent.NodeSet);
            do
            {
                if (((ActionFrame) this.actionStack.Peek()).Execute(this))
                {
                    this.actionStack.Pop();
                }
            }
            while (length < this.actionStack.Length);
            return parent.GetVariable(varKey);
        }

        internal void InitSortArray()
        {
            if (this.sortArray == null)
            {
                this.sortArray = new ArrayList();
            }
            else
            {
                this.sortArray.Clear();
            }
        }

        internal static bool IsRoot(XPathNavigator navigator)
        {
            if (navigator.NodeType == XPathNodeType.Root)
            {
                return true;
            }
            if (navigator.NodeType == XPathNodeType.Element)
            {
                XPathNavigator navigator2 = navigator.Clone();
                navigator2.MoveToRoot();
                return navigator2.IsSamePosition(navigator);
            }
            return false;
        }

        internal bool Matches(XPathNavigator context, int key)
        {
            bool flag2;
            Query valueQuery = this.GetValueQuery(key, this.GetMatchesContext());
            try
            {
                flag2 = valueQuery.MatchNode(context) != null;
            }
            catch (XPathException)
            {
                throw XsltException.Create("Xslt_InvalidPattern", new string[] { this.GetQueryExpression(key) });
            }
            return flag2;
        }

        internal void OnInstructionExecute()
        {
            DebuggerFrame frame = (DebuggerFrame) this.debuggerStack.Peek();
            frame.actionFrame = (ActionFrame) this.actionStack.Peek();
            this.Debugger.OnInstructionExecute(this);
        }

        internal void PopDebuggerStack()
        {
            this.debuggerStack.Pop();
        }

        internal RecordOutput PopOutput()
        {
            RecordBuilder builder = this.builder;
            this.builder = builder.Next;
            this.xsm.State = this.builder.OutputState;
            builder.TheEnd();
            return builder.Output;
        }

        internal void PushActionFrame(ActionFrame container)
        {
            this.PushActionFrame(container, container.NodeSet);
        }

        internal void PushActionFrame(Action action, XPathNodeIterator nodeSet)
        {
            this.PushNewFrame().Init(action, nodeSet);
        }

        internal void PushActionFrame(ActionFrame container, XPathNodeIterator nodeSet)
        {
            this.PushNewFrame().Init(container, nodeSet);
        }

        internal void PushDebuggerStack()
        {
            DebuggerFrame o = (DebuggerFrame) this.debuggerStack.Push();
            if (o == null)
            {
                o = new DebuggerFrame();
                this.debuggerStack.AddToTop(o);
            }
            o.actionFrame = (ActionFrame) this.actionStack.Peek();
        }

        internal ActionFrame PushNewFrame()
        {
            ActionFrame parent = (ActionFrame) this.actionStack.Peek();
            ActionFrame o = (ActionFrame) this.actionStack.Push();
            if (o == null)
            {
                o = new ActionFrame();
                this.actionStack.AddToTop(o);
            }
            if (parent != null)
            {
                o.Inherit(parent);
            }
            return o;
        }

        internal void PushOutput(RecordOutput output)
        {
            this.builder.OutputState = this.xsm.State;
            RecordBuilder builder = this.builder;
            this.builder = new RecordBuilder(output, this.nameTable);
            this.builder.Next = builder;
            this.xsm.Reset();
        }

        internal void PushTemplateLookup(XPathNodeIterator nodeSet, XmlQualifiedName mode, System.Xml.Xsl.XsltOld.Stylesheet importsOf)
        {
            this.templateLookup.Initialize(mode, importsOf);
            this.PushActionFrame(this.templateLookup, nodeSet);
        }

        [Conditional("DEBUG")]
        private void RecycleMatchesContext()
        {
            if (this.matchesContext != null)
            {
                this.matchesContext.Recycle();
            }
        }

        [Conditional("DEBUG")]
        private void RecycleValueOfContext()
        {
            if (this.valueOfContext != null)
            {
                this.valueOfContext.Recycle();
            }
        }

        internal void ReleaseSharedStringBuilder()
        {
        }

        internal void ResetOutput()
        {
            this.builder.Reset();
        }

        internal void ResetParams()
        {
            ((ActionFrame) this.actionStack[this.actionStack.Length - 1]).ResetParams();
        }

        internal object RunQuery(ActionFrame context, int key)
        {
            object obj2 = this.GetCompiledQuery(key).Evaluate(context.NodeSet);
            XPathNodeIterator nodeIterator = obj2 as XPathNodeIterator;
            if (nodeIterator != null)
            {
                return new XPathArrayIterator(nodeIterator);
            }
            return obj2;
        }

        internal void SetCurrentMode(XmlQualifiedName mode)
        {
            ((DebuggerFrame) this.debuggerStack[this.debuggerStack.Length - 1]).currentMode = mode;
        }

        internal bool SetDefaultOutput(XsltOutput.OutputMethod method)
        {
            if (this.Output.Method != method)
            {
                this.output = this.output.CreateDerivedOutput(method);
                return true;
            }
            return false;
        }

        internal void SetParameter(XmlQualifiedName name, object value)
        {
            ((ActionFrame) this.actionStack[this.actionStack.Length - 2]).SetParameter(name, value);
        }

        internal XPathNodeIterator StartQuery(XPathNodeIterator context, int key)
        {
            Query compiledQuery = this.GetCompiledQuery(key);
            if (!(compiledQuery.Evaluate(context) is XPathNodeIterator))
            {
                throw XsltException.Create("XPath_NodeSetExpected", new string[0]);
            }
            return new XPathSelectionIterator(context.Current, compiledQuery);
        }

        public ReaderOutput StartReader()
        {
            ReaderOutput output = new ReaderOutput(this);
            this.builder = new RecordBuilder(output, this.nameTable);
            return output;
        }

        IStackFrame IXsltProcessor.GetStackFrame(int depth) => 
            ((DebuggerFrame) this.debuggerStack[depth]).actionFrame;

        internal bool TextEvent(string text) => 
            this.TextEvent(text, false);

        internal bool TextEvent(string text, bool disableOutputEscaping)
        {
            if (this.ignoreLevel <= 0)
            {
                int state = this.xsm.BeginOutlook(XPathNodeType.Text);
                switch (this.builder.TextEvent(state, text, disableOutputEscaping))
                {
                    case OutputResult.Continue:
                        this.xsm.Begin(XPathNodeType.Text);
                        return true;

                    case OutputResult.Interrupt:
                        this.xsm.Begin(XPathNodeType.Text);
                        this.ExecutionResult = ExecResult.Interrupt;
                        return true;

                    case OutputResult.Overflow:
                        this.ExecutionResult = ExecResult.Interrupt;
                        return false;

                    case OutputResult.Error:
                    case OutputResult.Ignore:
                        return true;
                }
            }
            return true;
        }

        internal string ValueOf(XPathNavigator n)
        {
            if (this.stylesheet.Whitespace && (n.NodeType == XPathNodeType.Element))
            {
                StringBuilder sharedStringBuilder = this.GetSharedStringBuilder();
                this.ElementValueWithoutWS(n, sharedStringBuilder);
                this.ReleaseSharedStringBuilder();
                return sharedStringBuilder.ToString();
            }
            return n.Value;
        }

        internal string ValueOf(ActionFrame context, int key)
        {
            Query valueQuery = this.GetValueQuery(key, this.GetValueOfContext());
            object obj2 = valueQuery.Evaluate(context.NodeSet);
            if (obj2 is XPathNodeIterator)
            {
                XPathNavigator n = valueQuery.Advance();
                return ((n != null) ? this.ValueOf(n) : string.Empty);
            }
            return XmlConvert.ToXPathString(obj2);
        }

        internal HWStack ActionStack =>
            this.actionStack;

        internal RecordBuilder Builder =>
            this.builder;

        internal bool CanContinue =>
            (this.execResult == ExecResult.Continue);

        internal XPathNavigator Current
        {
            get
            {
                ActionFrame frame = (ActionFrame) this.actionStack.Peek();
                return frame?.Node;
            }
        }

        internal IXsltDebugger Debugger =>
            this.debugger;

        internal XPathNavigator Document =>
            this.document;

        internal bool ExecutionDone =>
            (this.execResult == ExecResult.Done);

        internal ExecResult ExecutionResult
        {
            get => 
                this.execResult;
            set
            {
                this.execResult = value;
            }
        }

        internal Key[] KeyList =>
            this.keyList;

        internal XmlNameTable NameTable =>
            this.nameTable;

        internal ArrayList NumberList
        {
            get
            {
                if (this.numberList == null)
                {
                    this.numberList = new ArrayList();
                }
                return this.numberList;
            }
        }

        internal XsltOutput Output =>
            this.output;

        internal XmlResolver Resolver =>
            this.resolver;

        internal System.Xml.Xsl.XsltOld.RootAction RootAction =>
            this.rootAction;

        internal ArrayList SortArray =>
            this.sortArray;

        internal System.Xml.Xsl.XsltOld.Stylesheet Stylesheet =>
            this.stylesheet;

        int IXsltProcessor.StackDepth =>
            this.debuggerStack.Length;

        internal class DebuggerFrame
        {
            internal ActionFrame actionFrame;
            internal XmlQualifiedName currentMode;
        }

        internal enum ExecResult
        {
            Continue,
            Interrupt,
            Done
        }

        internal enum OutputResult
        {
            Continue,
            Interrupt,
            Overflow,
            Error,
            Ignore
        }
    }
}

