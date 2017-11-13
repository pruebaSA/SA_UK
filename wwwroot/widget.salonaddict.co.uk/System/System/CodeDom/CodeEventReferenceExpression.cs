﻿namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeEventReferenceExpression : CodeExpression
    {
        private string eventName;
        private CodeExpression targetObject;

        public CodeEventReferenceExpression()
        {
        }

        public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
        {
            this.targetObject = targetObject;
            this.eventName = eventName;
        }

        public string EventName
        {
            get
            {
                if (this.eventName != null)
                {
                    return this.eventName;
                }
                return string.Empty;
            }
            set
            {
                this.eventName = value;
            }
        }

        public CodeExpression TargetObject
        {
            get => 
                this.targetObject;
            set
            {
                this.targetObject = value;
            }
        }
    }
}

