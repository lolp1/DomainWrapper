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
        private readonly string _dllPath;
        private AppDomain _hostDomain;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PathedDomainHost()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hostDomain != null)
            {
                AppDomain.Unload(_hostDomain);
                _hostDomain = null;
            }
        }

        public void Execute()
        {
            _hostDomain.ExecuteAssembly(_dllPath);
        }

        public enum ClickToMove
        {
            Push = 0x1C,
            X = 0x84,
            Y = X + 0X4,
            Z = +0X8,
            Guid = 0x20,
            Distance = 0xc
        }

  
    }
}