using System;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace DomainWrapper
{
    public class PathedDomainHost : CriticalFinalizerObject, IDisposable
    {
        private readonly string _dllPath;
        private AppDomain _hostDomain;

        public PathedDomainHost(string path)
        {
            var setupInfo = new AppDomainSetup
            {
                ApplicationBase = path,
                PrivateBinPath = path
            };

            _dllPath = path;
            var name = Path.GetFileNameWithoutExtension(path);

            if (name == null)
            {
                throw new ArgumentNullException(nameof(path), "The path can not be null");
            }

            _hostDomain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, setupInfo);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hostDomain == null)
            {
                return;
            }

            AppDomain.Unload(_hostDomain);
            _hostDomain = null;
        }

        public void Execute()
        {
            _hostDomain.ExecuteAssembly(_dllPath);
        }

        ~PathedDomainHost()

        {
            Dispose(false);
        }
    }
}