namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Web;

    [ComVisible(false), SuppressUnmanagedCodeSecurity]
    internal sealed class UnsafeIISMethods
    {
        private const string _IIS_NATIVE_DLL = "webengine.dll";
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private UnsafeIISMethods()
        {
        }

        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern void MgdAbortConnection(IntPtr pHandler);
        [DllImport("webengine.dll")]
        internal static extern IntPtr MgdAllocateRequestMemory(IntPtr pHandler, int cbSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdAppDomainShutdown(IntPtr appContext);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern bool MgdCanDisposeManagedContext(IntPtr pRequestContext, [MarshalAs(UnmanagedType.U4)] RequestNotificationStatus dwStatus);
        [DllImport("webengine.dll")]
        internal static extern int MgdClearResponse(IntPtr pRequestContext, bool fClearEntity, bool fClearHeaders);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern void MgdCloseConnection(IntPtr pHandler);
        [DllImport("webengine.dll")]
        internal static extern void MgdDisableKernelCache(IntPtr pHandler);
        [DllImport("webengine.dll")]
        internal static extern void MgdDisableNotifications(IntPtr pRequestContext, [MarshalAs(UnmanagedType.U4)] RequestNotification notifications, [MarshalAs(UnmanagedType.U4)] RequestNotification postNotifications);
        [DllImport("webengine.dll")]
        internal static extern void MgdDisableUserCache(IntPtr pHandler);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdEmitSimpleTrace(IntPtr pRequestContext, int type, string eventData);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdEmitWebEventTrace(IntPtr pRequestContext, int webEventType, int fieldCount, string[] fieldNames, int[] fieldTypes, string[] fieldData);
        [DllImport("webengine.dll")]
        internal static extern int MgdEtwGetTraceConfig(IntPtr pRequestContext, out bool providerEnabled, out int flags, out int level);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdExecuteUrl(IntPtr context, string url, bool resetQuerystring, bool preserveForm, byte[] entityBody, uint entityBodySize, string method, int numHeaders, string[] headersNames, string[] headersValues);
        [DllImport("webengine.dll")]
        internal static extern int MgdExplicitFlush(IntPtr context);
        [DllImport("webengine.dll")]
        internal static extern int MgdFlushCore(IntPtr pRequestContext, bool keepConnected, int numBodyFragments, IntPtr[] bodyFragments, int[] bodyFragmentLengths, int[] fragmentsNative);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdFlushKernelCache(string cacheKey);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetAnonymousUserToken(IntPtr pHandler, out IntPtr pToken);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetAppCollection(string siteName, string virtualPath, out IntPtr bstrPath, out int cchPath, out IntPtr pAppCollection, out int count);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetApplicationInfo(IntPtr pHandler, out IntPtr pVirtualPath, out int cchVirtualPath, out IntPtr pPhysPath, out int cchPhysPath);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetAppPathForPath([MarshalAs(UnmanagedType.U4)] uint siteId, string virtualPath, out IntPtr bstrPath, out int cchPath);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern IntPtr MgdGetBuffer(IntPtr pPool);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern IntPtr MgdGetBufferPool(int cbBufferSize);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetChannelBindingToken(IntPtr pHandler, out IntPtr ppbToken, out int pcbTokenSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetClientCertificate(IntPtr pHandler, out IntPtr ppbClientCert, out int pcbClientCert, out IntPtr ppbClientCertIssuer, out int pcbClientCertIssuer, out IntPtr ppbClientCertPublicKey, out int pcbClientCertPublicKey, out uint pdwCertEncodingType, out long ftNotBefore, out long ftNotAfter);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetCookieHeader(IntPtr pRequestContext, out IntPtr pBuffer, out int cbBufferSize);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetCurrentModuleIndex(IntPtr pRequestContext);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetCurrentModuleName(IntPtr pHandler, out IntPtr pBuffer, out int cbBufferSize);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetCurrentNotification(IntPtr pRequestContext);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetFileChunkInfo(IntPtr context, int chunkOffset, out long offset, out long length);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetHandlerTypeString(IntPtr pHandler, out IntPtr ppszTypeString, out int pcchTypeString);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetHeaderChanges(IntPtr pRequestContext, bool fResponse, out IntPtr knownHeaderSnapshot, out int unknownHeaderSnapshotCount, out IntPtr unknownHeaderSnapshotNames, out IntPtr unknownHeaderSnapshotValues, out IntPtr diffKnownIndicies, out int diffUnknownCount, out IntPtr diffUnknownIndicies);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetLocalPort(IntPtr context);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetMemoryLimitKB(out long limit);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetMethod(IntPtr pRequestContext, out IntPtr pBuffer, out int cbBufferSize);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetModuleCollection(IntPtr appContext, out IntPtr pModuleCollection, out int count);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetNextModule(IntPtr pModuleCollection, ref uint dwIndex, out IntPtr bstrModuleName, out int cchModuleName, out IntPtr bstrModuleType, out int cchModuleType, out IntPtr bstrModulePrecondition, out int cchModulePrecondition);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetNextNotification(IntPtr pRequestContext, [MarshalAs(UnmanagedType.U4)] RequestNotificationStatus dwStatus);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetNextVPath(IntPtr pAppCollection, uint dwIndex, out IntPtr bstrPath, out int cchPath);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetPreloadedContent(IntPtr pHandler, byte[] pBuffer, int lOffset, int cbLen, out int pcbReceived);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetPreloadedSize(IntPtr pHandler, out int pcbAvailable);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetPrincipal(IntPtr pHandler, out IntPtr pToken, out IntPtr ppAuthType, ref int pcchAuthType, out IntPtr ppUserName, ref int pcchUserName);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetQueryString(IntPtr pHandler, out IntPtr pBuffer, out int cchBufferSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetRemotePort(IntPtr context);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetRequestBasics(IntPtr pRequestContext, out int pContentType, out int pContentTotalLength, out IntPtr pPathTranslated, out int pcchPathTranslated);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetRequestTraceGuid(IntPtr pRequestContext, out Guid traceContextId);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetResponseChunks(IntPtr pRequestContext, ref int fragmentCount, IntPtr[] bodyFragments, int[] bodyFragmentLengths, int[] fragmentChunkType);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetServerVarChanges(IntPtr pRequestContext, out int count, out IntPtr names, out IntPtr values, out int diffCount, out IntPtr diffIndicies);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetServerVariableA(IntPtr pHandler, string pszVarName, out IntPtr ppBuffer, out int pcchBufferSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetServerVariableW(IntPtr pHandler, string pszVarName, out IntPtr ppBuffer, out int pcchBufferSize);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetSiteNameFromId([MarshalAs(UnmanagedType.U4)] uint siteId, out IntPtr bstrSiteName, out int cchSiteName);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetStatusChanges(IntPtr pRequestContext, out ushort statusCode, out ushort subStatusCode, out IntPtr pBuffer, out ushort cbBufferSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetUriPath(IntPtr pHandler, out IntPtr ppPath, out int pcchPath, bool fIncludePathInfo, bool fUseParentContext);
        [DllImport("webengine.dll")]
        internal static extern int MgdGetUserAgent(IntPtr pRequestContext, out IntPtr pBuffer, out int cbBufferSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetUserToken(IntPtr pHandler, out IntPtr pToken);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetVirtualToken(IntPtr pHandler, out IntPtr pToken);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdGetVrPathCreds(string siteName, string virtualPath, out IntPtr bstrUserName, out int cchUserName, out IntPtr bstrPassword, out int cchPassword);
        [DllImport("webengine.dll")]
        internal static extern bool MgdHasConfigChanged();
        [DllImport("webengine.dll")]
        internal static extern void MgdIndicateCompletion(IntPtr pHandler, [MarshalAs(UnmanagedType.U4)] ref RequestNotificationStatus notificationStatus);
        [DllImport("webengine.dll")]
        internal static extern int MgdInitNativeConfig();
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern bool MgdIsClientConnected(IntPtr pHandler);
        [DllImport("webengine.dll")]
        internal static extern bool MgdIsCurrentNotificationPost(IntPtr pRequestContext);
        [DllImport("webengine.dll")]
        internal static extern bool MgdIsHandlerExecutionDenied(IntPtr pHandler);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdIsInRole(IntPtr pHandler, string pszRoleName, out bool pfIsInRole);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern bool MgdIsLastNotification(IntPtr pRequestContext, [MarshalAs(UnmanagedType.U4)] RequestNotificationStatus dwStatus);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern bool MgdIsWithinApp(string siteName, string appPath, string virtualPath);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdMapHandler(IntPtr pHandler, string method, string virtualPath, out IntPtr ppszTypeString, out int pcchTypeString, bool convertNativeStaticFileModule, bool ignoreWildcardMappings);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdMapPathDirect(string siteName, string virtualPath, out IntPtr bstrPhysicalPath, out int cchPath);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdPostCompletion(IntPtr pHandler, [MarshalAs(UnmanagedType.U4)] RequestNotificationStatus notificationStatus);
        [DllImport("webengine.dll")]
        internal static extern int MgdReadChunkHandle(IntPtr context, IntPtr FileHandle, long startOffset, ref int length, IntPtr chunkEntity);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdRegisterEventSubscription(IntPtr pAppContext, string pszModuleName, [MarshalAs(UnmanagedType.U4)] RequestNotification requestNotifications, [MarshalAs(UnmanagedType.U4)] RequestNotification postRequestNotifications, string pszModuleType, string pszModulePrecondition, IntPtr moduleSpecificData, bool useHighPriority);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdReMapHandler(IntPtr pHandler, string pszVirtualPath, out IntPtr ppszTypeString, out int pcchTypeString, out bool pfHandlerExists);
        [return: MarshalAs(UnmanagedType.U4)]
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern uint MgdResolveSiteName(string siteName);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern IntPtr MgdReturnBuffer(IntPtr pBuffer);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdRewriteUrl(IntPtr pRequestContext, string pszUrl, bool fResetQueryString);
        [DllImport("webengine.dll")]
        internal static extern void MgdSetBadRequestStatus(IntPtr pHandler);
        [DllImport("webengine.dll")]
        internal static extern int MgdSetKernelCachePolicy(IntPtr pHandler, int secondsToLive);
        [DllImport("webengine.dll")]
        internal static extern int MgdSetKnownHeader(IntPtr pRequestContext, bool fRequest, bool fReplace, ushort uHeaderIndex, byte[] value, ushort valueSize);
        [DllImport("webengine.dll")]
        internal static extern void MgdSetManagedHttpContext(IntPtr pHandler, IntPtr pManagedHttpContext);
        [DllImport("webengine.dll")]
        internal static extern int MgdSetNativeConfiguration(IntPtr nativeConfig);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdSetRemapHandler(IntPtr pHandler, string pszName, string ppszType);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdSetRequestPrincipal(IntPtr pRequestContext, IntPtr pManagedPrincipal, string userName, string authType, IntPtr token);
        [DllImport("webengine.dll")]
        internal static extern void MgdSetResponseFilter(IntPtr context);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdSetServerVariableW(IntPtr context, string variableName, string variableValue);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdSetStatusW(IntPtr pRequestContext, int dwStatusCode, int dwSubStatusCode, string pszReason, string pszErrorDescription, bool fTrySkipCustomErrors);
        [DllImport("webengine.dll")]
        internal static extern int MgdSetUnknownHeader(IntPtr pRequestContext, bool fRequest, bool fReplace, byte[] header, byte[] value, ushort valueSize);
        [DllImport("webengine.dll", CharSet=CharSet.Unicode)]
        internal static extern int MgdSyncReadRequest(IntPtr pHandler, byte[] pBuffer, int offset, int cbBuffer, out int pBytesRead, [MarshalAs(UnmanagedType.U4)] uint timeout);
        [DllImport("webengine.dll")]
        internal static extern void MgdTerminateNativeConfig();
    }
}

