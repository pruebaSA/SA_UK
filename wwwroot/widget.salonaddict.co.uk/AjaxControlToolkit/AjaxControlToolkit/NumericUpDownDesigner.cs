namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;

    public class NumericUpDownDesigner : ExtenderControlBaseDesigner<NumericUpDownExtender>
    {
        [PageMethodSignature("\"Get Next\" NumericUpDown", "ServiceUpPath", "ServiceUpMethod")]
        private delegate int GetNextValue(int current, string tag);

        [PageMethodSignature("\"Get Previous\" NumericUpDown", "ServiceDownPath", "ServiceDownMethod")]
        private delegate int GetPreviousValue(int current, string tag);
    }
}

