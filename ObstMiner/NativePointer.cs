using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Elgraiv.ObstMiner
{
    internal class NativePointer:IDisposable
    {
        public IntPtr Pointer { get; }

        public NativePointer(int size)
        {
            Pointer= Marshal.AllocHGlobal(size);
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
                Marshal.FreeHGlobal(Pointer);
                _disposedValue = true;
            }
        }

        ~NativePointer()
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
