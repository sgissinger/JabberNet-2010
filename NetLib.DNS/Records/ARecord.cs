using System;
using System.Net;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents a DNS Address record (DNS_A_DATA)
    /// </summary>
    /// <remarks>
    /// The ARecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct ARecord
    {
        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <remarks>
        /// IPv4 address, in the form of an uint datatype.
        /// <see cref="System.Net.IPAddress"/> could be
        /// used to fill this property.
        /// </remarks>
        public uint Address;

        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public IPAddress IPAddress
        {
            get { return new IPAddress(this.Address); }
        }

        /// <summary>
        /// Returns a string representation of the A Record
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// IP Address: [ADDRESS]
        /// where [ADDRESS] = <see cref="System.Net.IPAddress.ToString()"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format("IP Address: {0}", this.IPAddress);
        }
    }
}