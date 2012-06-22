using System;

namespace NetLib.DNS
{
    /// <summary>
    /// DNS query types
    /// </summary>
    /// <remarks>
    /// This enum is used by the DnsQuery API call to describe the
    /// options to be given to a DNS server along with a query.
    /// </remarks>
    [Flags]
    enum DnsQueryType : uint
    {
        /// <summary>
        /// Standard
        /// </summary>
        STANDARD = 0x00000000,

        /// <summary>
        /// Accept truncated response
        /// </summary>
        ACCEPT_TRUNCATED_RESPONSE = 0x00000001,

        /// <summary>
        /// Use TCP only
        /// </summary>
        USE_TCP_ONLY = 0x00000002,

        /// <summary>
        /// No recursion
        /// </summary>
        NO_RECURSION = 0x00000004,

        /// <summary>
        /// Bypass cache
        /// </summary>
        BYPASS_CACHE = 0x00000008,

        /// <summary>
        /// Cache only
        /// </summary>
        NO_WIRE_QUERY = 0x00000010,

        /// <summary>
        /// Directs DNS to ignore the local name.
        /// </summary>
        NO_LOCAL_NAME = 0x00000020,

        /// <summary>
        /// Prevents the DNS query from consulting the HOSTS file.
        /// </summary>
        NO_HOSTS_FILE = 0x00000040,

        /// <summary>
        /// Prevents the DNS query from using NetBT for resolution.
        /// </summary>
        NO_NETBT = 0x00000080,

        /// <summary>
        /// Directs DNS to perform a query using the network only,
        /// bypassing local information.
        /// </summary>
        WIRE_ONLY = 0x00000100,

        /// <summary>
        /// Treat as FQDN
        /// </summary>
        TREAT_AS_FQDN = 0x00001000,

        /// <summary>
        /// Allow empty auth response
        /// </summary>
        [Obsolete]
        ALLOW_EMPTY_AUTH_RESP = 0x00010000,

        /// <summary>
        /// Don't reset TTL values
        /// </summary>
        DONT_RESET_TTL_VALUES = 0x00100000,

        /// <summary>
        /// Reserved.
        /// </summary>
        RESERVED = 0xff000000,

        /// <summary>
        /// obsolete.
        /// </summary>
        [Obsolete("use NO_WIRE_QUERY instead")]
        CACHE_ONLY = NO_WIRE_QUERY,

        /// <summary>
        /// Directs DNS to return the entire DNS response message.
        /// </summary>
        RETURN_MESSAGE = 0x00000200
    }
}