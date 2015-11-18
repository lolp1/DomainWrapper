using System;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace DomainWrapper
{
    /// <summary>
    ///     The class representing the object to host.
    /// </summary>
    public class PathedDomainHost : CriticalFinalizerObject, IDisposable
    {
        #region Fields, Private Properties
        private readonly string _dllPath;
        private AppDomain _hostDomain;
        #endregion

        #region Constructors, Destructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="PathedDomainHost"/> class.
        /// </summary>
        /// <param name="name">The name of the application to be hosted.</param>
        /// <param name="path">The path of the application being posted.</param>
        public PathedDomainHost(string name, string path)
        {
            var setupInfo = new AppDomainSetup
                            {
                                ApplicationBase = path,
                                PrivateBinPath = path
                            };

            _dllPath = Path.Combine(path, Path.ChangeExtension(name, "exe"));
            _hostDomain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, setupInfo);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~PathedDomainHost()
        {
            Dispose(false);
        }
        #endregion

        #region Interface Implementations
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_hostDomain == null) return;
            AppDomain.Unload(_hostDomain);
            _hostDomain = null;
        }
        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            _hostDomain.ExecuteAssembly(_dllPath);
        }
    }
}