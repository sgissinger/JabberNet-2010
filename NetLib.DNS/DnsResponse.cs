using System.Collections;
using NetLib.DNS.Records;

namespace NetLib.DNS
{
    /// <summary>
    /// Represents one DNS response. This class cannot be directly created -
    /// it is returned by the <see cref="DnsRequest.GetResponse"/> method.
    /// </summary>
    /// <remarks>
    /// The DnsResponse class represents the information returned by a DNS
    /// server in response to a <see cref="DnsRequest"/>. The DnsResponse
    /// class offers easy access to all of the returned DNS records for a given
    /// domain.
    /// </remarks>
    public class DnsResponse
    {
        private readonly DnsWrapperCollection rawrecords;

        internal DnsResponse()
        {
            rawrecords = new DnsWrapperCollection();
        }

        /// <summary>
        /// Gets a <see cref="DnsWrapperCollection" /> containing
        /// all of the DNS information that the server returned about
        /// the queried domain.
        /// </summary>
        /// <remarks>
        /// Returns all of the DNS records retrieved about the domain
        /// as a <see cref="DnsWrapperCollection"/>. This property
        /// is wrapped by the <see cref="GetRecords"/> method.
        /// </remarks>
        /// <value>Gets a collection of <see cref="DnsWrapper"/> objects.</value>
        public DnsWrapperCollection RawRecords
        {
            get
            {
                return rawrecords;
            }
        }

        /// <summary>
        /// Returns a collection of DNS records of a specified
        /// <see cref="DnsRecordType"/>. The collection's data type
        /// is determined by the type of record being sought in the
        /// type argument.
        /// </summary>
        /// <param name="type">A <see cref="DnsRecordType"/> enumeration
        /// value indicating the type of DNS record to get from the list of
        /// all DNS records (available in the <see cref="RawRecords"/>
        /// property.</param>
        /// <returns>an <see cref="ArrayList"/> of one of the types
        /// specified in the <see cref="NetLib.DNS.Records"/> namespace based
        /// on the <see cref="DnsRecordType"/> argument representing the
        /// type of DNS record desired.
        /// </returns>
        /// <remarks>
        /// It is recommended that you loop through the results of this
        /// method as follows for maximum convenience:
        /// <code>
        /// foreach (<see cref="NetLib.DNS.Records"/> record in obj.GetRecords(<see cref="DnsRecordType"/>))
        /// {
        ///     string s = record.ToString();
        /// }
        /// </code>
        /// The following table indicates the DNS record type you can expect to get
        /// back based on the <see cref="DnsRecordType"/> requested. Any items returning
        /// null are not currently supported.
        /// <list type="table">
        ///     <listheader>
        ///         <term>DnsRecordType enumeration value</term>
        ///         <term>GetRecords() returns</term>
        ///     </listheader>
        ///     <item>
        ///         <term>A</term>
        ///         <description><see cref="NetLib.DNS.Records.ARecord"/></description>
        ///     </item>
        ///     <item>
        ///         <term>CNAME</term>
        ///         <description><see cref="NetLib.DNS.Records.PTRRecord"/></description>
        ///     </item>
        ///     <item>
        ///         <term>PTR</term>
        ///         <description><see cref="NetLib.DNS.Records.PTRRecord"/></description>
        ///     </item>
        ///     <item>
        ///         <term>MX</term>
        ///         <description><see cref="NetLib.DNS.Records.MXRecord"/></description>
        ///     </item>
        ///     <item>
        ///         <term>SRV</term>
        ///         <description><see cref="NetLib.DNS.Records.SRVRecord"/></description>
        ///     </item>
        ///     <item>
        ///         <term>TEXT</term>
        ///         <description><see cref="NetLib.DNS.Records.TXTRecord"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public ArrayList GetRecords(DnsRecordType type)
        {
            ArrayList arr = new ArrayList();
            foreach (DnsWrapper dnsentry in rawrecords)
                if (dnsentry.Equals(type))
                    arr.Add(dnsentry.RecordData);

            return arr;
        }

        /// <summary>
        /// Gets all the <see cref="SRVRecord"/> for the queried domain.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="GetRecords"/> method to retrieve an
        /// array of <see cref="SRVRecord"/>s representing all the Address
        /// records for the domain.
        /// </remarks>
        /// <value>An array of <see cref="SRVRecord"/> objects.</value>
        public SRVRecord[] SRVRecords
        {
            get
            {
                ArrayList arr = GetRecords(DnsRecordType.SRV);
                return (SRVRecord[])arr.ToArray(typeof(SRVRecord));
            }
        }

        /// <summary>
        /// Gets all the <see cref="TXTRecord"/> for the queried domain.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="GetRecords"/> method to retrieve an
        /// array of <see cref="TXTRecord"/>s representing all the Address
        /// records for the domain.
        /// </remarks>
        /// <value>An array of <see cref="SRVRecord"/> objects.</value>
        public TXTRecord[] TXTRecords
        {
            get
            {
                ArrayList arr = GetRecords(DnsRecordType.TEXT);
                return (TXTRecord[])arr.ToArray(typeof(TXTRecord));
            }
        }
        /// <summary>
        /// Gets all the <see cref="MXRecord"/> for the queried domain.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="GetRecords"/> method to retrieve an
        /// array of <see cref="MXRecord"/>s representing all the Mail Exchanger
        /// records for the domain.
        /// </remarks>
        /// <value>An array of <see cref="MXRecord"/> objects.</value>
        public MXRecord[] MXRecords
        {
            get
            {
                ArrayList arr = GetRecords(DnsRecordType.MX);
                return (MXRecord[])arr.ToArray(typeof(MXRecord));
            }
        }
    }
}
