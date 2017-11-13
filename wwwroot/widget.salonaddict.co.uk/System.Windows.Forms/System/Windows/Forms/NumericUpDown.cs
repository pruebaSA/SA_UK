﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [ComVisible(true), DefaultBindingProperty("Value"), System.Windows.Forms.SRDescription("DescriptionNumericUpDown"), DefaultProperty("Value"), DefaultEvent("ValueChanged"), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class NumericUpDown : UpDownBase, ISupportInitialize
    {
        private NumericUpDownAccelerationCollection accelerations;
        private int accelerationsCurrentIndex;
        private long buttonPressedStartTime;
        private decimal currentValue = DefaultValue;
        private bool currentValueChanged;
        private int decimalPlaces;
        private const int DefaultDecimalPlaces = 0;
        private const bool DefaultHexadecimal = false;
        private static readonly decimal DefaultIncrement = 1M;
        private static readonly decimal DefaultMaximum = 100M;
        private static readonly decimal DefaultMinimum = 0M;
        private const bool DefaultThousandsSeparator = false;
        private static readonly decimal DefaultValue = 0M;
        private bool hexadecimal;
        private decimal increment = DefaultIncrement;
        private bool initializing;
        private const int InvalidValue = -1;
        private decimal maximum = DefaultMaximum;
        private decimal minimum = DefaultMinimum;
        private bool thousandsSeparator;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler PaddingChanged
        {
            add
            {
                base.PaddingChanged += value;
            }
            remove
            {
                base.PaddingChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        [System.Windows.Forms.SRCategory("CatAction"), System.Windows.Forms.SRDescription("NumericUpDownOnValueChangedDescr")]
        public event EventHandler ValueChanged;

        public NumericUpDown()
        {
            base.SetState2(0x800, true);
            this.Text = "0";
            this.StopAcceleration();
        }

        public void BeginInit()
        {
            this.initializing = true;
        }

        private decimal Constrain(decimal value)
        {
            if (value < this.minimum)
            {
                value = this.minimum;
            }
            if (value > this.maximum)
            {
                value = this.maximum;
            }
            return value;
        }

        protected override AccessibleObject CreateAccessibilityInstance() => 
            new NumericUpDownAccessibleObject(this);

        public override void DownButton()
        {
            this.SetNextAcceleration();
            if (base.UserEdit)
            {
                this.ParseEditText();
            }
            decimal currentValue = this.currentValue;
            try
            {
                currentValue -= this.Increment;
                if (currentValue < this.minimum)
                {
                    currentValue = this.minimum;
                    if (this.Spinning)
                    {
                        this.StopAcceleration();
                    }
                }
            }
            catch (OverflowException)
            {
                currentValue = this.minimum;
            }
            this.Value = currentValue;
        }

        public void EndInit()
        {
            this.initializing = false;
            this.Value = this.Constrain(this.currentValue);
            this.UpdateEditText();
        }

        private int GetLargestDigit(int start, int end)
        {
            int num = -1;
            int width = -1;
            for (int i = start; i < end; i++)
            {
                char ch;
                if (i < 10)
                {
                    ch = i.ToString(CultureInfo.InvariantCulture)[0];
                }
                else
                {
                    ch = (char) (0x41 + (i - 10));
                }
                Size size = TextRenderer.MeasureText(ch.ToString(), this.Font);
                if (size.Width >= width)
                {
                    width = size.Width;
                    num = i;
                }
            }
            return num;
        }

        private string GetNumberText(decimal num)
        {
            if (this.Hexadecimal)
            {
                long num2 = (long) num;
                return num2.ToString("X", CultureInfo.InvariantCulture);
            }
            return num.ToString((this.ThousandsSeparator ? "N" : "F") + this.DecimalPlaces.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            decimal num5;
            int preferredHeight = base.PreferredHeight;
            int end = this.Hexadecimal ? 0x10 : 10;
            int largestDigit = this.GetLargestDigit(0, end);
            int num4 = (int) Math.Floor(Math.Log(Math.Max(-((double) this.Minimum), (double) this.Maximum), (double) end));
            if ((largestDigit != 0) || (num4 == 1))
            {
                num5 = largestDigit;
            }
            else
            {
                num5 = this.GetLargestDigit(1, end);
            }
            for (int i = 0; i < num4; i++)
            {
                num5 = (num5 * end) + largestDigit;
            }
            int width = TextRenderer.MeasureText(this.GetNumberText(num5), this.Font).Width;
            int num8 = base.SizeFromClientSize(width, preferredHeight).Width + base.upDownButtons.Width;
            return (new Size(num8, preferredHeight) + this.Padding.Size);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((base.InterceptArrowKeys && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))) && !this.Spinning)
            {
                this.StartAcceleration();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (base.InterceptArrowKeys && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
            {
                this.StopAcceleration();
            }
            base.OnKeyUp(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (base.UserEdit)
            {
                this.UpdateEditText();
            }
        }

        internal override void OnStartTimer()
        {
            this.StartAcceleration();
        }

        internal override void OnStopTimer()
        {
            this.StopAcceleration();
        }

        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            base.OnTextBoxKeyPress(source, e);
            NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
            string numberGroupSeparator = numberFormat.NumberGroupSeparator;
            string negativeSign = numberFormat.NegativeSign;
            string str4 = e.KeyChar.ToString();
            if (((!char.IsDigit(e.KeyChar) && (!str4.Equals(numberDecimalSeparator) && !str4.Equals(numberGroupSeparator))) && !str4.Equals(negativeSign)) && (e.KeyChar != '\b'))
            {
                if (this.Hexadecimal)
                {
                    if ((e.KeyChar >= 'a') && (e.KeyChar <= 'f'))
                    {
                        return;
                    }
                    if ((e.KeyChar >= 'A') && (e.KeyChar <= 'F'))
                    {
                        return;
                    }
                }
                if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None)
                {
                    e.Handled = true;
                    System.Windows.Forms.SafeNativeMethods.MessageBeep(0);
                }
            }
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (this.onValueChanged != null)
            {
                this.onValueChanged(this, e);
            }
        }

        protected void ParseEditText()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.Text) && ((this.Text.Length != 1) || (this.Text != "-")))
                {
                    if (this.Hexadecimal)
                    {
                        this.Value = this.Constrain(Convert.ToDecimal(Convert.ToInt32(this.Text, 0x10)));
                    }
                    else
                    {
                        this.Value = this.Constrain(decimal.Parse(this.Text, CultureInfo.CurrentCulture));
                    }
                }
            }
            catch
            {
            }
            finally
            {
                base.UserEdit = false;
            }
        }

        private void ResetIncrement()
        {
            this.Increment = DefaultIncrement;
        }

        private void ResetMaximum()
        {
            this.Maximum = DefaultMaximum;
        }

        private void ResetMinimum()
        {
            this.Minimum = DefaultMinimum;
        }

        private void ResetValue()
        {
            this.Value = DefaultValue;
        }

        private void SetNextAcceleration()
        {
            if (this.Spinning && (this.accelerationsCurrentIndex < (this.accelerations.Count - 1)))
            {
                long ticks = DateTime.Now.Ticks;
                long num2 = ticks - this.buttonPressedStartTime;
                long num3 = 0x989680L * this.accelerations[this.accelerationsCurrentIndex + 1].Seconds;
                if (num2 > num3)
                {
                    this.buttonPressedStartTime = ticks;
                    this.accelerationsCurrentIndex++;
                }
            }
        }

        private bool ShouldSerializeIncrement() => 
            !this.Increment.Equals(DefaultIncrement);

        private bool ShouldSerializeMaximum() => 
            !this.Maximum.Equals(DefaultMaximum);

        private bool ShouldSerializeMinimum() => 
            !this.Minimum.Equals(DefaultMinimum);

        private bool ShouldSerializeValue() => 
            !this.Value.Equals(DefaultValue);

        private void StartAcceleration()
        {
            this.buttonPressedStartTime = DateTime.Now.Ticks;
        }

        private void StopAcceleration()
        {
            this.accelerationsCurrentIndex = -1;
            this.buttonPressedStartTime = -1L;
        }

        public override string ToString()
        {
            string str2 = base.ToString();
            return (str2 + ", Minimum = " + this.Minimum.ToString(CultureInfo.CurrentCulture) + ", Maximum = " + this.Maximum.ToString(CultureInfo.CurrentCulture));
        }

        public override void UpButton()
        {
            this.SetNextAcceleration();
            if (base.UserEdit)
            {
                this.ParseEditText();
            }
            decimal currentValue = this.currentValue;
            try
            {
                currentValue += this.Increment;
                if (currentValue > this.maximum)
                {
                    currentValue = this.maximum;
                    if (this.Spinning)
                    {
                        this.StopAcceleration();
                    }
                }
            }
            catch (OverflowException)
            {
                currentValue = this.maximum;
            }
            this.Value = currentValue;
        }

        protected override void UpdateEditText()
        {
            if (!this.initializing)
            {
                if (base.UserEdit)
                {
                    this.ParseEditText();
                }
                if (this.currentValueChanged || (!string.IsNullOrEmpty(this.Text) && ((this.Text.Length != 1) || (this.Text != "-"))))
                {
                    this.currentValueChanged = false;
                    base.ChangingText = true;
                    this.Text = this.GetNumberText(this.currentValue);
                }
            }
        }

        protected override void ValidateEditText()
        {
            this.ParseEditText();
            this.UpdateEditText();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NumericUpDownAccelerationCollection Accelerations
        {
            get
            {
                if (this.accelerations == null)
                {
                    this.accelerations = new NumericUpDownAccelerationCollection();
                }
                return this.accelerations;
            }
        }

        [System.Windows.Forms.SRDescription("NumericUpDownDecimalPlacesDescr"), System.Windows.Forms.SRCategory("CatData"), DefaultValue(0)]
        public int DecimalPlaces
        {
            get => 
                this.decimalPlaces;
            set
            {
                if ((value < 0) || (value > 0x63))
                {
                    object[] args = new object[] { "DecimalPlaces", value.ToString(CultureInfo.CurrentCulture), 0.ToString(CultureInfo.CurrentCulture), "99" };
                    throw new ArgumentOutOfRangeException("DecimalPlaces", System.Windows.Forms.SR.GetString("InvalidBoundArgument", args));
                }
                this.decimalPlaces = value;
                this.UpdateEditText();
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(false), System.Windows.Forms.SRDescription("NumericUpDownHexadecimalDescr")]
        public bool Hexadecimal
        {
            get => 
                this.hexadecimal;
            set
            {
                this.hexadecimal = value;
                this.UpdateEditText();
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("NumericUpDownIncrementDescr")]
        public decimal Increment
        {
            get
            {
                if (this.accelerationsCurrentIndex != -1)
                {
                    return this.Accelerations[this.accelerationsCurrentIndex].Increment;
                }
                return this.increment;
            }
            set
            {
                if (value < 0M)
                {
                    throw new ArgumentOutOfRangeException("Increment", System.Windows.Forms.SR.GetString("InvalidArgument", new object[] { "Increment", value.ToString(CultureInfo.CurrentCulture) }));
                }
                this.increment = value;
            }
        }

        [System.Windows.Forms.SRDescription("NumericUpDownMaximumDescr"), System.Windows.Forms.SRCategory("CatData"), RefreshProperties(RefreshProperties.All)]
        public decimal Maximum
        {
            get => 
                this.maximum;
            set
            {
                this.maximum = value;
                if (this.minimum > this.maximum)
                {
                    this.minimum = this.maximum;
                }
                this.Value = this.Constrain(this.currentValue);
            }
        }

        [System.Windows.Forms.SRDescription("NumericUpDownMinimumDescr"), System.Windows.Forms.SRCategory("CatData"), RefreshProperties(RefreshProperties.All)]
        public decimal Minimum
        {
            get => 
                this.minimum;
            set
            {
                this.minimum = value;
                if (this.minimum > this.maximum)
                {
                    this.maximum = value;
                }
                this.Value = this.Constrain(this.currentValue);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.Padding Padding
        {
            get => 
                base.Padding;
            set
            {
                base.Padding = value;
            }
        }

        private bool Spinning =>
            ((this.accelerations != null) && (this.buttonPressedStartTime != -1L));

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
            }
        }

        [System.Windows.Forms.SRDescription("NumericUpDownThousandsSeparatorDescr"), System.Windows.Forms.SRCategory("CatData"), DefaultValue(false), Localizable(true)]
        public bool ThousandsSeparator
        {
            get => 
                this.thousandsSeparator;
            set
            {
                this.thousandsSeparator = value;
                this.UpdateEditText();
            }
        }

        [Bindable(true), System.Windows.Forms.SRDescription("NumericUpDownValueDescr"), System.Windows.Forms.SRCategory("CatAppearance")]
        public decimal Value
        {
            get
            {
                if (base.UserEdit)
                {
                    this.ValidateEditText();
                }
                return this.currentValue;
            }
            set
            {
                if (value != this.currentValue)
                {
                    if (!this.initializing && ((value < this.minimum) || (value > this.maximum)))
                    {
                        throw new ArgumentOutOfRangeException("Value", System.Windows.Forms.SR.GetString("InvalidBoundArgument", new object[] { "Value", value.ToString(CultureInfo.CurrentCulture), "'Minimum'", "'Maximum'" }));
                    }
                    this.currentValue = value;
                    this.OnValueChanged(EventArgs.Empty);
                    this.currentValueChanged = true;
                    this.UpdateEditText();
                }
            }
        }

        [ComVisible(true)]
        internal class NumericUpDownAccessibleObject : Control.ControlAccessibleObject
        {
            public NumericUpDownAccessibleObject(NumericUpDown owner) : base(owner)
            {
            }

            public override AccessibleObject GetChild(int index)
            {
                if ((index >= 0) && (index < this.GetChildCount()))
                {
                    if (index == 0)
                    {
                        return ((UpDownBase) base.Owner).TextBox.AccessibilityObject.Parent;
                    }
                    if (index == 1)
                    {
                        return ((UpDownBase) base.Owner).UpDownButtonsInternal.AccessibilityObject.Parent;
                    }
                }
                return null;
            }

            public override int GetChildCount() => 
                2;

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole accessibleRole = base.Owner.AccessibleRole;
                    if (accessibleRole != AccessibleRole.Default)
                    {
                        return accessibleRole;
                    }
                    return AccessibleRole.ComboBox;
                }
            }
        }
    }
}

