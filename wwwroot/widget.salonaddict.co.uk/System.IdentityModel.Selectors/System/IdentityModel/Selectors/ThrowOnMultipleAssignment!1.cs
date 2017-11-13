namespace System.IdentityModel.Selectors
{
    using Microsoft.InfoCards.Diagnostics;
    using System;

    internal class ThrowOnMultipleAssignment<T>
    {
        private string m_errorString;
        private T m_value;

        public ThrowOnMultipleAssignment(string errorString)
        {
            this.m_errorString = errorString;
        }

        public T Value
        {
            get => 
                this.m_value;
            set
            {
                if ((this.m_value != null) && (value != null))
                {
                    throw InfoCardTrace.ThrowHelperArgument(this.m_errorString);
                }
                if (this.m_value == null)
                {
                    this.m_value = value;
                }
            }
        }
    }
}

