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
        #region PROPERTIES
        /// <summary>
        /// Contains the address family of the Address
        /// </summary>
        public AddressFamily AddressFamily
        {
            get
            {
                byte addressFamilyByte = this.Value[1];

                if (addressFamilyByte == StunAttribute.IPV4)
                    return AddressFamily.InterNetwork;
                else if (addressFamilyByte == StunAttribute.IPV6)
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
        /// <param name="attribute">The StunAttribute base for this MappedAddress</param>
        public MappedAddress(StunAttribute attribute)
            : base(StunAttributeType.MappedAddress, attribute.Value)
        {
            if (attribute.Type != StunAttributeType.MappedAddress)
                throw new ArgumentException("is not a valid MAPPED-ADDRESS attribute", "attribute");
        }

        /// <summary>
        /// Constructs a MappedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="type">The StunAttributeType associated with the attribute parameter</param>
        /// <param name="attribute">The StunAttribute base for this MappedAddress</param>
        public MappedAddress(StunAttributeType type, StunAttribute attribute)
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
        /// Constructs an AlternateServer based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this AlternateServer</param>
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
        /// Constructs a XorMappedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this XorMappedAddress</param>
        /// <param name="transactionID">
        /// The transaction ID of the message from where the attribute parameter originates
        /// It may be used by this XorMappedAddress to decode an IPV6 address
        /// </param>
        public XorMappedAddress(StunAttribute attribute, byte[] transactionID)
            : base(StunAttributeType.XorMappedAddress, attribute)
        {
            if (attribute.Type != StunAttributeType.XorMappedAddress)
                throw new ArgumentException("is not a valid XOR-MAPPED-ADDRESS attribute", "attribute");

            this.TransactionID = transactionID;
        }

        /// <summary>
        /// Constructs a XorMappedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="type">The StunAttributeType associated with the attribute parameter</param>
        /// <param name="attribute">The StunAttribute base for this XorMappedAddress</param>
        /// <param name="transactionID">
        /// The transaction ID of the message from where the attribute parameter originates
        /// It may be used by this XorMappedAddress to decode an IPV6 address
        /// </param>
        public XorMappedAddress(StunAttributeType type, StunAttribute attribute, byte[] transactionID)
            : base(type, attribute)
        {
            this.TransactionID = transactionID;
        }
        #endregion
    }

    /// <summary>
    /// This alternate XOR-MAPPED-ADDRESS attribute may be used in some STUN Servers
    /// implementation like Vovida or MS-TURN http://msdn.microsoft.com/en-us/library/dd909268
    /// </summary>
    public class XorMappedAddressAlt : XorMappedAddress
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a XorMappedAddressAlt based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this XorMappedAddressAlt</param>
        /// <param name="transactionID">
        /// The transaction ID of the message from where the attribute parameter originates
        /// It may be used by this XorMappedAddress to decode an IPV6 address
        /// </param>
        public XorMappedAddressAlt(StunAttribute attribute, byte[] transactionID)
            : base(StunAttributeType.XorMappedAddressAlt, attribute, transactionID)
        {
            if (attribute.Type != StunAttributeType.XorMappedAddressAlt)
                throw new ArgumentException("is not a valid XOR-MAPPED-ADDRESS-VOVIDA attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// The XOR-PEER-ADDRESS specifies the address and port of the peer as
    /// seen from the TURN server. (For example, the peer's server-reflexive
    /// transport address if the peer is behind a NAT)
    /// </summary>
    public class XorPeerAddress : XorMappedAddress
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a XorPeerAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this XorPeerAddress</param>
        /// <param name="transactionID">
        /// The transaction ID of the message from where the attribute parameter originates
        /// It may be used by this XorMappedAddress to decode an IPV6 address
        /// </param>
        public XorPeerAddress(StunAttribute attribute, byte[] transactionID)
            : base(StunAttributeType.XorPeerAddress, attribute, transactionID)
        {
            if (attribute.Type != StunAttributeType.XorPeerAddress)
                throw new ArgumentException("is not a valid XOR-PEER-ADDRESS attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// The XOR-RELAYED-ADDRESS is present in Allocate responses. It
    /// specifies the address and port that the server allocated to the client
    /// </summary>
    public class XorRelayedAddress : XorMappedAddress
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a XorRelayedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this XorRelayedAddress</param>
        /// <param name="transactionID">
        /// The transaction ID of the message from where the attribute parameter originates
        /// It may be used by this XorMappedAddress to decode an IPV6 address
        /// </param>
        public XorRelayedAddress(StunAttribute attribute, byte[] transactionID)
            : base(StunAttributeType.XorRelayedAddress, attribute, transactionID)
        {
            if (attribute.Type != StunAttributeType.XorRelayedAddress)
                throw new ArgumentException("is not a valid XOR-RELAYED-ADDRESS attribute", "attribute");
        }
        #endregion
    }
}