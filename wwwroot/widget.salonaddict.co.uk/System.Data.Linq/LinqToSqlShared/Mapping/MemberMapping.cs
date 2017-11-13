namespace LinqToSqlShared.Mapping
{
    using System;

    internal abstract class MemberMapping
    {
        private string member;
        private string name;
        private string storageMember;

        internal MemberMapping()
        {
        }

        internal string DbName
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal string MemberName
        {
            get => 
                this.member;
            set
            {
                this.member = value;
            }
        }

        internal string StorageMemberName
        {
            get => 
                this.storageMember;
            set
            {
                this.storageMember = value;
            }
        }
    }
}

