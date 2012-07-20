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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Jabber.Stun.Attributes;
using Jabber.Stun.AttributesByRFC;
using StringPrep;

namespace Jabber.Stun
{
    /// <summary>
    /// Represents a message according to STUN [RFC5389], TURN [RFC5766],
    /// TURN-TCP [RFC6062], TURN-IPV6 [RFC6156], ICE [RFC5245] and STUN Classic [RFC3489]
    /// </summary>
    public class StunMessage
    {
        #region CONSTANTS
        /// <summary>
        /// TODO: Documentation Constant
        /// </summary>
        public const byte ADDRESS_FAMILY_IPV4 = 0x01;
        /// <summary>
        /// TODO: Documentation Constant
        /// </summary>
        public const byte ADDRESS_FAMILY_IPV6 = 0x02;
        /// <summary>
        /// TODO: Documentation Constant
        /// </summary>
        public const byte CODE_POINT_TCP = 0X06;
        /// <summary>
        /// TODO: Documentation Constant
        /// </summary>
        public const byte CODE_POINT_UDP = 0x11;
        /// <summary>
        /// The magic cookie field MUST contain the fixed value 0x2112A442 in network byte order.
        /// In [RFC3489], this field was part of the transaction ID.
        /// Placing the magic cookie allows a server to detect if the client will
        /// understand certain attributes that were added in specification [RFC5389]
        /// </summary>
        public const UInt32 MAGIC_COOKIE = 0x2112A442;
        #endregion

        #region MEMBERS
        /// <summary>
        /// Default encoder used to UT8 encode strings attribute values
        /// </summary>
        public static Encoding Encoder = new UTF8Encoding();
        /// <summary>
        /// Contains the list of every managed attributes of type other than StunAttributeType.Unmanaged
        /// </summary>
        private Dictionary<StunAttributeType, StunAttribute> attributesList = new Dictionary<StunAttributeType, StunAttribute>();
        /// <summary>
        /// Contains the list of every unmanaged attributes of type StunAttributeType.Unmanaged
        /// </summary>
        private List<StunAttribute> unmanagedAttributesList = new List<StunAttribute>();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains the array of bytes representation of this message
        /// </summary>
        public byte[] Bytes
        {
            get { return this; }
        }

        /// <summary>
        /// Contains a copy of the list of every managed attributes of type other than StunAttributeType.Unmanaged
        /// </summary>
        public StunAttribute[] AttributesList
        {
            get
            {
                StunAttribute[] attrs = new StunAttribute[this.attributesList.Count];

                this.attributesList.Values.CopyTo(attrs, 0);

                return attrs;
            }
            set
            {
                this.attributesList.Clear();

                foreach (StunAttribute attribute in value)
                {
                    this.SetAttribute(attribute);
                }
            }
        }
        /// <summary>
        /// Contains a copy of the list of every unmanaged attributes of type StunAttributeType.Unmanaged
        /// </summary>
        public StunAttribute[] UnmanagedAttributesList
        {
            get
            {
                StunAttribute[] attrs = new StunAttribute[this.unmanagedAttributesList.Count];

                this.unmanagedAttributesList.CopyTo(attrs, 0);

                return attrs;
            }
            set
            {
                this.unmanagedAttributesList.Clear();

                foreach (StunAttribute attribute in value)
                {
                    this.SetAttribute(attribute);
                }
            }
        }
        /// <summary>
        /// Contains the method type this message encapsulates
        /// </summary>
        public StunMethodType MethodType { get; private set; }
        /// <summary>
        /// Contains the method class this message encapsulates
        /// </summary>
        public StunMethodClass MethodClass { get; private set; }
        /// <summary>
        /// Contains the size, in bytes, of the message not including the 20-byte STUN header
        /// </summary>
        public UInt16 MessageLength
        {
            get
            {
                return (UInt16)(StunUtilities.GetBytes(this.AttributesList).Length + StunUtilities.GetBytes(this.UnmanagedAttributesList).Length);
            }
        }
        /// <summary>
        /// The transaction ID is a 96-bit (12byte) identifier, used to uniquely identify
        /// STUN transactions.
        ///  * For request/response transactions, the transaction ID is chosen by the STUN
        ///    client for the request and echoed by the server in the response.
        ///  * For indications, it is chosen by the agent sending the indication.
        /// </summary>
        public byte[] TransactionID { get; private set; }

        #region ATTRIBUTES
        /// <summary>
        /// Contains a MAPPED-ADDRESS or XOR-MAPPED-ADDRESS attribute if this message contains one of these
        /// The XOR-MAPPED-ADDRESS is parsed prior to MAPPED-ADDRESS
        /// </summary>
        public MappedAddress MappedAddress
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.XorMappedAddress);

                if (attribute != null)
                {
                    return new XorMappedAddress(StunAttributeType.XorMappedAddress, attribute, this.TransactionID);
                }
                else
                {
                    attribute = this.GetAttribute(StunAttributeType.XorMappedAddressAlt);

                    if (attribute != null)
                    {
                        return new XorMappedAddress(StunAttributeType.XorMappedAddressAlt, attribute, this.TransactionID);
                    }
                    else
                    {
                        attribute = this.GetAttribute(StunAttributeType.MappedAddress);

                        if (attribute != null)
                            return new MappedAddress(StunAttributeType.MappedAddress, attribute);
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunRFC Stun
        {
            get { return new StunRFC(this); }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunClassicRFC StunClassic
        {
            get { return new StunClassicRFC(this); }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public TurnRFCs Turn
        {
            get { return new TurnRFCs(this); }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public IceRFC Ice
        {
            get { return new IceRFC(this); }
        }
        #endregion
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Construct a message with given type, class and transaction ID
        /// </summary>
        /// <param name="type">The type of the message</param>
        /// <param name="mclass">The class of the message</param>
        /// <param name="transactionId">The transaction ID of the message </param>
        public StunMessage(StunMethodType type, StunMethodClass mclass, byte[] transactionId)
        {
            this.MethodType = type;
            this.MethodClass = mclass;
            this.TransactionID = transactionId;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Add an attribute to this message
        /// Any existing attribute of the same type will be replaced by the attribute in parameter.
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        public void SetAttribute(StunAttribute attribute)
        {
            this.SetAttribute(attribute, true);
        }

        /// <summary>
        /// Add an attribute to this message
        /// If the attribute is not managed by this library, it will be added to the UnmanagedAttributes list
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        /// <param name="replaceExistingAttribute">
        /// If TRUE, any existing attribute of the same type as the attribute parameter
        /// will be replaced by the attribute in parameter
        /// </param>
        public void SetAttribute(StunAttribute attribute, Boolean replaceExistingAttribute)
        {
            if (attribute.Type == StunAttributeType.Unmanaged)
            {
                this.unmanagedAttributesList.Add(attribute);
            }
            else
            {
                if (!this.attributesList.ContainsKey(attribute.Type))
                    this.attributesList.Add(attribute.Type, attribute);
                else
                    if (replaceExistingAttribute)
                        this.attributesList[attribute.Type] = attribute;
            }
        }

        /// <summary>
        /// Retrieve the attribute of a given type in this message
        /// </summary>
        /// <param name="type">The type of the attribute to retrieve</param>
        /// <returns>The known attribute matching the given type or null if it doesn't exists</returns>
        public StunAttribute GetAttribute(StunAttributeType type)
        {
            return this.attributesList.ContainsKey(type) ? this.attributesList[type] : null;
        }

        /// <summary>
        /// Add the MESSAGE-INTEGRITY attribute to the message.
        /// This should be fired just before a call to StunClient.SendMessage as it needs existing
        /// attribute to be computed. But these attributes MUST be added before the MESSAGE-ATTRIBUTE
        /// except for FINGERPRINT attribute which MUST be added at the end of the StunMessage
        /// and also used in the MESSAGE-INTEGRITY computation
        /// </summary>
        /// <param name="password">The password used to authenticate the StunMessage</param>
        /// <param name="useLongTermCredentials">True if this StunMessage should authenticate using longterm credentials</param>
        public void AddMessageIntegrity(String password, Boolean useLongTermCredentials)
        {
            password = new SASLprep().Prepare(password);

            byte[] hmacSha1Key;

            if (useLongTermCredentials)
            {
                if (this.Stun.Username == null)
                    throw new ArgumentException("USERNAME attribute is mandatory for long-term credentials MESSAGE-INTEGRITY creation", "this.Username");

                if (this.Stun.Realm == null)
                    throw new ArgumentException("REALM attribute is mandatory for long-term credentials MESSAGE-INTEGRITY creation", "this.Realm");

                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    String valueToHashMD5 = String.Format(CultureInfo.CurrentCulture,
                                                          "{0}:{1}:{2}",
                                                          this.Stun.Username.ValueString,
                                                          this.Stun.Realm.ValueString,
                                                          password);

                    hmacSha1Key = md5.ComputeHash(StunMessage.Encoder.GetBytes(valueToHashMD5));
                }
            }
            else
            {
                hmacSha1Key = StunMessage.Encoder.GetBytes(password);
            }

            StunAttribute messageIntegrity = new StunAttribute(StunAttributeType.MessageIntegrity,
                                                               this.ComputeHMAC(hmacSha1Key));
            this.SetAttribute(messageIntegrity);
        }

        /// <summary>
        /// Computes a HMAC SHA1 based on this StunMessage attributes
        /// </summary>
        /// <param name="hmacSha1Key">The key of HMAC SHA1 computation algorithm</param>
        /// <returns>The HMAC computed value of this StunMessage</returns>
        private byte[] ComputeHMAC(byte[] hmacSha1Key)
        {
            byte[] hashed;

            using (HMACSHA1 hmacSha1 = new HMACSHA1(hmacSha1Key))
            {
                StunMessage thisCopy = new StunMessage(this.MethodType, this.MethodClass, this.TransactionID);

                foreach (var item in this.attributesList)
                {
                    if (item.Key == StunAttributeType.MessageIntegrity)
                        break;

                    thisCopy.SetAttribute(item.Value);
                }

                if (this.Stun.FingerPrint != null)
                    thisCopy.SetAttribute(this.Stun.FingerPrint);

                byte[] thisCopyBytes = thisCopy;

                // Insert a fake message length for HMAC computation as described in [RFC5489#15.4]
                UInt16 dummyLength = StunUtilities.ReverseBytes((UInt16)(thisCopy.MessageLength + 24)); // 20 hmac + 4 header

                BitConverter.GetBytes(dummyLength).CopyTo(thisCopyBytes, 2);

                hashed = hmacSha1.ComputeHash(thisCopyBytes);
            }

            return hashed;
        }

        /// <summary>
        /// Empty every managed and unmanaged attributes in this StunMessage
        /// </summary>
        public void ClearAttributes()
        {
            this.attributesList.Clear();
            this.unmanagedAttributesList.Clear();
        }

        /// <summary>
        /// Converts a StunMessage to a byte array implicitly (no cast needed)
        /// </summary>
        /// <param name="message">StunMessage who's byte array representation we want</param>
        /// <returns>Byte array version of the StunMessage</returns>
        public static implicit operator byte[](StunMessage message)
        {
            if (message == null)
                return null;

            byte[] attributesBytes = StunUtilities.GetBytes(message.AttributesList);

            UInt16 methodType = StunMessage.MethodTypeToUInt16(message.MethodType);
            UInt16 methodClass = StunMessage.MethodClassToUInt16(message.MethodClass);

            byte[] computedType = BitConverter.GetBytes(((UInt16)(methodType | methodClass)));
            Array.Reverse(computedType);

            byte[] messageLength = BitConverter.GetBytes((UInt16)attributesBytes.Length);
            Array.Reverse(messageLength);

            byte[] magicCookie = BitConverter.GetBytes(StunMessage.MAGIC_COOKIE);
            Array.Reverse(magicCookie);

            byte[] result = new byte[computedType.Length + messageLength.Length + magicCookie.Length +
                                     message.TransactionID.Length + attributesBytes.Length];

            computedType.CopyTo(result, 0);
            messageLength.CopyTo(result, computedType.Length);
            magicCookie.CopyTo(result, computedType.Length + messageLength.Length);
            message.TransactionID.CopyTo(result, computedType.Length + messageLength.Length + magicCookie.Length);
            attributesBytes.CopyTo(result, computedType.Length + messageLength.Length + magicCookie.Length + message.TransactionID.Length);

            return result;
        }

        /// <summary>
        /// Converts a byte array to a StunMessage implicitly (no cast needed)
        /// </summary>
        /// <param name="message">byte array who's StunMessage representation we want</param>
        /// <returns>StunMessage version of the Byte array </returns>
        public static implicit operator StunMessage(byte[] message)
        {
            if (message.Length < 20)
                return null;

            UInt16 computedType = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(message, 0, 2), 0));

            UInt16 extractedMethodClass = (UInt16)(computedType >> 4 << 4); // Erase method type bits
            StunMethodClass methodClass = StunMessage.UInt16ToMethodClass(extractedMethodClass);

            UInt16 extractedMethodType = (UInt16)(computedType - extractedMethodClass);
            StunMethodType methodType = StunMessage.UInt16ToMethodType(extractedMethodType);

            UInt16 length = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(message, 2, 2), 0));

            byte[] transactionId = StunUtilities.SubArray(message, 8, 12);

            StunMessage result = new StunMessage(methodType, methodClass, transactionId);

            result.ImportAttributes(StunUtilities.SubArray(message, 20, length));

            return result;
        }

        /// <summary>
        /// Parses an array of bytes containing every attributes of a message and
        /// add them to managed and unmanaged attributes lists
        /// </summary>
        /// <param name="attributes">The array of byte which contains every attributes of a message</param>
        private void ImportAttributes(byte[] attributes)
        {
            Int32 offset = 0;
            Int32 attributesLength = attributes.Length;

            while (offset < attributesLength)
            {
                // We retrieve length and add it attribute header length (4 bytes)
                UInt16 valueLength = BitConverter.ToUInt16(StunUtilities.SubArray(attributes, offset + 2, 2), 0);
                UInt16 attributeLength = (UInt16)(StunUtilities.ReverseBytes(valueLength) + 4);

                // Adjust original length to a padded 32bit length
                if (attributeLength % 4 != 0)
                    attributeLength = (UInt16)(attributeLength + (4 - attributeLength % 4));

                StunAttribute attr = StunUtilities.SubArray(attributes, offset, attributeLength);

                // When doing auto conversion of a byte array, according to STUN RFC 5389 
                // only the first attribute of a given type must be taken into account
                this.SetAttribute(attr, false);

                offset += attributeLength;
            }
        }
        #endregion

        #region STATICS
        /// <summary>
        /// Convert a StunMethodType to an host-byte ordered unsigned short
        /// </summary>
        /// <param name="mType">The StunMethodType to convert</param>
        /// <returns>
        /// The unsigned short (16bits) matching the StunMethodType
        /// Returns max UInt16 value if type parameter is StunMethodType.Unmanaged
        /// </returns>
        public static UInt16 MethodTypeToUInt16(StunMethodType mType)
        {
            foreach (FieldInfo field in typeof(StunMethodType).GetFields())
            {
                if (field.Name == mType.ToString())
                {
                    Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                    if (fieldAttributes.Length == 1)
                    {
                        StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                        return stunValueAttribute.Value;
                    }
                }
            }
            return 0xFFFF;
        }

        /// <summary>
        /// Convert a StunMethodClass to an host-byte ordered unsigned short
        /// </summary>
        /// <param name="mClass">The StunMethodClass to convert</param>
        /// <returns>
        /// The unsigned short (16bits) matching the StunMethodClass
        /// Return max UInt16 value if the mClass parameter is StunMethodClass.Unmanaged
        /// </returns>
        public static UInt16 MethodClassToUInt16(StunMethodClass mClass)
        {
            foreach (FieldInfo field in typeof(StunMethodClass).GetFields())
            {
                if (field.Name == mClass.ToString())
                {
                    Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                    if (fieldAttributes.Length == 1)
                    {
                        StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                        return stunValueAttribute.Value;
                    }
                }
            }
            return 0xFFFF;
        }

        /// <summary>
        /// Convert an host-byte ordered unsigned short to a StunMethodType
        /// </summary>
        /// <param name="mType">An unsigned short representing a method type</param>
        /// <returns>
        /// The StunMethodType matching the unsigned short (16bits)
        /// Returns StunMethodType.Unmanaged if the unsigned short doesn't match any StunMethodType StunValue's
        /// </returns>
        public static StunMethodType UInt16ToMethodType(UInt16 mType)
        {
            foreach (FieldInfo field in typeof(StunMethodType).GetFields())
            {
                Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                if (fieldAttributes.Length == 1)
                {
                    StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                    if (stunValueAttribute != null &&
                        stunValueAttribute.Value == mType)
                    {
                        return (StunMethodType)Enum.Parse(typeof(StunMethodType), field.Name);
                    }
                }
            }
            return StunMethodType.Unmanaged;
        }

        /// <summary>
        /// Convert an host-byte ordered unsigned short to a StunMethodClass
        /// </summary>
        /// <param name="mClass">An unsigned short representing a method class</param>
        /// <returns>
        /// The StunMethodClass matching the unsigned short (16bits)
        /// Returns StunMethodClass.Unmanaged if the unsigned short doesn't match any StunMethodClass StunValue's
        /// </returns>
        public static StunMethodClass UInt16ToMethodClass(UInt16 mClass)
        {
            foreach (FieldInfo field in typeof(StunMethodClass).GetFields())
            {
                Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                if (fieldAttributes.Length == 1)
                {
                    StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                    if (stunValueAttribute != null &&
                        stunValueAttribute.Value == mClass)
                    {
                        return (StunMethodClass)Enum.Parse(typeof(StunMethodClass), field.Name);
                    }
                }
            }
            return StunMethodClass.Unmanaged;
        }
        #endregion
    }

    /// <summary>
    /// Enumeration of any STUN message types
    /// Each method type has a matching constant defined in STUN [RFC5389] and in TURN [RFC5766]
    /// </summary>
    public enum StunMethodType
    {
        /// <summary>
        /// Represents methods whose type is not managed by this library
        /// </summary>
        [StunValue(0xFFFF)]
        Unmanaged,

        #region STUN Core
        /// <summary>
        /// Binding requests are used to determine the bindings allocated by
        /// NATs. The client sends a Binding Request to the server, over UDP or TCP.
        /// The server examines the source IP address and port of the request,
        /// and copies them into a response that is sent back to the client
        /// </summary>
        [StunValue(0x0001)]
        Binding,
        #endregion

        #region TURN Extension
        /// <summary>
        /// Only request/response semantics defined
        /// All TURN operations revolve around allocations, and all TURN messages
        /// are associated with an allocation.  An allocation conceptually
        /// consists of the following state data:
        ///  *  the relayed transport address
        ///  *  the 5-tuple: (client's IP address, client's port, server IP
        ///     address, server port, transport protocol)
        ///  *  the authentication information
        ///  *  the time-to-expiry
        ///  *  a list of permissions
        ///  *  a list of channel to peer bindings.
        /// </summary>
        [StunValue(0x0003)]
        Allocate,
        /// <summary>
        /// Only request/response semantics defined
        /// A Refresh transaction can be used to either (a) refresh an existing
        /// allocation and update its time-to-expiry or (b) delete an existing allocation
        /// </summary>
        [StunValue(0x0004)]
        Refresh,
        /// <summary>
        /// Only indication semantics defined
        /// The client can use a Send indication to pass data to the server for
        /// relaying to a peer. A client may use a Send indication even if a
        /// channel is bound to that peer
        /// </summary>
        [StunValue(0x0006)]
        Send,
        /// <summary>
        /// Only indication semantics defined
        /// If relaying is permitted but no channel is bound to a peer, then
        /// the server forms and sends a Data indication
        /// </summary>
        [StunValue(0x0007)]
        Data,
        /// <summary>
        /// Only request/response semantics defined
        /// TURN supports two ways for the client to install or refresh
        /// permissions on the server
        /// </summary>
        [StunValue(0x0008)]
        CreatePermission,
        /// <summary>
        /// Only request/response semantics defined
        /// Channels provide a way for the client and server to send application
        /// data using ChannelData messages, which have less overhead than Send
        /// and Data indications.
        /// 
        /// A channel binding consists of:
        ///  * a channel number;
        ///  * a transport address (of the peer); and
        ///  * A time-to-expiry timer.
        /// </summary>
        [StunValue(0x0009)]
        ChannelBind,
        #endregion

        #region TURN-TCP Extension
        /// <summary>
        /// To initiate a TCP connection to a peer, a client MUST send a Connect
        /// request over the control connection for the desired allocation.  The
        /// Connect request MUST include an XOR-PEER-ADDRESS attribute containing
        /// the transport address of the peer to which a connection is desired.
        /// If the connection is successfully established, the client will
        /// receive a success response.  That response will contain a CONNECTION-ID attribute
        /// </summary>
        [StunValue(0x000A)]
        Connect,
        /// <summary>
        /// The client MUST initiate a new TCP connection to the server that MUST
        /// be made using a different local transport address.
        /// Authentication of the client by the server MUST use the same method
        /// and credentials as for the control connection.  Once established, the
        /// client MUST send a ConnectionBind request over the new connection.
        /// That request MUST include the CONNECTION-ID attribute, echoed from
        /// the Connect Success response
        /// </summary>
        [StunValue(0x000B)]
        ConnectionBind,
        /// <summary>
        /// After an Allocate request is successfully processed by the server,
        /// the client will start receiving a ConnectionAttempt indication each
        /// time a peer for which a permission has been installed attempts a new
        /// connection to the relayed transport address.  This indication will
        /// contain CONNECTION-ID and XOR-PEER-ADDRESS attributes
        /// </summary>
        [StunValue(0x000C)]
        ConnectionAttempt,
        #endregion

        #region STUN Classic
        /// <summary>
        /// Shared Secret Requests ask the server to return a temporary username and password.
        /// This username and password are used in a subsequent Binding Request and
        /// Binding Response, for the purposes of authentication and message integrity
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(0x0002)]
        SharedSecret
        #endregion
    }

    /// <summary>
    /// Enumeration of any STUN message classes
    /// Each method class has a matching constant defined in STUN [RFC5389] and in TURN [RFC5766]
    /// </summary>
    public enum StunMethodClass
    {
        /// <summary>
        /// Represents methods whose class is not managed by this library
        /// </summary>
        [StunValue(0xFFFF)]
        Unmanaged,

        #region STUN Core
        /// <summary>
        /// Request waiting for a response from peer STUN agent
        /// </summary>
        [StunValue(0x0000)]
        Request,
        /// <summary>
        /// Request not waiting for any response from peer STUN agent
        /// </summary>
        [StunValue(0x0010)]
        Indication,
        /// <summary>
        /// Request indicating a success response from peer STUN agent
        /// </summary>
        [StunValue(0x0100)]
        SuccessResponse,
        /// <summary>
        /// Request indicating an error from peer STUN agent
        /// </summary>
        [StunValue(0x0110)]
        Error
        #endregion
    }
}