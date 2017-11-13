namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Module | AttributeTargets.Assembly)]
    public class CompilationRelaxationsAttribute : Attribute
    {
        private int m_relaxations;

        public CompilationRelaxationsAttribute(int relaxations)
        {
            this.m_relaxations = relaxations;
        }

        public CompilationRelaxationsAttribute(System.Runtime.CompilerServices.CompilationRelaxations relaxations)
        {
            this.m_relaxations = (int) relaxations;
        }

        public int CompilationRelaxations =>
            this.m_relaxations;
    }
}

