namespace System.Data.Mapping
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal sealed class StorageEntityTypeFunctionMapping
    {
        internal readonly StorageFunctionMapping DeleteFunctionMapping;
        internal readonly System.Data.Metadata.Edm.EntityType EntityType;
        internal readonly StorageFunctionMapping InsertFunctionMapping;
        internal readonly StorageFunctionMapping UpdateFunctionMapping;

        internal StorageEntityTypeFunctionMapping(System.Data.Metadata.Edm.EntityType entityType, StorageFunctionMapping deleteFunctionMapping, StorageFunctionMapping insertFunctionMapping, StorageFunctionMapping updateFunctionMapping)
        {
            this.EntityType = EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EntityType>(entityType, "entityType");
            this.DeleteFunctionMapping = EntityUtil.CheckArgumentNull<StorageFunctionMapping>(deleteFunctionMapping, "deleteFunctionMapping");
            this.InsertFunctionMapping = EntityUtil.CheckArgumentNull<StorageFunctionMapping>(insertFunctionMapping, "insertFunctionMapping");
            this.UpdateFunctionMapping = EntityUtil.CheckArgumentNull<StorageFunctionMapping>(updateFunctionMapping, "updateFunctionMapping");
        }

        internal void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("Entity Type Function Mapping");
            builder.Append("   ");
            builder.Append(this.ToString());
            Console.WriteLine(builder.ToString());
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "ET{{{0}}}:{4}DFunc={{{1}}},{4}IFunc={{{2}}},{4}UFunc={{{3}}}", new object[] { this.EntityType, this.DeleteFunctionMapping, this.InsertFunctionMapping, this.UpdateFunctionMapping, Environment.NewLine + "  " });
    }
}

