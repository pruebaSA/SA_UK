namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct InputForComputingCellGroups : IEquatable<InputForComputingCellGroups>, IEqualityComparer<InputForComputingCellGroups>
    {
        internal readonly StorageEntityContainerMapping ContainerMapping;
        internal readonly ConfigViewGenerator Config;
        internal InputForComputingCellGroups(StorageEntityContainerMapping containerMapping, ConfigViewGenerator config)
        {
            this.ContainerMapping = containerMapping;
            this.Config = config;
        }

        public bool Equals(InputForComputingCellGroups other) => 
            (this.ContainerMapping.Equals(other.ContainerMapping) && this.Config.Equals(other.Config));

        public bool Equals(InputForComputingCellGroups one, InputForComputingCellGroups two) => 
            (object.ReferenceEquals(one, two) || ((!object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) && one.Equals(two)));

        public int GetHashCode(InputForComputingCellGroups value) => 
            value.GetHashCode();

        public override int GetHashCode() => 
            this.ContainerMapping.GetHashCode();

        public override bool Equals(object obj) => 
            ((obj is InputForComputingCellGroups) && this.Equals((InputForComputingCellGroups) obj));

        public static bool operator ==(InputForComputingCellGroups input1, InputForComputingCellGroups input2) => 
            (object.ReferenceEquals(input1, input2) || input1.Equals(input2));

        public static bool operator !=(InputForComputingCellGroups input1, InputForComputingCellGroups input2) => 
            !(input1 == input2);
    }
}

