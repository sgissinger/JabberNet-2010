using System;

namespace NetLib.DNS
{
    /// <summary>
    /// DNS record types
    /// </summary>
    /// <remarks>
    /// This enum represents all possible DNS record types that
    /// could be returned by the DnsQuery API.
    /// </remarks>
    [Flags]
    public enum DnsRecordType : ushort
    {
        /// <summary>
        /// Address record
        /// </summary>
        A = 0x0001,      //  1

        /// <summary>
        /// Canonical Name record
        /// </summary>
        CNAME = 0x0005,      //  5

        /// <summary>
        /// Start Of Authority record
        /// </summary>
        SOA = 0x0006,      //  6

        /// <summary>
        /// Pointer record
        /// </summary>
        PTR = 0x000c,      //  12

        /// <summary>
        /// Mail Exchange record
        /// </summary>
        MX = 0x000f,      //  15

        /// <summary>
        /// Text record
        /// </summary>
        TEXT = 0x0010,      //  16

        //  RFC 2052    (Service location)
        /// <summary>
        /// Server record
        /// </summary>
        SRV = 0x0021,      //  33

        /// <summary>
        /// All records
        /// </summary>
        ALL = 0x00ff,      //  255

        /// <summary>
        /// Any records
        /// </summary>
        ANY = 0x00ff,      //  255
    }
}
