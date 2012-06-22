
namespace NetLib.DNS
{
    /// <summary>
    /// The possible return codes of the DNS API call. This enum can
    /// be used to decypher the <see cref="DnsException.ErrorCode"/>
    /// property's return value.
    /// </summary>
    /// <remarks>
    /// This enum is used to describe a failed return code by the
    /// DnsQuery API used in the <see cref="DnsRequest"/> class.
    /// </remarks>
    public enum DnsQueryReturnCode : ulong
    {
        /// <summary>
        /// Successful query
        /// </summary>
        SUCCESS = 0L,

        /// <summary>
        /// Base DNS error code
        /// </summary>
        UNSPECIFIED_ERROR = 9000,

        /// <summary>
        /// Base DNS error code
        /// </summary>
        MASK = 0x00002328, // 9000 or RESPONSE_CODES_BASE

        /// <summary>
        /// DNS server unable to interpret format.
        /// </summary>
        FORMAT_ERROR = 9001L,

        /// <summary>
        /// DNS server failure.
        /// </summary>
        SERVER_FAILURE = 9002L,

        /// <summary>
        /// DNS name does not exist.
        /// </summary>
        NAME_ERROR = 9003L,

        /// <summary>
        /// DNS request not supported by name server.
        /// </summary>
        NOT_IMPLEMENTED = 9004L,

        /// <summary>
        /// DNS operation refused.
        /// </summary>
        REFUSED = 9005L,

        /// <summary>
        /// DNS name that ought not exist, does exist.
        /// </summary>
        YXDOMAIN = 9006L,

        /// <summary>
        /// DNS RR set that ought not exist, does exist.
        /// </summary>
        YXRRSET = 9007L,

        /// <summary>
        /// DNS RR set that ought to exist, does not exist.
        /// </summary>
        NXRRSET = 9008L,

        /// <summary>
        /// DNS server not authoritative for zone.
        /// </summary>
        NOTAUTH = 9009L,

        /// <summary>
        /// DNS name in update or prereq is not in zone.
        /// </summary>
        NOTZONE = 9010L,

        /// <summary>
        /// DNS signature failed to verify.
        /// </summary>
        BADSIG = 9016L,

        /// <summary>
        /// DNS bad key.
        /// </summary>
        BADKEY = 9017L,

        /// <summary>
        /// DNS signature validity expired.
        /// </summary>
        BADTIME = 9018L,

        /// <summary>
        /// Packet format
        /// </summary>
        PACKET_FMT_BASE = 9500,

        /// <summary>
        /// No records found for given DNS query.
        /// </summary>
        NO_RECORDS = 9501L,

        /// <summary>
        /// Bad DNS packet.
        /// </summary>
        BAD_PACKET = 9502L,

        /// <summary>
        /// No DNS packet.
        /// </summary>
        NO_PACKET = 9503L,

        /// <summary>
        /// DNS error, check rcode.
        /// </summary>
        RCODE = 9504L,

        /// <summary>
        /// Unsecured DNS packet.
        /// </summary>
        UNSECURE_PACKET = 9505L
    }
}
