using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Elgraiv.ObstMiner
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileTime
    {
        public int LowDateTime { get; set; }
        public int HighDateTime { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RmUniqueProcess
    {
        public int ProcessId { get; set; }
        public FileTime ProcessStartTime { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RmProcessInfo
    {
        public RmUniqueProcess RmUniqueProcess { get; set; }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        private string _appName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        private string _serviceShortName;

        public RmAppType ApplicationType { get; set; }
        public uint AppStatus { get; set; }
        public int TsSessionId { get; set; }

        public bool Restartable { get; set; }
        public string AppName
        {
            get => _appName;
            set => _appName = value;
        }
        public string ServiceShortName
        {
            get => _serviceShortName;
            set => _serviceShortName = value;
        }
    }

    internal enum RmAppType
    {
        RmUnknownApp,
        RmMainWindow,
        RmOtherWindow,
        RmService,
        RmExplorer,
        RmConsole,
        RmCritical = 1000,
    }

    internal static partial class NativeMethods
    {

        private const string Rstrtmgr = "Rstrtmgr.dll";
        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode)]
        public static extern int RmStartSession(out int pSessionHandle, int dwSessionFlags,[Out] StringBuilder strSessionKey);

        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode)]
        public static extern int RmRegisterResources(
            int dwSessionHandle,
            uint nFiles,
            [In] string[] rgsFileNames,
            uint nApplications,
            [In] RmUniqueProcess[] rgApplications,
            uint nServices,
            [In] string[] rgsServiceNames
            );

        [DllImport(Rstrtmgr, CharSet = CharSet.Unicode)]
        public static extern int RmGetList(
            int dwSessionHandle,
            out uint pnProcInfoNeeded,
            ref uint pnProcInfo,
            [Out] RmProcessInfo[] rgAffectedApps,
            out int lpdwRebootReasons
            );

        [DllImport(Rstrtmgr)]
        public static extern int RmEndSession(int dwSessionHandle);


    }
}
