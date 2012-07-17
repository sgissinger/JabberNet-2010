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
using System.Text;
using StringPrep;

namespace Jabber.Stun
{
    /// <summary>
    /// Represents a message attribute according to STUN [RFC5389], TURN [RFC5766] and STUN Classic [RFC3489]
    /// </summary>
    public class StunAttribute
    {
        #region CONSTANTS
        #region STUN Core required
        private const UInt16 MAPPED_ADDRESS = 0x0001;
        private const UInt16 USERNAME = 0x0006;
        private const UInt16 MESSAGE_INTEGRITY = 0x0008;
        private const UInt16 ERROR_CODE = 0x0009;
        private const UInt16 UNKNOWN_ATTRIBUTES = 0x000A;
        private const UInt16 REALM = 0x0014;
        private const UInt16 NONCE = 0x0015;
        private const UInt16 XOR_MAPPED_ADDRESS = 0x0020;
        #endregion
        #region STUN Core optional
        private const UInt16 SOFTWARE = 0x8022;
        private const UInt16 ALTERNATE_SERVER = 0x8023;
        private const UInt16 FINGERPRINT = 0x8028;
        #endregion
        #region TURN Extension
        private const UInt16 CHANNEL_NUMBER = 0x000C;
        private const UInt16 LIFETIME = 0x000D;
        private const UInt16 XOR_PEER_ADDRESS = 0x0012;
        private const UInt16 DATA = 0x0013;
        private const UInt16 XOR_RELAYED_ADDRESS = 0x0016;
        private const UInt16 EVEN_PORT = 0x0018;
        private const UInt16 REQUESTED_TRANSPORT = 0x0019;
        private const UInt16 DONT_FRAGMENT = 0x001A;
        private const UInt16 RESERVATION_TOKEN = 0x0022;
        #endregion
        #region STUN Classic
        [Obsolete("Defined in RFC3489")]
        private const UInt16 RESPONSE_ADDRESS = 0x0002;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 CHANGE_REQUEST = 0x0003;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 SOURCE_ADDRESS = 0x0004;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 CHANGED_ADDRESS = 0x0005;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 PASSWORD = 0x0007;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 REFLECTED_FROM = 0x000B;
        [Obsolete("Defined in draft RFC3489bis-02")]
        private const UInt16 XOR_MAPPED_ADDRESS_ALT = 0x8020;
        #endregion
        #endregion

        #region MEMBERS
        /// <summary>
        /// Default encoder used to UT8 encode strings attribute values
        /// </summary>
        public static Encoding Encoder = new UTF8Encoding();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains the type of this attribute
        /// </summary>
        public StunAttributeType Type { get; private set; }
        /// <summary>
        /// Contains the byte array representation, in network-byte order, of this attribute's type
        /// </summary>
        public byte[] TypeValue { get; private set; }
        /// <summary>
        /// Contains the hexadecimal representation, in network-byte order, of this attribute's type
        /// </summary>
        public String TypeValueText
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture,
                                     "{0:X4}",
                                     StunUtilities.ReverseBytes(BitConverter.ToUInt16(this.TypeValue, 0)));
            }
        }
        /// <summary>
        /// Contains the byte array representation of this attribute's value
        /// </summary>
        public byte[] Value { get; private set; }
        /// <summary>
        /// Contains the length of this attribute's value
        /// </summary>
        public UInt16 ValueLength
        {
            get { return (UInt16)this.Value.Length; }
        }
        #endregion

        #region CONSTRUCTOR & FINALIZERS
        /// <summary>
        /// Constructs a new StunAttribute
        /// </summary>
        /// <param name="type">The type of this StunAttribute</param>
        /// <param name="value">The value of this StunAttribute</param>
        public StunAttribute(StunAttributeType type, String value)
            : this(type, StunAttribute.Encoder.GetBytes(value))
        { }

        /// <summary>
        /// Constructs a new StunAttribute
        /// </summary>
        /// <param name="type">The type of this StunAttribute</param>
        /// <param name="value">The value of this StunAttribute</param>
        public StunAttribute(StunAttributeType type, byte[] value)
            : this(type, StunAttribute.AttributeTypeToBytes(type), value)
        { }

        /// <summary>
        /// Constructs a new StunAttribute
        /// </summary>
        /// <param name="type">The type of this StunAttribute</param>
        /// <param name="typeBytes">The value of the type of this StunAttribute in bytes</param>
        /// <param name="value">The value of this StunAttribute</param>
        public StunAttribute(StunAttributeType type, byte[] typeBytes, byte[] value)
        {
            switch (type)
            {
                case StunAttributeType.Software:
                case StunAttributeType.Realm:
                case StunAttributeType.Nonce:
                case StunAttributeType.ErrorCode:
                    if (value.Length > 763)
                        throw new ArgumentOutOfRangeException("value", "cannot be larger than 763 bytes for the given type as described in RFC 5389");
                    break;

                case StunAttributeType.Username:
                    if (value.Length > 513)
                        throw new ArgumentOutOfRangeException("value", "cannot be larger than 513 bytes for the given type as described in RFC 5389");
                    break;
            }

            switch (type)
            {
                case StunAttributeType.Realm:
                case StunAttributeType.Username:
                    String saslPrepValue = new SASLprep().Prepare(StunAttribute.Encoder.GetString(value));

                    value = StunAttribute.Encoder.GetBytes(saslPrepValue);
                    break;
            }

            this.TypeValue = typeBytes;
            this.Type = type;
            this.Value = value;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// String summary of this attribute
        /// </summary>
        /// <returns>A String with informations about this attribute</returns>
        public override String ToString()
        {
            return String.Format(CultureInfo.CurrentCulture,
                                 "{0}, {1}",
                                 this.Type,
                                 StunAttribute.Encoder.GetString(this.Value));
        }

        /// <summary>
        /// Helper method to auto-cast this attribute to an array of bytes
        /// </summary>
        /// <returns>The array of bytes representation of this attribute</returns>
        public byte[] GetBytes()
        {
            return this;
        }

        /// <summary>
        /// Converts a StunAttribute to a byte array implicitly (no cast needed).
        /// </summary>
        /// <param name="attribute">StunAttribute who's byte array representation we want</param>
        /// <returns>Byte array version of the StunAttribute</returns>
        public static implicit operator byte[](StunAttribute attribute)
        {
            if (attribute == null)
                return null;

            byte[] type = StunAttribute.AttributeTypeToBytes(attribute.Type);

            byte[] length = BitConverter.GetBytes(attribute.ValueLength);
            Array.Reverse(length);

            byte[] value = StunUtilities.PadTo32Bits(attribute.Value);

            byte[] result = new byte[type.Length + length.Length + value.Length];

            type.CopyTo(result, 0);
            length.CopyTo(result, type.Length);
            value.CopyTo(result, type.Length + length.Length);

            return result;
        }

        /// <summary>
        /// Converts a byte array to a StunAttribute implicitly (no cast needed).
        /// </summary>
        /// <param name="attribute">Byte array who's StunAttribute representation we want</param>
        /// <returns>StunAttribute version of the byte array</returns>
        public static implicit operator StunAttribute(byte[] attribute)
        {
            if (attribute.Length < 5)
                return null;

            byte[] attributeTypeValue = StunUtilities.SubArray(attribute, 0, 2);
            StunAttributeType attributeType = StunAttribute.BytesToAttributeType(attributeTypeValue);

            UInt16 length = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(attribute, 2, 2), 0));

            byte[] value = StunUtilities.SubArray(attribute, 4, length);

            return new StunAttribute(attributeType, attributeTypeValue, value);
        }
        #endregion

        #region STATICS
        /// <summary>
        /// Convert a StunAttributeType to a network-byte ordered array of bytes
        /// </summary>
        /// <param name="type">The StunAttributeType to convert</param>
        /// <returns>
        /// The array of 2 bytes (16bits) matching the StunAttributeType
        /// Returns { 0, 0 } if the type parameter is StunAttributeType.Unknown
        /// </returns>
        public static byte[] AttributeTypeToBytes(StunAttributeType type)
        {
            byte[] typeBytes;

            switch (type)
            {
                case StunAttributeType.Unknown:
                default:
                    typeBytes = BitConverter.GetBytes((UInt16)0x0000);
                    break;

                // STUN Core required
                case StunAttributeType.MappedAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.MAPPED_ADDRESS);
                    break;

                case StunAttributeType.Username:
                    typeBytes = BitConverter.GetBytes(StunAttribute.USERNAME);
                    break;

                case StunAttributeType.MessageIntegrity:
                    typeBytes = BitConverter.GetBytes(StunAttribute.MESSAGE_INTEGRITY);
                    break;

                case StunAttributeType.ErrorCode:
                    typeBytes = BitConverter.GetBytes(StunAttribute.ERROR_CODE);
                    break;

                case StunAttributeType.UnknownAttributes:
                    typeBytes = BitConverter.GetBytes(StunAttribute.UNKNOWN_ATTRIBUTES);
                    break;

                case StunAttributeType.Realm:
                    typeBytes = BitConverter.GetBytes(StunAttribute.REALM);
                    break;

                case StunAttributeType.Nonce:
                    typeBytes = BitConverter.GetBytes(StunAttribute.NONCE);
                    break;

                case StunAttributeType.XorMappedAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.XOR_MAPPED_ADDRESS);
                    break;

                // STUN Core optional
                case StunAttributeType.Software:
                    typeBytes = BitConverter.GetBytes(StunAttribute.SOFTWARE);
                    break;

                case StunAttributeType.AlternateServer:
                    typeBytes = BitConverter.GetBytes(StunAttribute.ALTERNATE_SERVER);
                    break;

                case StunAttributeType.FingerPrint:
                    typeBytes = BitConverter.GetBytes(StunAttribute.FINGERPRINT);
                    break;

                // TURN Extension
                case StunAttributeType.ChannelNumber:
                    typeBytes = BitConverter.GetBytes(StunAttribute.CHANNEL_NUMBER);
                    break;

                case StunAttributeType.LifeTime:
                    typeBytes = BitConverter.GetBytes(StunAttribute.LIFETIME);
                    break;

                case StunAttributeType.XorPeerAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.XOR_PEER_ADDRESS);
                    break;

                case StunAttributeType.Data:
                    typeBytes = BitConverter.GetBytes(StunAttribute.DATA);
                    break;

                case StunAttributeType.XorRelayedAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.XOR_RELAYED_ADDRESS);
                    break;

                case StunAttributeType.EvenPort:
                    typeBytes = BitConverter.GetBytes(StunAttribute.EVEN_PORT);
                    break;

                case StunAttributeType.RequestedTransport:
                    typeBytes = BitConverter.GetBytes(StunAttribute.REQUESTED_TRANSPORT);
                    break;

                case StunAttributeType.DontFragment:
                    typeBytes = BitConverter.GetBytes(StunAttribute.DONT_FRAGMENT);
                    break;

                case StunAttributeType.ReservationToken:
                    typeBytes = BitConverter.GetBytes(StunAttribute.RESERVATION_TOKEN);
                    break;

                // STUN Classic
                case StunAttributeType.ResponseAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.RESPONSE_ADDRESS);
                    break;

                case StunAttributeType.ChangeRequest:
                    typeBytes = BitConverter.GetBytes(StunAttribute.CHANGE_REQUEST);
                    break;

                case StunAttributeType.SourceAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.SOURCE_ADDRESS);
                    break;

                case StunAttributeType.ChangedAddress:
                    typeBytes = BitConverter.GetBytes(StunAttribute.CHANGED_ADDRESS);
                    break;

                case StunAttributeType.Password:
                    typeBytes = BitConverter.GetBytes(StunAttribute.PASSWORD);
                    break;

                case StunAttributeType.ReflectedFrom:
                    typeBytes = BitConverter.GetBytes(StunAttribute.REFLECTED_FROM);
                    break;

                case StunAttributeType.XorMappedAddressAlt:
                    typeBytes = BitConverter.GetBytes(StunAttribute.XOR_MAPPED_ADDRESS_ALT);
                    break;
            }
            Array.Reverse(typeBytes);

            return typeBytes;
        }

        /// <summary>
        /// Convert a network-byte ordered array of bytes to a StunAttributeType
        /// </summary>
        /// <param name="bytes">An array of 2 bytes (16bits) representing an attribute type</param>
        /// <returns>
        /// The StunAttributeType matching the array of bytes
        /// Returns StunAttributeType.Unknown if the byte array doesn't match any constants
        /// </returns>
        public static StunAttributeType BytesToAttributeType(byte[] bytes)
        {
            UInt16 type = StunUtilities.ReverseBytes(BitConverter.ToUInt16(bytes, 0));

            StunAttributeType stunType;

            switch (type)
            {
                default:
                    stunType = StunAttributeType.Unknown;
                    break;

                // STUN Core required
                case StunAttribute.MAPPED_ADDRESS:
                    stunType = StunAttributeType.MappedAddress;
                    break;

                case StunAttribute.USERNAME:
                    stunType = StunAttributeType.Username;
                    break;

                case StunAttribute.MESSAGE_INTEGRITY:
                    stunType = StunAttributeType.MessageIntegrity;
                    break;

                case StunAttribute.ERROR_CODE:
                    stunType = StunAttributeType.ErrorCode;
                    break;

                case StunAttribute.UNKNOWN_ATTRIBUTES:
                    stunType = StunAttributeType.UnknownAttributes;
                    break;

                case StunAttribute.REALM:
                    stunType = StunAttributeType.Realm;
                    break;

                case StunAttribute.NONCE:
                    stunType = StunAttributeType.Nonce;
                    break;

                case StunAttribute.XOR_MAPPED_ADDRESS:
                    stunType = StunAttributeType.XorMappedAddress;
                    break;

                // STUN Core optional
                case StunAttribute.SOFTWARE:
                    stunType = StunAttributeType.Software;
                    break;

                case StunAttribute.ALTERNATE_SERVER:
                    stunType = StunAttributeType.AlternateServer;
                    break;

                case StunAttribute.FINGERPRINT:
                    stunType = StunAttributeType.FingerPrint;
                    break;

                // TURN Extension
                case StunAttribute.CHANNEL_NUMBER:
                    stunType = StunAttributeType.ChannelNumber;
                    break;

                case StunAttribute.LIFETIME:
                    stunType = StunAttributeType.LifeTime;
                    break;

                case StunAttribute.XOR_PEER_ADDRESS:
                    stunType = StunAttributeType.XorPeerAddress;
                    break;

                case StunAttribute.DATA:
                    stunType = StunAttributeType.Data;
                    break;

                case StunAttribute.XOR_RELAYED_ADDRESS:
                    stunType = StunAttributeType.XorRelayedAddress;
                    break;

                case StunAttribute.EVEN_PORT:
                    stunType = StunAttributeType.EvenPort;
                    break;

                case StunAttribute.REQUESTED_TRANSPORT:
                    stunType = StunAttributeType.RequestedTransport;
                    break;

                case StunAttribute.DONT_FRAGMENT:
                    stunType = StunAttributeType.DontFragment;
                    break;

                case StunAttribute.RESERVATION_TOKEN:
                    stunType = StunAttributeType.ReservationToken;
                    break;

                // STUN Classic
                case StunAttribute.RESPONSE_ADDRESS:
                    stunType = StunAttributeType.ResponseAddress;
                    break;

                case StunAttribute.CHANGE_REQUEST:
                    stunType = StunAttributeType.ChangeRequest;
                    break;

                case StunAttribute.SOURCE_ADDRESS:
                    stunType = StunAttributeType.SourceAddress;
                    break;

                case StunAttribute.CHANGED_ADDRESS:
                    stunType = StunAttributeType.ChangedAddress;
                    break;

                case StunAttribute.PASSWORD:
                    stunType = StunAttributeType.Password;
                    break;

                case StunAttribute.REFLECTED_FROM:
                    stunType = StunAttributeType.ReflectedFrom;
                    break;

                case StunAttribute.XOR_MAPPED_ADDRESS_ALT:
                    stunType = StunAttributeType.XorMappedAddressAlt;
                    break;
            }
            return stunType;
        }
        #endregion
    }

    /// <summary>
    /// Enumeration of any STUN attribute types
    /// Each attribute type has a matching constant defined in STUN [RFC5389] or in TURN [RFC5766]
    /// </summary>
    public enum StunAttributeType
    {
        /// <summary>
        /// Miscellaneous attributes unknown to this library
        /// </summary>
        Unknown,

        #region STUN Core required
        /// <summary>
        /// The MAPPED-ADDRESS attribute indicates a reflexive transport address of the client
        /// This attribute is used only by servers for achieving backwards compatibility with [RFC3489] clients.
        /// </summary>
        MappedAddress,
        /// <summary>
        /// The USERNAME attribute is used for message integrity. It identifies the username and password
        /// combination used in the message-integrity check
        /// The value of USERNAME is a variable-length value. It MUST contain a UTF-8 [RFC3629] encoded
        /// sequence of less than 513 bytes, and MUST have been processed using SASLprep [RFC4013]
        /// </summary>
        Username,
        /// <summary>
        /// The MESSAGE-INTEGRITY attribute contains an HMAC-SHA1 [RFC2104] of the STUN message.
        /// The MESSAGE-INTEGRITY attribute can be present in any STUN message type
        /// </summary>
        MessageIntegrity,
        /// <summary>
        ///  The ERROR-CODE attribute is used in error response messages. It contains a numeric
        ///  error code value in the range of 300 to 699 plus a textual reason phrase encoded
        ///  in UTF-8 [RFC3629], and is consistent in its code assignments and semantics
        ///  with SIP [RFC3261] and HTTP [RFC2616].
        /// </summary>
        ErrorCode,
        /// <summary>
        /// The UNKNOWN-ATTRIBUTES attribute is present only in an error response when
        /// the response code in the ERROR-CODE attribute is 420.
        /// The attribute contains a list of 16-bit values, each of which
        /// represents an attribute type that was not understood by the server.
        /// </summary>
        UnknownAttributes,
        /// <summary>
        /// The REALM attribute may be present in requests and responses. It contains text that
        /// meets the grammar for "realm-value" as described in [RFC3261] but without
        /// the double quotes and their surrounding whitespace
        /// </summary>
        Realm,
        /// <summary>
        /// The NONCE attribute may be present in requests and responses. It contains a sequence
        /// of qdtext or quoted-pair, which are defined in [RFC3261].
        /// Note that this means that the NONCE attribute will not contain actual quote characters
        /// </summary>
        Nonce,
        /// <summary>
        /// The XOR-MAPPED-ADDRESS attribute is identical to the MAPPED-ADDRESS attribute,
        /// except that the reflexive transport address is obfuscated through the XOR function.
        /// </summary>
        XorMappedAddress,
        #endregion

        #region STUN Core optional
        /// <summary>
        /// The SOFTWARE attribute contains a textual description of the software being used
        /// by the agent sending the message. It is used by clients and servers.
        /// Its value SHOULD include manufacturer and version number.
        /// The attribute has no impact on operation of the protocol,
        /// and serves only as a tool for diagnostic and debugging purposes
        /// </summary>
        Software,
        /// <summary>
        /// The alternate server represents an alternate transport address identifying a
        /// different STUN server that the STUN client should try.
        /// It is encoded in the same way as MAPPED-ADDRESS, and thus refers to a
        /// single server by IP address.  The IP address family MUST be identical
        /// to that of the source IP address of the request.
        /// </summary>
        AlternateServer,
        /// <summary>
        /// The FINGERPRINT attribute MAY be present in all STUN messages. The value of the
        /// attribute is computed as the CRC-32 of the STUN message up to (but excluding)
        /// the FINGERPRINT attribute itself, XOR'ed with the 32-bit value 0x5354554e
        /// </summary>
        FingerPrint,
        #endregion

        #region TURN Extension
        /// <summary>
        /// The CHANNEL-NUMBER attribute contains the number of the channel
        /// </summary>
        ChannelNumber,
        /// <summary>
        /// The LIFETIME attribute represents the duration for which the server
        /// will maintain an allocation in the absence of a refresh
        /// </summary>
        LifeTime,
        /// <summary>
        /// The XOR-PEER-ADDRESS specifies the address and port of the peer as
        /// seen from the TURN server.  (For example, the peer's server-reflexive
        /// transport address if the peer is behind a NAT.)  It is encoded in the
        /// same way as XOR-MAPPED-ADDRESS [RFC5389].
        /// </summary>
        XorPeerAddress,
        /// <summary>
        /// The DATA attribute is present in all Send and Data indications.  The
        /// value portion of this attribute is variable length and consists of
        /// the application data (that is, the data that would immediately follow
        /// the UDP header if the data was been sent directly between the client and the peer)
        /// </summary>
        Data,
        /// <summary>
        /// The XOR-RELAYED-ADDRESS is present in Allocate responses. It 
        /// specifies the address and port that the server allocated to the
        /// client. It is encoded in the same way as XOR-MAPPED-ADDRESS [RFC5389].
        /// </summary>
        XorRelayedAddress,
        /// <summary>
        /// This attribute allows the client to request that the port in the
        /// relayed transport address be even, and (optionally) that the server
        /// reserve the next-higher port number
        /// </summary>
        EvenPort,
        /// <summary>
        /// This attribute is used by the client to request a specific transport
        /// protocol for the allocated transport address
        /// </summary>
        RequestedTransport,
        /// <summary>
        /// This attribute is used by the client to request that the server set
        /// the DF (Don't Fragment) bit in the IP header when relaying the
        /// application data onward to the peer
        /// </summary>
        DontFragment,
        /// <summary>
        /// The RESERVATION-TOKEN attribute contains a token that uniquely
        /// identifies a relayed transport address being held in reserve by the
        /// server. The server includes this attribute in a success response to
        /// tell the client about the token, and the client includes this
        /// attribute in a subsequent Allocate request to request the server use
        /// that relayed transport address for the allocation.
        /// </summary>
        ReservationToken,
        #endregion

        #region STUN Classic
        /// <summary>
        /// The RESPONSE-ADDRESS attribute indicates where the response to a
        /// Binding Request should be sent. Its syntax is identical to MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        ResponseAddress,
        /// <summary>
        /// The CHANGE-REQUEST attribute is used by the client to request that
        /// the server use a different address and/or port when sending the response
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        ChangeRequest,
        /// <summary>
        /// The SOURCE-ADDRESS attribute is present in Binding Responses. It
        /// indicates the source IP address and port that the server is sending
        /// the response from. Its syntax is identical to that of MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        SourceAddress,
        /// <summary>
        /// The CHANGED-ADDRESS attribute indicates the IP address and port where responses
        /// would have been sent from if the "change IP" and "change port" flags had
        /// been set in the CHANGE-REQUEST attribute of the Binding Request.
        /// The attribute is always present in a Binding Response, independent
        /// of the value of the flags. Its syntax is identical to MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        ChangedAddress,
        /// <summary>
        /// The PASSWORD attribute is used in Shared Secret Responses. It is
        /// always present in a Shared Secret Response, along with the USERNAME.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        Password,
        /// <summary>
        /// The REFLECTED-FROM attribute is present only in Binding Responses, when
        /// the Binding Request contained a RESPONSE-ADDRESS attribute. The attribute
        /// contains the identity (in terms of IP address) of the source where the
        /// request came from. Its purpose is to provide traceability, so that a STUN server
        /// cannot be used as a reflector for denial-of-service attacks.
        /// Its syntax is identical to the MAPPED-ADDRESS attribute.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        ReflectedFrom,
        /// <summary>
        /// This alternate XOR-MAPPED-ADDRESS attribute may be used in some STUN Servers
        /// implementation like Vovida or MS-TURN http://msdn.microsoft.com/en-us/library/dd909268
        /// </summary>
        [Obsolete("Defined in draft RFC3489bis-02")]
        XorMappedAddressAlt
        #endregion
    }
}