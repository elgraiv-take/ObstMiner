using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Elgraiv.ObstMiner
{
    public class PortChecker
    {
        private static readonly int s_intSize = Marshal.SizeOf<int>();

        private List<PortItem> _portTable;

        public IReadOnlyCollection<PortItem> UsedPorts => _portTable;
        public PortChecker()
        {

        }

        private delegate int NativeApiCall(IntPtr ptr, out int size);
        public bool UpdateTable()
        {

            var newList = new List<PortItem>();
            {
                var result = CreateTable<MibUdpRowOwnerPid>(
                    (IntPtr ptr, out int size) => NativeMethods.GetExtendedUdpTable(ptr, out size, false, Af.Inet, UdpTableClass.OwnerPid, 0),
                    (item) => new PortItem() { Port = item.LocalPort, ProcessId = item.OwningPid }
                    );
                if (result == null)
                {
                    return false;
                }
                newList.AddRange(result);
            }
            {
                var result = CreateTable<MibTcpRowOwnerPid>(
                    (IntPtr ptr, out int size) => NativeMethods.GetExtendedTcpTable(ptr, out size, false, Af.Inet, TcpTableClass.TcpTableOwnerPidAll, 0),
                    (item) => new PortItem() { Port = item.LocalPort, ProcessId = item.OwningPid }
                    );
                if (result == null)
                {
                    return false;
                }
                newList.AddRange(result);
            }
            {
                var result = CreateTable<MibUdp6RowOwnerPid>(
                    (IntPtr ptr, out int size) => NativeMethods.GetExtendedUdpTable(ptr, out size, false, Af.Inet6, UdpTableClass.OwnerPid, 0),
                    (item) => new PortItem() { Port = item.LocalPort, ProcessId = item.OwningPid }
                    );
                if (result == null)
                {
                    return false;
                }
                newList.AddRange(result);
            }
            {
                var result = CreateTable<MibTcp6RowOwnerPid>(
                    (IntPtr ptr, out int size) => NativeMethods.GetExtendedTcpTable(ptr, out size, false, Af.Inet6, TcpTableClass.TcpTableOwnerPidAll, 0),
                    (item) => new PortItem() { Port = item.LocalPort, ProcessId = item.OwningPid }
                    );
                if (result == null)
                {
                    return false;
                }
                newList.AddRange(result);
            }

            _portTable = newList.Distinct().OrderBy(item => item.Port).ToList();

            return true;
        }

        private PortItem[] CreateTable<T>(NativeApiCall nativeApi,Func<T,PortItem> converter)
        {
            var result = nativeApi(IntPtr.Zero,out var size);
            if (result != 122)
            {
                return null;
            }
            while (true)
            {
                using (var pointer = new NativePointer(size))
                {
                    result = nativeApi(pointer.Pointer, out size);

                    if (result == 122)
                    {
                        continue;
                    }
                    if (result != 0)
                    {
                        return null;
                    }
                    var count = Marshal.ReadInt32(pointer.Pointer);
                    var current = IntPtr.Add(pointer.Pointer, s_intSize);
                    var table = MarshalTable<T>(current, count);
                    return table.Select(converter).ToArray();
                }
            }
        }

        private T[] MarshalTable<T>(IntPtr head,int count)
        {
            var table = new T[count];
            var current = head;
            var offset = Marshal.SizeOf<T>();
            for (var i = 0; i < count; i++)
            {
                table[i] = Marshal.PtrToStructure<T>(current);
                current = IntPtr.Add(current, offset);
            }
            return table;
        }

        public IReadOnlyCollection<ProcessInfo> UsingProcesses(params int[] ports)
        {
            var usingPorts = _portTable.Where(item => ports.Contains(item.Port)).ToList();
            var processes = new List<ProcessInfo>();
            foreach(var port in usingPorts)
            {
                try
                {
                    var name=Process.GetProcessById(port.ProcessId).ProcessName;
                    processes.Add(new ProcessInfo() { Name = name, ProcessId = port.ProcessId });
                }
                catch (ArgumentException)
                {
                    continue;
                }
                
            }
            return processes;
        }
    }
}
