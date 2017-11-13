﻿namespace System.Web.Services.Description
{
    using System;
    using System.Xml.Serialization;

    internal class MimeXmlReturn : MimeReturn
    {
        private XmlTypeMapping mapping;

        internal XmlTypeMapping TypeMapping
        {
            get => 
                this.mapping;
            set
            {
                this.mapping = value;
            }
        }
    }
}

