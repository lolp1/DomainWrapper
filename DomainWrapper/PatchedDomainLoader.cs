using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace DomainWrapper
{
    /// <summary>
    ///     The exported methods to call our methods to host a proxy domain in order to run injected .net code inside of a
    ///     process smoothly.
    /// </summary>
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public static class PatchedDomainLoader
    {
        #region Fields, Private Properties
        /// <summary>
        ///     The application to be hosteds name.
        /// </summary>
        internal static string ApplicationToHostName = "ExampleApplicationName";

        /// <summary>
        ///     The application path to be hosted. Must contain the ending '\' slash.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)] internal static string ApplicationToHostDirectory =
            @"C:\Users\User Name\Documents\Visual Studio 2015\Projects\ProjectName\ProjectFolder\bin\RelaseOrDebug\";
        #endregion

        /// <summary>
        ///     Hosts the given domain.
        /// </summary>
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

        /// <summary>
        ///     Loads the domain host settings. Call this with the desired params before calling <see cref="HostDomain()" />.
        /// </summary>
        /// <param name="loadDirectory">The directory the domain is contained in,</param>
        /// <param name="applicationName">Name of the application.</param>
        [DllExport("LoadDomainSettings", CallingConvention.Cdecl)]
        public static void LoadDomainHostSettings(string loadDirectory, string applicationName)
        {
            ApplicationToHostDirectory = loadDirectory;
            ApplicationToHostName = applicationName;
        }
    }
}