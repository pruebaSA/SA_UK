namespace System.Data.Objects.DataClasses
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable, DataContract(IsReference=true)]
    public abstract class StructuralObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public static readonly string EntityKeyPropertyName = "-EntityKey-";

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        protected StructuralObject()
        {
        }

        protected static DateTime DefaultDateTimeValue() => 
            DateTime.Now;

        protected internal static byte[] GetValidValue(byte[] currentValue)
        {
            if (currentValue == null)
            {
                return null;
            }
            return (byte[]) currentValue.Clone();
        }

        protected internal T GetValidValue<T>(T currentValue, string property, bool isNullable, bool isInitialized) where T: ComplexObject, new()
        {
            if (!isNullable && !isInitialized)
            {
                currentValue = this.SetValidValue<T>(currentValue, Activator.CreateInstance<T>(), property);
            }
            return currentValue;
        }

        protected virtual void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        protected virtual void OnPropertyChanging(string property)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(property));
            }
        }

        internal abstract void ReportComplexPropertyChanged(string entityMemberName, ComplexObject complexObject, string complexMemberName);
        internal abstract void ReportComplexPropertyChanging(string entityMemberName, ComplexObject complexObject, string complexMemberName);
        protected virtual void ReportPropertyChanged(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            this.OnPropertyChanged(property);
        }

        protected virtual void ReportPropertyChanging(string property)
        {
            EntityUtil.CheckStringArgument(property, "property");
            this.OnPropertyChanging(property);
        }

        protected internal static bool SetValidValue(bool value) => 
            value;

        protected internal static byte SetValidValue(byte value) => 
            value;

        protected internal static DateTime SetValidValue(DateTime value) => 
            value;

        protected internal static DateTimeOffset SetValidValue(DateTimeOffset value) => 
            value;

        protected internal static decimal SetValidValue(decimal value) => 
            value;

        protected internal static double SetValidValue(double value) => 
            value;

        protected internal static Guid SetValidValue(Guid value) => 
            value;

        protected internal static short SetValidValue(short value) => 
            value;

        protected internal static int SetValidValue(int value) => 
            value;

        protected internal static long SetValidValue(long value) => 
            value;

        protected internal static bool? SetValidValue(bool? value) => 
            value;

        protected internal static byte? SetValidValue(byte? value) => 
            value;

        protected internal static DateTime? SetValidValue(DateTime? value) => 
            value;

        protected internal static DateTimeOffset? SetValidValue(DateTimeOffset? value) => 
            value;

        protected internal static decimal? SetValidValue(decimal? value) => 
            value;

        protected internal static double? SetValidValue(double? value) => 
            value;

        protected internal static Guid? SetValidValue(Guid? value) => 
            value;

        protected internal static short? SetValidValue(short? value) => 
            value;

        protected internal static int? SetValidValue(int? value) => 
            value;

        protected internal static long? SetValidValue(long? value) => 
            value;

        [CLSCompliant(false)]
        protected internal static sbyte? SetValidValue(sbyte? value) => 
            value;

        protected internal static float? SetValidValue(float? value) => 
            value;

        protected internal static TimeSpan? SetValidValue(TimeSpan? value) => 
            value;

        [CLSCompliant(false)]
        protected internal static ushort? SetValidValue(ushort? value) => 
            value;

        [CLSCompliant(false)]
        protected internal static uint? SetValidValue(uint? value) => 
            value;

        [CLSCompliant(false)]
        protected internal static ulong? SetValidValue(ulong? value) => 
            value;

        [CLSCompliant(false)]
        protected internal static sbyte SetValidValue(sbyte value) => 
            value;

        protected internal static float SetValidValue(float value) => 
            value;

        protected internal static TimeSpan SetValidValue(TimeSpan value) => 
            value;

        [CLSCompliant(false)]
        protected internal static ushort SetValidValue(ushort value) => 
            value;

        [CLSCompliant(false)]
        protected internal static uint SetValidValue(uint value) => 
            value;

        [CLSCompliant(false)]
        protected internal static ulong SetValidValue(ulong value) => 
            value;

        protected internal static byte[] SetValidValue(byte[] value, bool isNullable)
        {
            if (value != null)
            {
                return (byte[]) value.Clone();
            }
            if (!isNullable)
            {
                EntityUtil.ThrowPropertyIsNotNullable();
            }
            return value;
        }

        protected internal static string SetValidValue(string value, bool isNullable)
        {
            if ((value == null) && !isNullable)
            {
                EntityUtil.ThrowPropertyIsNotNullable();
            }
            return value;
        }

        protected internal T SetValidValue<T>(T oldValue, T newValue, string property) where T: ComplexObject
        {
            if ((newValue == null) && this.IsChangeTracked)
            {
                throw EntityUtil.NullableComplexTypesNotSupported(property);
            }
            if (oldValue != null)
            {
                oldValue.DetachFromParent();
            }
            if (newValue != null)
            {
                newValue.AttachToParent(this, property);
            }
            return newValue;
        }

        protected internal static TComplex VerifyComplexObjectIsNotNull<TComplex>(TComplex complexObject, string propertyName) where TComplex: ComplexObject
        {
            if (complexObject == null)
            {
                EntityUtil.ThrowPropertyIsNotNullable(propertyName);
            }
            return complexObject;
        }

        internal abstract bool IsChangeTracked { get; }
    }
}

