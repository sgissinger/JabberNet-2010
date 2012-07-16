/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Sébastien Gissinger
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Jabber.Stun.Attributes
{
    /// <summary>
    /// Represents MAPPED-ADDRESS attribute returned by a STUN Server
    /// indicating a reflexive transport address of the client
    /// </summary>
    public class MappedAddress : StunAttribute
    {
        #region CONSTANTS
        private const byte IPV4 = 0x01;
        private const byte IPV6 = 0x02;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains the address family of the Address
        /// </summary>
        public AddressFamily AddressFamily
        {
            get
            {
                byte addressFamilyByte = StunUtilities.SubArray(this.Value, 1, 1)[0];

                if (addressFamilyByte == MappedAddress.IPV4)
                    return AddressFamily.InterNetwork;
                else if (addressFamilyByte == MappedAddress.IPV6)
                    return AddressFamily.InterNetworkV6;

                return AddressFamily.Unknown;
            }
        }
        /// <summary>
        /// Contains the IPEndPoint as the result of Address and Port merging
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                return new IPEndPoint(this.Address, this.Port);
            }
        }
        /// <summary>
        /// Contains the Address value associated to this MappedAddres
        /// </summary>
        public virtual IPAddress Address
        {
            get
            {
                byte[] addressBytes = StunUtilities.SubArray(this.Value, 4, this.ValueLength - 4);

                return new IPAddress(addressBytes);
            }
        }
        /// <summary>
        /// Contains the Port value associated to this MappedAddres
        /// </summary>
        public virtual Int32 Port
        {
            get
            {
                byte[] portBytes = StunUtilities.SubArray(this.Value, 2, 2);

                return StunUtilities.ReverseBytes(BitConverter.ToUInt16(portBytes, 0));
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a MappedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">Base StunAttribute used to construct this specialized attribute</param>
        public MappedAddress(StunAttribute attribute)
            : this(StunAttributeType.MappedAddress, attribute)
        {
            if (attribute.Type != StunAttributeType.MappedAddress)
                throw new ArgumentException("is not a valid MAPPED-ADDRESS attribute", "attribute");
        }

        /// <summary>
        /// Constructs a MappedAddress attribute based on an existing StunAttribute and using another
        /// type than MAPPED-ADDRESS attribute type
        /// </summary>
        /// <param name="type">The StunAttributeType to associate to the StunAttribute</param>
        /// <param name="attribute">Base StunAttribute use to construct this specialized attribute</param>
        protected MappedAddress(StunAttributeType type, StunAttribute attribute)
            : base(type, attribute.Value)
        { }
        #endregion

        #region METHODS
        /// <summary>
        /// String summary of this MAPPED-ADDRESS attribute
        /// </summary>
        /// <returns>A String with informations about this MAPPED-ADDRESS attribute</returns>
        public override String ToString()
        {
            return String.Format(CultureInfo.CurrentCulture,
                                 "{0}, {1}",
                                 this.Type,
                                 this.EndPoint);
        }
        #endregion
    }

    /// <summary>
    /// Represents ALTERNATE-SERVER attribute returned by a STUN Server and
    /// identifying a different STUN server that the STUN client should try
    /// </summary>
    public class AlternateServer : MappedAddress
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an AlternateServer attribute based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">Base StunAttribute used to construct this specialized attribute</param>
        public AlternateServer(StunAttribute attribute)
            : base(StunAttributeType.AlternateServer, attribute)
        {
            if (attribute.Type != StunAttributeType.AlternateServer)
                throw new ArgumentException("is not a valid ALTERNATE-SERVER attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// Represents XOR-MAPPED-ADDRESS attribute returned by a STUN Server
    /// indicating a reflexive transport address of the client
    /// </summary>
    public class XorMappedAddress : MappedAddress
    {
        #region PROPERTIES
        /// <summary>
        /// Transaction ID of the StunMessage from which this aAttribute
        /// originates used to XOR'd IPV6 (128bits) addresses
        /// </summary>
        public byte[] TransactionID { get; private set; }
        /// <summary>
        /// Contains the Address value associated to this XorMappedAddres
        /// </summary>
        public override IPAddress Address
        {
            get
            {
                byte[] addressBytes = StunUtilities.SubArray(this.Value, 4, this.ValueLength - 4);

                if (this.AddressFamily == AddressFamily.InterNetwork)
                {
                    // Addressin network-byte order
                    UInt32 xoredAddress = BitConverter.ToUInt32(addressBytes, 0);

                    // MAGIC-COOKIE in host-byte order
                    byte[] magicCookieBytes = BitConverter.GetBytes(StunMessage.MAGIC_COOKIE);

                    // MAGIC-COOKIE in network-byte order
                    UInt32 xoringElement = StunUtilities.ReverseBytes(BitConverter.ToUInt32(magicCookieBytes, 0));

                    // XORing the two network-byte order values gives the right value
                    return new IPAddress(BitConverter.GetBytes(xoredAddress ^ xoringElement));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        /// <summary>
        /// Contains the Port value associated to this XorMappedAddres
        /// </summary>
        public override Int32 Port
        {
            get
            {
                // Port in network-byte order
                byte[] portBytes = StunUtilities.SubArray(this.Value, 2, 2);

                // Port in host-byte order
                UInt16 xoredPort = StunUtilities.ReverseBytes(BitConverter.ToUInt16(portBytes, 0));

                // MAGIC-COOKIE in host-byte order
                byte[] magicCookieBigBytes = StunUtilities.SubArray(BitConverter.GetBytes(StunMessage.MAGIC_COOKIE), 2, 2);

                UInt16 xoringElement = BitConverter.ToUInt16(magicCookieBigBytes, 0);

                // XORing the two host-byte order values gives the right value
                return xoredPort ^ xoringElement;
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a XorMappedAddress attribute based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">Base StunAttribute used to construct this specialized attribute</param>
        /// <param name="transactionID"></param>
        public XorMappedAddress(StunAttribute attribute, byte[] transactionID)
            : base(StunAttributeType.XorMappedAddress, attribute)
        {
            if (attribute.Type != StunAttributeType.XorMappedAddress)
                throw new ArgumentException("is not a valid XOR-MAPPED-ADDRESS attribute", "attribute");

            this.TransactionID = transactionID;
        }
        #endregion
    }
}
