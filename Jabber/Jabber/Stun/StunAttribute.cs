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
using System.Reflection;
using StringPrep;

namespace Jabber.Stun
{
    /// <summary>
    /// Represents a message attribute according to STUN [RFC5389], TURN [RFC5766],
    /// TURN-TCP [RFC6062], TURN-IPV6 [RFC6156], ICE [RFC5245] and STUN Classic [RFC3489]
    /// </summary>
    public class StunAttribute
    {
        #region PROPERTIES
        /// <summary>
        /// Contains the type of this attribute
        /// </summary>
        public StunAttributeType Type { get; private set; }
        /// <summary>
        /// Contains the byte array representation, in network-byte order, of this attribute's type
        /// </summary>
        public byte[] TypeBytes { get; private set; }
        /// <summary>
        /// Contains the hexadecimal representation, in network-byte order, of this attribute's type
        /// </summary>
        public String TypeHex
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture,
                                     "{0:X4}",
                                     StunUtilities.ReverseBytes(BitConverter.ToUInt16(this.TypeBytes, 0)));
            }
        }
        /// <summary>
        /// Contains the unsigned int representation, in network-byte order, of this attribute's type
        /// </summary>
        public UInt16 TypeShort
        {
            get { return StunUtilities.ReverseBytes(BitConverter.ToUInt16(this.TypeBytes, 0)); }
        }
        /// <summary>
        /// Contains the byte array representation of this attribute's value
        /// </summary>
        public byte[] Value { get; protected set; }
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
        /// Constructs an empty StunAttribute
        /// </summary>
        public StunAttribute(StunAttributeType type)
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
            if (typeBytes == null)
                throw new ArgumentNullException("value", "Cannot be null");

            if (value == null)
                throw new ArgumentNullException("value", "Cannot be null");

            switch (type)
            {
                case StunAttributeType.Software:
                case StunAttributeType.Realm:
                case StunAttributeType.Nonce:
                case StunAttributeType.ErrorCode:
                    if (value.Length > 763)
                        throw new ArgumentOutOfRangeException("value", "Cannot be greater than 763 bytes for the given type as described in RFC 5389");
                    break;

                case StunAttributeType.Username:
                    if (value.Length > 513)
                        throw new ArgumentOutOfRangeException("value", "Cannot be greater than 513 bytes for the given type as described in RFC 5389");
                    break;
            }

            switch (type)
            {
                case StunAttributeType.Realm:
                case StunAttributeType.Username:
                    String saslPrepValue = new SASLprep().Prepare(StunMessage.Encoder.GetString(value));

                    value = StunMessage.Encoder.GetBytes(saslPrepValue);
                    break;
            }

            this.TypeBytes = typeBytes;
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
                                 this.TypeHex);
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
        /// Returns max UInt16 value if the type parameter is StunAttributeType.Unmanaged
        /// </returns>
        public static byte[] AttributeTypeToBytes(StunAttributeType type)
        {
            foreach (FieldInfo field in typeof(StunAttributeType).GetFields())
            {
                if (field.Name == type.ToString())
                {
                    Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                    if (fieldAttributes.Length == 1)
                    {
                        StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                        byte[] typeBytes = BitConverter.GetBytes(stunValueAttribute.Value);
                        Array.Reverse(typeBytes);

                        return typeBytes;
                    }
                }
            }
            return BitConverter.GetBytes(0xFFFF);
        }

        /// <summary>
        /// Convert a network-byte ordered array of bytes to a StunAttributeType
        /// </summary>
        /// <param name="bytes">An array of 2 bytes (16bits) representing an attribute type</param>
        /// <returns>
        /// The StunAttributeType matching the array of bytes
        /// Returns StunAttributeType.Unmanaged if the byte array doesn't match any StunAttributeType StunValue's
        /// </returns>
        public static StunAttributeType BytesToAttributeType(byte[] bytes)
        {
            UInt16 type = StunUtilities.ReverseBytes(BitConverter.ToUInt16(bytes, 0));

            foreach (FieldInfo field in typeof(StunAttributeType).GetFields())
            {
                Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                if (fieldAttributes.Length == 1)
                {
                    StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                    if (stunValueAttribute != null &&
                        stunValueAttribute.Value == type)
                    {
                        return (StunAttributeType)Enum.Parse(typeof(StunAttributeType), field.Name);
                    }
                }
            }
            return StunAttributeType.Unmanaged;
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
        /// Represents attributes whose type is not managed by this library
        /// </summary>
        [StunValue(0xFFFF)]
        Unmanaged,

        #region STUN Core required
        /// <summary>
        /// The MAPPED-ADDRESS attribute indicates a reflexive transport address of the client
        /// This attribute is used only by servers for achieving backwards compatibility with [RFC3489] clients.
        /// </summary>
        [StunValue(0x0001)]
        MappedAddress,
        /// <summary>
        /// The USERNAME attribute is used for message integrity. It identifies the username and password
        /// combination used in the message-integrity check
        /// The value of USERNAME is a variable-length value. It MUST contain a UTF-8 [RFC3629] encoded
        /// sequence of less than 513 bytes, and MUST have been processed using SASLprep [RFC4013]
        /// </summary>
        [StunValue(0x0006)]
        Username,
        /// <summary>
        /// The MESSAGE-INTEGRITY attribute contains an HMAC-SHA1 [RFC2104] of the STUN message.
        /// The MESSAGE-INTEGRITY attribute can be present in any STUN message type
        /// </summary>
        [StunValue(0x0008)]
        MessageIntegrity,
        /// <summary>
        ///  The ERROR-CODE attribute is used in error response messages. It contains a numeric
        ///  error code value in the range of 300 to 699 plus a textual reason phrase encoded
        ///  in UTF-8 [RFC3629], and is consistent in its code assignments and semantics
        ///  with SIP [RFC3261] and HTTP [RFC2616].
        /// </summary>
        [StunValue(0x0009)]
        ErrorCode,
        /// <summary>
        /// The UNKNOWN-ATTRIBUTES attribute is present only in an error response when
        /// the response code in the ERROR-CODE attribute is 420.
        /// The attribute contains a list of 16-bit values, each of which
        /// represents an attribute type that was not understood by the server.
        /// </summary>
        [StunValue(0x000A)]
        UnknownAttributes,
        /// <summary>
        /// The REALM attribute may be present in requests and responses. It contains text that
        /// meets the grammar for "realm-value" as described in [RFC3261] but without
        /// the double quotes and their surrounding whitespace
        /// </summary>
        [StunValue(0x0014)]
        Realm,
        /// <summary>
        /// The NONCE attribute may be present in requests and responses. It contains a sequence
        /// of qdtext or quoted-pair, which are defined in [RFC3261].
        /// Note that this means that the NONCE attribute will not contain actual quote characters
        /// </summary>
        [StunValue(0x0015)]
        Nonce,
        /// <summary>
        /// The XOR-MAPPED-ADDRESS attribute is identical to the MAPPED-ADDRESS attribute,
        /// except that the reflexive transport address is obfuscated through the XOR function.
        /// </summary>
        [StunValue(0x0020)]
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
        [StunValue(0x8022)]
        Software,
        /// <summary>
        /// The alternate server represents an alternate transport address identifying a
        /// different STUN server that the STUN client should try.
        /// It is encoded in the same way as MAPPED-ADDRESS, and thus refers to a
        /// single server by IP address.  The IP address family MUST be identical
        /// to that of the source IP address of the request.
        /// </summary>
        [StunValue(0x8023)]
        AlternateServer,
        /// <summary>
        /// The FINGERPRINT attribute MAY be present in all STUN messages. The value of the
        /// attribute is computed as the CRC-32 of the STUN message up to (but excluding)
        /// the FINGERPRINT attribute itself, XOR'ed with the 32-bit value 0x5354554e
        /// </summary>
        [StunValue(0x8028)]
        FingerPrint,
        #endregion

        #region TURN Extension
        /// <summary>
        /// The CHANNEL-NUMBER attribute contains the number of the channel
        /// </summary>
        [StunValue(0x000C)]
        ChannelNumber,
        /// <summary>
        /// The LIFETIME attribute represents the duration for which the server
        /// will maintain an allocation in the absence of a refresh
        /// </summary>
        [StunValue(0x000D)]
        LifeTime,
        /// <summary>
        /// The XOR-PEER-ADDRESS specifies the address and port of the peer as
        /// seen from the TURN server.  (For example, the peer's server-reflexive
        /// transport address if the peer is behind a NAT.)  It is encoded in the
        /// same way as XOR-MAPPED-ADDRESS [RFC5389].
        /// </summary>
        [StunValue(0x0012)]
        XorPeerAddress,
        /// <summary>
        /// The DATA attribute is present in all Send and Data indications.  The
        /// value portion of this attribute is variable length and consists of
        /// the application data (that is, the data that would immediately follow
        /// the UDP header if the data was been sent directly between the client and the peer)
        /// </summary>
        [StunValue(0x0013)]
        Data,
        /// <summary>
        /// The XOR-RELAYED-ADDRESS is present in Allocate responses. It 
        /// specifies the address and port that the server allocated to the
        /// client. It is encoded in the same way as XOR-MAPPED-ADDRESS [RFC5389].
        /// </summary>
        [StunValue(0x0016)]
        XorRelayedAddress,
        /// <summary>
        /// This attribute allows the client to request that the port in the
        /// relayed transport address be even, and (optionally) that the server
        /// reserve the next-higher port number
        /// </summary>
        [StunValue(0x0018)]
        EvenPort,
        /// <summary>
        /// This attribute is used by the client to request a specific transport
        /// protocol for the allocated transport address
        /// </summary>
        [StunValue(0x0019)]
        RequestedTransport,
        /// <summary>
        /// This attribute is used by the client to request that the server set
        /// the DF (Don't Fragment) bit in the IP header when relaying the
        /// application data onward to the peer
        /// </summary>
        [StunValue(0x001A)]
        DontFragment,
        /// <summary>
        /// The RESERVATION-TOKEN attribute contains a token that uniquely
        /// identifies a relayed transport address being held in reserve by the
        /// server. The server includes this attribute in a success response to
        /// tell the client about the token, and the client includes this
        /// attribute in a subsequent Allocate request to request the server use
        /// that relayed transport address for the allocation.
        /// </summary>
        [StunValue(0x0022)]
        ReservationToken,
        #endregion

        #region TURN-TCP Extension
        /// <summary>
        /// The CONNECTION-ID attribute uniquely identifies a peer data connection.
        /// </summary>
        [StunValue(0x002A)]
        ConnectionId,
        #endregion

        #region TURN-IPV6 Extension
        /// <summary>
        /// The REQUESTED-ADDRESS-FAMILY attribute is used by clients to request
        /// the allocation of a specific address type from a server.
        /// </summary>
        [StunValue(0X0017)]
        RequestedAddressFamily,
        #endregion

        #region ICE Extension
        /// <summary>
        /// Priority will be used by ICE to determine the order of the
        /// connectivity checks and the relative preference for candidates.
        /// </summary>
        [StunValue(0X0024)]
        Priority,
        /// <summary>
        /// The controlling agent MAY include the USE-CANDIDATE attribute in the Binding request.
        /// The controlled agent MUST NOT include it in its Binding request.
        /// This attribute signals that the controlling agent wishes to cease checks
        /// for this component, and use the candidate pair resulting from the check for this component
        /// </summary>
        [StunValue(0X0025)]
        UseCandidate,
        /// <summary>
        /// The agent MUST include the ICE-CONTROLLED attribute in the request
        /// if it is in the controlled role
        /// </summary>
        [StunValue(0X8029)]
        IceControlled,
        /// <summary>
        /// The agent MUST include the ICE-CONTROLLING attribute in the request
        /// if it is in the controlling role
        /// </summary>
        [StunValue(0X802A)]
        IceControlling,
        #endregion

        #region STUN Classic
        /// <summary>
        /// The RESPONSE-ADDRESS attribute indicates where the response to a
        /// Binding Request should be sent. Its syntax is identical to MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0002)]
        ResponseAddress,
        /// <summary>
        /// The CHANGE-REQUEST attribute is used by the client to request that
        /// the server use a different address and/or port when sending the response
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0003)]
        ChangeRequest,
        /// <summary>
        /// The SOURCE-ADDRESS attribute is present in Binding Responses. It
        /// indicates the source IP address and port that the server is sending
        /// the response from. Its syntax is identical to that of MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0004)]
        SourceAddress,
        /// <summary>
        /// The CHANGED-ADDRESS attribute indicates the IP address and port where responses
        /// would have been sent from if the "change IP" and "change port" flags had
        /// been set in the CHANGE-REQUEST attribute of the Binding Request.
        /// The attribute is always present in a Binding Response, independent
        /// of the value of the flags. Its syntax is identical to MAPPED-ADDRESS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0005)]
        ChangedAddress,
        /// <summary>
        /// The PASSWORD attribute is used in Shared Secret Responses. It is
        /// always present in a Shared Secret Response, along with the USERNAME.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0007)]
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
        [StunValue(0x000B)]
        ReflectedFrom,
        /// <summary>
        /// This alternate XOR-MAPPED-ADDRESS attribute may be used in some STUN Servers
        /// implementation like Vovida or MS-TURN http://msdn.microsoft.com/en-us/library/dd909268
        /// </summary>
        [Obsolete("Defined in draft RFC3489bis-02")]
        [StunValue(0x8020)]
        XorMappedAddressAlt
        #endregion
    }
}