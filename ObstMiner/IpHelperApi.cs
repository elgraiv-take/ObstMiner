using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Elgraiv.ObstMiner
{
    #region UDP
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibUdpRowOwnerPid
    {
        public int LocalAddr { get; set; }
        public int LocalPort { get; set; }
        public int OwningPid { get; set; }

    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibUdpTableOwnerPid
    {

        public int NumEntries { get; set; }

        [MarshalAs(UnmanagedType.ByValArray)]
        private MibUdpRowOwnerPid[] _table;
        public MibUdpRowOwnerPid[] Table
        {
            get => _table;
            set => _table = value;
        }
    }
    [StructLayout(LayoutKind.Sequential,CharSet =CharSet.Ansi)]
    internal struct MibUdp6RowOwnerPid
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        private string _localAddr;
        public string LocalAddr
        {
            get => _localAddr;
            set => _localAddr = value;
        }
        public int LocalScopeId { get; set; }
        public int LocalPort { get; set; }
        public int OwningPid { get; set; }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibUdp6TableOwnerPid
    {
        public int NumEntries { get; set; }

        public MibUdp6RowOwnerPid[] Table { get; set; }
    }
    #endregion

    #region TCP
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibTcpRowOwnerPid
    {
        public int State { get; set; }
        public int LocalAddr { get; set; }
        public int LocalPort { get; set; }
        public int RemoteAddr { get; set; }
        public int RemotePort { get; set; }
        public int OwningPid { get; set; }

    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibTcpTableOwnerPid
    {

        public int NumEntries { get; set; }

        public MibTcpRowOwnerPid[] Table { get; set; }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct MibTcp6RowOwnerPid
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        private string _localAddr;
        public string LocalAddr
        {
            get => _localAddr;
            set => _localAddr = value;
        }
        public int LocalScopeId { get; set; }
        public int LocalPort { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        private string _remoteAddr;
        public string RemoteAddr
        {
            get => _remoteAddr;
            set => _remoteAddr = value;
        }
        public int RemoteScopeId { get; set; }
        public int RemotePort { get; set; }
        public int State { get; set; }
        public int OwningPid { get; set; }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct MibTcp6TableOwnerPid
    {
        public int NumEntries { get; set; }

        public MibTcp6RowOwnerPid[] Table { get; set; }
    }
    #endregion

    internal enum UdpTableClass
    {
        Basic,
        OwnerPid,
        OwnerModule,
    }
    internal enum TcpTableClass
    {
        TcpTableBasicListener,
        TcpTableBasicConnections,
        TcpTableBasicAll,
        TcpTableOwnerPidListener,
        TcpTableOwnerPidConnections,
        TcpTableOwnerPidAll,
        TcpTableOwnerModuleListener,
        TcpTableOwnerModuleConnections,
        TcpTableOwnerModuleAll,
    }

    internal enum Af
    {
        Inet=2,
        Inet6=23,
    }

    internal static partial class NativeMethods
    {

        private const string Iphlpapi = "Iphlpapi.dll";

        [DllImport(Iphlpapi)]
        public static extern int GetExtendedUdpTable(
            IntPtr pUdpTable,
            out int pdwSize,
            bool bOrder,
            Af ulAf,
            UdpTableClass TableClass,
            uint Reserved
            );
        [DllImport(Iphlpapi)]
        public static extern int GetExtendedTcpTable(
            IntPtr pTcpTable,
            out int pdwSize,
            bool bOrder,
            Af ulAf,
            TcpTableClass TableClass,
            uint Reserved
            );
    }
}
