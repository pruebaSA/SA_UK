namespace System.Windows.Forms
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Security.Permissions;

    public class FileDialogCustomPlacesCollection : Collection<FileDialogCustomPlace>
    {
        public void Add(Guid knownFolderGuid)
        {
            base.Add(new FileDialogCustomPlace(knownFolderGuid));
        }

        public void Add(string path)
        {
            base.Add(new FileDialogCustomPlace(path));
        }

        internal void Apply(FileDialogNative.IFileDialog dialog)
        {
            new FileIOPermission(PermissionState.Unrestricted).Assert();
            for (int i = base.Items.Count - 1; i >= 0; i--)
            {
                FileDialogCustomPlace place = base.Items[i];
                try
                {
                    FileDialogNative.IShellItem nativePath = place.GetNativePath();
                    if (nativePath != null)
                    {
                        dialog.AddPlace(nativePath, 0);
                    }
                }
                catch (FileNotFoundException)
                {
                }
            }
        }
    }
}

