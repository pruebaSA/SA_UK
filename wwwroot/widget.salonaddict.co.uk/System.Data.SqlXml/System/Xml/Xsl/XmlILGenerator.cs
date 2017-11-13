namespace System.Xml.Xsl
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security;
    using System.Xml.XPath;
    using System.Xml.Xsl.IlGen;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class XmlILGenerator
    {
        private GenerateHelper helper;
        private XmlILModule module;
        private XmlILOptimizerVisitor optVisitor;
        private QilExpression qil;
        private XmlILVisitor xmlIlVisitor;

        private void CreateFunctionMetadata(IList<QilNode> funcList)
        {
            foreach (QilFunction function in funcList)
            {
                Type storageType;
                Type[] paramTypes = new Type[function.Arguments.Count];
                string[] paramNames = new string[function.Arguments.Count];
                for (int i = 0; i < function.Arguments.Count; i++)
                {
                    QilParameter parameter = (QilParameter) function.Arguments[i];
                    paramTypes[i] = XmlILTypeHelper.GetStorageType(parameter.XmlType);
                    if (parameter.DebugName != null)
                    {
                        paramNames[i] = parameter.DebugName;
                    }
                }
                if (XmlILConstructInfo.Read(function).PushToWriterLast)
                {
                    storageType = typeof(void);
                }
                else
                {
                    storageType = XmlILTypeHelper.GetStorageType(function.XmlType);
                }
                XmlILMethodAttributes xmlAttrs = (function.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None;
                MethodInfo info = this.module.DefineMethod(function.DebugName, storageType, paramTypes, paramNames, xmlAttrs);
                for (int j = 0; j < function.Arguments.Count; j++)
                {
                    XmlILAnnotation.Write(function.Arguments[j]).ArgumentPosition = j;
                }
                XmlILAnnotation.Write(function).FunctionBinding = info;
            }
        }

        private void CreateGlobalValueMetadata(IList<QilNode> globalList)
        {
            foreach (QilReference reference in globalList)
            {
                Type storageType = XmlILTypeHelper.GetStorageType(reference.XmlType);
                XmlILMethodAttributes xmlAttrs = (reference.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None;
                MethodInfo info = this.module.DefineMethod(reference.DebugName.ToString(), storageType, new Type[0], new string[0], xmlAttrs);
                XmlILAnnotation.Write(reference).FunctionBinding = info;
            }
        }

        private void CreateHelperFunctions()
        {
            string[] paramNames = new string[2];
            MethodInfo methInfo = this.module.DefineMethod("SyncToNavigator", typeof(XPathNavigator), new Type[] { typeof(XPathNavigator), typeof(XPathNavigator) }, paramNames, XmlILMethodAttributes.Raw | XmlILMethodAttributes.NonUser);
            this.helper.MethodBegin(methInfo, null, false);
            Label lblVal = this.helper.DefineLabel();
            this.helper.Emit(OpCodes.Ldarg_0);
            this.helper.Emit(OpCodes.Brfalse, lblVal);
            this.helper.Emit(OpCodes.Ldarg_0);
            this.helper.Emit(OpCodes.Ldarg_1);
            this.helper.Call(XmlILMethods.NavMoveTo);
            this.helper.Emit(OpCodes.Brfalse, lblVal);
            this.helper.Emit(OpCodes.Ldarg_0);
            this.helper.Emit(OpCodes.Ret);
            this.helper.MarkLabel(lblVal);
            this.helper.Emit(OpCodes.Ldarg_1);
            this.helper.Call(XmlILMethods.NavClone);
            this.helper.MethodEnd();
        }

        public void CreateTypeInitializer(XmlQueryStaticData staticData)
        {
            byte[] buffer;
            Type[] typeArray;
            staticData.GetObjectData(out buffer, out typeArray);
            FieldInfo fldInfo = this.module.DefineInitializedData("__staticData", buffer);
            FieldInfo info2 = this.module.DefineField("staticData", typeof(object));
            FieldInfo info3 = this.module.DefineField("ebTypes", typeof(Type[]));
            ConstructorInfo methInfo = this.module.DefineTypeInitializer();
            this.helper.MethodBegin(methInfo, null, false);
            this.helper.LoadInteger(buffer.Length);
            this.helper.Emit(OpCodes.Newarr, typeof(byte));
            this.helper.Emit(OpCodes.Dup);
            this.helper.Emit(OpCodes.Ldtoken, fldInfo);
            this.helper.Call(XmlILMethods.InitializeArray);
            this.helper.Emit(OpCodes.Stsfld, info2);
            if (typeArray != null)
            {
                LocalBuilder locBldr = this.helper.DeclareLocal("$$$types", typeof(Type[]));
                this.helper.LoadInteger(typeArray.Length);
                this.helper.Emit(OpCodes.Newarr, typeof(Type));
                this.helper.Emit(OpCodes.Stloc, locBldr);
                for (int i = 0; i < typeArray.Length; i++)
                {
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.LoadInteger(i);
                    this.helper.LoadType(typeArray[i]);
                    this.helper.Emit(OpCodes.Stelem_Ref);
                }
                this.helper.Emit(OpCodes.Ldloc, locBldr);
                this.helper.Emit(OpCodes.Stsfld, info3);
            }
            this.helper.MethodEnd();
        }

        private void EvaluateGlobalValues(IList<QilNode> iterList)
        {
            foreach (QilIterator iterator in iterList)
            {
                if (this.qil.IsDebug || OptimizerPatterns.Read(iterator).MatchesPattern(OptimizerPatternName.MaybeSideEffects))
                {
                    MethodInfo functionBinding = XmlILAnnotation.Write(iterator).FunctionBinding;
                    this.helper.LoadQueryRuntime();
                    this.helper.Call(functionBinding);
                    this.helper.Emit(OpCodes.Pop);
                }
            }
        }

        public XmlILCommand Generate(QilExpression query, TypeBuilder typeBldr)
        {
            XmlILCommand command;
            this.qil = query;
            bool useLRE = !this.qil.IsDebug && (typeBldr == null);
            bool isDebug = this.qil.IsDebug;
            XmlILTrace.WriteQil(this.qil, "qilbefore.xml");
            XmlILTrace.TraceOptimizations(this.qil, "qilopt.xml");
            if (XmlILTrace.IsEnabled)
            {
                useLRE = false;
            }
            this.optVisitor = new XmlILOptimizerVisitor(this.qil, !this.qil.IsDebug);
            this.qil = this.optVisitor.Optimize();
            XmlILTrace.WriteQil(this.qil, "qilafter.xml");
            try
            {
                XmlILModule.CreateModulePermissionSet.Assert();
                if (typeBldr != null)
                {
                    this.module = new XmlILModule(typeBldr);
                }
                else
                {
                    this.module = new XmlILModule(useLRE, isDebug);
                }
                this.helper = new GenerateHelper(this.module, this.qil.IsDebug);
                this.CreateHelperFunctions();
                MethodInfo methExec = this.module.DefineMethod("Execute", typeof(void), new Type[0], new string[0], XmlILMethodAttributes.NonUser);
                XmlILMethodAttributes xmlAttrs = (this.qil.Root.SourceLine == null) ? XmlILMethodAttributes.NonUser : XmlILMethodAttributes.None;
                MethodInfo methRoot = this.module.DefineMethod("Root", typeof(void), new Type[0], new string[0], xmlAttrs);
                foreach (EarlyBoundInfo info3 in this.qil.EarlyBoundTypes)
                {
                    this.helper.StaticData.DeclareEarlyBound(info3.NamespaceUri, info3.EarlyBoundType);
                }
                this.CreateFunctionMetadata(this.qil.FunctionList);
                this.CreateGlobalValueMetadata(this.qil.GlobalVariableList);
                this.CreateGlobalValueMetadata(this.qil.GlobalParameterList);
                this.GenerateExecuteFunction(methExec, methRoot);
                this.xmlIlVisitor = new XmlILVisitor();
                this.xmlIlVisitor.Visit(this.qil, this.helper, methRoot);
                XmlQueryStaticData staticData = new XmlQueryStaticData(this.qil.DefaultWriterSettings, this.qil.WhitespaceRules, this.helper.StaticData);
                if (typeBldr != null)
                {
                    this.CreateTypeInitializer(staticData);
                }
                this.module.BakeMethods();
                ExecuteDelegate delExec = (ExecuteDelegate) this.module.CreateDelegate("Execute", typeof(ExecuteDelegate));
                command = new XmlILCommand(delExec, staticData);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return command;
        }

        private MethodInfo GenerateExecuteFunction(MethodInfo methExec, MethodInfo methRoot)
        {
            this.helper.MethodBegin(methExec, null, false);
            this.EvaluateGlobalValues(this.qil.GlobalVariableList);
            this.EvaluateGlobalValues(this.qil.GlobalParameterList);
            this.helper.LoadQueryRuntime();
            this.helper.Call(methRoot);
            this.helper.MethodEnd();
            return methExec;
        }
    }
}

