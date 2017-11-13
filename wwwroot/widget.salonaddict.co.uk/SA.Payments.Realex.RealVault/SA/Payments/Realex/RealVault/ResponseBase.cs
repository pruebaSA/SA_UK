namespace SA.Payments.Realex.RealVault
{
    using System;

    public abstract class ResponseBase
    {
        protected ResponseBase()
        {
        }

        public abstract bool HasErrors();
        public abstract bool IsValid(ISettingsProvider settings);
        public abstract void LoadXML(string value);
    }
}

