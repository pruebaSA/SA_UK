namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml.XPath;

    internal class XPathFilterMatcher : QueryMatcher
    {
        private XPathFilterFlags flags = XPathFilterFlags.None;
        private static PushBooleanOpcode matchAlwaysFilter;
        private static OpcodeBlock rootFilter;

        static XPathFilterMatcher()
        {
            ValueDataType type;
            matchAlwaysFilter = new PushBooleanOpcode(true);
            rootFilter = QueryMatcher.CompileForInternalEngine("/", null, QueryCompilerFlags.None, out type);
            rootFilter.Append(new MatchQueryResultOpcode());
        }

        internal XPathFilterMatcher()
        {
        }

        internal void Compile(XPathMessageFilter filter)
        {
            if (base.query == null)
            {
                try
                {
                    this.CompileForInternal(filter);
                }
                catch (QueryCompileException)
                {
                }
                if (base.query == null)
                {
                    this.CompileForExternal(filter);
                }
            }
        }

        internal void CompileForExternal(XPathMessageFilter filter)
        {
            MatchSingleFxEngineOpcode first = (MatchSingleFxEngineOpcode) QueryMatcher.CompileForExternalEngine(filter).First;
            base.query = first;
            this.flags |= XPathFilterFlags.IsFxFilter;
        }

        internal void CompileForInternal(XPathMessageFilter filter)
        {
            base.query = null;
            string str = filter.XPath.Trim();
            if (str.Length == 0)
            {
                base.query = matchAlwaysFilter;
                this.flags |= XPathFilterFlags.AlwaysMatch;
            }
            else if ((1 == str.Length) && ('/' == str[0]))
            {
                base.query = rootFilter.First;
                this.flags |= XPathFilterFlags.AlwaysMatch;
            }
            else
            {
                ValueDataType type;
                OpcodeBlock block = QueryMatcher.CompileForInternalEngine(filter, QueryCompilerFlags.None, out type);
                block.Append(new MatchQueryResultOpcode());
                base.query = block.First;
            }
            this.flags &= ~XPathFilterFlags.IsFxFilter;
        }

        internal QueryResult Match(MessageBuffer messageBuffer)
        {
            QueryResult result;
            Message message = messageBuffer.CreateMessage();
            try
            {
                result = this.Match(message, true);
            }
            finally
            {
                message.Close();
            }
            return result;
        }

        internal QueryResult Match(SeekableXPathNavigator navigator)
        {
            if (this.IsAlwaysMatch)
            {
                return new QueryResult(true);
            }
            if (this.IsFxFilter)
            {
                return new QueryResult(this.MatchFx(navigator));
            }
            return base.Match(navigator, null);
        }

        internal QueryResult Match(XPathNavigator navigator)
        {
            if (this.IsAlwaysMatch)
            {
                return new QueryResult(true);
            }
            if (this.IsFxFilter)
            {
                return new QueryResult(this.MatchFx(navigator));
            }
            return base.Match(navigator, null);
        }

        internal QueryResult Match(Message message, bool matchBody)
        {
            if (this.IsAlwaysMatch)
            {
                return new QueryResult(true);
            }
            return base.Match(message, matchBody, null);
        }

        internal bool MatchFx(XPathNavigator navigator)
        {
            bool flag;
            INodeCounter counter = navigator as INodeCounter;
            if (counter == null)
            {
                navigator = new SafeSeekableNavigator(new GenericSeekableNavigator(navigator), base.NodeQuota);
            }
            else
            {
                counter.CounterMarker = base.NodeQuota;
                counter.MaxCounter = base.NodeQuota;
            }
            try
            {
                flag = ((MatchSingleFxEngineOpcode) base.query).Match(navigator);
            }
            catch (XPathNavigatorException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Process(base.query));
            }
            catch (NavigatorInvalidBodyAccessException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception2.Process(base.query));
            }
            return flag;
        }

        internal bool IsAlwaysMatch =>
            (XPathFilterFlags.None != (this.flags & XPathFilterFlags.AlwaysMatch));

        internal bool IsFxFilter =>
            (XPathFilterFlags.None != (this.flags & XPathFilterFlags.IsFxFilter));
    }
}

