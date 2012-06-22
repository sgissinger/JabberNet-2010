using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS
{
    /// <summary>
    /// Represents a complete DNS record (DNS_RECORD)
    /// </summary>
    /// <remarks>
    /// This structure is used to hold a complete DNS record
    /// as returned from the DnsQuery API.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    struct DnsRecord
    {
        /// <summary>
        /// Gets or sets the next record.
        /// </summary>
        public IntPtr Next;// 4 bytes

        /// <summary>
        /// Gets or sets the name of the record.
        /// </summary>
        public string Name;// 4 bytes

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U2)]
        public DnsRecordType RecordType;// 2 bytes

        /// <summary>
        /// Gets or sets the data length.
        /// </summary>
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U2)]
        public ushort DataLength;// 2 bytes

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public DnsRecordFlags Flags;

        /// <summary>
        /// Gets or sets the TTL count
        /// </summary>
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public uint Ttl;// 4 bytes

        /// <summary>
        /// Reserved.
        /// </summary>
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public uint Reserved;// 4 bytes
        // Can't fill in rest of the structure because if it doesn't line up, c# will complain.

        /// <summary>
        /// Represents the flags of a <see cref="DnsRecord"/>.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]// 4 bytes
        internal struct DnsRecordFlags
        {
            /// <summary>
            /// Reserved.
            /// </summary>
            [FieldOffset(0)]
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint DW;

            /// <summary>
            /// Reserved.
            /// </summary>
            [FieldOffset(0)]
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public uint S;
        }
    }
}
