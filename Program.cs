using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PCRepairKit.Utils.Logger;

namespace PCRepairKit
{
    public class Program
    {
        private static readonly Dictionary<string, Assembly> AssembliesDictionary = new Dictionary<string, Assembly>();
        static System.Threading.Mutex AppMutex = new System.Threading.Mutex(true, "{8F6F0AC5-B8A8-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        public static void Main(string[] args)
        {
            if (AppMutex.WaitOne(TimeSpan.Zero, true))
            {
#if DEBUG
                Debug.Listeners.Add(new TextWriterTraceListener("log.txt"));
                Debug.Listeners.Add(new ConsoleTraceListener());
                Debug.AutoFlush = true;
#endif
                ExtractNativeLibraries();

                App.Arguments = args;
                AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainOnAssemblyResolve;
                try
                {
                    App.Main();
                }
                catch (Exception tmpE)
                {
                    Log.Error(tmpE.Message);
                    Log.Error(tmpE.StackTrace);

                    Debug.WriteLine(tmpE.Message);
                    Debug.WriteLine(tmpE.StackTrace);
                }
            }
            else
            {
                var tmpCurrentProcessName = Process.GetCurrentProcess().ProcessName;
                tmpCurrentProcessName = Regex
                    .Replace(tmpCurrentProcessName, @"[\d-]", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace(",", string.Empty)
                    .Replace("_", string.Empty)
                    .Trim();

                var tmpProcessNames = Process.GetProcesses()
                    .Select(x => x.ProcessName)
                    .Where(x => x.Contains(tmpCurrentProcessName));
                foreach (var tmpProcessName in tmpProcessNames)
                {
                    foreach (var tmpProcess in Process.GetProcessesByName(tmpProcessName))
                    {
                        var tmpMainWindowHandle = tmpProcess.MainWindowHandle;
                        if (tmpMainWindowHandle == IntPtr.Zero)
                        {
                            tmpMainWindowHandle = GetMainWindowHandle(tmpProcess.Id);
                        }

                        try
                        {
                            ShowWindow(tmpMainWindowHandle, 1);
                            SetForegroundWindow(tmpMainWindowHandle);                                                       
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
            }
        }

        private static void ExtractNativeLibraries()
        {
            var tmpNativeLibraiesNamesList = new[]
            {
                $"{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}.sqlite3",
                //Helper.Localizer приходится извлекать, так как при создании локалайзера он берет путь относительно этой библиотеки, если она в ресурсах, то крэшится
                "Helper.Localizer"
            };

            var tmpExecutingAssembly = Assembly.GetExecutingAssembly();

            foreach (var tmpNativeLibraryName in tmpNativeLibraiesNamesList)
            {
                var tmpResource = tmpExecutingAssembly.GetManifestResourceNames().
                    FirstOrDefault(aResource => aResource.Contains(tmpNativeLibraryName));

                if (string.IsNullOrEmpty(tmpResource))
                {
                    throw new Exception($"Unable to find resource {tmpNativeLibraryName}");
                }

                var tmpFilePath = @$"{Environment.CurrentDirectory}\{tmpNativeLibraryName.Replace("x86.", "").Replace("x64.", "")}.dll";
                using Stream tmpStream = tmpExecutingAssembly.GetManifestResourceStream(tmpResource);
                if (!File.Exists(tmpFilePath))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(tmpFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(tmpFilePath));
                    }

                    File.WriteAllBytes(tmpFilePath, ReadAllBytes(tmpStream));

                }

                byte[] ReadAllBytes(Stream aStream)
                {
                    var tmpBuffer = new byte[16 * 1024];
                    using var tmpMs = new MemoryStream();
                    int tmpRead;
                    while ((tmpRead = aStream.Read(tmpBuffer, 0, tmpBuffer.Length)) > 0)
                        tmpMs.Write(tmpBuffer, 0, tmpRead);
                    return tmpMs.ToArray();
                }
            }

        }

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextLengthW")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
      
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetWindowTextW")]        
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        
        public delegate bool EventEnumWindows(IntPtr aWindow);

        public static string GetWindowText(IntPtr aHwnd)
        {
            int tmpLength = GetWindowTextLength(aHwnd);
            if (tmpLength < 1)
                return string.Empty;
            StringBuilder tmpBuffer = new StringBuilder(tmpLength + 1);
            return (GetWindowText(aHwnd, tmpBuffer, tmpBuffer.Capacity) > 0)
                ? tmpBuffer.ToString() : string.Empty;
        }
        public static void EnumWindows(int aProcessId, EventEnumWindows aProc)
        {
            if (aProc == null)
                return;
            var tmpCallback = new EnumWindowsProc(
                (hWnd, lParam) =>
                {
                    // Процесс окна
                    GetWindowThreadProcessId(hWnd, out var tmpProcessId);

                    // Отсеять чужие процессы
                    if ((int)tmpProcessId != aProcessId)
                        return true;

                    // Обработка события
                    return aProc(hWnd);
                });
            EnumWindows(tmpCallback, IntPtr.Zero);
        }
        public static IntPtr GetMainWindowHandle(int aProcessId)
        {
            IntPtr tmpHandle = IntPtr.Zero;
            EnumWindows(aProcessId, (aWindowHandle) =>
            {
                var tmpTitle = GetWindowText(aWindowHandle);
                if (string.IsNullOrEmpty(tmpTitle)
                    || tmpTitle.Contains("Default")
                    || tmpTitle.Contains("Window")                   
                    || tmpTitle.Contains("IME")
                    || tmpTitle.Contains("GDI")
                    || tmpTitle.Contains("UI")) return true;
                tmpHandle = aWindowHandle;
                return true;
            });
            return tmpHandle;
        }
        private static Assembly OnCurrentDomainOnAssemblyResolve(object o, ResolveEventArgs args)
        {
            Log.Info(args.Name);
            var assemblyName = new AssemblyName(args.Name);
            if (Equals(assemblyName.Name, "PCRepairKit.resources"))
                return null;
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var path = $"{assemblyName.Name}.dll";

                if (assemblyName.Name == "Helper.GA")
                    path = "Helper.GA_.dll";

                if (assemblyName.CultureInfo != null)
                {
                    if (!assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
                        path = $@"{assemblyName.CultureInfo}\{$"{assemblyName.Name}.resources.dll"}";
                }

                if (!AssembliesDictionary.ContainsKey(path))
                    using (var assemblyStream = executingAssembly.GetManifestResourceStream(path))
                        if (assemblyStream != null)
                        {
                            var assemblyRawBytes = new byte[assemblyStream.Length];
                            assemblyStream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                            using (var pdbStream = executingAssembly.GetManifestResourceStream(Path.ChangeExtension(path, "pdb")))
                                if (pdbStream != null)
                                {
                                    var pdbData = new byte[pdbStream.Length];
                                    pdbStream.Read(pdbData, 0, pdbData.Length);
                                    var assembly = Assembly.Load(assemblyRawBytes, pdbData);
                                    AssembliesDictionary.Add(path, assembly);
                                    return assembly;
                                }

                            AssembliesDictionary.Add(path, Assembly.Load(assemblyRawBytes));
                        }
                        else
                            AssembliesDictionary.Add(path, null);

                return AssembliesDictionary[path];
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.StackTrace);

                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                throw new Exception($"Fail to resolve assembly {assemblyName}", e);
            }
        }
    }
}