﻿namespace System.IO.Packaging
{
    using System;

    public abstract class PackageProperties : IDisposable
    {
        protected PackageProperties()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public abstract string Category { get; set; }

        public abstract string ContentStatus { get; set; }

        public abstract string ContentType { get; set; }

        public abstract DateTime? Created { get; set; }

        public abstract string Creator { get; set; }

        public abstract string Description { get; set; }

        public abstract string Identifier { get; set; }

        public abstract string Keywords { get; set; }

        public abstract string Language { get; set; }

        public abstract string LastModifiedBy { get; set; }

        public abstract DateTime? LastPrinted { get; set; }

        public abstract DateTime? Modified { get; set; }

        public abstract string Revision { get; set; }

        public abstract string Subject { get; set; }

        public abstract string Title { get; set; }

        public abstract string Version { get; set; }
    }
}

