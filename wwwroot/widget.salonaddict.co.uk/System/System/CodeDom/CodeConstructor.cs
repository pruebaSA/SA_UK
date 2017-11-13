﻿namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeConstructor : CodeMemberMethod
    {
        private CodeExpressionCollection baseConstructorArgs = new CodeExpressionCollection();
        private CodeExpressionCollection chainedConstructorArgs = new CodeExpressionCollection();

        public CodeConstructor()
        {
            base.Name = ".ctor";
        }

        public CodeExpressionCollection BaseConstructorArgs =>
            this.baseConstructorArgs;

        public CodeExpressionCollection ChainedConstructorArgs =>
            this.chainedConstructorArgs;
    }
}

