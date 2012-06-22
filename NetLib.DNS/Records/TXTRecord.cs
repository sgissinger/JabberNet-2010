using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents a DNS Text record (DNS_TXT_DATA)
    /// </summary>
    /// <remarks>
    /// The TXTRecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct TXTRecord
    {
        /// <summary>
        /// Gets or sets the string count
        /// </summary>
        /// <remarks>
        /// Number of strings represented in pStringArray.
        /// </remarks>
        public uint StringCount;

        /// <summary>
        /// Gets or sets the string array
        /// </summary>
        /// <remarks>
        /// Array of strings representing the descriptive text of the
        /// TXT resource record.
        /// </remarks>
        public string StringArray;

        /// <summary>
        /// Returns a string representation of this record.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// string count: [COUNT] string array: [ARR]
        /// where [COUNT] = string representation of <see cref="StringCount"/>
        /// and   [ARR] = string representation of <see cref="StringArray"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format(
                "string count: {0} string array: {1}",
                StringCount,
                StringArray
                );
        }
    }
}