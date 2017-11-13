namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    public abstract class ServiceModelEnhancedConfigurationElementCollection<TConfigurationElement> : ServiceModelConfigurationElementCollection<TConfigurationElement> where TConfigurationElement: ConfigurationElement, new()
    {
        internal ServiceModelEnhancedConfigurationElementCollection(string elementName) : base(ConfigurationElementCollectionType.AddRemoveClearMap, elementName)
        {
            base.AddElementName = elementName;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            object elementKey = this.GetElementKey(element);
            if (this.ContainsKey(elementKey))
            {
                ConfigurationElement element2 = base.BaseGet(elementKey);
                if (element2 != null)
                {
                    if (element2.ElementInformation.IsPresent)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigDuplicateKeyAtSameScope", new object[] { this.ElementName, elementKey })));
                    }
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>(6) {
                            { 
                                "ElementName",
                                this.ElementName
                            },
                            { 
                                "Name",
                                elementKey.ToString()
                            },
                            { 
                                "OldElementLocation",
                                element2.ElementInformation.Source
                            },
                            { 
                                "OldElementLineNumber",
                                element2.ElementInformation.LineNumber.ToString(NumberFormatInfo.CurrentInfo)
                            },
                            { 
                                "NewElementLocation",
                                element.ElementInformation.Source
                            },
                            { 
                                "NewElementLineNumber",
                                element.ElementInformation.LineNumber.ToString(NumberFormatInfo.CurrentInfo)
                            }
                        };
                        DictionaryTraceRecord extendedData = new DictionaryTraceRecord(dictionary);
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.OverridingDuplicateConfigurationKey, extendedData, this, null);
                    }
                }
            }
            base.BaseAdd(element);
        }

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

