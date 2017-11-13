namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI.WebControls;

    public class AccordionCommandEventArgs : CommandEventArgs
    {
        private AccordionContentPanel _container;

        internal AccordionCommandEventArgs(AccordionContentPanel container, string commandName, object commandArg) : base(commandName, commandArg)
        {
            this._container = container;
        }

        public AccordionContentPanel Container =>
            this._container;
    }
}

