namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AsyncPostBackTrigger : UpdatePanelControlTrigger
    {
        private Control _associatedControl;
        private bool _eventHandled;
        private static MethodInfo _eventHandler;
        private string _eventName;
        private IScriptManagerInternal _scriptManager;

        public AsyncPostBackTrigger()
        {
        }

        internal AsyncPostBackTrigger(IScriptManagerInternal scriptManager)
        {
            this._scriptManager = scriptManager;
        }

        protected internal override bool HasTriggered()
        {
            if (!string.IsNullOrEmpty(this.EventName))
            {
                return this._eventHandled;
            }
            string asyncPostBackSourceElementID = this.ScriptManager.AsyncPostBackSourceElementID;
            if (asyncPostBackSourceElementID != this._associatedControl.UniqueID)
            {
                return asyncPostBackSourceElementID.StartsWith(this._associatedControl.UniqueID + "$", StringComparison.Ordinal);
            }
            return true;
        }

        protected internal override void Initialize()
        {
            base.Initialize();
            this._associatedControl = base.FindTargetControl(true);
            this.ScriptManager.RegisterAsyncPostBackControl(this._associatedControl);
            string eventName = this.EventName;
            if (eventName.Length != 0)
            {
                EventInfo info = this._associatedControl.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (info == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.AsyncPostBackTrigger_CannotFindEvent, new object[] { eventName, this.ControlID, base.Owner.ID }));
                }
                MethodInfo method = info.EventHandlerType.GetMethod("Invoke");
                ParameterInfo[] parameters = method.GetParameters();
                if ((!method.ReturnType.Equals(typeof(void)) || (parameters.Length != 2)) || !typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.AsyncPostBackTrigger_InvalidEvent, new object[] { eventName, this.ControlID, base.Owner.ID }));
                }
                Delegate handler = Delegate.CreateDelegate(info.EventHandlerType, this, EventHandler);
                info.AddEventHandler(this._associatedControl, handler);
            }
        }

        public void OnEvent(object sender, EventArgs e)
        {
            this._eventHandled = true;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.ControlID))
            {
                return "AsyncPostBack";
            }
            return ("AsyncPostBack: " + this.ControlID + (string.IsNullOrEmpty(this.EventName) ? string.Empty : ("." + this.EventName)));
        }

        [TypeConverter("System.Web.UI.Design.AsyncPostBackTriggerControlIDConverter, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
        public string ControlID
        {
            get => 
                base.ControlID;
            set
            {
                base.ControlID = value;
            }
        }

        private static MethodInfo EventHandler
        {
            get
            {
                if (_eventHandler == null)
                {
                    _eventHandler = typeof(AsyncPostBackTrigger).GetMethod("OnEvent");
                }
                return _eventHandler;
            }
        }

        [DefaultValue(""), TypeConverter("System.Web.UI.Design.AsyncPostBackTriggerEventNameConverter, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), Category("Behavior"), ResourceDescription("AsyncPostBackTrigger_EventName")]
        public string EventName
        {
            get
            {
                if (this._eventName == null)
                {
                    return string.Empty;
                }
                return this._eventName;
            }
            set
            {
                this._eventName = value;
            }
        }

        internal IScriptManagerInternal ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    Page page = base.Owner.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { base.Owner.ID }));
                    }
                }
                return this._scriptManager;
            }
        }
    }
}

