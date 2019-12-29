using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Elgraiv.ObstMiner
{
    [DebuggerDisplay("Port:{Port} (PID={ProcessId})")]
    public class PortItem : IEquatable<PortItem>
    {
        public int Port { get; set; }
        public int ProcessId { get; set; }

        public bool Equals([AllowNull] PortItem other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Port == Port && other.ProcessId == ProcessId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PortItem);
        }

        public override int GetHashCode()
        {
            return Port.GetHashCode() + ProcessId.GetHashCode();
        }
    }
}
