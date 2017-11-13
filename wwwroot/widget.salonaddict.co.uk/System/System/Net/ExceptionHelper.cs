﻿namespace System.Net
{
    using System;
    using System.Security.Permissions;

    internal static class ExceptionHelper
    {
        internal static readonly SecurityPermission ControlPolicyPermission = new SecurityPermission(SecurityPermissionFlag.ControlPolicy);
        internal static readonly SecurityPermission ControlPrincipalPermission = new SecurityPermission(SecurityPermissionFlag.ControlPrincipal);
        internal static readonly SecurityPermission InfrastructurePermission = new SecurityPermission(SecurityPermissionFlag.Infrastructure);
        internal static readonly KeyContainerPermission KeyContainerPermissionOpen = new KeyContainerPermission(KeyContainerPermissionFlags.Open);
        internal static readonly SecurityPermission UnmanagedPermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
        internal static readonly SocketPermission UnrestrictedSocketPermission = new SocketPermission(PermissionState.Unrestricted);
        internal static readonly WebPermission WebPermissionUnrestricted = new WebPermission(NetworkAccess.Connect);

        internal static UriFormatException BadAuthorityException =>
            new UriFormatException(SR.GetString("net_uri_BadAuthority"));

        internal static UriFormatException BadAuthorityTerminatorException =>
            new UriFormatException(SR.GetString("net_uri_BadAuthorityTerminator"));

        internal static UriFormatException BadFormatException =>
            new UriFormatException(SR.GetString("net_uri_BadFormat"));

        internal static UriFormatException BadHostNameException =>
            new UriFormatException(SR.GetString("net_uri_BadHostName"));

        internal static UriFormatException BadPortException =>
            new UriFormatException(SR.GetString("net_uri_BadPort"));

        internal static UriFormatException BadSchemeException =>
            new UriFormatException(SR.GetString("net_uri_BadScheme"));

        internal static WebException CacheEntryNotFoundException =>
            new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.CacheEntryNotFound), WebExceptionStatus.CacheEntryNotFound);

        internal static UriFormatException CannotCreateRelativeException =>
            new UriFormatException(SR.GetString("net_uri_CannotCreateRelative"));

        internal static UriFormatException EmptyUriException =>
            new UriFormatException(SR.GetString("net_uri_EmptyUri"));

        internal static WebException IsolatedException =>
            new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.KeepAliveFailure), WebExceptionStatus.KeepAliveFailure, WebExceptionInternalStatus.Isolated, null);

        internal static NotImplementedException MethodNotImplementedException =>
            new NotImplementedException(SR.GetString("net_MethodNotImplementedException"));

        internal static NotSupportedException MethodNotSupportedException =>
            new NotSupportedException(SR.GetString("net_MethodNotSupportedException"));

        internal static UriFormatException MustRootedPathException =>
            new UriFormatException(SR.GetString("net_uri_MustRootedPath"));

        internal static NotImplementedException PropertyNotImplementedException =>
            new NotImplementedException(SR.GetString("net_PropertyNotImplementedException"));

        internal static NotSupportedException PropertyNotSupportedException =>
            new NotSupportedException(SR.GetString("net_PropertyNotSupportedException"));

        internal static WebException RequestAbortedException =>
            new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.RequestCanceled), WebExceptionStatus.RequestCanceled);

        internal static WebException RequestProhibitedByCachePolicyException =>
            new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.RequestProhibitedByCachePolicy), WebExceptionStatus.RequestProhibitedByCachePolicy);

        internal static UriFormatException SchemeLimitException =>
            new UriFormatException(SR.GetString("net_uri_SchemeLimit"));

        internal static UriFormatException SizeLimitException =>
            new UriFormatException(SR.GetString("net_uri_SizeLimit"));
    }
}

