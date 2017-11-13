namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public abstract class BaseChannelWithProperties : BaseChannelObjectWithProperties
    {
        protected IChannelSinkBase SinksWithProperties;

        protected BaseChannelWithProperties()
        {
        }

        public override IDictionary Properties
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
            get
            {
                ArrayList dictionaries = new ArrayList {
                    this
                };
                if (this.SinksWithProperties != null)
                {
                    IServerChannelSink sinksWithProperties = this.SinksWithProperties as IServerChannelSink;
                    if (sinksWithProperties != null)
                    {
                        while (sinksWithProperties != null)
                        {
                            IDictionary properties = sinksWithProperties.Properties;
                            if (properties != null)
                            {
                                dictionaries.Add(properties);
                            }
                            sinksWithProperties = sinksWithProperties.NextChannelSink;
                        }
                    }
                    else
                    {
                        for (IClientChannelSink sink2 = (IClientChannelSink) this.SinksWithProperties; sink2 != null; sink2 = sink2.NextChannelSink)
                        {
                            IDictionary dictionary2 = sink2.Properties;
                            if (dictionary2 != null)
                            {
                                dictionaries.Add(dictionary2);
                            }
                        }
                    }
                }
                return new AggregateDictionary(dictionaries);
            }
        }
    }
}

