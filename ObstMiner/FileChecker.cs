using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Elgraiv.ObstMiner
{
    public class FileChecker : IDisposable
    {

        private bool _setup = false;
        private int _sesstionHandle;
        public FileChecker()
        {

        }

        public bool Setup(params string[] targetFiles)
        {
            if (_setup)
            {
                return false;
            }
            var key = new StringBuilder(256);
            var result = NativeMethods.RmStartSession(out _sesstionHandle, 0, key);
            if (result != 0)
            {
                return false;
            }

            result = NativeMethods.RmRegisterResources(_sesstionHandle, (uint)targetFiles.Length, targetFiles, 0, Array.Empty<RmUniqueProcess>(), 0, Array.Empty<string>());
            if (result != 0)
            {
                EndSession();
                return false;
            }
            _setup = true;

            return true;
        }

        public IReadOnlyCollection<ProcessInfo> GetLockingProcesses()
        {
            var num = 0u;
            var result = NativeMethods.RmGetList(_sesstionHandle, out var needed, ref num, null, out _);
            if (result == 0)
            {
                return new List<ProcessInfo>();
            }
            if (result != 234)
            {
                return null;
            }
            RmProcessInfo[] processes = null;
            while(needed > num)
            {
                num = needed;
                processes = new RmProcessInfo[needed];
                result= NativeMethods.RmGetList(_sesstionHandle, out needed, ref num, processes, out _);
                if (result != 0 && result!=234)
                {
                    return null;
                }
            }
            var list = new List<ProcessInfo>();

            for(var i = 0; i < num; i++)
            {
                list.Add(
                    new ProcessInfo()
                    {
                        ProcessId = processes[i].RmUniqueProcess.ProcessId,
                        Name = processes[i].AppName.Clone() as string,
                    }
                    );
            }

            return list;
        }

        private void EndSession()
        {
            _ = NativeMethods.RmEndSession(_sesstionHandle);
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                }
                if (_setup)
                {
                    EndSession();
                }

                _disposedValue = true;
            }
        }

        ~FileChecker()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
