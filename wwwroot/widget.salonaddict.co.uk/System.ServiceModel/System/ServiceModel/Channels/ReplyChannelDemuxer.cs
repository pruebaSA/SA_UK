namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;

    internal class ReplyChannelDemuxer : DatagramChannelDemuxer<IReplyChannel, RequestContext>
    {
        public ReplyChannelDemuxer(BindingContext context) : base(context)
        {
        }

        protected override void AbortItem(RequestContext request)
        {
            TypedChannelDemuxer.AbortMessage(request.RequestMessage);
            request.Abort();
        }

        protected override IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginReceiveRequest(timeout, callback, state);

        protected override LayeredChannelListener<TChannel> CreateListener<TChannel>(ChannelDemuxerFilter filter) where TChannel: class, IChannel
        {
            if (typeof(TChannel) == typeof(IInputChannel))
            {
                SingletonChannelListener<IInputChannel, InputChannel, Message> listener = new SingletonChannelListener<IInputChannel, InputChannel, Message>(filter, this);
                listener.Acceptor = new InputChannelAcceptor(listener);
                return (LayeredChannelListener<TChannel>) listener;
            }
            if (typeof(TChannel) != typeof(IReplyChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
            SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext> channelManager = new SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext>(filter, this);
            channelManager.Acceptor = new ReplyChannelAcceptor(channelManager);
            return (LayeredChannelListener<TChannel>) channelManager;
        }

        protected override void Dispatch(IChannelListener listener)
        {
            SingletonChannelListener<IInputChannel, InputChannel, Message> listener2 = listener as SingletonChannelListener<IInputChannel, InputChannel, Message>;
            if (listener2 != null)
            {
                listener2.Dispatch();
            }
            else
            {
                SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext> listener3 = listener as SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext>;
                if (listener3 == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                listener3.Dispatch();
            }
        }

        protected override void EndpointNotFound(RequestContext request)
        {
            bool flag = true;
            try
            {
                if (base.DemuxFailureHandler != null)
                {
                    try
                    {
                        ReplyChannelDemuxFailureAsyncResult result = new ReplyChannelDemuxFailureAsyncResult(base.DemuxFailureHandler, request, DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.EndpointNotFoundCallback)), request);
                        result.Start();
                        if (!result.CompletedSynchronously)
                        {
                            flag = false;
                        }
                        else
                        {
                            ReplyChannelDemuxFailureAsyncResult.End(result);
                            flag = false;
                        }
                    }
                    catch (CommunicationException exception)
                    {
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                        }
                    }
                    catch (TimeoutException exception2)
                    {
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                        }
                    }
                    catch (ObjectDisposedException exception3)
                    {
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            DiagnosticUtility.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Information);
                        }
                    }
                    catch (Exception exception4)
                    {
                        if (DiagnosticUtility.IsFatal(exception4))
                        {
                            throw;
                        }
                        base.HandleUnknownException(exception4);
                    }
                }
            }
            finally
            {
                if (flag)
                {
                    this.AbortItem(request);
                }
            }
        }

        private void EndpointNotFoundCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                RequestContext asyncState = (RequestContext) result.AsyncState;
                bool flag = true;
                try
                {
                    ReplyChannelDemuxFailureAsyncResult.End(result);
                    flag = false;
                }
                catch (TimeoutException exception)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                    }
                }
                catch (CommunicationException exception2)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                    }
                }
                catch (ObjectDisposedException exception3)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Information);
                    }
                }
                catch (Exception exception4)
                {
                    if (DiagnosticUtility.IsFatal(exception4))
                    {
                        throw;
                    }
                    base.HandleUnknownException(exception4);
                }
                finally
                {
                    if (flag)
                    {
                        this.AbortItem(asyncState);
                    }
                }
            }
        }

        protected override RequestContext EndReceive(IAsyncResult result) => 
            base.InnerChannel.EndReceiveRequest(result);

        protected override void EnqueueAndDispatch(IChannelListener listener, Exception exception, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            SingletonChannelListener<IInputChannel, InputChannel, Message> listener2 = listener as SingletonChannelListener<IInputChannel, InputChannel, Message>;
            if (listener2 != null)
            {
                listener2.EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
            }
            else
            {
                SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext> listener3 = listener as SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext>;
                if (listener3 == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                listener3.EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
            }
        }

        protected override void EnqueueAndDispatch(IChannelListener listener, RequestContext request, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            SingletonChannelListener<IInputChannel, InputChannel, Message> listener2 = listener as SingletonChannelListener<IInputChannel, InputChannel, Message>;
            if (listener2 != null)
            {
                listener2.EnqueueAndDispatch(request.RequestMessage, dequeuedCallback, canDispatchOnThisThread);
                try
                {
                    request.Close();
                }
                catch (CommunicationException exception)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                    }
                }
                catch (TimeoutException exception2)
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                    }
                }
            }
            SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext> listener3 = listener as SingletonChannelListener<IReplyChannel, ReplyChannel, RequestContext>;
            if (listener3 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            listener3.EnqueueAndDispatch(request, dequeuedCallback, canDispatchOnThisThread);
        }

        protected override Message GetMessage(RequestContext request) => 
            request.RequestMessage;
    }
}

