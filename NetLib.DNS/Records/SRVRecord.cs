using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents a DNS Server record. (DNS_SRV_DATA)
    /// </summary>
    /// <remarks>
    /// The SRVRecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct SRVRecord
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        /// <remarks>
        /// Pointer to a string representing the target host.
        /// </remarks>
        public string NameNext;

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        /// <remarks>
        /// Priority of the target host specified in the owner name. Lower numbers imply higher priority.
        /// </remarks>
        public ushort Priority;

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        /// <remarks>
        /// Weight of the target host. Useful when selecting among hosts with the same priority. The chances of using this host should be proportional to its weight.
        /// </remarks>
        public ushort Weight;

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        /// <remarks>
        /// Port used on the terget host for the service.
        /// </remarks>
        public ushort Port;

        /// <summary>
        /// Reserved.
        /// </summary>
        /// <remarks>
        /// Reserved. Used to keep pointers DWORD aligned.
        /// </remarks>
        public ushort Pad;

        /// <summary>
        /// Returns a string representation of this record.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// name next: [SERVER] priority: [PRIOR] weight: [WEIGHT] port: [PORT]
        /// where [SERVER] = string representation of <see cref="NameNext"/>
        /// and   [PRIOR] = string representation of <see cref="Priority"/>
        /// and   [WEIGHT] = string representation of <see cref="Weight"/>
        /// and   [PORT] = string representation of <see cref="Port"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format(
                "name next: {0} priority: {1} weight: {2} port: {3}",
                NameNext,
                Priority,
                Weight,
                Port
                );
        }
    }
}
