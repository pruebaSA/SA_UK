namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Runtime;

    internal class RootAction : TemplateBaseAction
    {
        private Hashtable attributeSetTable = new Hashtable();
        public Stylesheet builtInSheet;
        private Hashtable decimalFormatTable = new Hashtable();
        private List<Key> keyList;
        private XsltOutput output;
        public PermissionSet permissions;
        private const int QueryInitialized = 2;
        private const int RootProcessed = 3;

        internal void AddDecimalFormat(XmlQualifiedName name, DecimalFormat formatinfo)
        {
            DecimalFormat format = (DecimalFormat) this.decimalFormatTable[name];
            if (format != null)
            {
                NumberFormatInfo info = format.info;
                NumberFormatInfo info2 = formatinfo.info;
                if ((((info.NumberDecimalSeparator != info2.NumberDecimalSeparator) || (info.NumberGroupSeparator != info2.NumberGroupSeparator)) || ((info.PositiveInfinitySymbol != info2.PositiveInfinitySymbol) || (info.NegativeSign != info2.NegativeSign))) || ((((info.NaNSymbol != info2.NaNSymbol) || (info.PercentSymbol != info2.PercentSymbol)) || ((info.PerMilleSymbol != info2.PerMilleSymbol) || (format.zeroDigit != formatinfo.zeroDigit))) || ((format.digit != formatinfo.digit) || (format.patternSeparator != formatinfo.patternSeparator))))
                {
                    throw XsltException.Create("Xslt_DupDecimalFormat", new string[] { name.ToString() });
                }
            }
            this.decimalFormatTable[name] = formatinfo;
        }

        private void CheckAttributeSets_RecurceInContainer(Hashtable markTable, ContainerAction container)
        {
            if (container.containedActions != null)
            {
                foreach (Action action in container.containedActions)
                {
                    if (action is UseAttributeSetsAction)
                    {
                        this.CheckAttributeSets_RecurceInList(markTable, ((UseAttributeSetsAction) action).UsedSets);
                    }
                    else if (action is ContainerAction)
                    {
                        this.CheckAttributeSets_RecurceInContainer(markTable, (ContainerAction) action);
                    }
                }
            }
        }

        private void CheckAttributeSets_RecurceInList(Hashtable markTable, ICollection setQNames)
        {
            foreach (XmlQualifiedName name in setQNames)
            {
                object obj2 = markTable[name];
                if (obj2 == "P")
                {
                    throw XsltException.Create("Xslt_CircularAttributeSet", new string[] { name.ToString() });
                }
                if (obj2 != "D")
                {
                    markTable[name] = "P";
                    this.CheckAttributeSets_RecurceInContainer(markTable, this.GetAttributeSet(name));
                    markTable[name] = "D";
                }
            }
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileDocument(compiler, false);
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                {
                    frame.AllocateVariables(base.variableCount);
                    XPathNavigator nav = processor.Document.Clone();
                    nav.MoveToRoot();
                    frame.InitNodeSet(new XPathSingletonIterator(nav));
                    if ((base.containedActions != null) && (base.containedActions.Count > 0))
                    {
                        processor.PushActionFrame(frame);
                    }
                    frame.State = 2;
                    return;
                }
                case 1:
                    break;

                case 2:
                    frame.NextNode(processor);
                    if (processor.Debugger != null)
                    {
                        processor.PopDebuggerStack();
                    }
                    processor.PushTemplateLookup(frame.NodeSet, null, null);
                    frame.State = 3;
                    return;

                case 3:
                    frame.Finished();
                    break;

                default:
                    return;
            }
        }

        internal AttributeSetAction GetAttributeSet(XmlQualifiedName name)
        {
            AttributeSetAction action = (AttributeSetAction) this.attributeSetTable[name];
            if (action == null)
            {
                throw XsltException.Create("Xslt_NoAttributeSet", new string[] { name.ToString() });
            }
            return action;
        }

        internal DecimalFormat GetDecimalFormat(XmlQualifiedName name) => 
            (this.decimalFormatTable[name] as DecimalFormat);

        internal void InsertKey(XmlQualifiedName name, int MatchKey, int UseKey)
        {
            if (this.keyList == null)
            {
                this.keyList = new List<Key>();
            }
            this.keyList.Add(new Key(name, MatchKey, UseKey));
        }

        private void MirgeAttributeSets(Stylesheet stylesheet)
        {
            if (stylesheet.AttributeSetTable != null)
            {
                foreach (AttributeSetAction action in stylesheet.AttributeSetTable.Values)
                {
                    ArrayList containedActions = action.containedActions;
                    AttributeSetAction action2 = (AttributeSetAction) this.attributeSetTable[action.Name];
                    if (action2 == null)
                    {
                        action2 = new AttributeSetAction {
                            name = action.Name,
                            containedActions = new ArrayList()
                        };
                        this.attributeSetTable[action.Name] = action2;
                    }
                    ArrayList list2 = action2.containedActions;
                    if (containedActions != null)
                    {
                        for (int i = containedActions.Count - 1; 0 <= i; i--)
                        {
                            list2.Add(containedActions[i]);
                        }
                    }
                }
            }
            foreach (Stylesheet stylesheet2 in stylesheet.Imports)
            {
                this.MirgeAttributeSets(stylesheet2);
            }
        }

        public void PorcessAttributeSets(Stylesheet rootStylesheet)
        {
            this.MirgeAttributeSets(rootStylesheet);
            foreach (AttributeSetAction action in this.attributeSetTable.Values)
            {
                if (action.containedActions != null)
                {
                    action.containedActions.Reverse();
                }
            }
            this.CheckAttributeSets_RecurceInList(new Hashtable(), this.attributeSetTable.Keys);
        }

        internal List<Key> KeyList =>
            this.keyList;

        internal XsltOutput Output
        {
            get
            {
                if (this.output == null)
                {
                    this.output = new XsltOutput();
                }
                return this.output;
            }
        }
    }
}

