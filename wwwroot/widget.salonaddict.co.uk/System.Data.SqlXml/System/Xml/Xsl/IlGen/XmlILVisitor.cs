namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Utils;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class XmlILVisitor : QilVisitor
    {
        private GenerateHelper helper;
        private int indexId;
        private IteratorDescriptor iterCurr;
        private IteratorDescriptor iterNested;
        private QilExpression qil;

        private void AfterEndChecks(QilNode ndCtor)
        {
            if (XmlILConstructInfo.Read(ndCtor).FinalStates == PossibleXmlStates.WithinSequence)
            {
                this.helper.CallEndTree();
            }
        }

        private QilNode ArithmeticOp(QilBinary ndOp)
        {
            this.NestedVisitEnsureStack(ndOp.Left, ndOp.Right);
            this.helper.CallArithmeticOp(ndOp.NodeType, ndOp.XmlType.TypeCode);
            this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(ndOp), false);
            return ndOp;
        }

        private void BeforeStartChecks(QilNode ndCtor)
        {
            switch (XmlILConstructInfo.Read(ndCtor).InitialStates)
            {
                case PossibleXmlStates.WithinSequence:
                    this.helper.CallStartTree(this.QilConstructorToNodeType(ndCtor.NodeType));
                    return;

                case PossibleXmlStates.EnumAttrs:
                    switch (ndCtor.NodeType)
                    {
                        case QilNodeType.ElementCtor:
                        case QilNodeType.CommentCtor:
                        case QilNodeType.PICtor:
                        case QilNodeType.TextCtor:
                        case QilNodeType.RawTextCtor:
                            this.helper.CallStartElementContent();
                            return;
                    }
                    return;

                default:
                    return;
            }
        }

        private bool CachesResult(QilNode nd)
        {
            switch (nd.NodeType)
            {
                case QilNodeType.Let:
                case QilNodeType.Parameter:
                case QilNodeType.Invoke:
                case QilNodeType.XsltInvokeLateBound:
                case QilNodeType.XsltInvokeEarlyBound:
                    return !nd.XmlType.IsSingleton;

                case QilNodeType.Filter:
                    return OptimizerPatterns.Read(nd).MatchesPattern(OptimizerPatternName.EqualityIndex);

                case QilNodeType.DocOrderDistinct:
                    if (!nd.XmlType.IsSingleton)
                    {
                        OptimizerPatterns patterns = OptimizerPatterns.Read(nd);
                        return (!patterns.MatchesPattern(OptimizerPatternName.JoinAndDod) && !patterns.MatchesPattern(OptimizerPatternName.DodReverse));
                    }
                    return false;

                case QilNodeType.TypeAssert:
                {
                    QilTargetType type = (QilTargetType) nd;
                    return (this.CachesResult(type.Source) && (this.GetItemStorageType(type.Source) == this.GetItemStorageType(type)));
                }
            }
            return false;
        }

        private bool CheckEnumAttrs(XmlILConstructInfo info)
        {
            switch (info.InitialStates)
            {
                case PossibleXmlStates.WithinSequence:
                case PossibleXmlStates.EnumAttrs:
                    return false;
            }
            return true;
        }

        private bool CheckWithinContent(XmlILConstructInfo info)
        {
            switch (info.InitialStates)
            {
                case PossibleXmlStates.WithinSequence:
                case PossibleXmlStates.EnumAttrs:
                case PossibleXmlStates.WithinContent:
                    return false;
            }
            return true;
        }

        private void ClrCompare(QilNodeType relOp, XmlTypeCode code)
        {
            OpCode nop;
            Label label;
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.OnTrue:
                    switch (relOp)
                    {
                        case QilNodeType.Ne:
                            nop = OpCodes.Bne_Un;
                            goto Label_0170;

                        case QilNodeType.Eq:
                            nop = OpCodes.Beq;
                            goto Label_0170;

                        case QilNodeType.Gt:
                            nop = OpCodes.Bgt;
                            goto Label_0170;

                        case QilNodeType.Ge:
                            nop = OpCodes.Bge;
                            goto Label_0170;

                        case QilNodeType.Lt:
                            nop = OpCodes.Blt;
                            goto Label_0170;

                        case QilNodeType.Le:
                            nop = OpCodes.Ble;
                            goto Label_0170;
                    }
                    nop = OpCodes.Nop;
                    goto Label_0170;

                case BranchingContext.OnFalse:
                    if ((code != XmlTypeCode.Double) && (code != XmlTypeCode.Float))
                    {
                        switch (relOp)
                        {
                            case QilNodeType.Ne:
                                nop = OpCodes.Beq;
                                goto Label_00EB;

                            case QilNodeType.Eq:
                                nop = OpCodes.Bne_Un;
                                goto Label_00EB;

                            case QilNodeType.Gt:
                                nop = OpCodes.Ble;
                                goto Label_00EB;

                            case QilNodeType.Ge:
                                nop = OpCodes.Blt;
                                goto Label_00EB;

                            case QilNodeType.Lt:
                                nop = OpCodes.Bge;
                                goto Label_00EB;

                            case QilNodeType.Le:
                                nop = OpCodes.Bgt;
                                goto Label_00EB;
                        }
                        nop = OpCodes.Nop;
                        break;
                    }
                    switch (relOp)
                    {
                        case QilNodeType.Ne:
                            nop = OpCodes.Beq;
                            goto Label_00EB;

                        case QilNodeType.Eq:
                            nop = OpCodes.Bne_Un;
                            goto Label_00EB;

                        case QilNodeType.Gt:
                            nop = OpCodes.Ble_Un;
                            goto Label_00EB;

                        case QilNodeType.Ge:
                            nop = OpCodes.Blt_Un;
                            goto Label_00EB;

                        case QilNodeType.Lt:
                            nop = OpCodes.Bge_Un;
                            goto Label_00EB;

                        case QilNodeType.Le:
                            nop = OpCodes.Bgt_Un;
                            goto Label_00EB;
                    }
                    nop = OpCodes.Nop;
                    break;

                default:
                    switch (relOp)
                    {
                        case QilNodeType.Eq:
                            this.helper.Emit(OpCodes.Ceq);
                            goto Label_0255;

                        case QilNodeType.Gt:
                            this.helper.Emit(OpCodes.Cgt);
                            goto Label_0255;

                        case QilNodeType.Lt:
                            this.helper.Emit(OpCodes.Clt);
                            goto Label_0255;

                        case QilNodeType.Ge:
                            nop = OpCodes.Bge_S;
                            goto Label_022F;

                        case QilNodeType.Le:
                            nop = OpCodes.Ble_S;
                            goto Label_022F;

                        case QilNodeType.Ne:
                            nop = OpCodes.Bne_Un_S;
                            goto Label_022F;
                    }
                    nop = OpCodes.Nop;
                    goto Label_022F;
            }
        Label_00EB:
            this.helper.Emit(nop, this.iterCurr.LabelBranch);
            this.iterCurr.Storage = StorageDescriptor.None();
            return;
        Label_0170:
            this.helper.Emit(nop, this.iterCurr.LabelBranch);
            this.iterCurr.Storage = StorageDescriptor.None();
            return;
        Label_022F:
            label = this.helper.DefineLabel();
            this.helper.Emit(nop, label);
            this.helper.ConvBranchToBool(label, true);
        Label_0255:
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
        }

        private void Compare(QilBinary ndComp)
        {
            QilNodeType nodeType = ndComp.NodeType;
            switch (nodeType)
            {
                case QilNodeType.Eq:
                case QilNodeType.Ne:
                    if (this.TryZeroCompare(nodeType, ndComp.Left, ndComp.Right))
                    {
                        return;
                    }
                    if (this.TryZeroCompare(nodeType, ndComp.Right, ndComp.Left))
                    {
                        return;
                    }
                    if (this.TryNameCompare(nodeType, ndComp.Left, ndComp.Right))
                    {
                        return;
                    }
                    if (this.TryNameCompare(nodeType, ndComp.Right, ndComp.Left))
                    {
                        return;
                    }
                    break;
            }
            this.NestedVisitEnsureStack(ndComp.Left, ndComp.Right);
            XmlTypeCode typeCode = ndComp.Left.XmlType.TypeCode;
            switch (typeCode)
            {
                case XmlTypeCode.Integer:
                case XmlTypeCode.Int:
                case XmlTypeCode.Boolean:
                case XmlTypeCode.Double:
                    this.ClrCompare(nodeType, typeCode);
                    break;

                case XmlTypeCode.String:
                case XmlTypeCode.Decimal:
                case XmlTypeCode.QName:
                    switch (nodeType)
                    {
                        case QilNodeType.Eq:
                        case QilNodeType.Ne:
                            this.helper.CallCompareEquals(typeCode);
                            this.ZeroCompare((nodeType == QilNodeType.Eq) ? QilNodeType.Ne : QilNodeType.Eq, true);
                            return;
                    }
                    this.helper.CallCompare(typeCode);
                    this.helper.Emit(OpCodes.Ldc_I4_0);
                    this.ClrCompare(nodeType, typeCode);
                    return;

                case XmlTypeCode.Float:
                    break;

                default:
                    return;
            }
        }

        private void ComparePosition(QilBinary ndComp)
        {
            this.helper.LoadQueryRuntime();
            this.NestedVisitEnsureStack(ndComp.Left, ndComp.Right);
            this.helper.Call(XmlILMethods.CompPos);
            this.helper.LoadInteger(0);
            this.ClrCompare((ndComp.NodeType == QilNodeType.Before) ? QilNodeType.Lt : QilNodeType.Gt, XmlTypeCode.String);
        }

        private void ConditionalBranch(QilNode ndBranch, Type itemStorageType, LocalBuilder locResult)
        {
            if (locResult == null)
            {
                if (this.iterCurr.IsBranching)
                {
                    this.NestedVisitWithBranch(ndBranch, this.iterCurr.CurrentBranchingContext, this.iterCurr.LabelBranch);
                }
                else
                {
                    this.NestedVisitEnsureStack(ndBranch, itemStorageType, false);
                }
            }
            else
            {
                this.NestedVisit(ndBranch, this.iterCurr.GetLabelNext());
                this.iterCurr.EnsureItemStorageType(ndBranch.XmlType, itemStorageType);
                this.iterCurr.EnsureLocalNoCache(locResult);
            }
        }

        private void CopySequence(QilNode nd)
        {
            bool flag;
            Label label;
            XmlQueryType xmlType = nd.XmlType;
            this.StartWriterLoop(nd, out flag, out label);
            if (xmlType.IsSingleton)
            {
                this.helper.LoadQueryOutput();
                base.Visit(nd);
                this.iterCurr.EnsureItemStorageType(nd.XmlType, typeof(XPathItem));
            }
            else
            {
                base.Visit(nd);
                this.iterCurr.EnsureItemStorageType(nd.XmlType, typeof(XPathItem));
                this.iterCurr.EnsureNoStackNoCache("$$$copyTemp");
                this.helper.LoadQueryOutput();
            }
            this.iterCurr.EnsureStackNoCache();
            this.helper.Call(XmlILMethods.WriteItem);
            this.EndWriterLoop(nd, flag, label);
        }

        private QilNode CreateAggregator(QilUnary ndAgg, string aggName, XmlILStorageMethods methods, MethodInfo methAgg, MethodInfo methResult)
        {
            Label lblOnEnd = this.helper.DefineLabel();
            Type declaringType = methAgg.DeclaringType;
            LocalBuilder locBldr = this.helper.DeclareLocal(aggName, declaringType);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.helper.Call(methods.AggCreate);
            this.StartNestedIterator(ndAgg.Child, lblOnEnd);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.Visit(ndAgg.Child);
            this.iterCurr.EnsureStackNoCache();
            this.iterCurr.EnsureItemStorageType(ndAgg.XmlType, this.GetItemStorageType(ndAgg));
            this.helper.Call(methAgg);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.iterCurr.LoopToEnd(lblOnEnd);
            this.EndNestedIterator(ndAgg.Child);
            if (ndAgg.XmlType.MaybeEmpty)
            {
                this.helper.Call(methods.AggIsEmpty);
                this.helper.Emit(OpCodes.Brtrue, this.iterCurr.GetLabelNext());
                this.helper.Emit(OpCodes.Ldloca, locBldr);
            }
            this.helper.Call(methResult);
            this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(ndAgg), false);
            return ndAgg;
        }

        private void CreateContainerIterator(QilUnary ndDod, string iterName, Type iterType, MethodInfo methCreate, MethodInfo methNext, XmlNodeKindFlags kinds, QilName ndName, TriState orSelf)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal(iterName, iterType);
            QilLoop child = (QilLoop) ndDod.Child;
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.LoadSelectFilter(kinds, ndName);
            if (orSelf != TriState.Unknown)
            {
                this.helper.LoadBoolean(orSelf == TriState.True);
            }
            this.helper.Call(methCreate);
            Label lblOnEnd = this.helper.DefineLabel();
            this.StartNestedIterator(child, lblOnEnd);
            this.StartBinding(child.Variable);
            this.EndBinding(child.Variable);
            this.EndNestedIterator(child.Variable);
            this.iterCurr.Storage = this.iterNested.Storage;
            this.GenerateContainerIterator(ndDod, locBldr, lblOnEnd, methNext, typeof(XPathNavigator));
        }

        private void CreateFilteredIterator(QilNode ndCtxt, string iterName, Type iterType, MethodInfo methCreate, MethodInfo methNext, XmlNodeKindFlags kinds, QilName ndName, TriState orSelf, QilNode ndEnd)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal(iterName, iterType);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.NestedVisitEnsureStack(ndCtxt);
            this.LoadSelectFilter(kinds, ndName);
            if (orSelf != TriState.Unknown)
            {
                this.helper.LoadBoolean(orSelf == TriState.True);
            }
            if (ndEnd != null)
            {
                this.NestedVisitEnsureStack(ndEnd);
            }
            this.helper.Call(methCreate);
            this.GenerateSimpleIterator(typeof(XPathNavigator), locBldr, methNext);
        }

        private QilNode CreateSetIterator(QilBinary ndSet, string iterName, Type iterType, MethodInfo methCreate, MethodInfo methNext)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal(iterName, iterType);
            LocalBuilder bldr = this.helper.DeclareLocal("$$$navSet", typeof(XPathNavigator));
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.helper.LoadQueryRuntime();
            this.helper.Call(methCreate);
            Label lblOnEnd = this.helper.DefineLabel();
            Label lblTarget = this.helper.DefineLabel();
            Label lbl = this.helper.DefineLabel();
            this.NestedVisit(ndSet.Left, lblOnEnd);
            Label labelNext = this.iterNested.GetLabelNext();
            this.iterCurr.EnsureLocal(bldr);
            this.helper.EmitUnconditionalBranch(OpCodes.Brtrue, lblTarget);
            this.helper.MarkLabel(lbl);
            this.NestedVisit(ndSet.Right, lblOnEnd);
            Label label4 = this.iterNested.GetLabelNext();
            this.iterCurr.EnsureLocal(bldr);
            this.helper.EmitUnconditionalBranch(OpCodes.Brtrue, lblTarget);
            this.helper.MarkLabel(lblOnEnd);
            this.helper.Emit(OpCodes.Ldnull);
            this.helper.Emit(OpCodes.Stloc, bldr);
            this.helper.MarkLabel(lblTarget);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.helper.Emit(OpCodes.Ldloc, bldr);
            this.helper.Call(methNext);
            if (ndSet.XmlType.IsSingleton)
            {
                Label[] labelArray = new Label[] { lbl, labelNext, label4 };
                this.helper.Emit(OpCodes.Switch, labelArray);
                this.iterCurr.Storage = StorageDescriptor.Current(locBldr, typeof(XPathNavigator));
                return ndSet;
            }
            Label[] arrLabels = new Label[] { this.iterCurr.GetLabelNext(), lbl, labelNext, label4 };
            this.helper.Emit(OpCodes.Switch, arrLabels);
            this.iterCurr.SetIterator(lblOnEnd, StorageDescriptor.Current(locBldr, typeof(XPathNavigator)));
            return ndSet;
        }

        private void CreateSimpleIterator(QilNode ndCtxt, string iterName, Type iterType, MethodInfo methCreate, MethodInfo methNext)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal(iterName, iterType);
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.NestedVisitEnsureStack(ndCtxt);
            this.helper.Call(methCreate);
            this.GenerateSimpleIterator(typeof(XPathNavigator), locBldr, methNext);
        }

        private bool ElementCachesAttributes(XmlILConstructInfo info)
        {
            if (!info.MightHaveDuplicateAttributes)
            {
                return info.MightHaveNamespacesAfterAttributes;
            }
            return true;
        }

        private void EndBinding(QilIterator ndIter)
        {
            if (this.qil.IsDebug && (ndIter.DebugName != null))
            {
                this.helper.DebugEndScope();
            }
        }

        private void EndConjunctiveTests(BranchingContext brctxt, Label lblBranch, Label lblOnFalse)
        {
            switch (brctxt)
            {
                case BranchingContext.None:
                    this.helper.ConvBranchToBool(lblOnFalse, false);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
                    return;

                case BranchingContext.OnTrue:
                    this.helper.MarkLabel(lblOnFalse);
                    break;

                case BranchingContext.OnFalse:
                    break;

                default:
                    return;
            }
            this.iterCurr.Storage = StorageDescriptor.None();
        }

        private void EndNestedIterator(QilNode nd)
        {
            if (this.iterCurr.IsBranching && (this.iterCurr.Storage.Location != ItemLocation.None))
            {
                this.iterCurr.EnsureItemStorageType(nd.XmlType, typeof(bool));
                this.iterCurr.EnsureStackNoCache();
                if (this.iterCurr.CurrentBranchingContext == BranchingContext.OnTrue)
                {
                    this.helper.Emit(OpCodes.Brtrue, this.iterCurr.LabelBranch);
                }
                else
                {
                    this.helper.Emit(OpCodes.Brfalse, this.iterCurr.LabelBranch);
                }
                this.iterCurr.Storage = StorageDescriptor.None();
            }
            this.iterNested = this.iterCurr;
            this.iterCurr = this.iterCurr.ParentIterator;
        }

        private void EndWriterLoop(QilNode nd, bool hasOnEnd, Label lblOnEnd)
        {
            if (XmlILConstructInfo.Read(nd).PushToWriterLast)
            {
                this.iterCurr.Storage = StorageDescriptor.None();
                if (!nd.XmlType.IsSingleton && hasOnEnd)
                {
                    this.iterCurr.LoopToEnd(lblOnEnd);
                }
            }
        }

        private void Function(QilFunction ndFunc)
        {
            int num;
            foreach (QilIterator iterator in ndFunc.Arguments)
            {
                IteratorDescriptor descriptor = new IteratorDescriptor(this.helper);
                num = XmlILAnnotation.Write(iterator).ArgumentPosition + 1;
                descriptor.Storage = StorageDescriptor.Parameter(num, this.GetItemStorageType(iterator), !iterator.XmlType.IsSingleton);
                XmlILAnnotation.Write(iterator).CachedIteratorDescriptor = descriptor;
            }
            MethodInfo functionBinding = XmlILAnnotation.Write(ndFunc).FunctionBinding;
            bool initWriters = XmlILConstructInfo.Read(ndFunc).ConstructMethod == XmlILConstructMethod.Writer;
            this.helper.MethodBegin(functionBinding, ndFunc.SourceLine, initWriters);
            foreach (QilIterator iterator2 in ndFunc.Arguments)
            {
                if (this.qil.IsDebug && (iterator2.SourceLine != null))
                {
                    this.helper.DebugSequencePoint(iterator2.SourceLine);
                }
                if (iterator2.Binding != null)
                {
                    num = (iterator2.Annotation as XmlILAnnotation).ArgumentPosition + 1;
                    Label lblVal = this.helper.DefineLabel();
                    this.helper.LoadQueryRuntime();
                    this.helper.LoadParameter(num);
                    this.helper.LoadInteger(0x1d);
                    this.helper.Call(XmlILMethods.SeqMatchesCode);
                    this.helper.Emit(OpCodes.Brfalse, lblVal);
                    this.StartNestedIterator(iterator2);
                    this.NestedVisitEnsureStack(iterator2.Binding, this.GetItemStorageType(iterator2), !iterator2.XmlType.IsSingleton);
                    this.EndNestedIterator(iterator2);
                    this.helper.SetParameter(num);
                    this.helper.MarkLabel(lblVal);
                }
            }
            this.StartNestedIterator(ndFunc);
            if (initWriters)
            {
                this.NestedVisit(ndFunc.Definition);
            }
            else
            {
                this.NestedVisitEnsureStack(ndFunc.Definition, this.GetItemStorageType(ndFunc), !ndFunc.XmlType.IsSingleton);
            }
            this.EndNestedIterator(ndFunc);
            this.helper.MethodEnd();
        }

        private void GenerateConcat(QilNode ndStr, LocalBuilder locStringConcat)
        {
            Label lblOnEnd = this.helper.DefineLabel();
            this.StartNestedIterator(ndStr, lblOnEnd);
            this.Visit(ndStr);
            this.iterCurr.EnsureStackNoCache();
            this.iterCurr.EnsureItemStorageType(ndStr.XmlType, typeof(string));
            this.helper.Call(XmlILMethods.StrCatCat);
            this.helper.Emit(OpCodes.Ldloca, locStringConcat);
            this.iterCurr.LoopToEnd(lblOnEnd);
            this.EndNestedIterator(ndStr);
        }

        private void GenerateContainerIterator(QilNode nd, LocalBuilder locIter, Label lblOnEndNested, MethodInfo methNext, Type itemStorageType)
        {
            Label lblTarget = this.helper.DefineLabel();
            this.iterCurr.EnsureNoStackNoCache(nd.XmlType.IsNode ? "$$$navInput" : "$$$itemInput");
            this.helper.Emit(OpCodes.Ldloca, locIter);
            this.iterCurr.PushValue();
            this.helper.EmitUnconditionalBranch(OpCodes.Br, lblTarget);
            this.helper.MarkLabel(lblOnEndNested);
            this.helper.Emit(OpCodes.Ldloca, locIter);
            this.helper.Emit(OpCodes.Ldnull);
            this.helper.MarkLabel(lblTarget);
            this.helper.Call(methNext);
            if (nd.XmlType.IsSingleton)
            {
                this.helper.LoadInteger(1);
                this.helper.Emit(OpCodes.Beq, this.iterNested.GetLabelNext());
                this.iterCurr.Storage = StorageDescriptor.Current(locIter, itemStorageType);
            }
            else
            {
                Label[] arrLabels = new Label[] { this.iterCurr.GetLabelNext(), this.iterNested.GetLabelNext() };
                this.helper.Emit(OpCodes.Switch, arrLabels);
                this.iterCurr.SetIterator(lblOnEndNested, StorageDescriptor.Current(locIter, itemStorageType));
            }
        }

        private void GenerateSimpleIterator(Type itemStorageType, LocalBuilder locIter, MethodInfo methNext)
        {
            Label lbl = this.helper.DefineLabel();
            this.helper.MarkLabel(lbl);
            this.helper.Emit(OpCodes.Ldloca, locIter);
            this.helper.Call(methNext);
            this.helper.Emit(OpCodes.Brfalse, this.iterCurr.GetLabelNext());
            this.iterCurr.SetIterator(lbl, StorageDescriptor.Current(locIter, itemStorageType));
        }

        private Type GetItemStorageType(QilNode nd) => 
            XmlILTypeHelper.GetStorageType(nd.XmlType.Prime);

        private Type GetItemStorageType(XmlQueryType typ) => 
            XmlILTypeHelper.GetStorageType(typ.Prime);

        private Type GetStorageType(QilNode nd) => 
            XmlILTypeHelper.GetStorageType(nd.XmlType);

        private Type GetStorageType(XmlQueryType typ) => 
            XmlILTypeHelper.GetStorageType(typ);

        private bool GetXsltConvertMethod(XmlQueryType typSrc, XmlQueryType typDst, out MethodInfo meth)
        {
            meth = null;
            if (object.Equals(typDst, XmlQueryTypeFactory.BooleanX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.Item))
                {
                    meth = XmlILMethods.ItemToBool;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.ItemS))
                {
                    meth = XmlILMethods.ItemsToBool;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.DateTimeX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.StringX))
                {
                    meth = XmlILMethods.StrToDT;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.DecimalX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.DoubleX))
                {
                    meth = XmlILMethods.DblToDec;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.DoubleX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.DecimalX))
                {
                    meth = XmlILMethods.DecToDbl;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.IntX))
                {
                    meth = XmlILMethods.IntToDbl;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.Item))
                {
                    meth = XmlILMethods.ItemToDbl;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.ItemS))
                {
                    meth = XmlILMethods.ItemsToDbl;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.LongX))
                {
                    meth = XmlILMethods.LngToDbl;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.StringX))
                {
                    meth = XmlILMethods.StrToDbl;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.IntX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.DoubleX))
                {
                    meth = XmlILMethods.DblToInt;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.LongX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.DoubleX))
                {
                    meth = XmlILMethods.DblToLng;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.NodeNotRtf))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.Item))
                {
                    meth = XmlILMethods.ItemToNode;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.ItemS))
                {
                    meth = XmlILMethods.ItemsToNode;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.NodeDodS) || object.Equals(typDst, XmlQueryTypeFactory.NodeNotRtfS))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.Item))
                {
                    meth = XmlILMethods.ItemToNodes;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.ItemS))
                {
                    meth = XmlILMethods.ItemsToNodes;
                }
            }
            else if (object.Equals(typDst, XmlQueryTypeFactory.StringX))
            {
                if (object.Equals(typSrc, XmlQueryTypeFactory.DateTimeX))
                {
                    meth = XmlILMethods.DTToStr;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.DoubleX))
                {
                    meth = XmlILMethods.DblToStr;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.Item))
                {
                    meth = XmlILMethods.ItemToStr;
                }
                else if (object.Equals(typSrc, XmlQueryTypeFactory.ItemS))
                {
                    meth = XmlILMethods.ItemsToStr;
                }
            }
            if (meth == null)
            {
                return false;
            }
            return true;
        }

        private bool HandleDodPatterns(QilUnary ndDod)
        {
            OptimizerPatterns patterns = OptimizerPatterns.Read(ndDod);
            bool flag = patterns.MatchesPattern(OptimizerPatternName.JoinAndDod);
            if (flag || patterns.MatchesPattern(OptimizerPatternName.DodReverse))
            {
                XmlNodeKindFlags element;
                QilName name;
                OptimizerPatterns patterns2 = OptimizerPatterns.Read((QilNode) patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                if (patterns2.MatchesPattern(OptimizerPatternName.FilterElements))
                {
                    element = XmlNodeKindFlags.Element;
                    name = (QilName) patterns2.GetArgument(OptimizerPatternArgument.ElementQName);
                }
                else if (patterns2.MatchesPattern(OptimizerPatternName.FilterContentKind))
                {
                    element = ((XmlQueryType) patterns2.GetArgument(OptimizerPatternArgument.ElementQName)).NodeKinds;
                    name = null;
                }
                else
                {
                    element = ((ndDod.XmlType.NodeKinds & XmlNodeKindFlags.Attribute) != XmlNodeKindFlags.None) ? XmlNodeKindFlags.Any : XmlNodeKindFlags.Content;
                    name = null;
                }
                QilNode argument = (QilNode) patterns2.GetArgument(OptimizerPatternArgument.StepNode);
                if (flag)
                {
                    switch (argument.NodeType)
                    {
                        case QilNodeType.XPathFollowing:
                            this.CreateContainerIterator(ndDod, "$$$iterFoll", typeof(XPathFollowingMergeIterator), XmlILMethods.XPFollMergeCreate, XmlILMethods.XPFollMergeNext, element, name, TriState.Unknown);
                            return true;

                        case QilNodeType.XPathPreceding:
                            this.CreateContainerIterator(ndDod, "$$$iterPrec", typeof(XPathPrecedingMergeIterator), XmlILMethods.XPPrecMergeCreate, XmlILMethods.XPPrecMergeNext, element, name, TriState.Unknown);
                            return true;

                        case QilNodeType.FollowingSibling:
                            this.CreateContainerIterator(ndDod, "$$$iterFollSib", typeof(FollowingSiblingMergeIterator), XmlILMethods.FollSibMergeCreate, XmlILMethods.FollSibMergeNext, element, name, TriState.Unknown);
                            return true;

                        case QilNodeType.Descendant:
                        case QilNodeType.DescendantOrSelf:
                            this.CreateContainerIterator(ndDod, "$$$iterDesc", typeof(DescendantMergeIterator), XmlILMethods.DescMergeCreate, XmlILMethods.DescMergeNext, element, name, (argument.NodeType == QilNodeType.Descendant) ? TriState.False : TriState.True);
                            return true;

                        case QilNodeType.Content:
                            this.CreateContainerIterator(ndDod, "$$$iterContent", typeof(ContentMergeIterator), XmlILMethods.ContentMergeCreate, XmlILMethods.ContentMergeNext, element, name, TriState.Unknown);
                            return true;
                    }
                }
                else
                {
                    QilNode ndCtxt = (QilNode) patterns2.GetArgument(OptimizerPatternArgument.StepInput);
                    switch (argument.NodeType)
                    {
                        case QilNodeType.Ancestor:
                        case QilNodeType.AncestorOrSelf:
                            this.CreateFilteredIterator(ndCtxt, "$$$iterAnc", typeof(AncestorDocOrderIterator), XmlILMethods.AncDOCreate, XmlILMethods.AncDONext, element, name, (argument.NodeType == QilNodeType.Ancestor) ? TriState.False : TriState.True, null);
                            return true;

                        case QilNodeType.PrecedingSibling:
                            this.CreateFilteredIterator(ndCtxt, "$$$iterPreSib", typeof(PrecedingSiblingDocOrderIterator), XmlILMethods.PreSibDOCreate, XmlILMethods.PreSibDONext, element, name, TriState.Unknown, null);
                            return true;

                        case QilNodeType.XPathPreceding:
                            this.CreateFilteredIterator(ndCtxt, "$$$iterPrec", typeof(XPathPrecedingDocOrderIterator), XmlILMethods.XPPrecDOCreate, XmlILMethods.XPPrecDONext, element, name, TriState.Unknown, null);
                            return true;
                    }
                }
            }
            else if (patterns.MatchesPattern(OptimizerPatternName.DodMerge))
            {
                LocalBuilder locBldr = this.helper.DeclareLocal("$$$dodMerge", typeof(DodSequenceMerge));
                Label lblOnEnd = this.helper.DefineLabel();
                this.helper.Emit(OpCodes.Ldloca, locBldr);
                this.helper.LoadQueryRuntime();
                this.helper.Call(XmlILMethods.DodMergeCreate);
                this.helper.Emit(OpCodes.Ldloca, locBldr);
                this.StartNestedIterator(ndDod.Child, lblOnEnd);
                this.Visit(ndDod.Child);
                this.iterCurr.EnsureStack();
                this.helper.Call(XmlILMethods.DodMergeAdd);
                this.helper.Emit(OpCodes.Ldloca, locBldr);
                this.iterCurr.LoopToEnd(lblOnEnd);
                this.EndNestedIterator(ndDod.Child);
                this.helper.Call(XmlILMethods.DodMergeSeq);
                this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathNavigator), true);
                return true;
            }
            return false;
        }

        private bool HandleFilterPatterns(QilLoop ndFilter)
        {
            QilNode argument;
            OptimizerPatterns patterns = OptimizerPatterns.Read(ndFilter);
            bool flag = patterns.MatchesPattern(OptimizerPatternName.FilterElements);
            if (!flag && !patterns.MatchesPattern(OptimizerPatternName.FilterContentKind))
            {
                if (patterns.MatchesPattern(OptimizerPatternName.FilterAttributeKind))
                {
                    argument = (QilNode) patterns.GetArgument(OptimizerPatternArgument.StepInput);
                    this.CreateSimpleIterator(argument, "$$$iterAttr", typeof(AttributeIterator), XmlILMethods.AttrCreate, XmlILMethods.AttrNext);
                    return true;
                }
                if (patterns.MatchesPattern(OptimizerPatternName.EqualityIndex))
                {
                    Label lblOnEnd = this.helper.DefineLabel();
                    Label lblVal = this.helper.DefineLabel();
                    QilIterator nd = (QilIterator) patterns.GetArgument(OptimizerPatternArgument.StepNode);
                    QilNode n = (QilNode) patterns.GetArgument(OptimizerPatternArgument.StepInput);
                    LocalBuilder locBldr = this.helper.DeclareLocal("$$$index", typeof(XmlILIndex));
                    this.helper.LoadQueryRuntime();
                    this.helper.Emit(OpCodes.Ldarg_1);
                    this.helper.LoadInteger(this.indexId);
                    this.helper.Emit(OpCodes.Ldloca, locBldr);
                    this.helper.Call(XmlILMethods.FindIndex);
                    this.helper.Emit(OpCodes.Brtrue, lblVal);
                    this.helper.LoadQueryRuntime();
                    this.helper.Emit(OpCodes.Ldarg_1);
                    this.helper.LoadInteger(this.indexId);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.StartNestedIterator(nd, lblOnEnd);
                    this.StartBinding(nd);
                    this.Visit(n);
                    this.iterCurr.EnsureStackNoCache();
                    this.VisitFor(nd);
                    this.iterCurr.EnsureStackNoCache();
                    this.iterCurr.EnsureItemStorageType(nd.XmlType, typeof(XPathNavigator));
                    this.helper.Call(XmlILMethods.IndexAdd);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.iterCurr.LoopToEnd(lblOnEnd);
                    this.EndBinding(nd);
                    this.EndNestedIterator(nd);
                    this.helper.Call(XmlILMethods.AddNewIndex);
                    this.helper.MarkLabel(lblVal);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.Emit(OpCodes.Ldarg_2);
                    this.helper.Call(XmlILMethods.IndexLookup);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathNavigator), true);
                    this.indexId++;
                    return true;
                }
            }
            else
            {
                XmlNodeKindFlags element;
                QilName name;
                if (flag)
                {
                    element = XmlNodeKindFlags.Element;
                    name = (QilName) patterns.GetArgument(OptimizerPatternArgument.ElementQName);
                }
                else
                {
                    element = ((XmlQueryType) patterns.GetArgument(OptimizerPatternArgument.ElementQName)).NodeKinds;
                    name = null;
                }
                QilNode node2 = (QilNode) patterns.GetArgument(OptimizerPatternArgument.StepNode);
                argument = (QilNode) patterns.GetArgument(OptimizerPatternArgument.StepInput);
                switch (node2.NodeType)
                {
                    case QilNodeType.Content:
                        LocalBuilder builder;
                        if (!flag)
                        {
                            if (element == XmlNodeKindFlags.Content)
                            {
                                this.CreateSimpleIterator(argument, "$$$iterContent", typeof(ContentIterator), XmlILMethods.ContentCreate, XmlILMethods.ContentNext);
                            }
                            else
                            {
                                builder = this.helper.DeclareLocal("$$$iterContent", typeof(NodeKindContentIterator));
                                this.helper.Emit(OpCodes.Ldloca, builder);
                                this.NestedVisitEnsureStack(argument);
                                this.helper.LoadInteger((int) this.QilXmlToXPathNodeType(element));
                                this.helper.Call(XmlILMethods.KindContentCreate);
                                this.GenerateSimpleIterator(typeof(XPathNavigator), builder, XmlILMethods.KindContentNext);
                            }
                            break;
                        }
                        builder = this.helper.DeclareLocal("$$$iterElemContent", typeof(ElementContentIterator));
                        this.helper.Emit(OpCodes.Ldloca, builder);
                        this.NestedVisitEnsureStack(argument);
                        this.helper.CallGetAtomizedName(this.helper.StaticData.DeclareName(name.LocalName));
                        this.helper.CallGetAtomizedName(this.helper.StaticData.DeclareName(name.NamespaceUri));
                        this.helper.Call(XmlILMethods.ElemContentCreate);
                        this.GenerateSimpleIterator(typeof(XPathNavigator), builder, XmlILMethods.ElemContentNext);
                        break;

                    case QilNodeType.Attribute:
                    case QilNodeType.Root:
                    case QilNodeType.XmlContext:
                        goto Label_05CC;

                    case QilNodeType.Parent:
                        this.CreateFilteredIterator(argument, "$$$iterPar", typeof(ParentIterator), XmlILMethods.ParentCreate, XmlILMethods.ParentNext, element, name, TriState.Unknown, null);
                        return true;

                    case QilNodeType.Descendant:
                    case QilNodeType.DescendantOrSelf:
                        this.CreateFilteredIterator(argument, "$$$iterDesc", typeof(DescendantIterator), XmlILMethods.DescCreate, XmlILMethods.DescNext, element, name, (node2.NodeType == QilNodeType.Descendant) ? TriState.False : TriState.True, null);
                        return true;

                    case QilNodeType.Ancestor:
                    case QilNodeType.AncestorOrSelf:
                        this.CreateFilteredIterator(argument, "$$$iterAnc", typeof(AncestorIterator), XmlILMethods.AncCreate, XmlILMethods.AncNext, element, name, (node2.NodeType == QilNodeType.Ancestor) ? TriState.False : TriState.True, null);
                        return true;

                    case QilNodeType.Preceding:
                        this.CreateFilteredIterator(argument, "$$$iterPrec", typeof(PrecedingIterator), XmlILMethods.PrecCreate, XmlILMethods.PrecNext, element, name, TriState.Unknown, null);
                        return true;

                    case QilNodeType.FollowingSibling:
                        this.CreateFilteredIterator(argument, "$$$iterFollSib", typeof(FollowingSiblingIterator), XmlILMethods.FollSibCreate, XmlILMethods.FollSibNext, element, name, TriState.Unknown, null);
                        return true;

                    case QilNodeType.PrecedingSibling:
                        this.CreateFilteredIterator(argument, "$$$iterPreSib", typeof(PrecedingSiblingIterator), XmlILMethods.PreSibCreate, XmlILMethods.PreSibNext, element, name, TriState.Unknown, null);
                        return true;

                    case QilNodeType.NodeRange:
                        this.CreateFilteredIterator(argument, "$$$iterRange", typeof(NodeRangeIterator), XmlILMethods.NodeRangeCreate, XmlILMethods.NodeRangeNext, element, name, TriState.Unknown, ((QilBinary) node2).Right);
                        return true;

                    case QilNodeType.XPathFollowing:
                        this.CreateFilteredIterator(argument, "$$$iterFoll", typeof(XPathFollowingIterator), XmlILMethods.XPFollCreate, XmlILMethods.XPFollNext, element, name, TriState.Unknown, null);
                        return true;

                    case QilNodeType.XPathPreceding:
                        this.CreateFilteredIterator(argument, "$$$iterPrec", typeof(XPathPrecedingIterator), XmlILMethods.XPPrecCreate, XmlILMethods.XPPrecNext, element, name, TriState.Unknown, null);
                        return true;

                    default:
                        goto Label_05CC;
                }
                return true;
            }
        Label_05CC:
            return false;
        }

        private static bool IsNodeTypeUnion(XmlNodeKindFlags xmlTypes) => 
            ((xmlTypes & (xmlTypes - 1)) != XmlNodeKindFlags.None);

        private GenerateNameType LoadNameAndType(XPathNodeType nodeType, QilNode ndName, bool isStart, bool callChk)
        {
            this.helper.LoadQueryOutput();
            GenerateNameType stackName = GenerateNameType.StackName;
            if (ndName.NodeType == QilNodeType.LiteralQName)
            {
                if (!isStart && callChk)
                {
                    return stackName;
                }
                QilName name = ndName as QilName;
                string prefix = name.Prefix;
                string localName = name.LocalName;
                string namespaceUri = name.NamespaceUri;
                if (name.NamespaceUri.Length == 0)
                {
                    this.helper.Emit(OpCodes.Ldstr, name.LocalName);
                    return GenerateNameType.LiteralLocalName;
                }
                if (!ValidateNames.ValidateName(prefix, localName, namespaceUri, nodeType, ValidateNames.Flags.CheckPrefixMapping))
                {
                    if (isStart)
                    {
                        this.helper.Emit(OpCodes.Ldstr, localName);
                        this.helper.Emit(OpCodes.Ldstr, namespaceUri);
                        this.helper.Construct(XmlILConstructors.QName);
                        stackName = GenerateNameType.QName;
                    }
                    return stackName;
                }
                this.helper.Emit(OpCodes.Ldstr, prefix);
                this.helper.Emit(OpCodes.Ldstr, localName);
                this.helper.Emit(OpCodes.Ldstr, namespaceUri);
                return GenerateNameType.LiteralName;
            }
            if (!isStart)
            {
                return stackName;
            }
            if (ndName.NodeType == QilNodeType.NameOf)
            {
                this.NestedVisitEnsureStack((ndName as QilUnary).Child);
                return GenerateNameType.CopiedName;
            }
            if (ndName.NodeType == QilNodeType.StrParseQName)
            {
                this.VisitStrParseQName(ndName as QilBinary, true);
                if ((ndName as QilBinary).Right.XmlType.TypeCode == XmlTypeCode.String)
                {
                    return GenerateNameType.TagNameAndNamespace;
                }
                return GenerateNameType.TagNameAndMappings;
            }
            this.NestedVisitEnsureStack(ndName);
            return GenerateNameType.QName;
        }

        private void LoadSelectFilter(XmlNodeKindFlags xmlTypes, QilName ndName)
        {
            if (ndName != null)
            {
                this.helper.CallGetNameFilter(this.helper.StaticData.DeclareNameFilter(ndName.LocalName, ndName.NamespaceUri));
            }
            else if (IsNodeTypeUnion(xmlTypes))
            {
                if ((xmlTypes & XmlNodeKindFlags.Attribute) != XmlNodeKindFlags.None)
                {
                    this.helper.CallGetTypeFilter(XPathNodeType.All);
                }
                else
                {
                    this.helper.CallGetTypeFilter(XPathNodeType.Attribute);
                }
            }
            else
            {
                this.helper.CallGetTypeFilter(this.QilXmlToXPathNodeType(xmlTypes));
            }
        }

        private bool MatchesNodeKinds(QilTargetType ndIsType, XmlQueryType typDerived, XmlQueryType typBase)
        {
            XPathNodeType all;
            bool flag = true;
            if (!typBase.IsNode || !typBase.IsSingleton)
            {
                return false;
            }
            if ((!typDerived.IsNode || !typDerived.IsSingleton) || !typDerived.IsNotRtf)
            {
                return false;
            }
            XmlNodeKindFlags none = XmlNodeKindFlags.None;
            foreach (XmlQueryType type2 in typBase)
            {
                if (object.Equals(type2, XmlQueryTypeFactory.Element))
                {
                    none |= XmlNodeKindFlags.Element;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.Attribute))
                {
                    none |= XmlNodeKindFlags.Attribute;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.Text))
                {
                    none |= XmlNodeKindFlags.Text;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.Document))
                {
                    none |= XmlNodeKindFlags.Document;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.Comment))
                {
                    none |= XmlNodeKindFlags.Comment;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.PI))
                {
                    none |= XmlNodeKindFlags.PI;
                }
                else if (object.Equals(type2, XmlQueryTypeFactory.Namespace))
                {
                    none |= XmlNodeKindFlags.Namespace;
                }
                else
                {
                    return false;
                }
            }
            none = typDerived.NodeKinds & none;
            if (!Bits.ExactlyOne((uint) none))
            {
                none = ~none & XmlNodeKindFlags.Any;
                flag = !flag;
            }
            XmlNodeKindFlags flags2 = none;
            if (flags2 <= XmlNodeKindFlags.Comment)
            {
                switch (flags2)
                {
                    case XmlNodeKindFlags.Document:
                        all = XPathNodeType.Root;
                        goto Label_017B;

                    case XmlNodeKindFlags.Element:
                        all = XPathNodeType.Element;
                        goto Label_017B;

                    case XmlNodeKindFlags.Attribute:
                        all = XPathNodeType.Attribute;
                        goto Label_017B;

                    case XmlNodeKindFlags.Comment:
                        all = XPathNodeType.Comment;
                        goto Label_017B;
                }
            }
            else
            {
                if (flags2 != XmlNodeKindFlags.PI)
                {
                    if (flags2 != XmlNodeKindFlags.Namespace)
                    {
                        goto Label_0168;
                    }
                    all = XPathNodeType.Namespace;
                }
                else
                {
                    all = XPathNodeType.ProcessingInstruction;
                }
                goto Label_017B;
            }
        Label_0168:
            this.helper.Emit(OpCodes.Ldc_I4_1);
            all = XPathNodeType.All;
        Label_017B:
            this.NestedVisitEnsureStack(ndIsType.Source);
            this.helper.Call(XmlILMethods.NavType);
            if (all == XPathNodeType.All)
            {
                this.helper.Emit(OpCodes.Shl);
                int intVal = 0;
                if ((none & XmlNodeKindFlags.Document) != XmlNodeKindFlags.None)
                {
                    intVal |= 1;
                }
                if ((none & XmlNodeKindFlags.Element) != XmlNodeKindFlags.None)
                {
                    intVal |= 2;
                }
                if ((none & XmlNodeKindFlags.Attribute) != XmlNodeKindFlags.None)
                {
                    intVal |= 4;
                }
                if ((none & XmlNodeKindFlags.Text) != XmlNodeKindFlags.None)
                {
                    intVal |= 0x70;
                }
                if ((none & XmlNodeKindFlags.Comment) != XmlNodeKindFlags.None)
                {
                    intVal |= 0x100;
                }
                if ((none & XmlNodeKindFlags.PI) != XmlNodeKindFlags.None)
                {
                    intVal |= 0x80;
                }
                if ((none & XmlNodeKindFlags.Namespace) != XmlNodeKindFlags.None)
                {
                    intVal |= 8;
                }
                this.helper.LoadInteger(intVal);
                this.helper.Emit(OpCodes.And);
                this.ZeroCompare(flag ? QilNodeType.Ne : QilNodeType.Eq, false);
            }
            else
            {
                this.helper.LoadInteger((int) all);
                this.ClrCompare(flag ? QilNodeType.Eq : QilNodeType.Ne, XmlTypeCode.Int);
            }
            return true;
        }

        private bool MightHaveNamespacesAfterAttributes(XmlILConstructInfo info)
        {
            if (info != null)
            {
                info = info.ParentElementInfo;
            }
            return ((info == null) || info.MightHaveNamespacesAfterAttributes);
        }

        private void NestedConstruction(QilNode nd)
        {
            this.helper.CallStartSequenceConstruction();
            base.Visit(nd);
            this.helper.CallEndSequenceConstruction();
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathItem), true);
        }

        private void NestedVisit(QilNode nd)
        {
            this.NestedVisit(nd, this.GetItemStorageType(nd), !nd.XmlType.IsSingleton);
        }

        private void NestedVisit(QilNode nd, Label lblOnEnd)
        {
            this.StartNestedIterator(nd, lblOnEnd);
            this.Visit(nd);
            this.iterCurr.EnsureNoCache();
            this.iterCurr.EnsureItemStorageType(nd.XmlType, this.GetItemStorageType(nd));
            this.EndNestedIterator(nd);
            this.iterCurr.Storage = this.iterNested.Storage;
        }

        private void NestedVisit(QilNode nd, Type itemStorageType, bool isCached)
        {
            if (XmlILConstructInfo.Read(nd).PushToWriterLast)
            {
                this.StartNestedIterator(nd);
                this.Visit(nd);
                this.EndNestedIterator(nd);
                this.iterCurr.Storage = StorageDescriptor.None();
            }
            else if (!isCached && nd.XmlType.IsSingleton)
            {
                this.StartNestedIterator(nd);
                this.Visit(nd);
                this.iterCurr.EnsureNoCache();
                this.iterCurr.EnsureItemStorageType(nd.XmlType, itemStorageType);
                this.EndNestedIterator(nd);
                this.iterCurr.Storage = this.iterNested.Storage;
            }
            else
            {
                this.NestedVisitEnsureCache(nd, itemStorageType);
            }
        }

        private void NestedVisitEnsureCache(QilNode nd, Type itemStorageType)
        {
            bool flag = this.CachesResult(nd);
            Label lblOnEnd = this.helper.DefineLabel();
            if (flag)
            {
                this.StartNestedIterator(nd);
                this.Visit(nd);
                this.EndNestedIterator(nd);
                this.iterCurr.Storage = this.iterNested.Storage;
                if (this.iterCurr.Storage.ItemStorageType == itemStorageType)
                {
                    return;
                }
                if ((this.iterCurr.Storage.ItemStorageType == typeof(XPathNavigator)) || (itemStorageType == typeof(XPathNavigator)))
                {
                    this.iterCurr.EnsureItemStorageType(nd.XmlType, itemStorageType);
                    return;
                }
                this.iterCurr.EnsureNoStack("$$$cacheResult");
            }
            Type type = (this.GetItemStorageType(nd) == typeof(XPathNavigator)) ? typeof(XPathNavigator) : itemStorageType;
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[type];
            LocalBuilder locBldr = this.helper.DeclareLocal("$$$cache", methods.SeqType);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            if (nd.XmlType.IsSingleton)
            {
                this.NestedVisitEnsureStack(nd, type, false);
                this.helper.CallToken(methods.SeqReuseSgl);
                this.helper.Emit(OpCodes.Stloc, locBldr);
            }
            else
            {
                this.helper.CallToken(methods.SeqReuse);
                this.helper.Emit(OpCodes.Stloc, locBldr);
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.StartNestedIterator(nd, lblOnEnd);
                if (flag)
                {
                    this.iterCurr.Storage = this.iterCurr.ParentIterator.Storage;
                }
                else
                {
                    this.Visit(nd);
                }
                this.iterCurr.EnsureItemStorageType(nd.XmlType, type);
                this.iterCurr.EnsureStackNoCache();
                this.helper.Call(methods.SeqAdd);
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.iterCurr.LoopToEnd(lblOnEnd);
                this.EndNestedIterator(nd);
                this.helper.Emit(OpCodes.Pop);
            }
            this.iterCurr.Storage = StorageDescriptor.Local(locBldr, itemStorageType, true);
        }

        private void NestedVisitEnsureLocal(QilNode nd, LocalBuilder loc)
        {
            this.NestedVisit(nd);
            this.iterCurr.EnsureLocal(loc);
        }

        private void NestedVisitEnsureStack(QilNode nd)
        {
            this.NestedVisit(nd);
            this.iterCurr.EnsureStack();
        }

        private void NestedVisitEnsureStack(QilNode ndLeft, QilNode ndRight)
        {
            this.NestedVisitEnsureStack(ndLeft);
            this.NestedVisitEnsureStack(ndRight);
        }

        private void NestedVisitEnsureStack(QilNode nd, Type itemStorageType, bool isCached)
        {
            this.NestedVisit(nd, itemStorageType, isCached);
            this.iterCurr.EnsureStack();
        }

        private void NestedVisitWithBranch(QilNode nd, BranchingContext brctxt, Label lblBranch)
        {
            this.StartNestedIterator(nd);
            this.iterCurr.SetBranching(brctxt, lblBranch);
            this.Visit(nd);
            this.EndNestedIterator(nd);
            this.iterCurr.Storage = StorageDescriptor.None();
        }

        private void PrepareGlobalValues(QilList globalIterators)
        {
            foreach (QilIterator iterator in globalIterators)
            {
                MethodInfo functionBinding = XmlILAnnotation.Write(iterator).FunctionBinding;
                IteratorDescriptor descriptor = new IteratorDescriptor(this.helper) {
                    Storage = StorageDescriptor.Global(functionBinding, this.GetItemStorageType(iterator), !iterator.XmlType.IsSingleton)
                };
                XmlILAnnotation.Write(iterator).CachedIteratorDescriptor = descriptor;
            }
        }

        private XPathNodeType QilConstructorToNodeType(QilNodeType typ)
        {
            switch (typ)
            {
                case QilNodeType.ElementCtor:
                    return XPathNodeType.Element;

                case QilNodeType.AttributeCtor:
                    return XPathNodeType.Attribute;

                case QilNodeType.CommentCtor:
                    return XPathNodeType.Comment;

                case QilNodeType.PICtor:
                    return XPathNodeType.ProcessingInstruction;

                case QilNodeType.TextCtor:
                    return XPathNodeType.Text;

                case QilNodeType.RawTextCtor:
                    return XPathNodeType.Text;

                case QilNodeType.DocumentCtor:
                    return XPathNodeType.Root;

                case QilNodeType.NamespaceDecl:
                    return XPathNodeType.Namespace;
            }
            return XPathNodeType.All;
        }

        private XPathNodeType QilXmlToXPathNodeType(XmlNodeKindFlags xmlTypes)
        {
            switch (xmlTypes)
            {
                case XmlNodeKindFlags.Element:
                    return XPathNodeType.Element;

                case XmlNodeKindFlags.Attribute:
                    return XPathNodeType.Attribute;

                case XmlNodeKindFlags.Text:
                    return XPathNodeType.Text;

                case XmlNodeKindFlags.Comment:
                    return XPathNodeType.Comment;
            }
            return XPathNodeType.ProcessingInstruction;
        }

        private void Sequence(QilList ndSeq)
        {
            Label lblOnEnd = new Label();
            Type itemStorageType = this.GetItemStorageType(ndSeq);
            if (ndSeq.XmlType.IsSingleton)
            {
                foreach (QilNode node in ndSeq)
                {
                    if (node.XmlType.IsSingleton)
                    {
                        this.NestedVisitEnsureStack(node);
                    }
                    else
                    {
                        lblOnEnd = this.helper.DefineLabel();
                        this.NestedVisit(node, lblOnEnd);
                        this.iterCurr.DiscardStack();
                        this.helper.MarkLabel(lblOnEnd);
                    }
                }
                this.iterCurr.Storage = StorageDescriptor.Stack(itemStorageType, false);
            }
            else
            {
                LocalBuilder bldr = this.helper.DeclareLocal("$$$itemList", itemStorageType);
                LocalBuilder locBldr = this.helper.DeclareLocal("$$$idxList", typeof(int));
                Label[] arrLabels = new Label[ndSeq.Count];
                Label lblTarget = this.helper.DefineLabel();
                for (int i = 0; i < ndSeq.Count; i++)
                {
                    if (i != 0)
                    {
                        this.helper.MarkLabel(lblOnEnd);
                    }
                    if (i == (ndSeq.Count - 1))
                    {
                        lblOnEnd = this.iterCurr.GetLabelNext();
                    }
                    else
                    {
                        lblOnEnd = this.helper.DefineLabel();
                    }
                    this.helper.LoadInteger(i);
                    this.helper.Emit(OpCodes.Stloc, locBldr);
                    this.NestedVisit(ndSeq[i], lblOnEnd);
                    this.iterCurr.EnsureItemStorageType(ndSeq[i].XmlType, itemStorageType);
                    this.iterCurr.EnsureLocalNoCache(bldr);
                    arrLabels[i] = this.iterNested.GetLabelNext();
                    this.helper.EmitUnconditionalBranch(OpCodes.Brtrue, lblTarget);
                }
                Label lbl = this.helper.DefineLabel();
                this.helper.MarkLabel(lbl);
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.helper.Emit(OpCodes.Switch, arrLabels);
                this.helper.MarkLabel(lblTarget);
                this.iterCurr.SetIterator(lbl, StorageDescriptor.Local(bldr, itemStorageType, false));
            }
        }

        private void StartBinding(QilIterator ndIter)
        {
            OptimizerPatterns patt = OptimizerPatterns.Read(ndIter);
            if (this.qil.IsDebug && (ndIter.SourceLine != null))
            {
                this.helper.DebugSequencePoint(ndIter.SourceLine);
            }
            if ((ndIter.NodeType == QilNodeType.For) || ndIter.XmlType.IsSingleton)
            {
                this.StartForBinding(ndIter, patt);
            }
            else
            {
                this.StartLetBinding(ndIter);
            }
            XmlILAnnotation.Write(ndIter).CachedIteratorDescriptor = this.iterNested;
        }

        private Label StartConjunctiveTests(BranchingContext brctxt, Label lblBranch)
        {
            if (brctxt == BranchingContext.OnFalse)
            {
                this.iterCurr.SetBranching(BranchingContext.OnFalse, lblBranch);
                return lblBranch;
            }
            Label label = this.helper.DefineLabel();
            this.iterCurr.SetBranching(BranchingContext.OnFalse, label);
            return label;
        }

        private void StartForBinding(QilIterator ndFor, OptimizerPatterns patt)
        {
            LocalBuilder locBldr = null;
            if (this.iterCurr.HasLabelNext)
            {
                this.StartNestedIterator(ndFor.Binding, this.iterCurr.GetLabelNext());
            }
            else
            {
                this.StartNestedIterator(ndFor.Binding);
            }
            if (patt.MatchesPattern(OptimizerPatternName.IsPositional))
            {
                locBldr = this.helper.DeclareLocal("$$$pos", typeof(int));
                this.helper.Emit(OpCodes.Ldc_I4_0);
                this.helper.Emit(OpCodes.Stloc, locBldr);
            }
            this.Visit(ndFor.Binding);
            if (this.qil.IsDebug && (ndFor.DebugName != null))
            {
                this.helper.DebugStartScope();
                this.iterCurr.EnsureLocalNoCache("$$$for");
                this.iterCurr.Storage.LocalLocation.SetLocalSymInfo(ndFor.DebugName);
            }
            else
            {
                this.iterCurr.EnsureNoStackNoCache("$$$for");
            }
            if (patt.MatchesPattern(OptimizerPatternName.IsPositional))
            {
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.helper.Emit(OpCodes.Ldc_I4_1);
                this.helper.Emit(OpCodes.Add);
                this.helper.Emit(OpCodes.Stloc, locBldr);
                if (patt.MatchesPattern(OptimizerPatternName.MaxPosition))
                {
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.LoadInteger((int) patt.GetArgument(OptimizerPatternArgument.ElementQName));
                    this.helper.Emit(OpCodes.Bgt, this.iterCurr.ParentIterator.GetLabelNext());
                }
                this.iterCurr.LocalPosition = locBldr;
            }
            this.EndNestedIterator(ndFor.Binding);
            this.iterCurr.SetIterator(this.iterNested);
        }

        private void StartLastConjunctiveTest(BranchingContext brctxt, Label lblBranch, Label lblOnFalse)
        {
            if (brctxt == BranchingContext.OnTrue)
            {
                this.iterCurr.SetBranching(BranchingContext.OnTrue, lblBranch);
            }
            else
            {
                this.iterCurr.SetBranching(BranchingContext.OnFalse, lblOnFalse);
            }
        }

        public void StartLetBinding(QilIterator ndLet)
        {
            this.StartNestedIterator(ndLet);
            this.NestedVisit(ndLet.Binding, this.GetItemStorageType(ndLet), !ndLet.XmlType.IsSingleton);
            if (this.qil.IsDebug && (ndLet.DebugName != null))
            {
                this.helper.DebugStartScope();
                this.iterCurr.EnsureLocal("$$$cache");
                this.iterCurr.Storage.LocalLocation.SetLocalSymInfo(ndLet.DebugName);
            }
            else
            {
                this.iterCurr.EnsureNoStack("$$$cache");
            }
            this.EndNestedIterator(ndLet);
        }

        private void StartNestedIterator(QilNode nd)
        {
            IteratorDescriptor iterCurr = this.iterCurr;
            if (iterCurr == null)
            {
                this.iterCurr = new IteratorDescriptor(this.helper);
            }
            else
            {
                this.iterCurr = new IteratorDescriptor(iterCurr);
            }
            this.iterNested = null;
        }

        private void StartNestedIterator(QilNode nd, Label lblOnEnd)
        {
            this.StartNestedIterator(nd);
            this.iterCurr.SetIterator(lblOnEnd, StorageDescriptor.None());
        }

        private void StartWriterLoop(QilNode nd, out bool hasOnEnd, out Label lblOnEnd)
        {
            XmlILConstructInfo info = XmlILConstructInfo.Read(nd);
            hasOnEnd = false;
            lblOnEnd = new Label();
            if ((info.PushToWriterLast && !nd.XmlType.IsSingleton) && !this.iterCurr.HasLabelNext)
            {
                hasOnEnd = true;
                lblOnEnd = this.helper.DefineLabel();
                this.iterCurr.SetIterator(lblOnEnd, StorageDescriptor.None());
            }
        }

        private void SyncToNavigator(LocalBuilder locNav, QilNode ndCtxt)
        {
            this.helper.Emit(OpCodes.Ldloc, locNav);
            this.NestedVisitEnsureStack(ndCtxt);
            this.helper.CallSyncToNavigator();
            this.helper.Emit(OpCodes.Stloc, locNav);
        }

        private bool TryNameCompare(QilNodeType relOp, QilNode ndFirst, QilNode ndSecond)
        {
            if (ndFirst.NodeType == QilNodeType.NameOf)
            {
                switch (ndSecond.NodeType)
                {
                    case QilNodeType.LiteralQName:
                    case QilNodeType.NameOf:
                        this.helper.LoadQueryRuntime();
                        this.NestedVisitEnsureStack((ndFirst as QilUnary).Child);
                        if (ndSecond.NodeType == QilNodeType.LiteralQName)
                        {
                            QilName name = ndSecond as QilName;
                            this.helper.LoadInteger(this.helper.StaticData.DeclareName(name.LocalName));
                            this.helper.LoadInteger(this.helper.StaticData.DeclareName(name.NamespaceUri));
                            this.helper.Call(XmlILMethods.QNameEqualLit);
                        }
                        else
                        {
                            this.NestedVisitEnsureStack(ndSecond);
                            this.helper.Call(XmlILMethods.QNameEqualNav);
                        }
                        this.ZeroCompare((relOp == QilNodeType.Eq) ? QilNodeType.Ne : QilNodeType.Eq, true);
                        return true;
                }
            }
            return false;
        }

        private bool TryZeroCompare(QilNodeType relOp, QilNode ndFirst, QilNode ndSecond)
        {
            switch (ndFirst.NodeType)
            {
                case QilNodeType.True:
                    relOp = (relOp == QilNodeType.Eq) ? QilNodeType.Ne : QilNodeType.Eq;
                    break;

                case QilNodeType.False:
                    break;

                case QilNodeType.LiteralInt32:
                    if (((QilLiteral) ndFirst) == null)
                    {
                        break;
                    }
                    return false;

                case QilNodeType.LiteralInt64:
                    if (((QilLiteral) ndFirst) == null)
                    {
                        break;
                    }
                    return false;

                default:
                    return false;
            }
            this.NestedVisitEnsureStack(ndSecond);
            this.ZeroCompare(relOp, ndSecond.XmlType.TypeCode == XmlTypeCode.Boolean);
            return true;
        }

        protected override QilNode Visit(QilNode nd)
        {
            if (nd == null)
            {
                return null;
            }
            if ((this.qil.IsDebug && (nd.SourceLine != null)) && !(nd is QilIterator))
            {
                this.helper.DebugSequencePoint(nd.SourceLine);
            }
            switch (XmlILConstructInfo.Read(nd).ConstructMethod)
            {
                case XmlILConstructMethod.WriterThenIterator:
                    this.NestedConstruction(nd);
                    return nd;

                case XmlILConstructMethod.IteratorThenWriter:
                    this.CopySequence(nd);
                    return nd;
            }
            base.Visit(nd);
            return nd;
        }

        public void Visit(QilExpression qil, GenerateHelper helper, MethodInfo methRoot)
        {
            this.qil = qil;
            this.helper = helper;
            this.iterNested = null;
            this.indexId = 0;
            this.PrepareGlobalValues(qil.GlobalParameterList);
            this.PrepareGlobalValues(qil.GlobalVariableList);
            this.VisitGlobalValues(qil.GlobalParameterList);
            this.VisitGlobalValues(qil.GlobalVariableList);
            foreach (QilFunction function in qil.FunctionList)
            {
                this.Function(function);
            }
            this.helper.MethodBegin(methRoot, null, true);
            this.StartNestedIterator(qil.Root);
            this.Visit(qil.Root);
            this.EndNestedIterator(qil.Root);
            this.helper.MethodEnd();
        }

        protected override QilNode VisitAdd(QilBinary ndPlus) => 
            this.ArithmeticOp(ndPlus);

        protected override QilNode VisitAfter(QilBinary ndAfter)
        {
            this.ComparePosition(ndAfter);
            return ndAfter;
        }

        protected override QilNode VisitAncestor(QilUnary ndAnc)
        {
            this.CreateFilteredIterator(ndAnc.Child, "$$$iterAnc", typeof(AncestorIterator), XmlILMethods.AncCreate, XmlILMethods.AncNext, XmlNodeKindFlags.Any, null, TriState.False, null);
            return ndAnc;
        }

        protected override QilNode VisitAncestorOrSelf(QilUnary ndAnc)
        {
            this.CreateFilteredIterator(ndAnc.Child, "$$$iterAnc", typeof(AncestorIterator), XmlILMethods.AncCreate, XmlILMethods.AncNext, XmlNodeKindFlags.Any, null, TriState.True, null);
            return ndAnc;
        }

        protected override QilNode VisitAnd(QilBinary ndAnd)
        {
            IteratorDescriptor iterCurr = this.iterCurr;
            this.StartNestedIterator(ndAnd.Left);
            Label lblOnFalse = this.StartConjunctiveTests(iterCurr.CurrentBranchingContext, iterCurr.LabelBranch);
            this.Visit(ndAnd.Left);
            this.EndNestedIterator(ndAnd.Left);
            this.StartNestedIterator(ndAnd.Right);
            this.StartLastConjunctiveTest(iterCurr.CurrentBranchingContext, iterCurr.LabelBranch, lblOnFalse);
            this.Visit(ndAnd.Right);
            this.EndNestedIterator(ndAnd.Right);
            this.EndConjunctiveTests(iterCurr.CurrentBranchingContext, iterCurr.LabelBranch, lblOnFalse);
            return ndAnd;
        }

        protected override QilNode VisitAttribute(QilBinary ndAttr)
        {
            QilName right = ndAttr.Right as QilName;
            LocalBuilder locNav = this.helper.DeclareLocal("$$$navAttr", typeof(XPathNavigator));
            this.SyncToNavigator(locNav, ndAttr.Left);
            this.helper.Emit(OpCodes.Ldloc, locNav);
            this.helper.CallGetAtomizedName(this.helper.StaticData.DeclareName(right.LocalName));
            this.helper.CallGetAtomizedName(this.helper.StaticData.DeclareName(right.NamespaceUri));
            this.helper.Call(XmlILMethods.NavMoveAttr);
            this.helper.Emit(OpCodes.Brfalse, this.iterCurr.GetLabelNext());
            this.iterCurr.Storage = StorageDescriptor.Local(locNav, typeof(XPathNavigator), false);
            return ndAttr;
        }

        protected override QilNode VisitAttributeCtor(QilBinary ndAttr)
        {
            XmlILConstructInfo info = XmlILConstructInfo.Read(ndAttr);
            bool callChk = this.CheckEnumAttrs(info) || !info.IsNamespaceInScope;
            if (!callChk)
            {
                this.BeforeStartChecks(ndAttr);
            }
            GenerateNameType nameType = this.LoadNameAndType(XPathNodeType.Attribute, ndAttr.Left, true, callChk);
            this.helper.CallWriteStartAttribute(nameType, callChk);
            this.NestedVisit(ndAttr.Right);
            this.helper.CallWriteEndAttribute(callChk);
            if (!callChk)
            {
                this.AfterEndChecks(ndAttr);
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndAttr;
        }

        protected override QilNode VisitAverage(QilUnary ndAvg)
        {
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[this.GetItemStorageType(ndAvg)];
            return this.CreateAggregator(ndAvg, "$$$aggAvg", methods, methods.AggAvg, methods.AggAvgResult);
        }

        protected override QilNode VisitBefore(QilBinary ndBefore)
        {
            this.ComparePosition(ndBefore);
            return ndBefore;
        }

        protected override QilNode VisitChildren(QilNode parent) => 
            parent;

        protected override QilNode VisitChoice(QilChoice ndChoice)
        {
            int num2;
            this.NestedVisit(ndChoice.Expression);
            QilNode branches = ndChoice.Branches;
            int num = branches.Count - 1;
            Label[] arrLabels = new Label[num];
            for (num2 = 0; num2 < num; num2++)
            {
                arrLabels[num2] = this.helper.DefineLabel();
            }
            Label lblTarget = this.helper.DefineLabel();
            Label label2 = this.helper.DefineLabel();
            this.helper.Emit(OpCodes.Switch, arrLabels);
            this.helper.EmitUnconditionalBranch(OpCodes.Br, lblTarget);
            num2 = 0;
            while (num2 < num)
            {
                this.helper.MarkLabel(arrLabels[num2]);
                this.NestedVisit(branches[num2]);
                this.helper.EmitUnconditionalBranch(OpCodes.Br, label2);
                num2++;
            }
            this.helper.MarkLabel(lblTarget);
            this.NestedVisit(branches[num2]);
            this.helper.MarkLabel(label2);
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndChoice;
        }

        protected override QilNode VisitCommentCtor(QilUnary ndComment)
        {
            this.helper.CallWriteStartComment();
            this.NestedVisit(ndComment.Child);
            this.helper.CallWriteEndComment();
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndComment;
        }

        protected override QilNode VisitConditional(QilTernary ndCond)
        {
            if (XmlILConstructInfo.Read(ndCond).ConstructMethod == XmlILConstructMethod.Writer)
            {
                Label label = this.helper.DefineLabel();
                this.NestedVisitWithBranch(ndCond.Left, BranchingContext.OnFalse, label);
                this.NestedVisit(ndCond.Center);
                if ((ndCond.Right.NodeType == QilNodeType.Sequence) && (ndCond.Right.Count == 0))
                {
                    this.helper.MarkLabel(label);
                    this.NestedVisit(ndCond.Right);
                }
                else
                {
                    Label label2 = this.helper.DefineLabel();
                    this.helper.EmitUnconditionalBranch(OpCodes.Br, label2);
                    this.helper.MarkLabel(label);
                    this.NestedVisit(ndCond.Right);
                    this.helper.MarkLabel(label2);
                }
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndCond;
            }
            LocalBuilder loc = null;
            LocalBuilder locResult = null;
            Type itemStorageType = this.GetItemStorageType(ndCond);
            Label lblBranch = this.helper.DefineLabel();
            if (ndCond.XmlType.IsSingleton)
            {
                this.NestedVisitWithBranch(ndCond.Left, BranchingContext.OnFalse, lblBranch);
            }
            else
            {
                locResult = this.helper.DeclareLocal("$$$cond", itemStorageType);
                loc = this.helper.DeclareLocal("$$$boolResult", typeof(bool));
                this.NestedVisitEnsureLocal(ndCond.Left, loc);
                this.helper.Emit(OpCodes.Ldloc, loc);
                this.helper.Emit(OpCodes.Brfalse, lblBranch);
            }
            this.ConditionalBranch(ndCond.Center, itemStorageType, locResult);
            IteratorDescriptor iterNested = this.iterNested;
            Label lblTarget = this.helper.DefineLabel();
            this.helper.EmitUnconditionalBranch(OpCodes.Br, lblTarget);
            this.helper.MarkLabel(lblBranch);
            this.ConditionalBranch(ndCond.Right, itemStorageType, locResult);
            if (!ndCond.XmlType.IsSingleton)
            {
                this.helper.EmitUnconditionalBranch(OpCodes.Brtrue, lblTarget);
                Label lbl = this.helper.DefineLabel();
                this.helper.MarkLabel(lbl);
                this.helper.Emit(OpCodes.Ldloc, loc);
                this.helper.Emit(OpCodes.Brtrue, iterNested.GetLabelNext());
                this.helper.EmitUnconditionalBranch(OpCodes.Br, this.iterNested.GetLabelNext());
                this.iterCurr.SetIterator(lbl, StorageDescriptor.Local(locResult, itemStorageType, false));
            }
            this.helper.MarkLabel(lblTarget);
            return ndCond;
        }

        protected override QilNode VisitContent(QilUnary ndContent)
        {
            this.CreateSimpleIterator(ndContent.Child, "$$$iterAttrContent", typeof(AttributeContentIterator), XmlILMethods.AttrContentCreate, XmlILMethods.AttrContentNext);
            return ndContent;
        }

        protected override QilNode VisitDataSource(QilDataSource ndSrc)
        {
            this.helper.LoadQueryContext();
            this.NestedVisitEnsureStack(ndSrc.Name);
            this.NestedVisitEnsureStack(ndSrc.BaseUri);
            this.helper.Call(XmlILMethods.GetDataSource);
            LocalBuilder locBldr = this.helper.DeclareLocal("$$$navDoc", typeof(XPathNavigator));
            this.helper.Emit(OpCodes.Stloc, locBldr);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            this.helper.Emit(OpCodes.Brfalse, this.iterCurr.GetLabelNext());
            this.iterCurr.Storage = StorageDescriptor.Local(locBldr, typeof(XPathNavigator), false);
            return ndSrc;
        }

        protected override QilNode VisitDeref(QilBinary ndDeref)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal("$$$iterId", typeof(IdIterator));
            this.helper.Emit(OpCodes.Ldloca, locBldr);
            this.NestedVisitEnsureStack(ndDeref.Left);
            this.NestedVisitEnsureStack(ndDeref.Right);
            this.helper.Call(XmlILMethods.IdCreate);
            this.GenerateSimpleIterator(typeof(XPathNavigator), locBldr, XmlILMethods.IdNext);
            return ndDeref;
        }

        protected override QilNode VisitDescendant(QilUnary ndDesc)
        {
            this.CreateFilteredIterator(ndDesc.Child, "$$$iterDesc", typeof(DescendantIterator), XmlILMethods.DescCreate, XmlILMethods.DescNext, XmlNodeKindFlags.Any, null, TriState.False, null);
            return ndDesc;
        }

        protected override QilNode VisitDescendantOrSelf(QilUnary ndDesc)
        {
            this.CreateFilteredIterator(ndDesc.Child, "$$$iterDesc", typeof(DescendantIterator), XmlILMethods.DescCreate, XmlILMethods.DescNext, XmlNodeKindFlags.Any, null, TriState.True, null);
            return ndDesc;
        }

        protected override QilNode VisitDifference(QilBinary ndDiff) => 
            this.CreateSetIterator(ndDiff, "$$$iterDiff", typeof(DifferenceIterator), XmlILMethods.DiffCreate, XmlILMethods.DiffNext);

        protected override QilNode VisitDivide(QilBinary ndDiv) => 
            this.ArithmeticOp(ndDiv);

        protected override QilNode VisitDocOrderDistinct(QilUnary ndDod)
        {
            if (ndDod.XmlType.IsSingleton)
            {
                return this.Visit(ndDod.Child);
            }
            if (!this.HandleDodPatterns(ndDod))
            {
                this.helper.LoadQueryRuntime();
                this.NestedVisitEnsureCache(ndDod.Child, typeof(XPathNavigator));
                this.iterCurr.EnsureStack();
                this.helper.Call(XmlILMethods.DocOrder);
            }
            return ndDod;
        }

        protected override QilNode VisitDocumentCtor(QilUnary ndDoc)
        {
            this.helper.CallWriteStartRoot();
            this.NestedVisit(ndDoc.Child);
            this.helper.CallWriteEndRoot();
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndDoc;
        }

        protected override QilNode VisitElementCtor(QilBinary ndElem)
        {
            XmlILConstructInfo info = XmlILConstructInfo.Read(ndElem);
            bool callChk = (this.CheckWithinContent(info) || !info.IsNamespaceInScope) || this.ElementCachesAttributes(info);
            if (XmlILConstructInfo.Read(ndElem.Right).FinalStates == PossibleXmlStates.Any)
            {
                callChk = true;
            }
            if (info.FinalStates == PossibleXmlStates.Any)
            {
                callChk = true;
            }
            if (!callChk)
            {
                this.BeforeStartChecks(ndElem);
            }
            GenerateNameType nameType = this.LoadNameAndType(XPathNodeType.Element, ndElem.Left, true, callChk);
            this.helper.CallWriteStartElement(nameType, callChk);
            this.NestedVisit(ndElem.Right);
            if ((XmlILConstructInfo.Read(ndElem.Right).FinalStates == PossibleXmlStates.EnumAttrs) && !callChk)
            {
                this.helper.CallStartElementContent();
            }
            nameType = this.LoadNameAndType(XPathNodeType.Element, ndElem.Left, false, callChk);
            this.helper.CallWriteEndElement(nameType, callChk);
            if (!callChk)
            {
                this.AfterEndChecks(ndElem);
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndElem;
        }

        private void VisitEmpty(QilNode nd)
        {
            this.helper.EmitUnconditionalBranch(OpCodes.Brtrue, this.iterCurr.GetLabelNext());
            this.helper.Emit(OpCodes.Ldnull);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathItem), false);
        }

        protected override QilNode VisitEq(QilBinary ndEq)
        {
            this.Compare(ndEq);
            return ndEq;
        }

        protected override QilNode VisitError(QilUnary ndErr)
        {
            this.helper.LoadQueryRuntime();
            this.NestedVisitEnsureStack(ndErr.Child);
            this.helper.Call(XmlILMethods.ThrowException);
            if (XmlILConstructInfo.Read(ndErr).ConstructMethod == XmlILConstructMethod.Writer)
            {
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndErr;
            }
            this.helper.Emit(OpCodes.Ldnull);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathItem), false);
            return ndErr;
        }

        protected override QilNode VisitFalse(QilNode ndFalse)
        {
            if (this.iterCurr.CurrentBranchingContext != BranchingContext.None)
            {
                this.helper.EmitUnconditionalBranch((this.iterCurr.CurrentBranchingContext == BranchingContext.OnFalse) ? OpCodes.Brtrue : OpCodes.Brfalse, this.iterCurr.LabelBranch);
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndFalse;
            }
            this.helper.LoadBoolean(false);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
            return ndFalse;
        }

        protected override QilNode VisitFilter(QilLoop ndFilter)
        {
            if (!this.HandleFilterPatterns(ndFilter))
            {
                this.StartBinding(ndFilter.Variable);
                this.iterCurr.SetIterator(this.iterNested);
                this.StartNestedIterator(ndFilter.Body);
                this.iterCurr.SetBranching(BranchingContext.OnFalse, this.iterCurr.ParentIterator.GetLabelNext());
                this.Visit(ndFilter.Body);
                this.EndNestedIterator(ndFilter.Body);
                this.EndBinding(ndFilter.Variable);
            }
            return ndFilter;
        }

        protected override QilNode VisitFollowingSibling(QilUnary ndFollSib)
        {
            this.CreateFilteredIterator(ndFollSib.Child, "$$$iterFollSib", typeof(FollowingSiblingIterator), XmlILMethods.FollSibCreate, XmlILMethods.FollSibNext, XmlNodeKindFlags.Any, null, TriState.Unknown, null);
            return ndFollSib;
        }

        protected override QilNode VisitFor(QilIterator ndFor)
        {
            IteratorDescriptor cachedIteratorDescriptor = XmlILAnnotation.Write(ndFor).CachedIteratorDescriptor;
            this.iterCurr.Storage = cachedIteratorDescriptor.Storage;
            if (this.iterCurr.Storage.Location == ItemLocation.Global)
            {
                this.iterCurr.EnsureStack();
            }
            return ndFor;
        }

        protected override QilNode VisitGe(QilBinary ndGe)
        {
            this.Compare(ndGe);
            return ndGe;
        }

        private void VisitGlobalValues(QilList globalIterators)
        {
            foreach (QilIterator iterator in globalIterators)
            {
                QilParameter parameter = iterator as QilParameter;
                MethodInfo globalLocation = XmlILAnnotation.Write(iterator).CachedIteratorDescriptor.Storage.GlobalLocation;
                bool isCached = !iterator.XmlType.IsSingleton;
                int intVal = this.helper.StaticData.DeclareGlobalValue(iterator.DebugName);
                this.helper.MethodBegin(globalLocation, iterator.SourceLine, false);
                Label lblVal = this.helper.DefineLabel();
                Label label2 = this.helper.DefineLabel();
                this.helper.LoadQueryRuntime();
                this.helper.LoadInteger(intVal);
                this.helper.Call(XmlILMethods.GlobalComputed);
                this.helper.Emit(OpCodes.Brtrue, lblVal);
                this.StartNestedIterator(iterator);
                if (parameter != null)
                {
                    LocalBuilder locBldr = this.helper.DeclareLocal("$$$param", typeof(object));
                    this.helper.CallGetParameter(parameter.Name.LocalName, parameter.Name.NamespaceUri);
                    this.helper.Emit(OpCodes.Stloc, locBldr);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.Emit(OpCodes.Brfalse, label2);
                    this.helper.LoadQueryRuntime();
                    this.helper.LoadInteger(intVal);
                    this.helper.LoadQueryRuntime();
                    this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(XmlQueryTypeFactory.ItemS));
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.Call(XmlILMethods.ChangeTypeXsltResult);
                    this.helper.CallSetGlobalValue(typeof(object));
                    this.helper.EmitUnconditionalBranch(OpCodes.Br, lblVal);
                }
                this.helper.MarkLabel(label2);
                if (iterator.Binding != null)
                {
                    this.helper.LoadQueryRuntime();
                    this.helper.LoadInteger(intVal);
                    this.NestedVisitEnsureStack(iterator.Binding, this.GetItemStorageType(iterator), isCached);
                    this.helper.CallSetGlobalValue(this.GetStorageType(iterator));
                }
                else
                {
                    this.helper.LoadQueryRuntime();
                    this.helper.Emit(OpCodes.Ldstr, System.Xml.Utils.Res.GetString("XmlIl_UnknownParam", new string[] { parameter.Name.LocalName, parameter.Name.NamespaceUri }));
                    this.helper.Call(XmlILMethods.ThrowException);
                }
                this.EndNestedIterator(iterator);
                this.helper.MarkLabel(lblVal);
                this.helper.CallGetGlobalValue(intVal, this.GetStorageType(iterator));
                this.helper.MethodEnd();
            }
        }

        protected override QilNode VisitGt(QilBinary ndGt)
        {
            this.Compare(ndGt);
            return ndGt;
        }

        protected override QilNode VisitIntersection(QilBinary ndInter) => 
            this.CreateSetIterator(ndInter, "$$$iterInter", typeof(IntersectIterator), XmlILMethods.InterCreate, XmlILMethods.InterNext);

        protected override QilNode VisitInvoke(QilInvoke ndInvoke)
        {
            QilFunction nd = ndInvoke.Function;
            MethodInfo functionBinding = XmlILAnnotation.Write(nd).FunctionBinding;
            bool flag = XmlILConstructInfo.Read(nd).ConstructMethod == XmlILConstructMethod.Writer;
            this.helper.LoadQueryRuntime();
            for (int i = 0; i < ndInvoke.Arguments.Count; i++)
            {
                QilNode node = ndInvoke.Arguments[i];
                QilNode node2 = ndInvoke.Function.Arguments[i];
                this.NestedVisitEnsureStack(node, this.GetItemStorageType(node2), !node2.XmlType.IsSingleton);
            }
            if (OptimizerPatterns.Read(ndInvoke).MatchesPattern(OptimizerPatternName.TailCall))
            {
                this.helper.TailCall(functionBinding);
            }
            else
            {
                this.helper.Call(functionBinding);
            }
            if (!flag)
            {
                this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(ndInvoke), !ndInvoke.XmlType.IsSingleton);
                return ndInvoke;
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndInvoke;
        }

        protected override QilNode VisitIs(QilBinary ndIs)
        {
            this.NestedVisitEnsureStack(ndIs.Left, ndIs.Right);
            this.helper.Call(XmlILMethods.NavSamePos);
            this.ZeroCompare(QilNodeType.Ne, true);
            return ndIs;
        }

        protected override QilNode VisitIsEmpty(QilUnary ndIsEmpty)
        {
            if (this.CachesResult(ndIsEmpty.Child))
            {
                this.NestedVisitEnsureStack(ndIsEmpty.Child);
                this.helper.CallCacheCount(this.iterNested.Storage.ItemStorageType);
                switch (this.iterCurr.CurrentBranchingContext)
                {
                    case BranchingContext.OnTrue:
                        this.helper.TestAndBranch(0, this.iterCurr.LabelBranch, OpCodes.Beq);
                        goto Label_018B;

                    case BranchingContext.OnFalse:
                        this.helper.TestAndBranch(0, this.iterCurr.LabelBranch, OpCodes.Bne_Un);
                        goto Label_018B;
                }
                Label lblVal = this.helper.DefineLabel();
                this.helper.Emit(OpCodes.Brfalse_S, lblVal);
                this.helper.ConvBranchToBool(lblVal, true);
            }
            else
            {
                Label lblOnEnd = this.helper.DefineLabel();
                IteratorDescriptor iterCurr = this.iterCurr;
                if (iterCurr.CurrentBranchingContext == BranchingContext.OnTrue)
                {
                    this.StartNestedIterator(ndIsEmpty.Child, this.iterCurr.LabelBranch);
                }
                else
                {
                    this.StartNestedIterator(ndIsEmpty.Child, lblOnEnd);
                }
                this.Visit(ndIsEmpty.Child);
                this.iterCurr.EnsureNoCache();
                this.iterCurr.DiscardStack();
                switch (iterCurr.CurrentBranchingContext)
                {
                    case BranchingContext.None:
                        this.helper.ConvBranchToBool(lblOnEnd, true);
                        break;

                    case BranchingContext.OnFalse:
                        this.helper.EmitUnconditionalBranch(OpCodes.Br, iterCurr.LabelBranch);
                        this.helper.MarkLabel(lblOnEnd);
                        break;
                }
                this.EndNestedIterator(ndIsEmpty.Child);
            }
        Label_018B:
            if (this.iterCurr.IsBranching)
            {
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndIsEmpty;
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
            return ndIsEmpty;
        }

        protected override QilNode VisitIsType(QilTargetType ndIsType)
        {
            XmlQueryType xmlType = ndIsType.Source.XmlType;
            XmlQueryType targetType = ndIsType.TargetType;
            if (xmlType.IsSingleton && object.Equals(targetType, XmlQueryTypeFactory.Node))
            {
                this.NestedVisitEnsureStack(ndIsType.Source);
                this.helper.Call(XmlILMethods.ItemIsNode);
                this.ZeroCompare(QilNodeType.Ne, true);
                return ndIsType;
            }
            if (!this.MatchesNodeKinds(ndIsType, xmlType, targetType))
            {
                XmlTypeCode boolean;
                if (object.Equals(targetType, XmlQueryTypeFactory.Double))
                {
                    boolean = XmlTypeCode.Double;
                }
                else if (object.Equals(targetType, XmlQueryTypeFactory.String))
                {
                    boolean = XmlTypeCode.String;
                }
                else if (object.Equals(targetType, XmlQueryTypeFactory.Boolean))
                {
                    boolean = XmlTypeCode.Boolean;
                }
                else if (object.Equals(targetType, XmlQueryTypeFactory.Node))
                {
                    boolean = XmlTypeCode.Node;
                }
                else
                {
                    boolean = XmlTypeCode.None;
                }
                if (boolean != XmlTypeCode.None)
                {
                    this.helper.LoadQueryRuntime();
                    this.NestedVisitEnsureStack(ndIsType.Source, typeof(XPathItem), !xmlType.IsSingleton);
                    this.helper.LoadInteger((int) boolean);
                    this.helper.Call(xmlType.IsSingleton ? XmlILMethods.ItemMatchesCode : XmlILMethods.SeqMatchesCode);
                    this.ZeroCompare(QilNodeType.Ne, true);
                    return ndIsType;
                }
                this.helper.LoadQueryRuntime();
                this.NestedVisitEnsureStack(ndIsType.Source, typeof(XPathItem), !xmlType.IsSingleton);
                this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(targetType));
                this.helper.Call(xmlType.IsSingleton ? XmlILMethods.ItemMatchesType : XmlILMethods.SeqMatchesType);
                this.ZeroCompare(QilNodeType.Ne, true);
            }
            return ndIsType;
        }

        protected override QilNode VisitLe(QilBinary ndLe)
        {
            this.Compare(ndLe);
            return ndLe;
        }

        protected override QilNode VisitLength(QilUnary ndSetLen)
        {
            Label lblOnEnd = this.helper.DefineLabel();
            OptimizerPatterns patterns = OptimizerPatterns.Read(ndSetLen);
            if (this.CachesResult(ndSetLen.Child))
            {
                this.NestedVisitEnsureStack(ndSetLen.Child);
                this.helper.CallCacheCount(this.iterNested.Storage.ItemStorageType);
            }
            else
            {
                this.helper.Emit(OpCodes.Ldc_I4_0);
                this.StartNestedIterator(ndSetLen.Child, lblOnEnd);
                this.Visit(ndSetLen.Child);
                this.iterCurr.EnsureNoCache();
                this.iterCurr.DiscardStack();
                this.helper.Emit(OpCodes.Ldc_I4_1);
                this.helper.Emit(OpCodes.Add);
                if (patterns.MatchesPattern(OptimizerPatternName.MaxPosition))
                {
                    this.helper.Emit(OpCodes.Dup);
                    this.helper.LoadInteger((int) patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                    this.helper.Emit(OpCodes.Bgt, lblOnEnd);
                }
                this.iterCurr.LoopToEnd(lblOnEnd);
                this.EndNestedIterator(ndSetLen.Child);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(int), false);
            return ndSetLen;
        }

        protected override QilNode VisitLet(QilIterator ndLet) => 
            this.VisitFor(ndLet);

        protected override QilNode VisitLiteralDecimal(QilLiteral ndDec)
        {
            this.helper.ConstructLiteralDecimal((decimal) ndDec);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(decimal), false);
            return ndDec;
        }

        protected override QilNode VisitLiteralDouble(QilLiteral ndDbl)
        {
            this.helper.Emit(OpCodes.Ldc_R8, (double) ndDbl);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(double), false);
            return ndDbl;
        }

        protected override QilNode VisitLiteralInt32(QilLiteral ndInt)
        {
            this.helper.LoadInteger((int) ndInt);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(int), false);
            return ndInt;
        }

        protected override QilNode VisitLiteralInt64(QilLiteral ndLong)
        {
            this.helper.Emit(OpCodes.Ldc_I8, (long) ndLong);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(long), false);
            return ndLong;
        }

        protected override QilNode VisitLiteralQName(QilName ndQName)
        {
            this.helper.ConstructLiteralQName(ndQName.LocalName, ndQName.NamespaceUri);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XmlQualifiedName), false);
            return ndQName;
        }

        protected override QilNode VisitLiteralString(QilLiteral ndStr)
        {
            this.helper.Emit(OpCodes.Ldstr, (string) ndStr);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
            return ndStr;
        }

        protected override QilNode VisitLocalNameOf(QilUnary ndName) => 
            this.VisitNodeProperty(ndName);

        protected override QilNode VisitLoop(QilLoop ndLoop)
        {
            bool flag;
            Label label;
            this.StartWriterLoop(ndLoop, out flag, out label);
            this.StartBinding(ndLoop.Variable);
            this.Visit(ndLoop.Body);
            this.EndBinding(ndLoop.Variable);
            this.EndWriterLoop(ndLoop, flag, label);
            return ndLoop;
        }

        protected override QilNode VisitLt(QilBinary ndLt)
        {
            this.Compare(ndLt);
            return ndLt;
        }

        protected override QilNode VisitMaximum(QilUnary ndMax)
        {
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[this.GetItemStorageType(ndMax)];
            return this.CreateAggregator(ndMax, "$$$aggMax", methods, methods.AggMax, methods.AggMaxResult);
        }

        protected override QilNode VisitMinimum(QilUnary ndMin)
        {
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[this.GetItemStorageType(ndMin)];
            return this.CreateAggregator(ndMin, "$$$aggMin", methods, methods.AggMin, methods.AggMinResult);
        }

        protected override QilNode VisitModulo(QilBinary ndMod) => 
            this.ArithmeticOp(ndMod);

        protected override QilNode VisitMultiply(QilBinary ndMul) => 
            this.ArithmeticOp(ndMul);

        protected override QilNode VisitNameOf(QilUnary ndName) => 
            this.VisitNodeProperty(ndName);

        protected override QilNode VisitNamespaceDecl(QilBinary ndNmsp)
        {
            XmlILConstructInfo info = XmlILConstructInfo.Read(ndNmsp);
            bool callChk = this.CheckEnumAttrs(info) || this.MightHaveNamespacesAfterAttributes(info);
            if (!callChk)
            {
                this.BeforeStartChecks(ndNmsp);
            }
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndNmsp.Left);
            this.NestedVisitEnsureStack(ndNmsp.Right);
            this.helper.CallWriteNamespaceDecl(callChk);
            if (!callChk)
            {
                this.AfterEndChecks(ndNmsp);
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndNmsp;
        }

        protected override QilNode VisitNamespaceUriOf(QilUnary ndName) => 
            this.VisitNodeProperty(ndName);

        protected override QilNode VisitNe(QilBinary ndNe)
        {
            this.Compare(ndNe);
            return ndNe;
        }

        protected override QilNode VisitNegate(QilUnary ndNeg)
        {
            this.NestedVisitEnsureStack(ndNeg.Child);
            this.helper.CallArithmeticOp(QilNodeType.Negate, ndNeg.XmlType.TypeCode);
            this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(ndNeg), false);
            return ndNeg;
        }

        private QilNode VisitNodeProperty(QilUnary ndProp)
        {
            this.NestedVisitEnsureStack(ndProp.Child);
            switch (ndProp.NodeType)
            {
                case QilNodeType.NameOf:
                    this.helper.Emit(OpCodes.Dup);
                    this.helper.Call(XmlILMethods.NavLocalName);
                    this.helper.Call(XmlILMethods.NavNmsp);
                    this.helper.Construct(XmlILConstructors.QName);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XmlQualifiedName), false);
                    return ndProp;

                case QilNodeType.LocalNameOf:
                    this.helper.Call(XmlILMethods.NavLocalName);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
                    return ndProp;

                case QilNodeType.NamespaceUriOf:
                    this.helper.Call(XmlILMethods.NavNmsp);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
                    return ndProp;

                case QilNodeType.PrefixOf:
                    this.helper.Call(XmlILMethods.NavPrefix);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
                    return ndProp;
            }
            return ndProp;
        }

        protected override QilNode VisitNodeRange(QilBinary ndRange)
        {
            this.CreateFilteredIterator(ndRange.Left, "$$$iterRange", typeof(NodeRangeIterator), XmlILMethods.NodeRangeCreate, XmlILMethods.NodeRangeNext, XmlNodeKindFlags.Any, null, TriState.Unknown, ndRange.Right);
            return ndRange;
        }

        protected override QilNode VisitNop(QilUnary ndNop) => 
            this.Visit(ndNop.Child);

        protected override QilNode VisitNot(QilUnary ndNot)
        {
            Label lblBranch = new Label();
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.OnTrue:
                    this.NestedVisitWithBranch(ndNot.Child, BranchingContext.OnFalse, this.iterCurr.LabelBranch);
                    break;

                case BranchingContext.OnFalse:
                    this.NestedVisitWithBranch(ndNot.Child, BranchingContext.OnTrue, this.iterCurr.LabelBranch);
                    break;

                default:
                    lblBranch = this.helper.DefineLabel();
                    this.NestedVisitWithBranch(ndNot.Child, BranchingContext.OnTrue, lblBranch);
                    break;
            }
            if (this.iterCurr.CurrentBranchingContext == BranchingContext.None)
            {
                this.helper.ConvBranchToBool(lblBranch, false);
                this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
                return ndNot;
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndNot;
        }

        protected override QilNode VisitOptimizeBarrier(QilUnary ndBarrier) => 
            this.Visit(ndBarrier.Child);

        protected override QilNode VisitOr(QilBinary ndOr)
        {
            Label lblBranch = new Label();
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.OnTrue:
                    this.NestedVisitWithBranch(ndOr.Left, BranchingContext.OnTrue, this.iterCurr.LabelBranch);
                    break;

                case BranchingContext.OnFalse:
                    lblBranch = this.helper.DefineLabel();
                    this.NestedVisitWithBranch(ndOr.Left, BranchingContext.OnTrue, lblBranch);
                    break;

                default:
                    lblBranch = this.helper.DefineLabel();
                    this.NestedVisitWithBranch(ndOr.Left, BranchingContext.OnTrue, lblBranch);
                    break;
            }
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.OnTrue:
                    this.NestedVisitWithBranch(ndOr.Right, BranchingContext.OnTrue, this.iterCurr.LabelBranch);
                    break;

                case BranchingContext.OnFalse:
                    this.NestedVisitWithBranch(ndOr.Right, BranchingContext.OnFalse, this.iterCurr.LabelBranch);
                    break;

                default:
                    this.NestedVisitWithBranch(ndOr.Right, BranchingContext.OnTrue, lblBranch);
                    break;
            }
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.None:
                    this.helper.ConvBranchToBool(lblBranch, true);
                    this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
                    return ndOr;

                case BranchingContext.OnTrue:
                    break;

                case BranchingContext.OnFalse:
                    this.helper.MarkLabel(lblBranch);
                    break;

                default:
                    return ndOr;
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndOr;
        }

        protected override QilNode VisitParameter(QilParameter ndParameter) => 
            this.VisitFor(ndParameter);

        protected override QilNode VisitParent(QilUnary ndParent)
        {
            LocalBuilder locNav = this.helper.DeclareLocal("$$$navParent", typeof(XPathNavigator));
            this.SyncToNavigator(locNav, ndParent.Child);
            this.helper.Emit(OpCodes.Ldloc, locNav);
            this.helper.Call(XmlILMethods.NavMoveParent);
            this.helper.Emit(OpCodes.Brfalse, this.iterCurr.GetLabelNext());
            this.iterCurr.Storage = StorageDescriptor.Local(locNav, typeof(XPathNavigator), false);
            return ndParent;
        }

        protected override QilNode VisitPICtor(QilBinary ndPI)
        {
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndPI.Left);
            this.helper.CallWriteStartPI();
            this.NestedVisit(ndPI.Right);
            this.helper.CallWriteEndPI();
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndPI;
        }

        protected override QilNode VisitPositionOf(QilUnary ndPos)
        {
            QilIterator child = ndPos.Child as QilIterator;
            LocalBuilder localPosition = XmlILAnnotation.Write(child).CachedIteratorDescriptor.LocalPosition;
            this.iterCurr.Storage = StorageDescriptor.Local(localPosition, typeof(int), false);
            return ndPos;
        }

        protected override QilNode VisitPreceding(QilUnary ndPrec)
        {
            this.CreateFilteredIterator(ndPrec.Child, "$$$iterPrec", typeof(PrecedingIterator), XmlILMethods.PrecCreate, XmlILMethods.PrecNext, XmlNodeKindFlags.Any, null, TriState.Unknown, null);
            return ndPrec;
        }

        protected override QilNode VisitPrecedingSibling(QilUnary ndPreSib)
        {
            this.CreateFilteredIterator(ndPreSib.Child, "$$$iterPreSib", typeof(PrecedingSiblingIterator), XmlILMethods.PreSibCreate, XmlILMethods.PreSibNext, XmlNodeKindFlags.Any, null, TriState.Unknown, null);
            return ndPreSib;
        }

        protected override QilNode VisitPrefixOf(QilUnary ndName) => 
            this.VisitNodeProperty(ndName);

        protected override QilNode VisitRawTextCtor(QilUnary ndText) => 
            this.VisitTextCtor(ndText, true);

        protected override QilNode VisitRoot(QilUnary ndRoot)
        {
            LocalBuilder locNav = this.helper.DeclareLocal("$$$navRoot", typeof(XPathNavigator));
            this.SyncToNavigator(locNav, ndRoot.Child);
            this.helper.Emit(OpCodes.Ldloc, locNav);
            this.helper.Call(XmlILMethods.NavMoveRoot);
            this.iterCurr.Storage = StorageDescriptor.Local(locNav, typeof(XPathNavigator), false);
            return ndRoot;
        }

        protected override QilNode VisitRtfCtor(QilBinary ndRtf)
        {
            OptimizerPatterns patterns = OptimizerPatterns.Read(ndRtf);
            string right = (string) ((QilLiteral) ndRtf.Right);
            if (patterns.MatchesPattern(OptimizerPatternName.SingleTextRtf))
            {
                this.helper.LoadQueryRuntime();
                this.NestedVisitEnsureStack((QilNode) patterns.GetArgument(OptimizerPatternArgument.ElementQName));
                this.helper.Emit(OpCodes.Ldstr, right);
                this.helper.Call(XmlILMethods.RtfConstr);
            }
            else
            {
                this.helper.CallStartRtfConstruction(right);
                this.NestedVisit(ndRtf.Left);
                this.helper.CallEndRtfConstruction();
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathNavigator), false);
            return ndRtf;
        }

        protected override QilNode VisitSequence(QilList ndSeq)
        {
            if (XmlILConstructInfo.Read(ndSeq).ConstructMethod == XmlILConstructMethod.Writer)
            {
                foreach (QilNode node in ndSeq)
                {
                    this.NestedVisit(node);
                }
                return ndSeq;
            }
            if (ndSeq.Count == 0)
            {
                this.VisitEmpty(ndSeq);
                return ndSeq;
            }
            this.Sequence(ndSeq);
            return ndSeq;
        }

        protected override QilNode VisitSort(QilLoop ndSort)
        {
            Type itemStorageType = this.GetItemStorageType(ndSort);
            Label lblOnEnd = this.helper.DefineLabel();
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[itemStorageType];
            LocalBuilder locBldr = this.helper.DeclareLocal("$$$cache", methods.SeqType);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            this.helper.CallToken(methods.SeqReuse);
            this.helper.Emit(OpCodes.Stloc, locBldr);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            LocalBuilder builder2 = this.helper.DeclareLocal("$$$keys", typeof(XmlSortKeyAccumulator));
            this.helper.Emit(OpCodes.Ldloca, builder2);
            this.helper.Call(XmlILMethods.SortKeyCreate);
            this.StartNestedIterator(ndSort.Variable, lblOnEnd);
            this.StartBinding(ndSort.Variable);
            this.iterCurr.EnsureStackNoCache();
            this.iterCurr.EnsureItemStorageType(ndSort.Variable.XmlType, this.GetItemStorageType(ndSort.Variable));
            this.helper.Call(methods.SeqAdd);
            this.helper.Emit(OpCodes.Ldloca, builder2);
            foreach (QilSortKey key in ndSort.Body)
            {
                this.VisitSortKey(key, builder2);
            }
            this.helper.Call(XmlILMethods.SortKeyFinish);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            this.iterCurr.LoopToEnd(lblOnEnd);
            this.helper.Emit(OpCodes.Pop);
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            this.helper.Emit(OpCodes.Ldloca, builder2);
            this.helper.Call(XmlILMethods.SortKeyKeys);
            this.helper.Call(methods.SeqSortByKeys);
            this.iterCurr.Storage = StorageDescriptor.Local(locBldr, itemStorageType, true);
            this.EndBinding(ndSort.Variable);
            this.EndNestedIterator(ndSort.Variable);
            this.iterCurr.SetIterator(this.iterNested);
            return ndSort;
        }

        private void VisitSortKey(QilSortKey ndKey, LocalBuilder locKeys)
        {
            this.helper.Emit(OpCodes.Ldloca, locKeys);
            if (ndKey.Collation.NodeType == QilNodeType.LiteralString)
            {
                this.helper.CallGetCollation(this.helper.StaticData.DeclareCollation((string) ((QilLiteral) ndKey.Collation)));
            }
            else
            {
                this.helper.LoadQueryRuntime();
                this.NestedVisitEnsureStack(ndKey.Collation);
                this.helper.Call(XmlILMethods.CreateCollation);
            }
            if (ndKey.XmlType.IsSingleton)
            {
                this.NestedVisitEnsureStack(ndKey.Key);
                this.helper.AddSortKey(ndKey.Key.XmlType);
            }
            else
            {
                Label lblOnEnd = this.helper.DefineLabel();
                this.StartNestedIterator(ndKey.Key, lblOnEnd);
                this.Visit(ndKey.Key);
                this.iterCurr.EnsureStackNoCache();
                this.iterCurr.EnsureItemStorageType(ndKey.Key.XmlType, this.GetItemStorageType(ndKey.Key));
                this.helper.AddSortKey(ndKey.Key.XmlType);
                Label lblTarget = this.helper.DefineLabel();
                this.helper.EmitUnconditionalBranch(OpCodes.Br_S, lblTarget);
                this.helper.MarkLabel(lblOnEnd);
                this.helper.AddSortKey(null);
                this.helper.MarkLabel(lblTarget);
                this.EndNestedIterator(ndKey.Key);
            }
        }

        protected override QilNode VisitStrConcat(QilStrConcat ndStrConcat)
        {
            bool flag;
            QilNode delimiter = ndStrConcat.Delimiter;
            if ((delimiter.NodeType == QilNodeType.LiteralString) && (((QilLiteral) delimiter).Length == 0))
            {
                delimiter = null;
            }
            QilNode values = ndStrConcat.Values;
            if ((values.NodeType == QilNodeType.Sequence) && (values.Count < 5))
            {
                flag = true;
                foreach (QilNode node3 in values)
                {
                    if (!node3.XmlType.IsSingleton)
                    {
                        flag = false;
                    }
                }
            }
            else
            {
                flag = false;
            }
            if (flag)
            {
                foreach (QilNode node4 in values)
                {
                    this.NestedVisitEnsureStack(node4);
                }
                this.helper.CallConcatStrings(values.Count);
            }
            else
            {
                LocalBuilder locBldr = this.helper.DeclareLocal("$$$strcat", typeof(StringConcat));
                this.helper.Emit(OpCodes.Ldloca, locBldr);
                this.helper.Call(XmlILMethods.StrCatClear);
                if (delimiter != null)
                {
                    this.helper.Emit(OpCodes.Ldloca, locBldr);
                    this.NestedVisitEnsureStack(delimiter);
                    this.helper.Call(XmlILMethods.StrCatDelim);
                }
                this.helper.Emit(OpCodes.Ldloca, locBldr);
                if (values.NodeType == QilNodeType.Sequence)
                {
                    foreach (QilNode node5 in values)
                    {
                        this.GenerateConcat(node5, locBldr);
                    }
                }
                else
                {
                    this.GenerateConcat(values, locBldr);
                }
                this.helper.Call(XmlILMethods.StrCatResult);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
            return ndStrConcat;
        }

        protected override QilNode VisitStrLength(QilUnary ndLen)
        {
            this.NestedVisitEnsureStack(ndLen.Child);
            this.helper.Call(XmlILMethods.StrLen);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(int), false);
            return ndLen;
        }

        protected override QilNode VisitStrParseQName(QilBinary ndParsedTagName)
        {
            this.VisitStrParseQName(ndParsedTagName, false);
            return ndParsedTagName;
        }

        private void VisitStrParseQName(QilBinary ndParsedTagName, bool preservePrefix)
        {
            if (!preservePrefix)
            {
                this.helper.LoadQueryRuntime();
            }
            this.NestedVisitEnsureStack(ndParsedTagName.Left);
            if (ndParsedTagName.Right.XmlType.TypeCode == XmlTypeCode.String)
            {
                this.NestedVisitEnsureStack(ndParsedTagName.Right);
                if (!preservePrefix)
                {
                    this.helper.CallParseTagName(GenerateNameType.TagNameAndNamespace);
                }
            }
            else
            {
                if (ndParsedTagName.Right.NodeType == QilNodeType.Sequence)
                {
                    this.helper.LoadInteger(this.helper.StaticData.DeclarePrefixMappings(ndParsedTagName.Right));
                }
                else
                {
                    this.helper.LoadInteger(this.helper.StaticData.DeclarePrefixMappings(new QilNode[] { ndParsedTagName.Right }));
                }
                if (!preservePrefix)
                {
                    this.helper.CallParseTagName(GenerateNameType.TagNameAndMappings);
                }
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XmlQualifiedName), false);
        }

        protected override QilNode VisitSubtract(QilBinary ndMinus) => 
            this.ArithmeticOp(ndMinus);

        protected override QilNode VisitSum(QilUnary ndSum)
        {
            XmlILStorageMethods methods = XmlILMethods.StorageMethods[this.GetItemStorageType(ndSum)];
            return this.CreateAggregator(ndSum, "$$$aggSum", methods, methods.AggSum, methods.AggSumResult);
        }

        protected override QilNode VisitTextCtor(QilUnary ndText) => 
            this.VisitTextCtor(ndText, false);

        private QilNode VisitTextCtor(QilUnary ndText, bool disableOutputEscaping)
        {
            bool flag;
            XmlILConstructInfo info = XmlILConstructInfo.Read(ndText);
            switch (info.InitialStates)
            {
                case PossibleXmlStates.WithinAttr:
                case PossibleXmlStates.WithinComment:
                case PossibleXmlStates.WithinPI:
                    flag = false;
                    break;

                default:
                    flag = this.CheckWithinContent(info);
                    break;
            }
            if (!flag)
            {
                this.BeforeStartChecks(ndText);
            }
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndText.Child);
            switch (info.InitialStates)
            {
                case PossibleXmlStates.WithinAttr:
                    this.helper.CallWriteString(false, flag);
                    break;

                case PossibleXmlStates.WithinComment:
                    this.helper.Call(XmlILMethods.CommentText);
                    break;

                case PossibleXmlStates.WithinPI:
                    this.helper.Call(XmlILMethods.PIText);
                    break;

                default:
                    this.helper.CallWriteString(disableOutputEscaping, flag);
                    break;
            }
            if (!flag)
            {
                this.AfterEndChecks(ndText);
            }
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndText;
        }

        protected override QilNode VisitTrue(QilNode ndTrue)
        {
            if (this.iterCurr.CurrentBranchingContext != BranchingContext.None)
            {
                this.helper.EmitUnconditionalBranch((this.iterCurr.CurrentBranchingContext == BranchingContext.OnTrue) ? OpCodes.Brtrue : OpCodes.Brfalse, this.iterCurr.LabelBranch);
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndTrue;
            }
            this.helper.LoadBoolean(true);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
            return ndTrue;
        }

        protected override QilNode VisitTypeAssert(QilTargetType ndTypeAssert)
        {
            if ((!ndTypeAssert.Source.XmlType.IsSingleton && ndTypeAssert.XmlType.IsSingleton) && !this.iterCurr.HasLabelNext)
            {
                Label lbl = this.helper.DefineLabel();
                this.helper.MarkLabel(lbl);
                this.NestedVisit(ndTypeAssert.Source, lbl);
            }
            else
            {
                this.Visit(ndTypeAssert.Source);
            }
            this.iterCurr.EnsureItemStorageType(ndTypeAssert.Source.XmlType, this.GetItemStorageType(ndTypeAssert));
            return ndTypeAssert;
        }

        protected override QilNode VisitUnion(QilBinary ndUnion) => 
            this.CreateSetIterator(ndUnion, "$$$iterUnion", typeof(UnionIterator), XmlILMethods.UnionCreate, XmlILMethods.UnionNext);

        protected override QilNode VisitWarning(QilUnary ndWarning)
        {
            this.helper.LoadQueryRuntime();
            this.NestedVisitEnsureStack(ndWarning.Child);
            this.helper.Call(XmlILMethods.SendMessage);
            if (XmlILConstructInfo.Read(ndWarning).ConstructMethod == XmlILConstructMethod.Writer)
            {
                this.iterCurr.Storage = StorageDescriptor.None();
                return ndWarning;
            }
            this.VisitEmpty(ndWarning);
            return ndWarning;
        }

        protected override QilNode VisitXmlContext(QilNode ndCtxt)
        {
            this.helper.LoadQueryContext();
            this.helper.Call(XmlILMethods.GetDefaultDataSource);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathNavigator), false);
            return ndCtxt;
        }

        protected override QilNode VisitXPathFollowing(QilUnary ndFoll)
        {
            this.CreateFilteredIterator(ndFoll.Child, "$$$iterFoll", typeof(XPathFollowingIterator), XmlILMethods.XPFollCreate, XmlILMethods.XPFollNext, XmlNodeKindFlags.Any, null, TriState.Unknown, null);
            return ndFoll;
        }

        protected override QilNode VisitXPathNamespace(QilUnary ndNmsp)
        {
            this.CreateSimpleIterator(ndNmsp.Child, "$$$iterNmsp", typeof(NamespaceIterator), XmlILMethods.NmspCreate, XmlILMethods.NmspNext);
            return ndNmsp;
        }

        protected override QilNode VisitXPathNodeValue(QilUnary ndVal)
        {
            if (ndVal.Child.XmlType.IsSingleton)
            {
                this.NestedVisitEnsureStack(ndVal.Child, typeof(XPathNavigator), false);
                this.helper.Call(XmlILMethods.Value);
            }
            else
            {
                Label lblOnEnd = this.helper.DefineLabel();
                this.StartNestedIterator(ndVal.Child, lblOnEnd);
                this.Visit(ndVal.Child);
                this.iterCurr.EnsureStackNoCache();
                this.helper.Call(XmlILMethods.Value);
                Label lblTarget = this.helper.DefineLabel();
                this.helper.EmitUnconditionalBranch(OpCodes.Br, lblTarget);
                this.helper.MarkLabel(lblOnEnd);
                this.helper.Emit(OpCodes.Ldstr, "");
                this.helper.MarkLabel(lblTarget);
                this.EndNestedIterator(ndVal.Child);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
            return ndVal;
        }

        protected override QilNode VisitXPathPreceding(QilUnary ndPrec)
        {
            this.CreateFilteredIterator(ndPrec.Child, "$$$iterPrec", typeof(XPathPrecedingIterator), XmlILMethods.XPPrecCreate, XmlILMethods.XPPrecNext, XmlNodeKindFlags.Any, null, TriState.Unknown, null);
            return ndPrec;
        }

        protected override QilNode VisitXsltConvert(QilTargetType ndConv)
        {
            MethodInfo info;
            XmlQueryType xmlType = ndConv.Source.XmlType;
            XmlQueryType targetType = ndConv.TargetType;
            if (this.GetXsltConvertMethod(xmlType, targetType, out info))
            {
                this.NestedVisitEnsureStack(ndConv.Source);
            }
            else
            {
                this.NestedVisitEnsureStack(ndConv.Source, typeof(XPathItem), !xmlType.IsSingleton);
                this.GetXsltConvertMethod(xmlType.IsSingleton ? XmlQueryTypeFactory.Item : XmlQueryTypeFactory.ItemS, targetType, out info);
            }
            if (info != null)
            {
                this.helper.Call(info);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(targetType), !targetType.IsSingleton);
            return ndConv;
        }

        protected override QilNode VisitXsltCopy(QilBinary ndCopy)
        {
            Label lblVal = this.helper.DefineLabel();
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndCopy.Left);
            this.helper.Call(XmlILMethods.StartCopy);
            this.helper.Emit(OpCodes.Brfalse, lblVal);
            this.NestedVisit(ndCopy.Right);
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndCopy.Left);
            this.helper.Call(XmlILMethods.EndCopy);
            this.helper.MarkLabel(lblVal);
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndCopy;
        }

        protected override QilNode VisitXsltCopyOf(QilUnary ndCopyOf)
        {
            this.helper.LoadQueryOutput();
            this.NestedVisitEnsureStack(ndCopyOf.Child);
            this.helper.Call(XmlILMethods.CopyOf);
            this.iterCurr.Storage = StorageDescriptor.None();
            return ndCopyOf;
        }

        protected override QilNode VisitXsltGenerateId(QilUnary ndGenId)
        {
            this.helper.LoadQueryRuntime();
            if (ndGenId.Child.XmlType.IsSingleton)
            {
                this.NestedVisitEnsureStack(ndGenId.Child, typeof(XPathNavigator), false);
                this.helper.Call(XmlILMethods.GenId);
            }
            else
            {
                Label lblOnEnd = this.helper.DefineLabel();
                this.StartNestedIterator(ndGenId.Child, lblOnEnd);
                this.Visit(ndGenId.Child);
                this.iterCurr.EnsureStackNoCache();
                this.iterCurr.EnsureItemStorageType(ndGenId.Child.XmlType, typeof(XPathNavigator));
                this.helper.Call(XmlILMethods.GenId);
                Label lblTarget = this.helper.DefineLabel();
                this.helper.EmitUnconditionalBranch(OpCodes.Br, lblTarget);
                this.helper.MarkLabel(lblOnEnd);
                this.helper.Emit(OpCodes.Pop);
                this.helper.Emit(OpCodes.Ldstr, "");
                this.helper.MarkLabel(lblTarget);
                this.EndNestedIterator(ndGenId.Child);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(string), false);
            return ndGenId;
        }

        protected override QilNode VisitXsltInvokeEarlyBound(QilInvokeEarlyBound ndInvoke)
        {
            QilName name = ndInvoke.Name;
            XmlExtensionFunction function = new XmlExtensionFunction(name.LocalName, name.NamespaceUri, ndInvoke.ClrMethod);
            Type clrReturnType = function.ClrReturnType;
            Type storageType = this.GetStorageType(ndInvoke);
            if ((clrReturnType != storageType) && !ndInvoke.XmlType.IsEmpty)
            {
                this.helper.LoadQueryRuntime();
                this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(ndInvoke.XmlType));
            }
            if (!function.Method.IsStatic)
            {
                if (name.NamespaceUri.Length == 0)
                {
                    this.helper.LoadXsltLibrary();
                }
                else
                {
                    this.helper.CallGetEarlyBoundObject(this.helper.StaticData.DeclareEarlyBound(name.NamespaceUri, function.Method.DeclaringType), function.Method.DeclaringType);
                }
            }
            for (int i = 0; i < ndInvoke.Arguments.Count; i++)
            {
                QilNode nd = ndInvoke.Arguments[i];
                XmlQueryType xmlArgumentType = function.GetXmlArgumentType(i);
                Type clrArgumentType = function.GetClrArgumentType(i);
                if (name.NamespaceUri.Length == 0)
                {
                    Type itemStorageType = this.GetItemStorageType(nd);
                    if (clrArgumentType == XmlILMethods.StorageMethods[itemStorageType].IListType)
                    {
                        this.NestedVisitEnsureStack(nd, itemStorageType, true);
                    }
                    else if (clrArgumentType == XmlILMethods.StorageMethods[typeof(XPathItem)].IListType)
                    {
                        this.NestedVisitEnsureStack(nd, typeof(XPathItem), true);
                    }
                    else if ((nd.XmlType.IsSingleton && (clrArgumentType == itemStorageType)) || (nd.XmlType.TypeCode == XmlTypeCode.None))
                    {
                        this.NestedVisitEnsureStack(nd, clrArgumentType, false);
                    }
                    else if (nd.XmlType.IsSingleton && (clrArgumentType == typeof(XPathItem)))
                    {
                        this.NestedVisitEnsureStack(nd, typeof(XPathItem), false);
                    }
                }
                else
                {
                    Type c = this.GetStorageType(xmlArgumentType);
                    if ((xmlArgumentType.TypeCode == XmlTypeCode.Item) || !clrArgumentType.IsAssignableFrom(c))
                    {
                        this.helper.LoadQueryRuntime();
                        this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(xmlArgumentType));
                        this.NestedVisitEnsureStack(nd, this.GetItemStorageType(xmlArgumentType), !xmlArgumentType.IsSingleton);
                        this.helper.TreatAs(c, typeof(object));
                        this.helper.LoadType(clrArgumentType);
                        this.helper.Call(XmlILMethods.ChangeTypeXsltArg);
                        this.helper.TreatAs(typeof(object), clrArgumentType);
                    }
                    else
                    {
                        this.NestedVisitEnsureStack(nd, this.GetItemStorageType(xmlArgumentType), !xmlArgumentType.IsSingleton);
                    }
                }
            }
            this.helper.Call(function.Method);
            if (ndInvoke.XmlType.IsEmpty)
            {
                this.helper.Emit(OpCodes.Ldsfld, XmlILMethods.StorageMethods[typeof(XPathItem)].SeqEmpty);
            }
            else if (clrReturnType != storageType)
            {
                this.helper.TreatAs(clrReturnType, typeof(object));
                this.helper.Call(XmlILMethods.ChangeTypeXsltResult);
                this.helper.TreatAs(typeof(object), storageType);
            }
            else if ((name.NamespaceUri.Length != 0) && !clrReturnType.IsValueType)
            {
                Label lblVal = this.helper.DefineLabel();
                this.helper.Emit(OpCodes.Dup);
                this.helper.Emit(OpCodes.Brtrue, lblVal);
                this.helper.LoadQueryRuntime();
                this.helper.Emit(OpCodes.Ldstr, System.Xml.Utils.Res.GetString("Xslt_ItemNull"));
                this.helper.Call(XmlILMethods.ThrowException);
                this.helper.MarkLabel(lblVal);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(this.GetItemStorageType(ndInvoke), !ndInvoke.XmlType.IsSingleton);
            return ndInvoke;
        }

        protected override QilNode VisitXsltInvokeLateBound(QilInvokeLateBound ndInvoke)
        {
            LocalBuilder locBldr = this.helper.DeclareLocal("$$$args", typeof(IList<XPathItem>[]));
            QilName name = ndInvoke.Name;
            this.helper.LoadQueryContext();
            this.helper.Emit(OpCodes.Ldstr, name.LocalName);
            this.helper.Emit(OpCodes.Ldstr, name.NamespaceUri);
            this.helper.LoadInteger(ndInvoke.Arguments.Count);
            this.helper.Emit(OpCodes.Newarr, typeof(IList<XPathItem>));
            this.helper.Emit(OpCodes.Stloc, locBldr);
            for (int i = 0; i < ndInvoke.Arguments.Count; i++)
            {
                QilNode nd = ndInvoke.Arguments[i];
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.helper.LoadInteger(i);
                this.helper.Emit(OpCodes.Ldelema, typeof(IList<XPathItem>));
                this.NestedVisitEnsureCache(nd, typeof(XPathItem));
                this.iterCurr.EnsureStack();
                this.helper.Emit(OpCodes.Stobj, typeof(IList<XPathItem>));
            }
            this.helper.Emit(OpCodes.Ldloc, locBldr);
            this.helper.Call(XmlILMethods.InvokeXsltLate);
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(XPathItem), true);
            return ndInvoke;
        }

        private void ZeroCompare(QilNodeType relOp, bool isBoolVal)
        {
            switch (this.iterCurr.CurrentBranchingContext)
            {
                case BranchingContext.OnTrue:
                    this.helper.Emit((relOp == QilNodeType.Eq) ? OpCodes.Brfalse : OpCodes.Brtrue, this.iterCurr.LabelBranch);
                    this.iterCurr.Storage = StorageDescriptor.None();
                    return;

                case BranchingContext.OnFalse:
                    this.helper.Emit((relOp == QilNodeType.Eq) ? OpCodes.Brtrue : OpCodes.Brfalse, this.iterCurr.LabelBranch);
                    this.iterCurr.Storage = StorageDescriptor.None();
                    return;
            }
            if (!isBoolVal || (relOp == QilNodeType.Eq))
            {
                Label lblVal = this.helper.DefineLabel();
                this.helper.Emit((relOp == QilNodeType.Eq) ? OpCodes.Brfalse : OpCodes.Brtrue, lblVal);
                this.helper.ConvBranchToBool(lblVal, true);
            }
            this.iterCurr.Storage = StorageDescriptor.Stack(typeof(bool), false);
        }
    }
}

