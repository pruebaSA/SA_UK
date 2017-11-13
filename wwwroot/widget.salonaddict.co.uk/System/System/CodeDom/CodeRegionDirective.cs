namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeRegionDirective : CodeDirective
    {
        private CodeRegionMode regionMode;
        private string regionText;

        public CodeRegionDirective()
        {
        }

        public CodeRegionDirective(CodeRegionMode regionMode, string regionText)
        {
            this.RegionText = regionText;
            this.regionMode = regionMode;
        }

        public CodeRegionMode RegionMode
        {
            get => 
                this.regionMode;
            set
            {
                this.regionMode = value;
            }
        }

        public string RegionText
        {
            get
            {
                if (this.regionText != null)
                {
                    return this.regionText;
                }
                return string.Empty;
            }
            set
            {
                this.regionText = value;
            }
        }
    }
}

