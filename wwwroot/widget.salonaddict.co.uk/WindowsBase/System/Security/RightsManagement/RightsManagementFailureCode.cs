﻿namespace System.Security.RightsManagement
{
    using System;

    public enum RightsManagementFailureCode
    {
        Aborted = -2147168447,
        ActivationFailed = -2147168448,
        AdEntryNotFound = -2147168419,
        AlreadyInProgress = -2147168456,
        AuthenticationFailed = -2147168445,
        BadGetInfoQuery = -2147168494,
        BindAccessPrincipalNotEnabling = -2147168478,
        BindAccessUnsatisfied = -2147168477,
        BindContentNotInEndUseLicense = -2147168479,
        BindIndicatedPrincipalMissing = -2147168476,
        BindIntervalTimeViolated = -2147168465,
        BindMachineNotFoundInGroupIdentity = -2147168475,
        BindNoApplicableRevocationList = -2147168472,
        BindNoSatisfiedRightsGroup = -2147168464,
        BindPolicyViolation = -2147168485,
        BindRevocationListStale = -2147168473,
        BindRevokedIssuer = -2147168483,
        BindRevokedLicense = -2147168484,
        BindRevokedModule = -2147168480,
        BindRevokedPrincipal = -2147168482,
        BindRevokedResource = -2147168481,
        BindSpecifiedWorkMissing = -2147168463,
        BindValidityTimeViolated = -2147168488,
        BrokenCertChain = -2147168487,
        ClockRollbackDetected = -2147168491,
        CryptoOperationUnsupported = -2147168492,
        DebuggerDetected = -2147168416,
        EmailNotVerified = -2147168422,
        EnablingPrincipalFailure = -2147168496,
        EncryptionNotPermitted = -2147168508,
        EnvironmentCannotLoad = -2147168501,
        EnvironmentNotLoaded = -2147168502,
        ExpiredOfficialIssuanceLicenseTemplate = -2147168425,
        GlobalOptionAlreadySet = -2147168396,
        GroupIdentityNotSet = -2147168455,
        HidCorrupted = -2147168442,
        HidInvalid = -2147168423,
        IdMismatch = -2147168459,
        IncompatibleObjects = -2147168498,
        InfoNotInLicense = -2147168511,
        InfoNotPresent = -2147168495,
        InstallationFailed = -2147168443,
        InvalidAlgorithmType = -2147168503,
        InvalidClientLicensorCertificate = -2147168424,
        InvalidEmail = -2147168437,
        InvalidEncodingType = -2147168505,
        InvalidHandle = -2147168468,
        InvalidIssuanceLicenseTemplate = -2147168428,
        InvalidKeyLength = -2147168427,
        InvalidLicense = -2147168512,
        InvalidLicenseSignature = -2147168510,
        InvalidLockboxPath = -2147168399,
        InvalidLockboxType = -2147168400,
        InvalidNumericalValue = -2147168504,
        InvalidRegistryPath = -2147168398,
        InvalidServerResponse = -2147168441,
        InvalidTimeInfo = -2147168431,
        InvalidVersion = -2147168506,
        KeyTypeUnsupported = -2147168493,
        LibraryFail = -2147168497,
        LibraryUnsupportedPlugIn = -2147168474,
        LicenseAcquisitionFailed = -2147168460,
        LicenseBindingToWindowsIdentityFailed = -2147168429,
        ManifestPolicyViolation = -2147183860,
        MetadataNotSet = -2147168433,
        NeedsGroupIdentityActivation = -2147168450,
        NeedsMachineActivation = -2147168451,
        NoAesCryptoProvider = -2147168397,
        NoConnect = -2147168453,
        NoDistributionPointUrlFound = -2147168457,
        NoLicense = -2147168452,
        NoMoreData = -2147168461,
        NotAChain = -2147168418,
        NotSet = -2147168434,
        OutdatedModule = -2147168435,
        OutOfQuota = -2147168446,
        OwnerLicenseNotFound = -2147168395,
        QueryReportsNoResults = -2147168490,
        RecordNotFound = -2147168454,
        RequestDenied = -2147168417,
        RevocationInfoNotSet = -2147168432,
        RightNotGranted = -2147168507,
        RightNotSet = -2147168430,
        ServerError = -2147168444,
        ServerNotFound = -2147168438,
        ServiceGone = -2147168420,
        ServiceMoved = -2147168421,
        ServiceNotFound = -2147168440,
        Success = 0,
        TooManyCertificates = -2147168458,
        TooManyLoadedEnvironments = -2147168500,
        UnexpectedException = -2147168489,
        UseDefault = -2147168439,
        ValidityTimeViolation = -2147168436
    }
}
