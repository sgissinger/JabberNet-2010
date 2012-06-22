using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents the DNS pointer record (DNS_PTR_DATA)
    /// </summary>
    /// <remarks>
    /// The PTRRecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct PTRRecord
    {
        /// <summary>
        /// Gets or sets the hostname of the record.
        /// </summary>
        /// <remarks>
        /// Pointer to a string representing the pointer (PTR) record data.
        /// </remarks>
        public string HostName;

        /// <summary>
        /// Returns a string representation of the pointer record.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// Hostname: [HOST]
        /// where [HOST] = string representation of <see cref="HostName"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format("Hostname: {0}", HostName);
        }
    }
}
