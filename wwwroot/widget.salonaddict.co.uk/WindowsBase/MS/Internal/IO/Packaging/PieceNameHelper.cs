namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Zip;
    using System;
    using System.Globalization;
    using System.IO.Packaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class PieceNameHelper
    {
        private static MS.Internal.IO.Packaging.PieceNameComparer _pieceNameComparer = new MS.Internal.IO.Packaging.PieceNameComparer();

        internal static string CreatePieceName(string partName, int pieceNumber, bool isLastPiece)
        {
            Invariant.Assert(pieceNumber >= 0, "Negative piece number.");
            return string.Format(CultureInfo.InvariantCulture, "{0}/[{1:D}]{2}.piece", new object[] { partName, pieceNumber, isLastPiece ? ".last" : "" });
        }

        private static bool FindIsLast(string path, ref int position, ref ScanStepDelegate nextStep, ref PieceNameInfo parseResults)
        {
            if (path[position - 1] == ']')
            {
                parseResults.IsLastPiece = false;
                position--;
                nextStep = new ScanStepDelegate(PieceNameHelper.FindPieceNumber);
                return true;
            }
            if (!FindString(path, ref position, "].last"))
            {
                return false;
            }
            parseResults.IsLastPiece = true;
            nextStep = new ScanStepDelegate(PieceNameHelper.FindPieceNumber);
            return true;
        }

        private static bool FindPartName(string path, ref int position, ref ScanStepDelegate nextStep, ref PieceNameInfo parseResults)
        {
            parseResults.PrefixName = path.Substring(0, position);
            position = 0;
            if (parseResults.PrefixName.Length == 0)
            {
                return false;
            }
            Uri partUri = new Uri(ZipPackage.GetOpcNameFromZipItemName(parseResults.PrefixName), UriKind.Relative);
            PackUriHelper.TryValidatePartUri(partUri, out parseResults.PartUri);
            return true;
        }

        private static bool FindPieceExtension(string path, ref int position, ref ScanStepDelegate nextStep, ref PieceNameInfo parseResults)
        {
            if (!FindString(path, ref position, ".piece"))
            {
                return false;
            }
            nextStep = new ScanStepDelegate(PieceNameHelper.FindIsLast);
            return true;
        }

        private static bool FindPieceNumber(string path, ref int position, ref ScanStepDelegate nextStep, ref PieceNameInfo parseResults)
        {
            int num3;
            if (!char.IsDigit(path[position - 1]))
            {
                return false;
            }
            int num = 0;
            int num2 = 1;
            position--;
            do
            {
                num += num2 * ((int) char.GetNumericValue(path[position]));
                num2 *= 10;
                position = num3 = position - 1;
            }
            while (char.IsDigit(path[num3]));
            position++;
            if ((num2 > 10) && (((int) char.GetNumericValue(path[position])) == 0))
            {
                return false;
            }
            if (!FindString(path, ref position, "/["))
            {
                return false;
            }
            parseResults.PieceNumber = num;
            nextStep = new ScanStepDelegate(PieceNameHelper.FindPartName);
            return true;
        }

        private static bool FindString(string input, ref int position, string query)
        {
            int length = query.Length;
            if (position >= length)
            {
                while (--length >= 0)
                {
                    position--;
                    if (char.ToUpperInvariant(input[position]) != char.ToUpperInvariant(query[length]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        internal static bool TryCreatePieceInfo(ZipFileInfo zipFileInfo, out PieceInfo pieceInfo)
        {
            PieceNameInfo info;
            Invariant.Assert(zipFileInfo != null);
            pieceInfo = null;
            bool flag = TryParseAsPieceName(zipFileInfo.Name, out info);
            if (flag)
            {
                pieceInfo = new PieceInfo(zipFileInfo, info.PartUri, info.PrefixName, info.PieceNumber, info.IsLastPiece);
            }
            return flag;
        }

        private static bool TryParseAsPieceName(string path, out PieceNameInfo parseResults)
        {
            parseResults = new PieceNameInfo();
            int length = path.Length;
            ScanStepDelegate nextStep = new ScanStepDelegate(PieceNameHelper.FindPieceExtension);
            while (length > 0)
            {
                if (!nextStep(path, ref length, ref nextStep, ref parseResults))
                {
                    parseResults.IsLastPiece = false;
                    parseResults.PieceNumber = 0;
                    parseResults.PrefixName = path;
                    parseResults.PartUri = null;
                    return false;
                }
            }
            return true;
        }

        internal static MS.Internal.IO.Packaging.PieceNameComparer PieceNameComparer =>
            _pieceNameComparer;

        [StructLayout(LayoutKind.Sequential)]
        private struct PieceNameInfo
        {
            internal PackUriHelper.ValidatedPartUri PartUri;
            internal string PrefixName;
            internal int PieceNumber;
            internal bool IsLastPiece;
        }

        private delegate bool ScanStepDelegate(string path, ref int position, ref PieceNameHelper.ScanStepDelegate nextStep, ref PieceNameHelper.PieceNameInfo parseResults);
    }
}

