namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class RecordBuilder
    {
        private OutKeywords atoms;
        private int attributeCount;
        private ArrayList attributeList = new ArrayList();
        private BuilderInfo currentInfo;
        private BuilderInfo dummy = new BuilderInfo();
        private const int HaveRecord = 2;
        private BuilderInfo mainNode = new BuilderInfo();
        private int namespaceCount;
        private ArrayList namespaceList = new ArrayList();
        private XmlNameTable nameTable;
        private RecordBuilder next;
        private const int NoRecord = 0;
        private RecordOutput output;
        private int outputState;
        private bool popScope;
        private const string PrefixFormat = "xp_{0}";
        private int recordDepth;
        private int recordState;
        private const char s_Greater = '>';
        private const char s_Minus = '-';
        private const char s_Question = '?';
        private const string s_Space = " ";
        private const string s_SpaceDefault = "default";
        private const string s_SpaceGreater = " >";
        private const string s_SpaceMinus = " -";
        private const string s_SpacePreserve = "preserve";
        private OutputScopeManager scopeManager;
        private const int SomeRecord = 1;

        internal RecordBuilder(RecordOutput output, XmlNameTable nameTable)
        {
            this.output = output;
            this.nameTable = (nameTable != null) ? nameTable : new NameTable();
            this.atoms = new OutKeywords(this.nameTable);
            this.scopeManager = new OutputScopeManager(this.nameTable, this.atoms);
        }

        private void AdjustDepth(int state)
        {
            switch ((state & 0x300))
            {
                case 0x100:
                    this.recordDepth++;
                    return;

                case 0x200:
                    this.recordDepth--;
                    break;
            }
        }

        private void AnalyzeComment()
        {
            StringBuilder builder = null;
            string str = this.mainNode.Value;
            bool flag = false;
            int length = 0;
            int startIndex = 0;
            while (length < str.Length)
            {
                char ch = str[length];
                if (ch == '-')
                {
                    if (flag)
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(str, startIndex, length, 2 * str.Length);
                        }
                        else
                        {
                            builder.Append(str, startIndex, length - startIndex);
                        }
                        builder.Append(" -");
                        startIndex = length + 1;
                    }
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                length++;
            }
            if (builder != null)
            {
                if (startIndex < str.Length)
                {
                    builder.Append(str, startIndex, str.Length - startIndex);
                }
                if (flag)
                {
                    builder.Append(" ");
                }
                this.mainNode.Value = builder.ToString();
            }
            else if (flag)
            {
                this.mainNode.ValueAppend(" ", false);
            }
        }

        private void AnalyzeProcessingInstruction()
        {
            StringBuilder builder = null;
            string str = this.mainNode.Value;
            bool flag = false;
            int length = 0;
            int startIndex = 0;
            while (length < str.Length)
            {
                switch (str[length])
                {
                    case '>':
                        if (!flag)
                        {
                            goto Label_0071;
                        }
                        if (builder != null)
                        {
                            break;
                        }
                        builder = new StringBuilder(str, startIndex, length, 2 * str.Length);
                        goto Label_0060;

                    case '?':
                        flag = true;
                        goto Label_0077;

                    default:
                        flag = false;
                        goto Label_0077;
                }
                builder.Append(str, startIndex, length - startIndex);
            Label_0060:
                builder.Append(" >");
                startIndex = length + 1;
            Label_0071:
                flag = false;
            Label_0077:
                length++;
            }
            if (builder != null)
            {
                if (startIndex < str.Length)
                {
                    builder.Append(str, startIndex, str.Length - startIndex);
                }
                this.mainNode.Value = builder.ToString();
            }
        }

        private void AnalyzeSpaceLang()
        {
            for (int i = 0; i < this.attributeCount; i++)
            {
                BuilderInfo info = (BuilderInfo) this.attributeList[i];
                if (Keywords.Equals(info.Prefix, this.atoms.Xml))
                {
                    OutputScope currentElementScope = this.scopeManager.CurrentElementScope;
                    if (Keywords.Equals(info.LocalName, this.atoms.Lang))
                    {
                        currentElementScope.Lang = info.Value;
                    }
                    else if (Keywords.Equals(info.LocalName, this.atoms.Space))
                    {
                        currentElementScope.Space = TranslateXmlSpace(info.Value);
                    }
                }
            }
        }

        private void AppendNamespaces()
        {
            for (int i = this.namespaceCount - 1; i >= 0; i--)
            {
                ((BuilderInfo) this.attributeList[this.NewAttribute()]).Initialize((BuilderInfo) this.namespaceList[i]);
            }
        }

        private void BeginAttribute(string prefix, string name, string nspace, object htmlAttrProps, bool search)
        {
            int num = this.FindAttribute(name, nspace, ref prefix);
            if (num == -1)
            {
                num = this.NewAttribute();
            }
            BuilderInfo info = (BuilderInfo) this.attributeList[num];
            info.Initialize(prefix, name, nspace);
            info.Depth = this.recordDepth;
            info.NodeType = XmlNodeType.Attribute;
            info.htmlAttrProps = htmlAttrProps as HtmlAttributeProps;
            info.search = search;
            this.currentInfo = info;
        }

        private void BeginComment()
        {
            this.currentInfo.NodeType = XmlNodeType.Comment;
            this.currentInfo.Depth = this.recordDepth;
        }

        private void BeginElement(string prefix, string name, string nspace, bool empty)
        {
            this.currentInfo.NodeType = XmlNodeType.Element;
            this.currentInfo.Prefix = prefix;
            this.currentInfo.LocalName = name;
            this.currentInfo.NamespaceURI = nspace;
            this.currentInfo.Depth = this.recordDepth;
            this.currentInfo.IsEmptyTag = empty;
            this.scopeManager.PushScope(name, nspace, prefix);
        }

        internal Processor.OutputResult BeginEvent(int state, XPathNodeType nodeType, string prefix, string name, string nspace, bool empty, object htmlProps, bool search)
        {
            if (!this.CanOutput(state))
            {
                return Processor.OutputResult.Overflow;
            }
            this.AdjustDepth(state);
            this.ResetRecord(state);
            this.PopElementScope();
            prefix = (prefix != null) ? this.nameTable.Add(prefix) : this.atoms.Empty;
            name = (name != null) ? this.nameTable.Add(name) : this.atoms.Empty;
            nspace = (nspace != null) ? this.nameTable.Add(nspace) : this.atoms.Empty;
            switch (nodeType)
            {
                case XPathNodeType.Element:
                    this.mainNode.htmlProps = htmlProps as HtmlElementProps;
                    this.mainNode.search = search;
                    this.BeginElement(prefix, name, nspace, empty);
                    break;

                case XPathNodeType.Attribute:
                    this.BeginAttribute(prefix, name, nspace, htmlProps, search);
                    break;

                case XPathNodeType.Namespace:
                    this.BeginNamespace(name, nspace);
                    break;

                case XPathNodeType.ProcessingInstruction:
                    if (this.BeginProcessingInstruction(prefix, name, nspace))
                    {
                        break;
                    }
                    return Processor.OutputResult.Error;

                case XPathNodeType.Comment:
                    this.BeginComment();
                    break;
            }
            return this.CheckRecordBegin(state);
        }

        private void BeginNamespace(string name, string nspace)
        {
            bool thisScope = false;
            if (Keywords.Equals(name, this.atoms.Empty))
            {
                if (!Keywords.Equals(nspace, this.scopeManager.DefaultNamespace) && !Keywords.Equals(this.mainNode.NamespaceURI, this.atoms.Empty))
                {
                    this.DeclareNamespace(nspace, name);
                }
            }
            else
            {
                string strB = this.scopeManager.ResolveNamespace(name, out thisScope);
                if (strB != null)
                {
                    if (!Keywords.Equals(nspace, strB) && !thisScope)
                    {
                        this.DeclareNamespace(nspace, name);
                    }
                }
                else
                {
                    this.DeclareNamespace(nspace, name);
                }
            }
            this.currentInfo = this.dummy;
            this.currentInfo.NodeType = XmlNodeType.Attribute;
        }

        private bool BeginProcessingInstruction(string prefix, string name, string nspace)
        {
            this.currentInfo.NodeType = XmlNodeType.ProcessingInstruction;
            this.currentInfo.Prefix = prefix;
            this.currentInfo.LocalName = name;
            this.currentInfo.NamespaceURI = nspace;
            this.currentInfo.Depth = this.recordDepth;
            return true;
        }

        private bool CanOutput(int state)
        {
            if ((this.recordState == 0) || ((state & 0x2000) == 0))
            {
                return true;
            }
            this.recordState = 2;
            this.FinalizeRecord();
            this.SetEmptyFlag(state);
            return (this.output.RecordDone(this) == Processor.OutputResult.Continue);
        }

        private Processor.OutputResult CheckRecordBegin(int state)
        {
            if ((state & 0x4000) != 0)
            {
                this.recordState = 2;
                this.FinalizeRecord();
                this.SetEmptyFlag(state);
                return this.output.RecordDone(this);
            }
            this.recordState = 1;
            return Processor.OutputResult.Continue;
        }

        private Processor.OutputResult CheckRecordEnd(int state)
        {
            if ((state & 0x4000) != 0)
            {
                this.recordState = 2;
                this.FinalizeRecord();
                this.SetEmptyFlag(state);
                return this.output.RecordDone(this);
            }
            return Processor.OutputResult.Continue;
        }

        private void DeclareNamespace(string nspace, string prefix)
        {
            int num = this.NewNamespace();
            BuilderInfo info = (BuilderInfo) this.namespaceList[num];
            if (prefix == this.atoms.Empty)
            {
                info.Initialize(this.atoms.Empty, this.atoms.Xmlns, this.atoms.XmlnsNamespace);
            }
            else
            {
                info.Initialize(this.atoms.Xmlns, prefix, this.atoms.XmlnsNamespace);
            }
            info.Depth = this.recordDepth;
            info.NodeType = XmlNodeType.Attribute;
            info.Value = nspace;
            this.scopeManager.PushNamespace(prefix, nspace);
        }

        private string DeclareNewNamespace(string nspace)
        {
            string prefix = this.scopeManager.GeneratePrefix("xp_{0}");
            this.DeclareNamespace(nspace, prefix);
            return prefix;
        }

        private void EndElement()
        {
            OutputScope currentElementScope = this.scopeManager.CurrentElementScope;
            this.currentInfo.NodeType = XmlNodeType.EndElement;
            this.currentInfo.Prefix = currentElementScope.Prefix;
            this.currentInfo.LocalName = currentElementScope.Name;
            this.currentInfo.NamespaceURI = currentElementScope.Namespace;
            this.currentInfo.Depth = this.recordDepth;
        }

        internal Processor.OutputResult EndEvent(int state, XPathNodeType nodeType)
        {
            if (!this.CanOutput(state))
            {
                return Processor.OutputResult.Overflow;
            }
            this.AdjustDepth(state);
            this.PopElementScope();
            this.popScope = (state & 0x10000) != 0;
            if (((state & 0x1000) != 0) && this.mainNode.IsEmptyTag)
            {
                return Processor.OutputResult.Continue;
            }
            this.ResetRecord(state);
            if (((state & 0x2000) != 0) && (nodeType == XPathNodeType.Element))
            {
                this.EndElement();
            }
            return this.CheckRecordEnd(state);
        }

        private void FinalizeRecord()
        {
            switch (this.mainNode.NodeType)
            {
                case XmlNodeType.ProcessingInstruction:
                    this.AnalyzeProcessingInstruction();
                    return;

                case XmlNodeType.Comment:
                    this.AnalyzeComment();
                    return;

                case XmlNodeType.Element:
                {
                    int attributeCount = this.attributeCount;
                    this.FixupElement();
                    this.FixupAttributes(attributeCount);
                    this.AnalyzeSpaceLang();
                    this.AppendNamespaces();
                    return;
                }
            }
        }

        private int FindAttribute(string name, string nspace, ref string prefix)
        {
            for (int i = 0; i < this.attributeCount; i++)
            {
                BuilderInfo info = (BuilderInfo) this.attributeList[i];
                if (Keywords.Equals(info.LocalName, name))
                {
                    if (Keywords.Equals(info.NamespaceURI, nspace))
                    {
                        return i;
                    }
                    if (Keywords.Equals(info.Prefix, prefix))
                    {
                        prefix = string.Empty;
                    }
                }
            }
            return -1;
        }

        private void FixupAttributes(int attributeCount)
        {
            for (int i = 0; i < attributeCount; i++)
            {
                BuilderInfo info = (BuilderInfo) this.attributeList[i];
                if (Keywords.Equals(info.NamespaceURI, this.atoms.Empty))
                {
                    info.Prefix = this.atoms.Empty;
                }
                else if (Keywords.Equals(info.Prefix, this.atoms.Empty))
                {
                    info.Prefix = this.GetPrefixForNamespace(info.NamespaceURI);
                }
                else
                {
                    bool thisScope = false;
                    string strB = this.scopeManager.ResolveNamespace(info.Prefix, out thisScope);
                    if (strB != null)
                    {
                        if (!Keywords.Equals(info.NamespaceURI, strB))
                        {
                            if (thisScope)
                            {
                                info.Prefix = this.GetPrefixForNamespace(info.NamespaceURI);
                            }
                            else
                            {
                                this.DeclareNamespace(info.NamespaceURI, info.Prefix);
                            }
                        }
                    }
                    else
                    {
                        this.DeclareNamespace(info.NamespaceURI, info.Prefix);
                    }
                }
            }
        }

        private void FixupElement()
        {
            if (Keywords.Equals(this.mainNode.NamespaceURI, this.atoms.Empty))
            {
                this.mainNode.Prefix = this.atoms.Empty;
            }
            if (Keywords.Equals(this.mainNode.Prefix, this.atoms.Empty))
            {
                if (!Keywords.Equals(this.mainNode.NamespaceURI, this.scopeManager.DefaultNamespace))
                {
                    this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
                }
            }
            else
            {
                bool thisScope = false;
                string strB = this.scopeManager.ResolveNamespace(this.mainNode.Prefix, out thisScope);
                if (strB != null)
                {
                    if (!Keywords.Equals(this.mainNode.NamespaceURI, strB))
                    {
                        if (thisScope)
                        {
                            this.mainNode.Prefix = this.GetPrefixForNamespace(this.mainNode.NamespaceURI);
                        }
                        else
                        {
                            this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
                        }
                    }
                }
                else
                {
                    this.DeclareNamespace(this.mainNode.NamespaceURI, this.mainNode.Prefix);
                }
            }
            this.scopeManager.CurrentElementScope.Prefix = this.mainNode.Prefix;
        }

        internal string GetPrefixForNamespace(string nspace)
        {
            string prefix = null;
            if (this.scopeManager.FindPrefix(nspace, out prefix))
            {
                return prefix;
            }
            return this.DeclareNewNamespace(nspace);
        }

        private int NewAttribute()
        {
            if (this.attributeCount >= this.attributeList.Count)
            {
                this.attributeList.Add(new BuilderInfo());
            }
            return this.attributeCount++;
        }

        private int NewNamespace()
        {
            if (this.namespaceCount >= this.namespaceList.Count)
            {
                this.namespaceList.Add(new BuilderInfo());
            }
            return this.namespaceCount++;
        }

        private void PopElementScope()
        {
            if (this.popScope)
            {
                this.scopeManager.PopScope();
                this.popScope = false;
            }
        }

        internal void Reset()
        {
            if (this.recordState == 2)
            {
                this.recordState = 0;
            }
        }

        private void ResetRecord(int state)
        {
            if ((state & 0x2000) != 0)
            {
                this.attributeCount = 0;
                this.namespaceCount = 0;
                this.currentInfo = this.mainNode;
                this.currentInfo.Initialize(this.atoms.Empty, this.atoms.Empty, this.atoms.Empty);
                this.currentInfo.NodeType = XmlNodeType.None;
                this.currentInfo.IsEmptyTag = false;
                this.currentInfo.htmlProps = null;
                this.currentInfo.htmlAttrProps = null;
            }
        }

        private void SetEmptyFlag(int state)
        {
            if ((state & 0x400) != 0)
            {
                this.mainNode.IsEmptyTag = false;
            }
        }

        internal Processor.OutputResult TextEvent(int state, string text, bool disableOutputEscaping)
        {
            if (!this.CanOutput(state))
            {
                return Processor.OutputResult.Overflow;
            }
            this.AdjustDepth(state);
            this.ResetRecord(state);
            this.PopElementScope();
            if ((state & 0x2000) != 0)
            {
                this.currentInfo.Depth = this.recordDepth;
                this.currentInfo.NodeType = XmlNodeType.Text;
            }
            this.ValueAppend(text, disableOutputEscaping);
            return this.CheckRecordBegin(state);
        }

        internal void TheEnd()
        {
            if (this.recordState == 1)
            {
                this.recordState = 2;
                this.FinalizeRecord();
                this.output.RecordDone(this);
            }
            this.output.TheEnd();
        }

        private static XmlSpace TranslateXmlSpace(string space)
        {
            if (Keywords.Compare(space, "default"))
            {
                return XmlSpace.Default;
            }
            if (Keywords.Compare(space, "preserve"))
            {
                return XmlSpace.Preserve;
            }
            return XmlSpace.None;
        }

        private void ValueAppend(string s, bool disableOutputEscaping)
        {
            this.currentInfo.ValueAppend(s, disableOutputEscaping);
        }

        internal int AttributeCount =>
            this.attributeCount;

        internal ArrayList AttributeList =>
            this.attributeList;

        internal BuilderInfo MainNode =>
            this.mainNode;

        internal OutputScopeManager Manager =>
            this.scopeManager;

        internal RecordBuilder Next
        {
            get => 
                this.next;
            set
            {
                this.next = value;
            }
        }

        internal RecordOutput Output =>
            this.output;

        internal int OutputState
        {
            get => 
                this.outputState;
            set
            {
                this.outputState = value;
            }
        }
    }
}

