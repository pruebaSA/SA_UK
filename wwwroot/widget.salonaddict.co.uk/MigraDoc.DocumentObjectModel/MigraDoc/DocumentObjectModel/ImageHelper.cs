namespace MigraDoc.DocumentObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class ImageHelper
    {
        public static string ExtractPageNumber(string path, out int pageNumber)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            pageNumber = 0;
            int length = path.Length;
            if (length != 0)
            {
                length--;
                if (!char.IsDigit(path, length))
                {
                    return path;
                }
                while (char.IsDigit(path, length) && (length >= 0))
                {
                    length--;
                }
                if (((length > 0) && (path[length] == '#')) && (path.IndexOf('.') != -1))
                {
                    pageNumber = int.Parse(path.Substring(length + 1));
                    path = path.Substring(0, length);
                }
            }
            return path;
        }

        public static string GetImageName(string root, string filename, string imagePath)
        {
            try
            {
                List<string> list = new List<string>(imagePath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) { "" };
                foreach (string str in list)
                {
                    int num;
                    string path = Path.Combine(Path.Combine(root, str), filename);
                    if (File.Exists(ExtractPageNumber(path, out num)))
                    {
                        return path;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static bool InSubfolder(string root, string filename, string imagePath, string referenceFilename)
        {
            List<string> list = new List<string>(imagePath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) { "" };
            foreach (string str in list)
            {
                int num;
                string path = Path.Combine(Path.Combine(root, str), filename);
                if (File.Exists(ExtractPageNumber(path, out num)) && (path == referenceFilename))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

