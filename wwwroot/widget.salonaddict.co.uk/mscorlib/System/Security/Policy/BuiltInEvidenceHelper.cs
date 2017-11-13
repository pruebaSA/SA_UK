namespace System.Security.Policy
{
    using System;

    internal static class BuiltInEvidenceHelper
    {
        internal const char idApplicationDirectory = '\0';
        internal const char idGac = '\t';
        internal const char idHash = '\b';
        internal const char idPermissionRequestEvidence = '\a';
        internal const char idPublisher = '\x0001';
        internal const char idSite = '\x0006';
        internal const char idStrongName = '\x0002';
        internal const char idUrl = '\x0004';
        internal const char idWebPage = '\x0005';
        internal const char idZone = '\x0003';

        internal static void CopyIntToCharArray(int value, char[] buffer, int position)
        {
            buffer[position] = (char) ((value >> 0x10) & 0xffff);
            buffer[position + 1] = (char) (value & 0xffff);
        }

        internal static void CopyLongToCharArray(long value, char[] buffer, int position)
        {
            buffer[position] = (char) ((ushort) ((value >> 0x30) & 0xffffL));
            buffer[position + 1] = (char) ((ushort) ((value >> 0x20) & 0xffffL));
            buffer[position + 2] = (char) ((ushort) ((value >> 0x10) & 0xffffL));
            buffer[position + 3] = (char) ((ushort) (value & 0xffffL));
        }

        internal static int GetIntFromCharArray(char[] buffer, int position)
        {
            int num = buffer[position];
            num = num << 0x10;
            return (num + buffer[position + 1]);
        }

        internal static long GetLongFromCharArray(char[] buffer, int position)
        {
            long num = (long) buffer[position];
            num = num << 0x10;
            num += (long) buffer[position + 1];
            num = num << 0x10;
            num += (long) buffer[position + 2];
            num = num << 0x10;
            return (num + ((long) buffer[position + 3]));
        }
    }
}

