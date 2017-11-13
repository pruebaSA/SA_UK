namespace MS.Internal.IO.Zip
{
    using System;

    internal interface IZipIOBlock
    {
        bool GetDirtyFlag(bool closingFlag);
        void Move(long shiftSize);
        PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size);
        void Save();
        void UpdateReferences(bool closingFlag);

        long Offset { get; }

        long Size { get; }
    }
}

