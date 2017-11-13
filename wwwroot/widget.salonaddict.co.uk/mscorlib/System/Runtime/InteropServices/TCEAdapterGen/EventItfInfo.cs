﻿namespace System.Runtime.InteropServices.TCEAdapterGen
{
    using System;
    using System.Reflection;

    internal class EventItfInfo
    {
        private Assembly m_asmImport;
        private Assembly m_asmSrcItf;
        private string m_strEventItfName;
        private string m_strEventProviderName;
        private string m_strSrcItfName;

        public EventItfInfo(string strEventItfName, string strSrcItfName, string strEventProviderName, Assembly asmImport, Assembly asmSrcItf)
        {
            this.m_strEventItfName = strEventItfName;
            this.m_strSrcItfName = strSrcItfName;
            this.m_strEventProviderName = strEventProviderName;
            this.m_asmImport = asmImport;
            this.m_asmSrcItf = asmSrcItf;
        }

        public Type GetEventItfType()
        {
            Type type = this.m_asmImport.GetType(this.m_strEventItfName, true, false);
            if ((type != null) && !type.IsVisible)
            {
                type = null;
            }
            return type;
        }

        public string GetEventProviderName() => 
            this.m_strEventProviderName;

        public Type GetSrcItfType()
        {
            Type type = this.m_asmSrcItf.GetType(this.m_strSrcItfName, true, false);
            if ((type != null) && !type.IsVisible)
            {
                type = null;
            }
            return type;
        }
    }
}

