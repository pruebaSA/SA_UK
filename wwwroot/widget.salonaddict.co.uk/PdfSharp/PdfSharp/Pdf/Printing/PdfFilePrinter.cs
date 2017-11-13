namespace PdfSharp.Pdf.Printing
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class PdfFilePrinter
    {
        private static string adobeReaderPath;
        private static string defaultPrinterName;
        private string pdfFileName;
        private string printerName;
        private static Process runningAcro;
        private string workingDirectory;

        public PdfFilePrinter()
        {
        }

        public PdfFilePrinter(string pdfFileName)
        {
            this.PdfFileName = pdfFileName;
        }

        public PdfFilePrinter(string pdfFileName, string printerName)
        {
            this.pdfFileName = pdfFileName;
            this.printerName = printerName;
        }

        private void DoSomeVeryDirtyHacksToMakeItWork()
        {
            if (runningAcro != null)
            {
                if (!runningAcro.HasExited)
                {
                    return;
                }
                runningAcro.Dispose();
                runningAcro = null;
            }
            Process[] processes = Process.GetProcesses();
            int length = processes.Length;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    Process process = processes[i];
                    if (string.Compare(Path.GetFileName(process.MainModule.FileName), Path.GetFileName(adobeReaderPath), true) == 0)
                    {
                        runningAcro = process;
                        break;
                    }
                }
                catch
                {
                }
            }
            if (runningAcro == null)
            {
                runningAcro = Process.Start(adobeReaderPath);
                runningAcro.WaitForInputIdle();
            }
        }

        public void Print()
        {
            this.Print(-1);
        }

        public void Print(int milliseconds)
        {
            if ((this.printerName == null) || (this.printerName.Length == 0))
            {
                this.printerName = defaultPrinterName;
            }
            if ((adobeReaderPath == null) || (adobeReaderPath.Length == 0))
            {
                throw new InvalidOperationException("No full qualified path to AcroRd32.exe or Acrobat.exe is set.");
            }
            if ((this.printerName == null) || (this.printerName.Length == 0))
            {
                throw new InvalidOperationException("No printer name set.");
            }
            string path = string.Empty;
            if ((this.workingDirectory != null) && (this.workingDirectory.Length != 0))
            {
                path = Path.Combine(this.workingDirectory, this.pdfFileName);
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), this.pdfFileName);
            }
            if (!File.Exists(path))
            {
                throw new InvalidOperationException($"The file {path} does not exist.");
            }
            try
            {
                this.DoSomeVeryDirtyHacksToMakeItWork();
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = adobeReaderPath
                };
                string str2 = $"/t "{this.pdfFileName}" "{this.printerName}"";
                startInfo.Arguments = str2;
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = false;
                startInfo.UseShellExecute = false;
                if ((this.workingDirectory != null) && (this.workingDirectory.Length != 0))
                {
                    startInfo.WorkingDirectory = this.workingDirectory;
                }
                Process process = Process.Start(startInfo);
                if (!process.WaitForExit(milliseconds))
                {
                    process.Kill();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public static string AdobeReaderPath
        {
            get => 
                adobeReaderPath;
            set
            {
                adobeReaderPath = value;
            }
        }

        public static string DefaultPrinterName
        {
            get => 
                defaultPrinterName;
            set
            {
                defaultPrinterName = value;
            }
        }

        public string PdfFileName
        {
            get => 
                this.pdfFileName;
            set
            {
                this.pdfFileName = value;
            }
        }

        public string PrinterName
        {
            get => 
                this.printerName;
            set
            {
                this.printerName = value;
            }
        }

        public string WorkingDirectory
        {
            get => 
                this.workingDirectory;
            set
            {
                this.workingDirectory = value;
            }
        }
    }
}

