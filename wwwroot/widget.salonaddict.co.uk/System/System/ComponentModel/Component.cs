namespace System.ComponentModel
{
    using System;
    using System.Runtime.InteropServices;

    [DesignerCategory("Component"), ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class Component : MarshalByRefObject, IComponent, IDisposable
    {
        private static readonly object EventDisposed = new object();
        private EventHandlerList events;
        private ISite site;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Disposed
        {
            add
            {
                this.Events.AddHandler(EventDisposed, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventDisposed, value);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    if ((this.site != null) && (this.site.Container != null))
                    {
                        this.site.Container.Remove(this);
                    }
                    if (this.events != null)
                    {
                        EventHandler handler = (EventHandler) this.events[EventDisposed];
                        if (handler != null)
                        {
                            handler(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        ~Component()
        {
            this.Dispose(false);
        }

        protected virtual object GetService(Type service)
        {
            ISite site = this.site;
            if (site != null)
            {
                return site.GetService(service);
            }
            return null;
        }

        public override string ToString()
        {
            ISite site = this.site;
            if (site != null)
            {
                return (site.Name + " [" + base.GetType().FullName + "]");
            }
            return base.GetType().FullName;
        }

        protected virtual bool CanRaiseEvents =>
            true;

        internal bool CanRaiseEventsInternal =>
            this.CanRaiseEvents;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IContainer Container
        {
            get
            {
                ISite site = this.site;
                if (site != null)
                {
                    return site.Container;
                }
                return null;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected bool DesignMode
        {
            get
            {
                ISite site = this.site;
                return ((site != null) && site.DesignMode);
            }
        }

        protected EventHandlerList Events
        {
            get
            {
                if (this.events == null)
                {
                    this.events = new EventHandlerList(this);
                }
                return this.events;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual ISite Site
        {
            get => 
                this.site;
            set
            {
                this.site = value;
            }
        }
    }
}

