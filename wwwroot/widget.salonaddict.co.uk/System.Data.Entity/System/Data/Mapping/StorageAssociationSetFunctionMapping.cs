namespace System.Data.Mapping
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal sealed class StorageAssociationSetFunctionMapping
    {
        internal readonly System.Data.Metadata.Edm.AssociationSet AssociationSet;
        internal readonly StorageFunctionMapping DeleteFunctionMapping;
        internal readonly StorageFunctionMapping InsertFunctionMapping;

        internal StorageAssociationSetFunctionMapping(System.Data.Metadata.Edm.AssociationSet associationSet, StorageFunctionMapping deleteFunctionMapping, StorageFunctionMapping insertFunctionMapping)
        {
            this.AssociationSet = EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.AssociationSet>(associationSet, "associationSet");
            this.DeleteFunctionMapping = EntityUtil.CheckArgumentNull<StorageFunctionMapping>(deleteFunctionMapping, "deleteFunctionMapping");
            this.InsertFunctionMapping = EntityUtil.CheckArgumentNull<StorageFunctionMapping>(insertFunctionMapping, "insertFunctionMapping");
        }

        internal void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("Association Set Function Mapping");
            builder.Append("   ");
            builder.Append(this.ToString());
            Console.WriteLine(builder.ToString());
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "AS{{{0}}}:{3}DFunc={{{1}}},{3}IFunc={{{2}}}", new object[] { this.AssociationSet, this.DeleteFunctionMapping, this.InsertFunctionMapping, Environment.NewLine + "  " });
    }
}

