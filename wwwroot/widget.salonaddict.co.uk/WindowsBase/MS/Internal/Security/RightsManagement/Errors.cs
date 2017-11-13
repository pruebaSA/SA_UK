namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.RightsManagement;
    using System.Windows;

    internal static class Errors
    {
        private static string GetLocalizedFailureCodeMessage(RightsManagementFailureCode failureCode)
        {
            string str;
            switch (failureCode)
            {
                case RightsManagementFailureCode.InvalidLicense:
                    str = "RmExceptionInvalidLicense";
                    break;

                case RightsManagementFailureCode.InfoNotInLicense:
                    str = "RmExceptionInfoNotInLicense";
                    break;

                case RightsManagementFailureCode.InvalidLicenseSignature:
                    str = "RmExceptionInvalidLicenseSignature";
                    break;

                case RightsManagementFailureCode.EncryptionNotPermitted:
                    str = "RmExceptionEncryptionNotPermitted";
                    break;

                case RightsManagementFailureCode.RightNotGranted:
                    str = "RmExceptionRightNotGranted";
                    break;

                case RightsManagementFailureCode.InvalidVersion:
                    str = "RmExceptionInvalidVersion";
                    break;

                case RightsManagementFailureCode.InvalidEncodingType:
                    str = "RmExceptionInvalidEncodingType";
                    break;

                case RightsManagementFailureCode.InvalidNumericalValue:
                    str = "RmExceptionInvalidNumericalValue";
                    break;

                case RightsManagementFailureCode.InvalidAlgorithmType:
                    str = "RmExceptionInvalidAlgorithmType";
                    break;

                case RightsManagementFailureCode.EnvironmentNotLoaded:
                    str = "RmExceptionEnvironmentNotLoaded";
                    break;

                case RightsManagementFailureCode.EnvironmentCannotLoad:
                    str = "RmExceptionEnvironmentCannotLoad";
                    break;

                case RightsManagementFailureCode.TooManyLoadedEnvironments:
                    str = "RmExceptionTooManyLoadedEnvironments";
                    break;

                case RightsManagementFailureCode.IncompatibleObjects:
                    str = "RmExceptionIncompatibleObjects";
                    break;

                case RightsManagementFailureCode.LibraryFail:
                    str = "RmExceptionLibraryFail";
                    break;

                case RightsManagementFailureCode.EnablingPrincipalFailure:
                    str = "RmExceptionEnablingPrincipalFailure";
                    break;

                case RightsManagementFailureCode.InfoNotPresent:
                    str = "RmExceptionInfoNotPresent";
                    break;

                case RightsManagementFailureCode.BadGetInfoQuery:
                    str = "RmExceptionBadGetInfoQuery";
                    break;

                case RightsManagementFailureCode.KeyTypeUnsupported:
                    str = "RmExceptionKeyTypeUnsupported";
                    break;

                case RightsManagementFailureCode.CryptoOperationUnsupported:
                    str = "RmExceptionCryptoOperationUnsupported";
                    break;

                case RightsManagementFailureCode.ClockRollbackDetected:
                    str = "RmExceptionClockRollbackDetected";
                    break;

                case RightsManagementFailureCode.QueryReportsNoResults:
                    str = "RmExceptionQueryReportsNoResults";
                    break;

                case RightsManagementFailureCode.UnexpectedException:
                    str = "RmExceptionUnexpectedException";
                    break;

                case RightsManagementFailureCode.BindValidityTimeViolated:
                    str = "RmExceptionBindValidityTimeViolated";
                    break;

                case RightsManagementFailureCode.BrokenCertChain:
                    str = "RmExceptionBrokenCertChain";
                    break;

                case RightsManagementFailureCode.BindPolicyViolation:
                    str = "RmExceptionBindPolicyViolation";
                    break;

                case RightsManagementFailureCode.BindRevokedLicense:
                    str = "RmExceptionBindRevokedLicense";
                    break;

                case RightsManagementFailureCode.BindRevokedIssuer:
                    str = "RmExceptionBindRevokedIssuer";
                    break;

                case RightsManagementFailureCode.BindRevokedPrincipal:
                    str = "RmExceptionBindRevokedPrincipal";
                    break;

                case RightsManagementFailureCode.BindRevokedResource:
                    str = "RmExceptionBindRevokedResource";
                    break;

                case RightsManagementFailureCode.BindRevokedModule:
                    str = "RmExceptionBindRevokedModule";
                    break;

                case RightsManagementFailureCode.BindContentNotInEndUseLicense:
                    str = "RmExceptionBindContentNotInEndUseLicense";
                    break;

                case RightsManagementFailureCode.BindAccessPrincipalNotEnabling:
                    str = "RmExceptionBindAccessPrincipalNotEnabling";
                    break;

                case RightsManagementFailureCode.BindAccessUnsatisfied:
                    str = "RmExceptionBindAccessUnsatisfied";
                    break;

                case RightsManagementFailureCode.BindIndicatedPrincipalMissing:
                    str = "RmExceptionBindIndicatedPrincipalMissing";
                    break;

                case RightsManagementFailureCode.BindMachineNotFoundInGroupIdentity:
                    str = "RmExceptionBindMachineNotFoundInGroupIdentity";
                    break;

                case RightsManagementFailureCode.LibraryUnsupportedPlugIn:
                    str = "RmExceptionLibraryUnsupportedPlugIn";
                    break;

                case RightsManagementFailureCode.BindRevocationListStale:
                    str = "RmExceptionBindRevocationListStale";
                    break;

                case RightsManagementFailureCode.BindNoApplicableRevocationList:
                    str = "RmExceptionBindNoApplicableRevocationList";
                    break;

                case RightsManagementFailureCode.InvalidHandle:
                    str = "RmExceptionInvalidHandle";
                    break;

                case RightsManagementFailureCode.BindIntervalTimeViolated:
                    str = "RmExceptionBindIntervalTimeViolated";
                    break;

                case RightsManagementFailureCode.BindNoSatisfiedRightsGroup:
                    str = "RmExceptionBindNoSatisfiedRightsGroup";
                    break;

                case RightsManagementFailureCode.BindSpecifiedWorkMissing:
                    str = "RmExceptionBindSpecifiedWorkMissing";
                    break;

                case RightsManagementFailureCode.NoMoreData:
                    str = "RmExceptionNoMoreData";
                    break;

                case RightsManagementFailureCode.LicenseAcquisitionFailed:
                    str = "RmExceptionLicenseAcquisitionFailed";
                    break;

                case RightsManagementFailureCode.IdMismatch:
                    str = "RmExceptionIdMismatch";
                    break;

                case RightsManagementFailureCode.TooManyCertificates:
                    str = "RmExceptionTooManyCertificates";
                    break;

                case RightsManagementFailureCode.NoDistributionPointUrlFound:
                    str = "RmExceptionNoDistributionPointUrlFound";
                    break;

                case RightsManagementFailureCode.AlreadyInProgress:
                    str = "RmExceptionAlreadyInProgress";
                    break;

                case RightsManagementFailureCode.GroupIdentityNotSet:
                    str = "RmExceptionGroupIdentityNotSet";
                    break;

                case RightsManagementFailureCode.RecordNotFound:
                    str = "RmExceptionRecordNotFound";
                    break;

                case RightsManagementFailureCode.NoConnect:
                    str = "RmExceptionNoConnect";
                    break;

                case RightsManagementFailureCode.NoLicense:
                    str = "RmExceptionNoLicense";
                    break;

                case RightsManagementFailureCode.NeedsMachineActivation:
                    str = "RmExceptionNeedsMachineActivation";
                    break;

                case RightsManagementFailureCode.NeedsGroupIdentityActivation:
                    str = "RmExceptionNeedsGroupIdentityActivation";
                    break;

                case RightsManagementFailureCode.ActivationFailed:
                    str = "RmExceptionActivationFailed";
                    break;

                case RightsManagementFailureCode.Aborted:
                    str = "RmExceptionAborted";
                    break;

                case RightsManagementFailureCode.OutOfQuota:
                    str = "RmExceptionOutOfQuota";
                    break;

                case RightsManagementFailureCode.AuthenticationFailed:
                    str = "RmExceptionAuthenticationFailed";
                    break;

                case RightsManagementFailureCode.ServerError:
                    str = "RmExceptionServerError";
                    break;

                case RightsManagementFailureCode.InstallationFailed:
                    str = "RmExceptionInstallationFailed";
                    break;

                case RightsManagementFailureCode.HidCorrupted:
                    str = "RmExceptionHidCorrupted";
                    break;

                case RightsManagementFailureCode.InvalidServerResponse:
                    str = "RmExceptionInvalidServerResponse";
                    break;

                case RightsManagementFailureCode.ServiceNotFound:
                    str = "RmExceptionServiceNotFound";
                    break;

                case RightsManagementFailureCode.UseDefault:
                    str = "RmExceptionUseDefault";
                    break;

                case RightsManagementFailureCode.ServerNotFound:
                    str = "RmExceptionServerNotFound";
                    break;

                case RightsManagementFailureCode.InvalidEmail:
                    str = "RmExceptionInvalidEmail";
                    break;

                case RightsManagementFailureCode.ValidityTimeViolation:
                    str = "RmExceptionValidityTimeViolation";
                    break;

                case RightsManagementFailureCode.OutdatedModule:
                    str = "RmExceptionOutdatedModule";
                    break;

                case RightsManagementFailureCode.NotSet:
                    str = "RmExceptionNotSet";
                    break;

                case RightsManagementFailureCode.MetadataNotSet:
                    str = "RmExceptionMetadataNotSet";
                    break;

                case RightsManagementFailureCode.RevocationInfoNotSet:
                    str = "RmExceptionRevocationInfoNotSet";
                    break;

                case RightsManagementFailureCode.InvalidTimeInfo:
                    str = "RmExceptionInvalidTimeInfo";
                    break;

                case RightsManagementFailureCode.RightNotSet:
                    str = "RmExceptionRightNotSet";
                    break;

                case RightsManagementFailureCode.LicenseBindingToWindowsIdentityFailed:
                    str = "RmExceptionLicenseBindingToWindowsIdentityFailed";
                    break;

                case RightsManagementFailureCode.InvalidIssuanceLicenseTemplate:
                    str = "RmExceptionInvalidIssuanceLicenseTemplate";
                    break;

                case RightsManagementFailureCode.InvalidKeyLength:
                    str = "RmExceptionInvalidKeyLength";
                    break;

                case RightsManagementFailureCode.ExpiredOfficialIssuanceLicenseTemplate:
                    str = "RmExceptionExpiredOfficialIssuanceLicenseTemplate";
                    break;

                case RightsManagementFailureCode.InvalidClientLicensorCertificate:
                    str = "RmExceptionInvalidClientLicensorCertificate";
                    break;

                case RightsManagementFailureCode.HidInvalid:
                    str = "RmExceptionHidInvalid";
                    break;

                case RightsManagementFailureCode.EmailNotVerified:
                    str = "RmExceptionEmailNotVerified";
                    break;

                case RightsManagementFailureCode.ServiceMoved:
                    str = "RmExceptionServiceMoved";
                    break;

                case RightsManagementFailureCode.ServiceGone:
                    str = "RmExceptionServiceGone";
                    break;

                case RightsManagementFailureCode.AdEntryNotFound:
                    str = "RmExceptionAdEntryNotFound";
                    break;

                case RightsManagementFailureCode.NotAChain:
                    str = "RmExceptionNotAChain";
                    break;

                case RightsManagementFailureCode.RequestDenied:
                    str = "RmExceptionRequestDenied";
                    break;

                case RightsManagementFailureCode.DebuggerDetected:
                    str = "RmExceptionDebuggerDetected";
                    break;

                case RightsManagementFailureCode.InvalidLockboxType:
                    str = "RmExceptionInvalidLockboxType";
                    break;

                case RightsManagementFailureCode.InvalidLockboxPath:
                    str = "RmExceptionInvalidLockboxPath";
                    break;

                case RightsManagementFailureCode.InvalidRegistryPath:
                    str = "RmExceptionInvalidRegistryPath";
                    break;

                case RightsManagementFailureCode.NoAesCryptoProvider:
                    str = "RmExceptionNoAesCryptoProvider";
                    break;

                case RightsManagementFailureCode.GlobalOptionAlreadySet:
                    str = "RmExceptionGlobalOptionAlreadySet";
                    break;

                case RightsManagementFailureCode.OwnerLicenseNotFound:
                    str = "RmExceptionOwnerLicenseNotFound";
                    break;

                case RightsManagementFailureCode.ManifestPolicyViolation:
                    str = "RmExceptionManifestPolicyViolation";
                    break;

                default:
                    return null;
            }
            return System.Windows.SR.Get(str);
        }

        internal static string GetLocalizedFailureCodeMessageWithDefault(RightsManagementFailureCode failureCode)
        {
            string localizedFailureCodeMessage = GetLocalizedFailureCodeMessage(failureCode);
            if (localizedFailureCodeMessage != null)
            {
                return localizedFailureCodeMessage;
            }
            return System.Windows.SR.Get("RmExceptionGenericMessage");
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void ThrowOnErrorCode(int hr)
        {
            if (hr < 0)
            {
                string localizedFailureCodeMessage = GetLocalizedFailureCodeMessage((RightsManagementFailureCode) hr);
                if (localizedFailureCodeMessage != null)
                {
                    throw new RightsManagementException((RightsManagementFailureCode) hr, localizedFailureCodeMessage);
                }
                try
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
                catch (Exception exception)
                {
                    throw new RightsManagementException(System.Windows.SR.Get("RmExceptionGenericMessage"), exception);
                }
            }
        }
    }
}

