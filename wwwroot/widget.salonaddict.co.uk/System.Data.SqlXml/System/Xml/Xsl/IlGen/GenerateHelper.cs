namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.SymbolStore;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class GenerateHelper
    {
        private ILGenerator ilgen;
        private bool initWriters;
        private bool isDebug;
        private string lastFileName;
        private ISourceLineInfo lastSourceInfo;
        private string lastUriString;
        private LocalBuilder locXOut;
        private MethodBase methInfo;
        private MethodInfo methSyncToNav;
        private XmlILModule module;
        private StaticDataManager staticData;

        public GenerateHelper(XmlILModule module, bool isDebug)
        {
            this.isDebug = isDebug;
            this.module = module;
            this.staticData = new StaticDataManager();
        }

        public void AddSortKey(XmlQueryType keyType)
        {
            MethodInfo meth = null;
            if (keyType == null)
            {
                meth = XmlILMethods.SortKeyEmpty;
            }
            else
            {
                switch (keyType.TypeCode)
                {
                    case XmlTypeCode.AnyAtomicType:
                        return;

                    case XmlTypeCode.String:
                        meth = XmlILMethods.SortKeyString;
                        break;

                    case XmlTypeCode.Boolean:
                        meth = XmlILMethods.SortKeyInt;
                        break;

                    case XmlTypeCode.Decimal:
                        meth = XmlILMethods.SortKeyDecimal;
                        break;

                    case XmlTypeCode.Double:
                        meth = XmlILMethods.SortKeyDouble;
                        break;

                    case XmlTypeCode.DateTime:
                        meth = XmlILMethods.SortKeyDateTime;
                        break;

                    case XmlTypeCode.None:
                        this.Emit(OpCodes.Pop);
                        meth = XmlILMethods.SortKeyEmpty;
                        break;

                    case XmlTypeCode.Integer:
                        meth = XmlILMethods.SortKeyInteger;
                        break;

                    case XmlTypeCode.Int:
                        meth = XmlILMethods.SortKeyInt;
                        break;
                }
            }
            this.Call(meth);
        }

        public void BranchAndMark(Label lblBranch, Label lblMark)
        {
            if (!lblBranch.Equals(lblMark))
            {
                this.EmitUnconditionalBranch(OpCodes.Br, lblBranch);
            }
            this.MarkLabel(lblMark);
        }

        public void Call(MethodInfo meth)
        {
            OpCode opcode = (meth.IsVirtual || meth.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call;
            this.ilgen.Emit(opcode, meth);
            if (this.lastSourceInfo != null)
            {
                this.MarkSequencePoint(SourceLineInfo.NoSource);
            }
        }

        public void CallArithmeticOp(QilNodeType opType, XmlTypeCode code)
        {
            MethodInfo meth = null;
            switch (code)
            {
                case XmlTypeCode.Decimal:
                    switch (opType)
                    {
                        case QilNodeType.Negate:
                            meth = XmlILMethods.DecNeg;
                            goto Label_00E3;

                        case QilNodeType.Add:
                            meth = XmlILMethods.DecAdd;
                            goto Label_00E3;

                        case QilNodeType.Subtract:
                            meth = XmlILMethods.DecSub;
                            goto Label_00E3;

                        case QilNodeType.Multiply:
                            meth = XmlILMethods.DecMul;
                            goto Label_00E3;

                        case QilNodeType.Divide:
                            meth = XmlILMethods.DecDiv;
                            goto Label_00E3;

                        case QilNodeType.Modulo:
                            meth = XmlILMethods.DecRem;
                            goto Label_00E3;
                    }
                    break;

                case XmlTypeCode.Float:
                case XmlTypeCode.Double:
                case XmlTypeCode.Integer:
                case XmlTypeCode.Int:
                    switch (opType)
                    {
                        case QilNodeType.Negate:
                            this.Emit(OpCodes.Neg);
                            return;

                        case QilNodeType.Add:
                            this.Emit(OpCodes.Add);
                            return;

                        case QilNodeType.Subtract:
                            this.Emit(OpCodes.Sub);
                            return;

                        case QilNodeType.Multiply:
                            this.Emit(OpCodes.Mul);
                            return;

                        case QilNodeType.Divide:
                            this.Emit(OpCodes.Div);
                            return;

                        case QilNodeType.Modulo:
                            this.Emit(OpCodes.Rem);
                            return;
                    }
                    return;

                default:
                    return;
            }
        Label_00E3:
            this.Call(meth);
        }

        public void CallCacheCount(Type itemStorageType)
        {
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[itemStorageType];
            this.Call(methods.IListCount);
        }

        public void CallCacheItem(Type itemStorageType)
        {
            this.Call(XmlILMethods.StorageMethods[itemStorageType].IListItem);
        }

        public void CallCompare(XmlTypeCode code)
        {
            MethodInfo meth = null;
            switch (code)
            {
                case XmlTypeCode.String:
                    meth = XmlILMethods.StrCmp;
                    break;

                case XmlTypeCode.Decimal:
                    meth = XmlILMethods.DecCmp;
                    break;
            }
            this.Call(meth);
        }

        public void CallCompareEquals(XmlTypeCode code)
        {
            MethodInfo meth = null;
            switch (code)
            {
                case XmlTypeCode.String:
                    meth = XmlILMethods.StrEq;
                    break;

                case XmlTypeCode.Decimal:
                    meth = XmlILMethods.DecEq;
                    break;

                case XmlTypeCode.QName:
                    meth = XmlILMethods.QNameEq;
                    break;
            }
            this.Call(meth);
        }

        public void CallConcatStrings(int cStrings)
        {
            switch (cStrings)
            {
                case 0:
                    this.Emit(OpCodes.Ldstr, "");
                    return;

                case 1:
                    break;

                case 2:
                    this.Call(XmlILMethods.StrCat2);
                    return;

                case 3:
                    this.Call(XmlILMethods.StrCat3);
                    return;

                case 4:
                    this.Call(XmlILMethods.StrCat4);
                    break;

                default:
                    return;
            }
        }

        public void CallEndRtfConstruction()
        {
            this.LoadQueryRuntime();
            this.Emit(OpCodes.Ldloca, this.locXOut);
            this.Call(XmlILMethods.EndRtfConstr);
        }

        public void CallEndSequenceConstruction()
        {
            this.LoadQueryRuntime();
            this.Emit(OpCodes.Ldloca, this.locXOut);
            this.Call(XmlILMethods.EndSeqConstr);
        }

        public void CallEndTree()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.EndTree);
        }

        public void CallGetAtomizedName(int idxName)
        {
            this.LoadQueryRuntime();
            this.LoadInteger(idxName);
            this.Call(XmlILMethods.GetAtomizedName);
        }

        public void CallGetCollation(int idxName)
        {
            this.LoadQueryRuntime();
            this.LoadInteger(idxName);
            this.Call(XmlILMethods.GetCollation);
        }

        public void CallGetEarlyBoundObject(int idxObj, Type clrType)
        {
            this.LoadQueryRuntime();
            this.LoadInteger(idxObj);
            this.Call(XmlILMethods.GetEarly);
            this.TreatAs(typeof(object), clrType);
        }

        public void CallGetGlobalValue(int idxValue, Type clrType)
        {
            this.LoadQueryRuntime();
            this.LoadInteger(idxValue);
            this.Call(XmlILMethods.GetGlobalValue);
            this.TreatAs(typeof(object), clrType);
        }

        public void CallGetNameFilter(int idxFilter)
        {
            this.LoadQueryRuntime();
            this.LoadInteger(idxFilter);
            this.Call(XmlILMethods.GetNameFilter);
        }

        public void CallGetParameter(string localName, string namespaceUri)
        {
            this.LoadQueryContext();
            this.Emit(OpCodes.Ldstr, localName);
            this.Emit(OpCodes.Ldstr, namespaceUri);
            this.Call(XmlILMethods.GetParam);
        }

        public void CallGetTypeFilter(XPathNodeType nodeType)
        {
            this.LoadQueryRuntime();
            this.LoadInteger((int) nodeType);
            this.Call(XmlILMethods.GetTypeFilter);
        }

        public void CallParseTagName(GenerateNameType nameType)
        {
            if (nameType == GenerateNameType.TagNameAndMappings)
            {
                this.Call(XmlILMethods.TagAndMappings);
            }
            else
            {
                this.Call(XmlILMethods.TagAndNamespace);
            }
        }

        public void CallSetGlobalValue(Type clrType)
        {
            this.TreatAs(clrType, typeof(object));
            this.Call(XmlILMethods.SetGlobalValue);
        }

        public void CallStartElementContent()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.StartContentUn);
        }

        public void CallStartRtfConstruction(string baseUri)
        {
            this.EnsureWriter();
            this.LoadQueryRuntime();
            this.Emit(OpCodes.Ldstr, baseUri);
            this.Emit(OpCodes.Ldloca, this.locXOut);
            this.Call(XmlILMethods.StartRtfConstr);
        }

        public void CallStartSequenceConstruction()
        {
            this.EnsureWriter();
            this.LoadQueryRuntime();
            this.Emit(OpCodes.Ldloca, this.locXOut);
            this.Call(XmlILMethods.StartSeqConstr);
        }

        public void CallStartTree(XPathNodeType rootType)
        {
            this.LoadQueryOutput();
            this.LoadInteger((int) rootType);
            this.Call(XmlILMethods.StartTree);
        }

        public void CallSyncToNavigator()
        {
            if (this.methSyncToNav == null)
            {
                this.methSyncToNav = this.module.FindMethod("SyncToNavigator");
            }
            this.Call(this.methSyncToNav);
        }

        public void CallToken(MethodInfo meth)
        {
            MethodBuilder methInfo = this.methInfo as MethodBuilder;
            if (methInfo != null)
            {
                OpCode opcode = (meth.IsVirtual || meth.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call;
                this.ilgen.Emit(opcode, ((ModuleBuilder) methInfo.GetModule()).GetMethodToken(meth).Token);
                if (this.lastSourceInfo != null)
                {
                    this.MarkSequencePoint(SourceLineInfo.NoSource);
                }
            }
            else
            {
                this.Call(meth);
            }
        }

        public void CallValueAs(Type clrType)
        {
            MethodInfo valueAs = XmlILMethods.StorageMethods[clrType].ValueAs;
            if (valueAs == null)
            {
                this.LoadType(clrType);
                this.Emit(OpCodes.Ldnull);
                this.Call(XmlILMethods.ValueAsAny);
                this.TreatAs(typeof(object), clrType);
            }
            else
            {
                this.Call(valueAs);
            }
        }

        public void CallWriteEndAttribute(bool callChk)
        {
            this.LoadQueryOutput();
            if (callChk)
            {
                this.Call(XmlILMethods.EndAttr);
            }
            else
            {
                this.Call(XmlILMethods.EndAttrUn);
            }
        }

        public void CallWriteEndComment()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.EndComment);
        }

        public void CallWriteEndElement(GenerateNameType nameType, bool callChk)
        {
            MethodInfo meth = null;
            if (callChk)
            {
                meth = XmlILMethods.EndElemStackName;
            }
            else
            {
                switch (nameType)
                {
                    case GenerateNameType.LiteralLocalName:
                        meth = XmlILMethods.EndElemLocNameUn;
                        break;

                    case GenerateNameType.LiteralName:
                        meth = XmlILMethods.EndElemLitNameUn;
                        break;
                }
            }
            this.Call(meth);
        }

        public void CallWriteEndPI()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.EndPI);
        }

        public void CallWriteEndRoot()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.EndRoot);
        }

        public void CallWriteNamespaceDecl(bool callChk)
        {
            if (callChk)
            {
                this.Call(XmlILMethods.NamespaceDecl);
            }
            else
            {
                this.Call(XmlILMethods.NamespaceDeclUn);
            }
        }

        public void CallWriteStartAttribute(GenerateNameType nameType, bool callChk)
        {
            MethodInfo meth = null;
            if (callChk)
            {
                switch (nameType)
                {
                    case GenerateNameType.LiteralLocalName:
                        meth = XmlILMethods.StartAttrLocName;
                        break;

                    case GenerateNameType.LiteralName:
                        meth = XmlILMethods.StartAttrLitName;
                        break;

                    case GenerateNameType.CopiedName:
                        meth = XmlILMethods.StartAttrCopyName;
                        break;

                    case GenerateNameType.TagNameAndMappings:
                        meth = XmlILMethods.StartAttrMapName;
                        break;

                    case GenerateNameType.TagNameAndNamespace:
                        meth = XmlILMethods.StartAttrNmspName;
                        break;

                    case GenerateNameType.QName:
                        meth = XmlILMethods.StartAttrQName;
                        break;
                }
            }
            else
            {
                switch (nameType)
                {
                    case GenerateNameType.LiteralLocalName:
                        meth = XmlILMethods.StartAttrLocNameUn;
                        break;

                    case GenerateNameType.LiteralName:
                        meth = XmlILMethods.StartAttrLitNameUn;
                        break;
                }
            }
            this.Call(meth);
        }

        public void CallWriteStartComment()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.StartComment);
        }

        public void CallWriteStartElement(GenerateNameType nameType, bool callChk)
        {
            MethodInfo meth = null;
            if (callChk)
            {
                switch (nameType)
                {
                    case GenerateNameType.LiteralLocalName:
                        meth = XmlILMethods.StartElemLocName;
                        break;

                    case GenerateNameType.LiteralName:
                        meth = XmlILMethods.StartElemLitName;
                        break;

                    case GenerateNameType.CopiedName:
                        meth = XmlILMethods.StartElemCopyName;
                        break;

                    case GenerateNameType.TagNameAndMappings:
                        meth = XmlILMethods.StartElemMapName;
                        break;

                    case GenerateNameType.TagNameAndNamespace:
                        meth = XmlILMethods.StartElemNmspName;
                        break;

                    case GenerateNameType.QName:
                        meth = XmlILMethods.StartElemQName;
                        break;
                }
            }
            else
            {
                switch (nameType)
                {
                    case GenerateNameType.LiteralLocalName:
                        meth = XmlILMethods.StartElemLocNameUn;
                        break;

                    case GenerateNameType.LiteralName:
                        meth = XmlILMethods.StartElemLitNameUn;
                        break;
                }
            }
            this.Call(meth);
        }

        public void CallWriteStartPI()
        {
            this.Call(XmlILMethods.StartPI);
        }

        public void CallWriteStartRoot()
        {
            this.LoadQueryOutput();
            this.Call(XmlILMethods.StartRoot);
        }

        public void CallWriteString(bool disableOutputEscaping, bool callChk)
        {
            if (callChk)
            {
                if (disableOutputEscaping)
                {
                    this.Call(XmlILMethods.NoEntText);
                }
                else
                {
                    this.Call(XmlILMethods.Text);
                }
            }
            else if (disableOutputEscaping)
            {
                this.Call(XmlILMethods.NoEntTextUn);
            }
            else
            {
                this.Call(XmlILMethods.TextUn);
            }
        }

        public void Construct(ConstructorInfo constr)
        {
            this.Emit(OpCodes.Newobj, constr);
        }

        public void ConstructLiteralDecimal(decimal dec)
        {
            if (((dec >= -2147483648M) && (dec <= 2147483647M)) && (decimal.Truncate(dec) == dec))
            {
                this.LoadInteger((int) dec);
                this.Construct(XmlILConstructors.DecFromInt32);
            }
            else
            {
                int[] bits = decimal.GetBits(dec);
                this.LoadInteger(bits[0]);
                this.LoadInteger(bits[1]);
                this.LoadInteger(bits[2]);
                this.LoadBoolean(bits[3] < 0);
                this.LoadInteger(bits[3] >> 0x10);
                this.Construct(XmlILConstructors.DecFromParts);
            }
        }

        public void ConstructLiteralQName(string localName, string namespaceName)
        {
            this.Emit(OpCodes.Ldstr, localName);
            this.Emit(OpCodes.Ldstr, namespaceName);
            this.Construct(XmlILConstructors.QName);
        }

        public void ConvBranchToBool(Label lblBranch, bool isTrueBranch)
        {
            Label lblTarget = this.DefineLabel();
            this.Emit(isTrueBranch ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
            this.EmitUnconditionalBranch(OpCodes.Br_S, lblTarget);
            this.MarkLabel(lblBranch);
            this.Emit(isTrueBranch ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            this.MarkLabel(lblTarget);
        }

        public void DebugEndScope()
        {
            this.ilgen.EndScope();
        }

        public void DebugSequencePoint(ISourceLineInfo sourceInfo)
        {
            this.Emit(OpCodes.Nop);
            this.MarkSequencePoint(sourceInfo);
        }

        public void DebugStartScope()
        {
            this.ilgen.BeginScope();
        }

        public LocalBuilder DeclareLocal(string name, Type type) => 
            this.ilgen.DeclareLocal(type);

        public Label DefineLabel() => 
            this.ilgen.DefineLabel();

        public void Emit(OpCode opcode)
        {
            this.ilgen.Emit(opcode);
        }

        public void Emit(OpCode opcode, byte byteVal)
        {
            this.ilgen.Emit(opcode, byteVal);
        }

        public void Emit(OpCode opcode, double dblVal)
        {
            this.ilgen.Emit(opcode, dblVal);
        }

        public void Emit(OpCode opcode, short shrtVal)
        {
            this.ilgen.Emit(opcode, shrtVal);
        }

        public void Emit(OpCode opcode, int intVal)
        {
            this.ilgen.Emit(opcode, intVal);
        }

        public void Emit(OpCode opcode, long longVal)
        {
            this.ilgen.Emit(opcode, longVal);
        }

        public void Emit(OpCode opcode, ConstructorInfo constrInfo)
        {
            this.ilgen.Emit(opcode, constrInfo);
        }

        public void Emit(OpCode opcode, Label lblVal)
        {
            this.ilgen.Emit(opcode, lblVal);
        }

        public void Emit(OpCode opcode, LocalBuilder locBldr)
        {
            this.ilgen.Emit(opcode, locBldr);
        }

        public void Emit(OpCode opcode, FieldInfo fldInfo)
        {
            this.ilgen.Emit(opcode, fldInfo);
        }

        public void Emit(OpCode opcode, MethodInfo methInfo)
        {
            this.ilgen.Emit(opcode, methInfo);
        }

        public void Emit(OpCode opcode, sbyte sbyteVal)
        {
            this.ilgen.Emit(opcode, sbyteVal);
        }

        public void Emit(OpCode opcode, float fltVal)
        {
            this.ilgen.Emit(opcode, fltVal);
        }

        public void Emit(OpCode opcode, Label[] arrLabels)
        {
            this.ilgen.Emit(opcode, arrLabels);
        }

        public void Emit(OpCode opcode, string strVal)
        {
            this.ilgen.Emit(opcode, strVal);
        }

        public void Emit(OpCode opcode, Type typVal)
        {
            this.ilgen.Emit(opcode, typVal);
        }

        public void EmitUnconditionalBranch(OpCode opcode, Label lblTarget)
        {
            if (!opcode.Equals(OpCodes.Br) && !opcode.Equals(OpCodes.Br_S))
            {
                this.Emit(OpCodes.Ldc_I4_1);
            }
            this.ilgen.Emit(opcode, lblTarget);
            if ((this.lastSourceInfo != null) && (opcode.Equals(OpCodes.Br) || opcode.Equals(OpCodes.Br_S)))
            {
                this.MarkSequencePoint(SourceLineInfo.NoSource);
            }
        }

        private void EnsureWriter()
        {
            if (!this.initWriters)
            {
                this.locXOut = this.DeclareLocal("$$$xwrtChk", typeof(XmlQueryOutput));
                this.initWriters = true;
            }
        }

        private string GetFileName(ISourceLineInfo sourceInfo)
        {
            string uri = sourceInfo.Uri;
            if (uri != this.lastUriString)
            {
                this.lastUriString = uri;
                this.lastFileName = SourceLineInfo.GetFileName(uri);
            }
            return this.lastFileName;
        }

        public void LoadBoolean(bool boolVal)
        {
            this.Emit(boolVal ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
        }

        public void LoadInteger(int intVal)
        {
            if ((intVal < -1) || (intVal >= 9))
            {
                if ((intVal >= -128) && (intVal <= 0x7f))
                {
                    this.Emit(OpCodes.Ldc_I4_S, (sbyte) intVal);
                }
                else
                {
                    this.Emit(OpCodes.Ldc_I4, intVal);
                }
            }
            else
            {
                OpCode code;
                switch (intVal)
                {
                    case -1:
                        code = OpCodes.Ldc_I4_M1;
                        break;

                    case 0:
                        code = OpCodes.Ldc_I4_0;
                        break;

                    case 1:
                        code = OpCodes.Ldc_I4_1;
                        break;

                    case 2:
                        code = OpCodes.Ldc_I4_2;
                        break;

                    case 3:
                        code = OpCodes.Ldc_I4_3;
                        break;

                    case 4:
                        code = OpCodes.Ldc_I4_4;
                        break;

                    case 5:
                        code = OpCodes.Ldc_I4_5;
                        break;

                    case 6:
                        code = OpCodes.Ldc_I4_6;
                        break;

                    case 7:
                        code = OpCodes.Ldc_I4_7;
                        break;

                    case 8:
                        code = OpCodes.Ldc_I4_8;
                        break;

                    default:
                        return;
                }
                this.Emit(code);
            }
        }

        public void LoadParameter(int paramPos)
        {
            switch (paramPos)
            {
                case 0:
                    this.Emit(OpCodes.Ldarg_0);
                    return;

                case 1:
                    this.Emit(OpCodes.Ldarg_1);
                    return;

                case 2:
                    this.Emit(OpCodes.Ldarg_2);
                    return;

                case 3:
                    this.Emit(OpCodes.Ldarg_3);
                    return;
            }
            if (paramPos <= 0xff)
            {
                this.Emit(OpCodes.Ldarg_S, (byte) paramPos);
            }
            else
            {
                if (paramPos > 0xffff)
                {
                    throw new XslTransformException("XmlIl_TooManyParameters");
                }
                this.Emit(OpCodes.Ldarg, paramPos);
            }
        }

        public void LoadQueryContext()
        {
            this.Emit(OpCodes.Ldarg_0);
            this.Call(XmlILMethods.Context);
        }

        public void LoadQueryOutput()
        {
            this.Emit(OpCodes.Ldloc, this.locXOut);
        }

        public void LoadQueryRuntime()
        {
            this.Emit(OpCodes.Ldarg_0);
        }

        public void LoadType(Type clrTyp)
        {
            this.Emit(OpCodes.Ldtoken, clrTyp);
            this.Call(XmlILMethods.GetTypeFromHandle);
        }

        public void LoadXsltLibrary()
        {
            this.Emit(OpCodes.Ldarg_0);
            this.Call(XmlILMethods.XsltLib);
        }

        public void MarkLabel(Label lbl)
        {
            if ((this.lastSourceInfo != null) && !this.lastSourceInfo.IsNoSource)
            {
                this.DebugSequencePoint(SourceLineInfo.NoSource);
            }
            this.ilgen.MarkLabel(lbl);
        }

        private void MarkSequencePoint(ISourceLineInfo sourceInfo)
        {
            if ((!sourceInfo.IsNoSource || (this.lastSourceInfo == null)) || !this.lastSourceInfo.IsNoSource)
            {
                string fileName = this.GetFileName(sourceInfo);
                ISymbolDocumentWriter document = this.module.AddSourceDocument(fileName);
                this.ilgen.MarkSequencePoint(document, sourceInfo.StartLine, sourceInfo.StartPos, sourceInfo.EndLine, sourceInfo.EndPos);
                this.lastSourceInfo = sourceInfo;
            }
        }

        public void MethodBegin(MethodBase methInfo, ISourceLineInfo sourceInfo, bool initWriters)
        {
            this.methInfo = methInfo;
            this.ilgen = XmlILModule.DefineMethodBody(methInfo);
            this.lastSourceInfo = null;
            if (this.isDebug)
            {
                this.DebugStartScope();
                if (sourceInfo != null)
                {
                    this.MarkSequencePoint(sourceInfo);
                    this.Emit(OpCodes.Nop);
                }
            }
            this.initWriters = false;
            if (initWriters)
            {
                this.EnsureWriter();
                this.LoadQueryRuntime();
                this.Call(XmlILMethods.GetOutput);
                this.Emit(OpCodes.Stloc, this.locXOut);
            }
        }

        public void MethodEnd()
        {
            this.Emit(OpCodes.Ret);
            if (this.isDebug)
            {
                this.DebugEndScope();
            }
        }

        public void SetParameter(object paramId)
        {
            int intVal = (int) paramId;
            if (intVal <= 0xff)
            {
                this.Emit(OpCodes.Starg_S, (byte) intVal);
            }
            else
            {
                if (intVal > 0xffff)
                {
                    throw new XslTransformException("XmlIl_TooManyParameters");
                }
                this.Emit(OpCodes.Starg, intVal);
            }
        }

        public void TailCall(MethodInfo meth)
        {
            this.Emit(OpCodes.Tailcall);
            this.Call(meth);
            this.Emit(OpCodes.Ret);
        }

        public void TestAndBranch(int i4, Label lblBranch, OpCode opcodeBranch)
        {
            if (i4 == 0)
            {
                if (opcodeBranch.Value == OpCodes.Beq.Value)
                {
                    opcodeBranch = OpCodes.Brfalse;
                    goto Label_008A;
                }
                if (opcodeBranch.Value == OpCodes.Beq_S.Value)
                {
                    opcodeBranch = OpCodes.Brfalse_S;
                    goto Label_008A;
                }
                if (opcodeBranch.Value == OpCodes.Bne_Un.Value)
                {
                    opcodeBranch = OpCodes.Brtrue;
                    goto Label_008A;
                }
                if (opcodeBranch.Value == OpCodes.Bne_Un_S.Value)
                {
                    opcodeBranch = OpCodes.Brtrue_S;
                    goto Label_008A;
                }
            }
            this.LoadInteger(i4);
        Label_008A:
            this.Emit(opcodeBranch, lblBranch);
        }

        [Conditional("DEBUG")]
        private void TraceCall(OpCode opcode, MethodInfo meth)
        {
        }

        public void TreatAs(Type clrTypeSrc, Type clrTypeDst)
        {
            if (clrTypeSrc != clrTypeDst)
            {
                if (clrTypeSrc.IsValueType)
                {
                    this.Emit(OpCodes.Box, clrTypeSrc);
                }
                else if (clrTypeDst.IsValueType)
                {
                    this.Emit(OpCodes.Unbox, clrTypeDst);
                    this.Emit(OpCodes.Ldobj, clrTypeDst);
                }
                else if (clrTypeDst != typeof(object))
                {
                    this.Emit(OpCodes.Castclass, clrTypeDst);
                }
            }
        }

        public StaticDataManager StaticData =>
            this.staticData;
    }
}

