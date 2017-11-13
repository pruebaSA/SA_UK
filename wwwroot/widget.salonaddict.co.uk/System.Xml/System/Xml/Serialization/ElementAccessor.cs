﻿namespace System.Xml.Serialization
{
    using System;

    internal class ElementAccessor : Accessor
    {
        private bool isSoap;
        private bool nullable;
        private bool unbounded;

        internal ElementAccessor Clone() => 
            new ElementAccessor { 
                nullable = this.nullable,
                IsTopLevelInSchema = base.IsTopLevelInSchema,
                Form = base.Form,
                isSoap = this.isSoap,
                Name = this.Name,
                Default = base.Default,
                Namespace = base.Namespace,
                Mapping = base.Mapping,
                Any = base.Any
            };

        internal bool IsNullable
        {
            get => 
                this.nullable;
            set
            {
                this.nullable = value;
            }
        }

        internal bool IsSoap
        {
            get => 
                this.isSoap;
            set
            {
                this.isSoap = value;
            }
        }

        internal bool IsUnbounded
        {
            get => 
                this.unbounded;
            set
            {
                this.unbounded = value;
            }
        }
    }
}

