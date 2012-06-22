using System;
using System.Runtime.InteropServices;

namespace NetLib.DNS.Records
{
    /// <summary>
    /// Represents a DNS Start Of Authority record (DNS_SOA_DATA)
    /// </summary>
    /// <remarks>
    /// The SOARecord structure is used in conjunction with
    /// the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    /// classes to programmatically manage DNS entries.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct SOARecord
    {
        /// <summary>
        /// Gets or sets the primary server
        /// </summary>
        /// <remarks>
        /// Pointer to a string representing the name of the authoritative
        /// DNS server for the zone to which the record belongs.
        /// </remarks>
        public string PrimaryServer;

        /// <summary>
        /// Gets or sets the name of the administrator
        /// </summary>
        /// <remarks>
        /// Pointer to a string representing the name of the responsible party
        /// for the zone to which the record belongs.
        /// </remarks>
        public string Administrator;

        /// <summary>
        /// Gets or sets the serial number
        /// </summary>
        /// <remarks>
        /// Serial number of the SOA record.
        /// </remarks>
        public uint SerialNo;

        /// <summary>
        /// Gets or sets the refresh
        /// </summary>
        /// <remarks>
        /// Time, in seconds, before the zone containing this record should be
        /// refreshed.
        /// </remarks>
        public uint Refresh;

        /// <summary>
        /// Gets or sets the retry count
        /// </summary>
        /// <remarks>
        /// Time, in seconds, before retrying a failed refresh of the zone to
        /// which this record belongs
        /// </remarks>
        public uint Retry;

        /// <summary>
        /// Gets or sets the expiration
        /// </summary>
        /// <remarks>
        /// Time, in seconds, before an unresponsive zone is no longer authoritative.
        /// </remarks>
        public uint Expire;

        /// <summary>
        /// Gets or sets the default ttl
        /// </summary>
        /// <remarks>
        /// Lower limit on the time, in seconds, that a DNS server or caching
        /// resolver are allowed to cache any RRs from the zone to which this
        /// record belongs.
        /// </remarks>
        public uint DefaultTtl;

        /// <summary>
        /// Returns a string representation of the Start Of Authority record.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The string returned looks like:
        /// <code>
        /// administrator: [ADMIN] TTL: [TTL] primary server: [SERVER] refresh: [REFRESH] retry: [RETRY] serial number: [SERIAL]
        /// where [ADMIN] = string representation of <see cref="Administrator"/>
        /// and   [TTL] = string representation of <see cref="DefaultTtl"/>
        /// and   [SERVER] = string representation of <see cref="PrimaryServer"/>
        /// and   [REFRESH] = string representation of <see cref="Refresh"/>
        /// and   [RETRY] = string representation of <see cref="Retry"/>
        /// and   [SERIAL] = string representation of <see cref="SerialNo"/>
        /// </code>
        /// </remarks>
        public override string ToString()
        {
            return String.Format(
                "administrator: {0} TTL: {1} primary server: {2} refresh: {3} retry: {4} serial number: {5}",
                Administrator,
                DefaultTtl,
                PrimaryServer,
                Refresh,
                Retry,
                SerialNo
                );
        }
    }
}
