namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class ServiceModelTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
    {
        private TimeSpanValidatorAttribute innerValidatorAttribute = new TimeSpanValidatorAttribute();

        public ServiceModelTimeSpanValidatorAttribute()
        {
            this.innerValidatorAttribute.MaxValueString = TimeoutHelper.MaxWait.ToString();
        }

        public TimeSpan MaxValue =>
            this.innerValidatorAttribute.MaxValue;

        public string MaxValueString
        {
            get => 
                this.innerValidatorAttribute.MaxValueString;
            set
            {
                this.innerValidatorAttribute.MaxValueString = value;
            }
        }

        public TimeSpan MinValue =>
            this.innerValidatorAttribute.MinValue;

        public string MinValueString
        {
            get => 
                this.innerValidatorAttribute.MinValueString;
            set
            {
                this.innerValidatorAttribute.MinValueString = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new TimeSpanOrInfiniteValidator(this.MinValue, this.MaxValue);
    }
}

