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
using Jabber.Stun.Attributes;
using System.Reflection;

namespace Jabber.Stun
{
    /// <summary>
    /// Represents a message according to STUN [RFC5389], TURN [RFC5766] and STUN Classic [RFC3489]
    /// </summary>
    public class StunMessage
    {
        #region CONSTANTS
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
        /// Contains the list of every known attributes which have a StunAttributeType
        /// matching one of the StunAttribute constants
        /// </summary>
        private Dictionary<StunAttributeType, StunAttribute> attributesList = new Dictionary<StunAttributeType, StunAttribute>();
        /// <summary>
        /// Contains the list of every unknown attributes which haven't any StunAttributeType
        /// matching one of the StunAttribute constants
        /// </summary>
        private List<StunAttribute> unknownAttributesList = new List<StunAttribute>();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a copy of the list of every known attributes which have a StunAttributeType
        /// matching one of the StunAttribute constants
        /// </summary>
        public StunAttribute[] AttributesList
        {
            get
            {
                StunAttribute[] attrs = new StunAttribute[this.attributesList.Count];

                this.attributesList.Values.CopyTo(attrs, 0);

                return attrs;
            }
        }
        /// <summary>
        /// Contains a copy of the list of every unknown attributes which haven't any StunAttributeType
        /// matching one of the StunAttribute constants
        /// </summary>
        public StunAttribute[] UnknownAttributesList
        {
            get
            {
                StunAttribute[] attrs = new StunAttribute[this.unknownAttributesList.Count];

                this.unknownAttributesList.CopyTo(attrs, 0);

                return attrs;
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
                return (UInt16)(StunUtilities.GetBytes(this.AttributesList).Length + StunUtilities.GetBytes(this.UnknownAttributesList).Length);
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
                    return new XorMappedAddress(attribute, this.TransactionID);
                }
                else
                {
                    attribute = this.GetAttribute(StunAttributeType.XorMappedAddressAlt);

                    if (attribute != null)
                    {
                        return new XorMappedAddressAlt(attribute, this.TransactionID);
                    }
                    else
                    {
                        attribute = this.GetAttribute(StunAttributeType.MappedAddress);

                        if (attribute != null)
                            return new MappedAddress(attribute);
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Contains an ERROR-CODE attribute if this message contains one
        /// </summary>
        public ErrorCode Error
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.ErrorCode);

                if (attribute != null)
                    return new ErrorCode(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains an ALTERNATE-SERVER attribute if this message contains one, otherwise returns null
        /// </summary>
        public MappedAddress AlternateServer
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.AlternateServer);

                if (attribute != null)
                    return new AlternateServer(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains an UNKNOWN-ATTRIBUTES attribute if this message contains one, otherwise returns null
        /// </summary>
        public UnknownAttributes UnknownAttributes
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.UnknownAttributes);

                if (attribute != null)
                    return new UnknownAttributes(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains a USERNAME attribute if this message contains one, otherwise returns null
        /// </summary>
        public Username Username
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.Username);

                if (attribute != null)
                    return new Username(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains a REALM attribute if this message contains one, otherwise returns null
        /// </summary>
        public Realm Realm
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.Realm);

                if (attribute != null)
                    return new Realm(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains a SOFTWARE attribute if this message contains one, otherwise returns null
        /// </summary>
        public Software Software
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.Software);

                if (attribute != null)
                    return new Software(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains a NONCE attribute if this message contains one, otherwise returns null
        /// </summary>
        public Nonce Nonce 
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.Nonce);

                if (attribute != null)
                    return new Nonce(attribute);

                return null;
            }
        }
        /// <summary>
        /// Contains a FINGERPRINT attribute if this message contains one, otherwise returns null
        /// </summary>
        public FingerPrint FingerPrint
        {
            get
            {
                StunAttribute attribute = this.GetAttribute(StunAttributeType.FingerPrint);

                if (attribute != null)
                    return new FingerPrint(attribute);

                return null;
            }
        }
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
        /// Helper method to auto-cast this message to an array of bytes
        /// </summary>
        /// <returns>The array of bytes representation of this message</returns>
        public byte[] GetBytes()
        {
            return this;
        }

        /// <summary>
        /// Add an attribute to this message
        /// Any existing attribute of the same type will be replaced by the attribute in parameter
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        public void SetAttribute(StunAttribute attribute)
        {
            this.SetAttribute(attribute, true);
        }

        /// <summary>
        /// Add an attribute to this message
        /// If the attribute is not known, it will be added to the UnknownAttributes list
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        /// <param name="replaceExistingAttribute">
        /// If TRUE, any existing attribute of the same type as the attribute parameter
        /// will be replaced by the attribute in parameter
        /// </param>
        public void SetAttribute(StunAttribute attribute, Boolean replaceExistingAttribute)
        {
            if (attribute.Type == StunAttributeType.Unknown)
            {
                this.unknownAttributesList.Add(attribute);
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
        /// Empty every known and unknown attributes in this message
        /// </summary>
        public void ClearAttributes()
        {
            this.attributesList.Clear();
            this.unknownAttributesList.Clear();
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
        /// Parses an array of bytes containing every attributes of a message and add
        /// them to known and unknown attributes lists
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
        /// Returns 0 if type parameter is StunMethodType.Unknown
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
        /// Return 0 if the mClass parameter is StunMethodClass.Unknown
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
        /// Returns StunMethodType.Unknown if the unsigned short doesn't match any constants
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
            return StunMethodType.Unknown;
        }

        /// <summary>
        /// Convert an host-byte ordered unsigned short to a StunMethodClass
        /// </summary>
        /// <param name="mClass">An unsigned short representing a method class</param>
        /// <returns>
        /// The StunMethodClass matching the unsigned short (16bits)
        /// Returns StunMethodClass.Unknown if the unsigned short doesn't match any constants
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
            return StunMethodClass.Unknown;
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
        /// Miscellaneous method types unknown to this library
        /// </summary>
        Unknown,

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
        /// Miscellaneous method classes unknown to this library
        /// </summary>
        Unknown,

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