namespace System.Security
{
    using System.Reflection.Emit;

    internal class FrameSecurityDescriptorWithResolver : FrameSecurityDescriptor
    {
        private DynamicResolver m_resolver;

        public DynamicResolver Resolver =>
            this.m_resolver;
    }
}

