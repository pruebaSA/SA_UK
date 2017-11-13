namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml.Xsl.XsltOld.Debugger;

    internal class DbgCompiler : Compiler
    {
        private IXsltDebugger debugger;
        private ArrayList globalVars = new ArrayList();
        private VariableAction[] globalVarsCache;
        private ArrayList localVars = new ArrayList();
        private VariableAction[] localVarsCache;

        public DbgCompiler(IXsltDebugger debugger)
        {
            this.debugger = debugger;
        }

        public override ApplyImportsAction CreateApplyImportsAction()
        {
            ApplyImportsAction action = new ApplyImportsActionDbg();
            action.Compile(this);
            return action;
        }

        public override ApplyTemplatesAction CreateApplyTemplatesAction()
        {
            ApplyTemplatesAction action = new ApplyTemplatesActionDbg();
            action.Compile(this);
            return action;
        }

        public override AttributeAction CreateAttributeAction()
        {
            AttributeAction action = new AttributeActionDbg();
            action.Compile(this);
            return action;
        }

        public override AttributeSetAction CreateAttributeSetAction()
        {
            AttributeSetAction action = new AttributeSetActionDbg();
            action.Compile(this);
            return action;
        }

        public override BeginEvent CreateBeginEvent() => 
            new BeginEventDbg(this);

        public override CallTemplateAction CreateCallTemplateAction()
        {
            CallTemplateAction action = new CallTemplateActionDbg();
            action.Compile(this);
            return action;
        }

        public override ChooseAction CreateChooseAction()
        {
            ChooseAction action = new ChooseAction();
            action.Compile(this);
            return action;
        }

        public override CommentAction CreateCommentAction()
        {
            CommentAction action = new CommentActionDbg();
            action.Compile(this);
            return action;
        }

        public override CopyAction CreateCopyAction()
        {
            CopyAction action = new CopyActionDbg();
            action.Compile(this);
            return action;
        }

        public override CopyOfAction CreateCopyOfAction()
        {
            CopyOfAction action = new CopyOfActionDbg();
            action.Compile(this);
            return action;
        }

        public override ElementAction CreateElementAction()
        {
            ElementAction action = new ElementActionDbg();
            action.Compile(this);
            return action;
        }

        public override ForEachAction CreateForEachAction()
        {
            ForEachAction action = new ForEachActionDbg();
            action.Compile(this);
            return action;
        }

        public override IfAction CreateIfAction(IfAction.ConditionType type)
        {
            IfAction action = new IfActionDbg(type);
            action.Compile(this);
            return action;
        }

        public override MessageAction CreateMessageAction()
        {
            MessageAction action = new MessageActionDbg();
            action.Compile(this);
            return action;
        }

        public override NewInstructionAction CreateNewInstructionAction()
        {
            NewInstructionAction action = new NewInstructionActionDbg();
            action.Compile(this);
            return action;
        }

        public override NumberAction CreateNumberAction()
        {
            NumberAction action = new NumberActionDbg();
            action.Compile(this);
            return action;
        }

        public override ProcessingInstructionAction CreateProcessingInstructionAction()
        {
            ProcessingInstructionAction action = new ProcessingInstructionActionDbg();
            action.Compile(this);
            return action;
        }

        public override void CreateRootAction()
        {
            base.RootAction = new RootActionDbg();
            base.RootAction.Compile(this);
        }

        public override TemplateAction CreateSingleTemplateAction()
        {
            TemplateAction action = new TemplateActionDbg();
            action.CompileSingle(this);
            return action;
        }

        public override SortAction CreateSortAction()
        {
            SortAction action = new SortActionDbg();
            action.Compile(this);
            return action;
        }

        public override TemplateAction CreateTemplateAction()
        {
            TemplateAction action = new TemplateActionDbg();
            action.Compile(this);
            return action;
        }

        public override TextAction CreateTextAction()
        {
            TextAction action = new TextActionDbg();
            action.Compile(this);
            return action;
        }

        public override TextEvent CreateTextEvent() => 
            new TextEventDbg(this);

        public override UseAttributeSetsAction CreateUseAttributeSetsAction()
        {
            UseAttributeSetsAction action = new UseAttributeSetsActionDbg();
            action.Compile(this);
            return action;
        }

        public override ValueOfAction CreateValueOfAction()
        {
            ValueOfAction action = new ValueOfActionDbg();
            action.Compile(this);
            return action;
        }

        public override VariableAction CreateVariableAction(VariableType type)
        {
            VariableAction action = new VariableActionDbg(type);
            action.Compile(this);
            return action;
        }

        public override WithParamAction CreateWithParamAction()
        {
            WithParamAction action = new WithParamActionDbg();
            action.Compile(this);
            return action;
        }

        private void DefineVariable(VariableAction variable)
        {
            if (variable.IsGlobal)
            {
                for (int i = 0; i < this.globalVars.Count; i++)
                {
                    VariableAction action = (VariableAction) this.globalVars[i];
                    if (action.Name == variable.Name)
                    {
                        if (variable.Stylesheetid < action.Stylesheetid)
                        {
                            this.globalVars[i] = variable;
                            this.globalVarsCache = null;
                        }
                        return;
                    }
                }
                this.globalVars.Add(variable);
                this.globalVarsCache = null;
            }
            else
            {
                this.localVars.Add(variable);
                this.localVarsCache = null;
            }
        }

        internal override void PopScope()
        {
            this.UnDefineVariables(base.ScopeManager.CurrentScope.GetVeriablesCount());
            base.PopScope();
        }

        private void UnDefineVariables(int count)
        {
            if (count != 0)
            {
                this.localVars.RemoveRange(this.localVars.Count - count, count);
                this.localVarsCache = null;
            }
        }

        public override IXsltDebugger Debugger =>
            this.debugger;

        public virtual VariableAction[] GlobalVariables
        {
            get
            {
                if (this.globalVarsCache == null)
                {
                    this.globalVarsCache = (VariableAction[]) this.globalVars.ToArray(typeof(VariableAction));
                }
                return this.globalVarsCache;
            }
        }

        public virtual VariableAction[] LocalVariables
        {
            get
            {
                if (this.localVarsCache == null)
                {
                    this.localVarsCache = (VariableAction[]) this.localVars.ToArray(typeof(VariableAction));
                }
                return this.localVarsCache;
            }
        }

        private class ApplyImportsActionDbg : ApplyImportsAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class ApplyTemplatesActionDbg : ApplyTemplatesAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class AttributeActionDbg : AttributeAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class AttributeSetActionDbg : AttributeSetAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class BeginEventDbg : BeginEvent
        {
            private System.Xml.Xsl.XsltOld.DbgData dbgData;

            public BeginEventDbg(Compiler compiler) : base(compiler)
            {
                this.dbgData = new System.Xml.Xsl.XsltOld.DbgData(compiler);
            }

            public override bool Output(Processor processor, ActionFrame frame)
            {
                base.OnInstructionExecute(processor);
                return base.Output(processor, frame);
            }

            internal override System.Xml.Xsl.XsltOld.DbgData DbgData =>
                this.dbgData;
        }

        private class CallTemplateActionDbg : CallTemplateAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class CommentActionDbg : CommentAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class CopyActionDbg : CopyAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class CopyOfActionDbg : CopyOfAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class ElementActionDbg : ElementAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class ForEachActionDbg : ForEachAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
                if (frame.State == -1)
                {
                    processor.PopDebuggerStack();
                }
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class IfActionDbg : IfAction
        {
            private DbgData dbgData;

            internal IfActionDbg(IfAction.ConditionType type) : base(type)
            {
            }

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class MessageActionDbg : MessageAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class NewInstructionActionDbg : NewInstructionAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class NumberActionDbg : NumberAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class ProcessingInstructionActionDbg : ProcessingInstructionAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class RootActionDbg : RootAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
                string builtInTemplatesUri = compiler.Debugger.GetBuiltInTemplatesUri();
                if ((builtInTemplatesUri != null) && (builtInTemplatesUri.Length != 0))
                {
                    compiler.AllowBuiltInMode = true;
                    base.builtInSheet = compiler.RootAction.CompileImport(compiler, compiler.ResolveUri(builtInTemplatesUri), 0x7fffffff);
                    compiler.AllowBuiltInMode = false;
                }
                this.dbgData.ReplaceVariables(((DbgCompiler) compiler).GlobalVariables);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                    processor.PushDebuggerStack();
                }
                base.Execute(processor, frame);
                if (frame.State == -1)
                {
                    processor.PopDebuggerStack();
                    processor.PopDebuggerStack();
                }
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class SortActionDbg : SortAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class TemplateActionDbg : TemplateAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
                if (frame.State == -1)
                {
                    processor.PopDebuggerStack();
                }
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class TextActionDbg : TextAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class TextEventDbg : TextEvent
        {
            private System.Xml.Xsl.XsltOld.DbgData dbgData;

            public TextEventDbg(Compiler compiler) : base(compiler)
            {
                this.dbgData = new System.Xml.Xsl.XsltOld.DbgData(compiler);
            }

            public override bool Output(Processor processor, ActionFrame frame)
            {
                base.OnInstructionExecute(processor);
                return base.Output(processor, frame);
            }

            internal override System.Xml.Xsl.XsltOld.DbgData DbgData =>
                this.dbgData;
        }

        private class UseAttributeSetsActionDbg : UseAttributeSetsAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class ValueOfActionDbg : ValueOfAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class VariableActionDbg : VariableAction
        {
            private DbgData dbgData;

            internal VariableActionDbg(VariableType type) : base(type)
            {
            }

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
                ((DbgCompiler) compiler).DefineVariable(this);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }

        private class WithParamActionDbg : WithParamAction
        {
            private DbgData dbgData;

            internal override void Compile(Compiler compiler)
            {
                this.dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == 0)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }

            internal override DbgData GetDbgData(ActionFrame frame) => 
                this.dbgData;
        }
    }
}

