using System;

namespace NetLib.DNS
{
    /// <summary>
    /// Represents a container for a DNS record of any type
    /// </summary>
    /// <remarks>
    /// The <see cref="DnsWrapper.RecordType"/> property's value
    /// helps determine what type real type of the
    /// <see cref="DnsWrapper.RecordData"/> property returns as
    /// noted in this chart:
    /// <list type="table">
    ///     <listheader>
    ///         <term>RecordType</term>
    ///         <term>RecordData</term>
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
    ///         <term>SOA</term>
    ///         <description><see cref="NetLib.DNS.Records.SOARecord"/></description>
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
    public struct DnsWrapper : IComparable
    {
        /// <summary>
        /// Gets or sets the type of DNS record contained in the
        /// <see cref="RecordData"/> property.
        /// </summary>
        /// <remarks>
        /// This property indicates the type of DNS record
        /// that the <see cref="RecordData"/> property is
        /// holding.
        /// </remarks>
        public DnsRecordType RecordType;

        /// <summary>
        /// Gets or sets the DNS record object as denoted in the
        /// <see cref="RecordType"/> field.
        /// </summary>
        /// <remarks>
        /// This property holds the actual DNS record.
        /// </remarks>
        public object RecordData;

        /// <summary>
        /// Determines whether or not this <see cref="DnsWrapper"/>
        /// instance is equal to a specific <see cref="DnsRecordType"/>
        /// by comparing the <see cref="RecordType"/> property of the
        /// current <see cref="DnsWrapper"/> against the
        /// <see cref="DnsRecordType"/> argument.
        /// </summary>
        /// <param name="type">The <see cref="DnsRecordType"/> to compare to.</param>
        /// <returns>A boolean indicating whether or not this <see cref="DnsWrapper"/>
        /// object contains a DNS record matching the entered type.</returns>
        /// <remarks>
        /// Determines if this <see cref="DnsWrapper"/> is of a specific
        /// <see cref="DnsRecordType"/>. The comparison does not test the
        /// <see cref="RecordData"/> field.
        /// </remarks>
        public bool Equals(DnsRecordType type)
        {
            if (RecordType == type)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether or not this <see cref="DnsWrapper"/> instance
        /// is equal to another <see cref="DnsWrapper"/> or to a
        /// <see cref="DnsRecordType"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>A boolean indicating whether or not this <see cref="DnsWrapper"/>
        /// object equals the entered object.</returns>
        /// <remarks>
        /// Determines if this <see cref="DnsWrapper"/> instance is equal to
        /// an object. If the object is a <see cref="DnsRecordType"/>, the
        /// <see cref="Equals(DnsRecordType)"/> method is used to determine
        /// equality based on the record type. If the object is a <see cref="DnsWrapper"/>
        /// object, the <see cref="CompareTo"/> method is used to determine
        /// equality. If the object is any other type, the <see cref="Object"/>
        /// class's Equal method is used for comparison.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DnsRecordType)
                return Equals((DnsRecordType)obj);

            if (obj is DnsWrapper)
                return (CompareTo(obj) == 0 ? true : false);

            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type, suitable
        /// for use in hashing algorithms and data structures like a
        /// hash table.
        /// </summary>
        /// <returns>Integer value representing the hashcode of this
        /// instance of <see cref="DnsWrapper"/>.</returns>
        /// <remarks>
        /// The GetHashCode method uses the hash codes of the <see cref="RecordData"/>
        /// and <see cref="RecordType"/> properties to generate a unique code
        /// for this particular record type/data combination.
        /// </remarks>
        public override int GetHashCode()
        {
            return RecordData.GetHashCode() ^ RecordType.GetHashCode();
        }

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the
        /// comparands. The return value has these meanings:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <description>This instance is less than obj. The <see cref="RecordData"/>
        ///         types do not match.</description>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <description>This instance is equal to obj. </description>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <description>This instance is greater than obj. The <see cref="RecordType"/>
        ///         do not match.</description>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Compares a <see cref="DnsWrapper"/> to this instance by its
        /// <see cref="RecordType"/> and <see cref="RecordData"/> properties.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// obj is not the same type as this instance.
        /// </exception>
        public int CompareTo(object obj)
        {
            if (!(obj is DnsWrapper))
                throw new ArgumentException();

            DnsWrapper dnsw = (DnsWrapper)obj;
            if (RecordData.GetType() != dnsw.RecordData.GetType())
                return -1;

            if (RecordType != dnsw.RecordType)
                return 1;

            return 0;
        }

        #endregion
    }
}