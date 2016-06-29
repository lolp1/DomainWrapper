using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace DomainWrapper
{
    public static class PatchedDomainLoader
    {
        internal static string ApplicationToHostName;

        [MarshalAs(UnmanagedType.LPWStr)] internal static string ApplicationToHostDirectory;

        [STAThread]
        public static void HostDomain()
        {
#if LAUNCH_MDA
            System.Diagnostics.Debugger.Launch();
#endif
            var directory = Path.GetDirectoryName(ApplicationToHostDirectory);
            // ReSharper disable once AssignNullToNotNullAttribute
            var exists = Directory.Exists(directory);
            Trace.Assert(exists);
            try
            {
                using (var host = new PathedDomainHost(ApplicationToHostDirectory))
                {
                    host.Execute();
                }
            }
            catch (Exception e)
            {
               Trace.WriteLine(e.ToString());
            }
        }

        [DllExport("LoadDomainSettings", CallingConvention.StdCall)]
        public static void LoadDomainHostSettings([MarshalAs(UnmanagedType.LPWStr)] string applicationPath)
        {
            ApplicationToHostDirectory = applicationPath;
            ApplicationToHostName = Path.GetFileNameWithoutExtension(applicationPath);
            HostDomain();
        }
    }
}