namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QueryResult
    {
        private QueryProcessor processor;
        private bool result;
        internal QueryResult(QueryProcessor processor)
        {
            this.processor = processor;
            this.result = this.processor.Result;
        }

        internal QueryResult(bool result)
        {
            this.processor = null;
            this.result = result;
        }

        internal QueryProcessor Processor =>
            this.processor;
        internal bool Result =>
            this.result;
        internal MessageFilter GetSingleMatch()
        {
            Collection<MessageFilter> resultList = this.processor.ResultList;
            switch (resultList.Count)
            {
                case 0:
                    return null;

                case 1:
                    return resultList[0];
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MultipleFilterMatchesException(System.ServiceModel.SR.GetString("FilterMultipleMatches"), null, resultList));
        }
    }
}

