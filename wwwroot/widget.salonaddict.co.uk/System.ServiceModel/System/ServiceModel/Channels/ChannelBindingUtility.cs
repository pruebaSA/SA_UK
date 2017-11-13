namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Authentication.ExtendedProtection.Configuration;
    using System.ServiceModel;

    internal static class ChannelBindingUtility
    {
        private static ExtendedProtectionPolicy defaultPolicy = disabledPolicy;
        private static ExtendedProtectionPolicy disabledPolicy = new ExtendedProtectionPolicy(PolicyEnforcement.Never);
        private static object lockObject = new object();
        private static bool? osSupportsExtendedProtection;

        public static ExtendedProtectionPolicy BuildPolicy(ExtendedProtectionPolicyElement configurationPolicy)
        {
            if (configurationPolicy.ElementInformation.IsPresent)
            {
                return configurationPolicy.BuildPolicy();
            }
            return DefaultPolicy;
        }

        public static void CopyFrom(ExtendedProtectionPolicyElement source, ExtendedProtectionPolicyElement destination)
        {
            destination.PolicyEnforcement = source.PolicyEnforcement;
            destination.ProtectionScenario = source.ProtectionScenario;
            destination.CustomServiceNames.Clear();
            foreach (ServiceNameElement element in source.CustomServiceNames)
            {
                ServiceNameElement element2 = new ServiceNameElement {
                    Name = element.Name
                };
                destination.CustomServiceNames.Add(element2);
            }
        }

        public static ChannelBinding DuplicateToken(ChannelBinding source)
        {
            if (source == null)
            {
                return null;
            }
            return DuplicatedChannelBinding.CreateCopy(source);
        }

        public static ChannelBinding GetToken(SslStream stream) => 
            GetToken(stream.TransportContext);

        public static ChannelBinding GetToken(TransportContext context)
        {
            ChannelBinding channelBinding = null;
            if (context != null)
            {
                channelBinding = context.GetChannelBinding(ChannelBindingKind.Endpoint);
            }
            return channelBinding;
        }

        public static void InitializeFrom(ExtendedProtectionPolicy source, ExtendedProtectionPolicyElement destination)
        {
            if (!IsDefaultPolicy(source))
            {
                destination.PolicyEnforcement = source.PolicyEnforcement;
                destination.ProtectionScenario = source.ProtectionScenario;
                destination.CustomServiceNames.Clear();
                if (source.CustomServiceNames != null)
                {
                    foreach (string str in source.CustomServiceNames)
                    {
                        ServiceNameElement element = new ServiceNameElement {
                            Name = str
                        };
                        destination.CustomServiceNames.Add(element);
                    }
                }
            }
        }

        public static bool IsDefaultPolicy(ExtendedProtectionPolicy policy) => 
            object.ReferenceEquals(policy, DefaultPolicy);

        public static bool IsSubset(ServiceNameCollection primaryList, ServiceNameCollection subset)
        {
            if ((subset == null) || (subset.Count == 0))
            {
                return true;
            }
            if ((primaryList == null) || (primaryList.Count < subset.Count))
            {
                return false;
            }
            return (primaryList.Merge(subset).Count == primaryList.Count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool OSSupportsExtendedProtectionInternal()
        {
            try
            {
                HttpListener listener = new HttpListener {
                    ExtendedProtectionPolicy = new ExtendedProtectionPolicy(PolicyEnforcement.Always)
                };
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        public static void TryAddToMessage(ChannelBinding channelBindingToken, Message message, bool messagePropertyOwnsCleanup)
        {
            if (channelBindingToken != null)
            {
                ChannelBindingMessageProperty property = new ChannelBindingMessageProperty(channelBindingToken, messagePropertyOwnsCleanup);
                property.AddTo(message);
                property.Dispose();
            }
        }

        public static bool ValidatePolicies(ExtendedProtectionPolicy policy1, ExtendedProtectionPolicy policy2, bool throwOnMismatch)
        {
            if ((policy1.PolicyEnforcement == PolicyEnforcement.Never) && (policy2.PolicyEnforcement == PolicyEnforcement.Never))
            {
                return true;
            }
            if (policy1.PolicyEnforcement != policy2.PolicyEnforcement)
            {
                if (throwOnMismatch)
                {
                    string message = System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionPoliciesMustMatch2", new object[] { System.ServiceModel.SR.GetString("ExtendedProtectionPolicyEnforcementMismatch", new object[] { policy1.PolicyEnforcement, policy2.PolicyEnforcement }) });
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(message));
                }
                return false;
            }
            if (policy1.ProtectionScenario != policy2.ProtectionScenario)
            {
                if (throwOnMismatch)
                {
                    string str2 = System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionPoliciesMustMatch2", new object[] { System.ServiceModel.SR.GetString("ExtendedProtectionPolicyScenarioMismatch", new object[] { policy1.ProtectionScenario, policy2.ProtectionScenario }) });
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(str2));
                }
                return false;
            }
            if (policy1.CustomChannelBinding == policy2.CustomChannelBinding)
            {
                return true;
            }
            if (throwOnMismatch)
            {
                string str3 = System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionPoliciesMustMatch2", new object[] { System.ServiceModel.SR.GetString("ExtendedProtectionPolicyCustomChannelBindingMismatch") });
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(str3));
            }
            return false;
        }

        public static ExtendedProtectionPolicy DefaultPolicy =>
            defaultPolicy;

        public static ExtendedProtectionPolicy DisabledPolicy =>
            disabledPolicy;

        public static bool OSSupportsExtendedProtection
        {
            get
            {
                if (!osSupportsExtendedProtection.HasValue)
                {
                    lock (lockObject)
                    {
                        if (!osSupportsExtendedProtection.HasValue)
                        {
                            osSupportsExtendedProtection = new bool?(OSSupportsExtendedProtectionInternal());
                        }
                    }
                }
                return osSupportsExtendedProtection.Value;
            }
        }

        private class DuplicatedChannelBinding : ChannelBinding
        {
            [SecurityCritical]
            private int size;

            private DuplicatedChannelBinding()
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            private void AllocateMemory(int bytesToAllocate)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    base.SetHandle(Marshal.AllocHGlobal(bytesToAllocate));
                }
            }

            [SecurityCritical, SecurityTreatAsSafe]
            internal static ChannelBinding CreateCopy(ChannelBinding source)
            {
                if (source.IsInvalid || source.IsClosed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(source.GetType().FullName));
                }
                if (source.Size <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("source.Size", source.Size, System.ServiceModel.SR.GetString("ValueMustBePositive")));
                }
                ChannelBindingUtility.DuplicatedChannelBinding binding = new ChannelBindingUtility.DuplicatedChannelBinding();
                binding.Initialize(source);
                return binding;
            }

            [SecurityTreatAsSafe, SecurityCritical]
            private unsafe void Initialize(ChannelBinding source)
            {
                this.AllocateMemory(source.Size);
                byte* numPtr = (byte*) source.DangerousGetHandle().ToPointer();
                byte* numPtr2 = (byte*) this.handle.ToPointer();
                for (int i = 0; i < source.Size; i++)
                {
                    numPtr2[i] = numPtr[i];
                }
                this.size = source.Size;
            }

            [SecurityCritical, SecurityTreatAsSafe]
            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(base.handle);
                base.SetHandle(IntPtr.Zero);
                this.size = 0;
                return true;
            }

            public override int Size =>
                this.size;
        }
    }
}

