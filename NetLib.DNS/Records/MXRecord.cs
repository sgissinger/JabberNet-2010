using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents a DNS Mail Exchange record (DNS_MX_DATA).
    /// </summary>
    /// <remarks>
    /// The MXRecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MXRecord
    {
        /// <summary>
        /// Gets or sets the exchange's host name
        /// </summary>
        /// <remarks>
        /// Pointer to a string representing the fully qualified domain name
        /// (FQDN) of the host willing to act as a mail exchange.
        /// </remarks>
        public string Exchange;

        /// <summary>
        /// Gets or sets the preference of the exchange.
        /// </summary>
        /// <remarks>
        /// Preference given to this resource record among others at the same
        /// owner. Lower values are preferred.
        /// </remarks>
        public ushort Preference;

        /// <summary>
        /// Reserved.
        /// </summary>
        /// <remarks>
        /// Reserved. Used to keep pointers DWORD aligned.
        /// </remarks>
        public ushort Pad; // to keep dword aligned

        /// <summary>
        /// Returns a string representation of this mail exchange.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// exchange (preference): [EXCH] ([PREF])
        /// where [EXCH] = string representation of <see cref="Exchange"/>
        /// and   [PREF] = hexadecimal representation of <see cref="Preference"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format(
                "exchange (preference): {0} ({1})",
                Exchange,
                Preference
                );
        }
    }
}
