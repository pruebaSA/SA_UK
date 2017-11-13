﻿namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;

    public class DataServiceRequestArgs
    {
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>(EqualityComparer<string>.Default);

        private string GetHeaderValue(string header)
        {
            string str;
            if (!this.headers.TryGetValue(header, out str))
            {
                return null;
            }
            return str;
        }

        private void SetHeaderValue(string header, string value)
        {
            if (value == null)
            {
                if (this.headers.ContainsKey(header))
                {
                    this.headers.Remove(header);
                }
            }
            else
            {
                this.headers[header] = value;
            }
        }

        public string AcceptContentType
        {
            get => 
                this.GetHeaderValue("Accept");
            set
            {
                this.SetHeaderValue("Accept", value);
            }
        }

        public string ContentType
        {
            get => 
                this.GetHeaderValue("Content-Type");
            set
            {
                this.SetHeaderValue("Content-Type", value);
            }
        }

        public Dictionary<string, string> Headers =>
            this.headers;

        public string Slug
        {
            get => 
                this.GetHeaderValue("Slug");
            set
            {
                this.SetHeaderValue("Slug", value);
            }
        }
    }
}

