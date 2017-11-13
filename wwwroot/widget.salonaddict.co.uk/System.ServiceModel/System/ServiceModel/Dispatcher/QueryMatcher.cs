namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal abstract class QueryMatcher
    {
        private static IFunctionLibrary[] defaultFunctionLibs = new IFunctionLibrary[] { new XPathFunctionLibrary() };
        private static XPathNavigator fxCompiler;
        protected int maxNodes = 0x7fffffff;
        protected WeakReference processorPool = new WeakReference(null);
        protected Opcode query = null;
        protected int subExprVars = 0;

        static QueryMatcher()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<a/>");
            fxCompiler = document.CreateNavigator();
        }

        internal QueryMatcher()
        {
        }

        internal static OpcodeBlock CompileForExternalEngine(XPathMessageFilter filter)
        {
            XPathExpression expression = fxCompiler.Compile(filter.XPath);
            if (filter.namespaces != null)
            {
                if (filter.namespaces is XsltContext)
                {
                    XPathLexer lexer = new XPathLexer(filter.XPath, false);
                    while (lexer.MoveNext())
                    {
                        string prefix = lexer.Token.Prefix;
                        if ((prefix.Length > 0) && (filter.namespaces.LookupNamespace(prefix) == null))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XsltException(System.ServiceModel.SR.GetString("FilterUndefinedPrefix", new object[] { prefix })));
                        }
                    }
                }
                expression.SetContext(filter.namespaces);
            }
            if (XPathResultType.Error == expression.ReturnType)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XPathException(System.ServiceModel.SR.GetString("FilterCouldNotCompile", new object[] { filter.XPath })));
            }
            OpcodeBlock block = new OpcodeBlock();
            MatchSingleFxEngineOpcode opcode = new MatchSingleFxEngineOpcode {
                XPath = expression,
                Filter = filter
            };
            block.Append(opcode);
            return block;
        }

        internal static OpcodeBlock CompileForInternalEngine(XPathMessageFilter filter, QueryCompilerFlags flags, out ValueDataType returnType) => 
            CompileForInternalEngine(filter, flags, defaultFunctionLibs, out returnType);

        internal static OpcodeBlock CompileForInternalEngine(XPathMessageFilter filter, QueryCompilerFlags flags, IFunctionLibrary[] functionLibs, out ValueDataType returnType) => 
            CompileForInternalEngine(filter.XPath.Trim(), filter.namespaces, flags, functionLibs, out returnType);

        internal static OpcodeBlock CompileForInternalEngine(string xpath, XmlNamespaceManager ns, QueryCompilerFlags flags, out ValueDataType returnType) => 
            CompileForInternalEngine(xpath, ns, flags, defaultFunctionLibs, out returnType);

        internal static OpcodeBlock CompileForInternalEngine(string xpath, XmlNamespaceManager nsManager, QueryCompilerFlags flags, IFunctionLibrary[] functionLibs, out ValueDataType returnType)
        {
            returnType = ValueDataType.None;
            if (xpath.Length == 0)
            {
                OpcodeBlock block = new OpcodeBlock();
                block.Append(new PushBooleanOpcode(true));
                return block;
            }
            XPathExpr expr = new XPathParser(xpath, nsManager, functionLibs).Parse();
            if (expr == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QueryCompileException(QueryCompileError.CouldNotParseExpression));
            }
            returnType = expr.ReturnType;
            XPathCompiler compiler = new XPathCompiler(flags);
            return compiler.Compile(expr);
        }

        internal SeekableXPathNavigator CreateMessageNavigator(Message message, bool matchBody)
        {
            SeekableXPathNavigator navigator = message.GetNavigator(matchBody, this.maxNodes);
            navigator.MoveToRoot();
            return navigator;
        }

        internal QueryProcessor CreateProcessor()
        {
            QueryProcessor processor = null;
            lock (this.processorPool)
            {
                QueryProcessorPool target = this.processorPool.Target as QueryProcessorPool;
                if (target != null)
                {
                    processor = target.Pop();
                }
            }
            if (processor != null)
            {
                processor.ClearProcessor();
                return processor;
            }
            return new QueryProcessor(this);
        }

        internal SeekableXPathNavigator CreateSafeNavigator(SeekableXPathNavigator navigator)
        {
            INodeCounter counter = navigator as INodeCounter;
            if (counter != null)
            {
                counter.CounterMarker = this.maxNodes;
                counter.MaxCounter = this.maxNodes;
                return navigator;
            }
            navigator = new SafeSeekableNavigator(navigator, this.maxNodes);
            return navigator;
        }

        internal SeekableXPathNavigator CreateSeekableNavigator(XPathNavigator navigator) => 
            new GenericSeekableNavigator(navigator);

        internal QueryResult Match(MessageBuffer messageBuffer, ICollection<MessageFilter> matches)
        {
            QueryResult result;
            Message message = messageBuffer.CreateMessage();
            try
            {
                result = this.Match(message, true, matches);
            }
            finally
            {
                message.Close();
            }
            return result;
        }

        internal QueryResult Match(SeekableXPathNavigator navigator, ICollection<MessageFilter> matches)
        {
            if (this.maxNodes < 0x7fffffff)
            {
                navigator = this.CreateSafeNavigator(navigator);
            }
            QueryProcessor processor = this.CreateProcessor();
            processor.ResultSet = matches;
            processor.EnsureFilterCollection();
            try
            {
                processor.Eval(this.query, navigator);
            }
            catch (XPathNavigatorException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Process(this.query));
            }
            catch (NavigatorInvalidBodyAccessException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception2.Process(this.query));
            }
            return new QueryResult(processor);
        }

        internal QueryResult Match(XPathNavigator navigator, ICollection<MessageFilter> matches)
        {
            SeekableXPathNavigator navigator2 = this.CreateSeekableNavigator(navigator);
            return this.Match(navigator2, matches);
        }

        internal QueryResult Match(Message message, bool matchBody, ICollection<MessageFilter> matches)
        {
            QueryProcessor processor = this.CreateProcessor();
            processor.ResultSet = matches;
            processor.EnsureFilterCollection();
            try
            {
                processor.Eval(this.query, message, matchBody);
            }
            catch (XPathNavigatorException exception)
            {
                throw TraceUtility.ThrowHelperError(exception.Process(this.query), message);
            }
            catch (NavigatorInvalidBodyAccessException exception2)
            {
                throw TraceUtility.ThrowHelperError(exception2.Process(this.query), message);
            }
            return new QueryResult(processor);
        }

        internal void ReleaseProcessor(QueryProcessor processor)
        {
            lock (this.processorPool)
            {
                QueryProcessorPool target = this.processorPool.Target as QueryProcessorPool;
                if (target == null)
                {
                    target = new QueryProcessorPool();
                    this.processorPool.Target = target;
                }
                target.Push(processor);
            }
        }

        internal void ReleaseResult(QueryResult result)
        {
            if (result.Processor != null)
            {
                result.Processor.ResultSet = null;
                this.ReleaseProcessor(result.Processor);
            }
        }

        internal virtual void Trim()
        {
            if (this.query != null)
            {
                this.query.Trim();
            }
        }

        internal bool IsCompiled =>
            (null != this.query);

        internal int NodeQuota
        {
            get => 
                this.maxNodes;
            set
            {
                this.maxNodes = value;
            }
        }

        internal int SubExprVarCount =>
            this.subExprVars;

        internal class QueryProcessorPool
        {
            private QueryProcessor processor;

            internal QueryProcessorPool()
            {
            }

            internal QueryProcessor Pop()
            {
                QueryProcessor processor = this.processor;
                if (processor != null)
                {
                    this.processor = (QueryProcessor) processor.next;
                    processor.next = null;
                    return processor;
                }
                return null;
            }

            internal void Push(QueryProcessor p)
            {
                p.next = this.processor;
                this.processor = p;
            }
        }
    }
}

