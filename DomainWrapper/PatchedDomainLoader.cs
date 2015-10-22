using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace DomainWrapper
{
    /// <summary>
    ///     The exported methods to call to host our domain.
    /// </summary>
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public static class PatchedDomainLoader
    {
        internal static string ApplicationToHostName = "WhiteVex";

        [MarshalAs(UnmanagedType.LPWStr)] internal static string ApplicationToHostDirectory = @"C:\Users\Jacob Kemple\Documents\Visual Studio 2015\Projects\WhiteVex\WhiteVex\bin\Debug\";


        [DllExport]
        [STAThread]
        public static void HostDomain()
        {
#if LAUNCH_MDA
            System.Diagnostics.Debugger.Launch();
#endif
            ApplicationToHostDirectory = Path.GetDirectoryName(ApplicationToHostDirectory);
            Trace.Assert(Directory.Exists(ApplicationToHostDirectory));
            Trace.Listeners.Add(
                new TextWriterTraceListener(Path.Combine(ApplicationToHostDirectory, @"Logs\",
                    ApplicationToHostName + ".PatchedDomainLoader.log")));

            try
            {
                using (var host = new PathedDomainHost(ApplicationToHostName, ApplicationToHostDirectory))
                {
                    host.Execute();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        [DllExport("LoadDomainSettings", CallingConvention.Cdecl)]
        public static void LoadDomainHostSettings(string loadDirectory, string applicationName)
        {
            ApplicationToHostDirectory = loadDirectory;
            ApplicationToHostName = applicationName;
        }
    }
}