namespace System.ServiceModel.Web
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class OutgoingWebResponseContext
    {
        private OperationContext operationContext;

        internal OutgoingWebResponseContext(OperationContext operationContext)
        {
            this.operationContext = operationContext;
        }

        public void SetStatusAsCreated(Uri locationUri)
        {
            if (locationUri == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("locationUri");
            }
            this.StatusCode = HttpStatusCode.Created;
            this.Location = locationUri.ToString();
        }

        public void SetStatusAsNotFound()
        {
            this.StatusCode = HttpStatusCode.NotFound;
        }

        public void SetStatusAsNotFound(string description)
        {
            this.StatusCode = HttpStatusCode.NotFound;
            this.StatusDescription = description;
        }

        public long ContentLength
        {
            get => 
                long.Parse(this.MessageProperty.Headers[HttpResponseHeader.ContentLength], CultureInfo.InvariantCulture);
            set
            {
                this.MessageProperty.Headers[HttpResponseHeader.ContentLength] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string ContentType
        {
            get => 
                this.MessageProperty.Headers[HttpResponseHeader.ContentType];
            set
            {
                this.MessageProperty.Headers[HttpResponseHeader.ContentType] = value;
            }
        }

        public string ETag
        {
            get => 
                this.MessageProperty.Headers[HttpResponseHeader.ETag];
            set
            {
                this.MessageProperty.Headers[HttpResponseHeader.ETag] = value;
            }
        }

        public WebHeaderCollection Headers =>
            this.MessageProperty.Headers;

        public DateTime LastModified
        {
            get => 
                DateTime.Parse(this.MessageProperty.Headers[HttpResponseHeader.LastModified], CultureInfo.InvariantCulture);
            set
            {
                this.MessageProperty.Headers[HttpResponseHeader.LastModified] = (value.Kind == DateTimeKind.Utc) ? value.ToString("R", CultureInfo.InvariantCulture) : value.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
            }
        }

        public string Location
        {
            get => 
                this.MessageProperty.Headers[HttpResponseHeader.Location];
            set
            {
                this.MessageProperty.Headers[HttpResponseHeader.Location] = value;
            }
        }

        private HttpResponseMessageProperty MessageProperty
        {
            get
            {
                if (!this.operationContext.OutgoingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
                {
                    this.operationContext.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, new HttpResponseMessageProperty());
                }
                return (this.operationContext.OutgoingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty);
            }
        }

        public HttpStatusCode StatusCode
        {
            get => 
                this.MessageProperty.StatusCode;
            set
            {
                this.MessageProperty.StatusCode = value;
            }
        }

        public string StatusDescription
        {
            get => 
                this.MessageProperty.StatusDescription;
            set
            {
                this.MessageProperty.StatusDescription = value;
            }
        }

        public bool SuppressEntityBody
        {
            get => 
                this.MessageProperty.SuppressEntityBody;
            set
            {
                this.MessageProperty.SuppressEntityBody = value;
            }
        }
    }
}

