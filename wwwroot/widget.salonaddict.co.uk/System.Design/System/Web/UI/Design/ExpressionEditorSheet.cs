﻿namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;

    public abstract class ExpressionEditorSheet
    {
        private IServiceProvider _serviceProvider;

        protected ExpressionEditorSheet(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public abstract string GetExpression();

        [Browsable(false)]
        public virtual bool IsValid =>
            true;

        [Browsable(false)]
        public IServiceProvider ServiceProvider =>
            this._serviceProvider;
    }
}

