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
using System.Text;
using System.Globalization;

namespace Jabber.Stun
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class StunAttribute
    {
        #region CONSTANTS
        // STUN Core required
        private const UInt16 MAPPED_ADDRESS = 0x0001;
        private const UInt16 USERNAME = 0x0006;
        private const UInt16 MESSAGE_INTEGRITY = 0x0008;
        private const UInt16 ERROR_CODE = 0x0009;
        private const UInt16 UNKNOWN_ATTRIBUTE = 0x000A;
        private const UInt16 REALM = 0x0014;
        private const UInt16 NONCE = 0x0015;
        private const UInt16 XOR_MAPPED_ADDRESS = 0x0020;
        // STUN Core optional
        private const UInt16 SOFTWARE = 0x8022;
        private const UInt16 ALTERNATE_SERVER = 0x8023;
        private const UInt16 FINGERPRINT = 0x8028;
        // TURN Extension
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

        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        public static Encoding Encoder = new UTF8Encoding();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunAttributeType Type { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public UInt16 Length { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public byte[] Value { get; private set; }
        #endregion

        #region CONSTRUCTOR & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public StunAttribute(StunAttributeType type, String value)
            : this(type, StunAttribute.Encoder.GetBytes(value))
        { }

        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public StunAttribute(StunAttributeType type, byte[] value)
        {
            this.Type = type;
            this.Value = value;

            this.Length = (UInt16)value.Length;
        }

        /// <summary>
        /// TODO: Documentation ToString
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format(CultureInfo.CurrentCulture,
                                 "{0}, {1}",
                                 this.Type,
                                 StunAttribute.Encoder.GetString(this.Value));
        }
        #endregion

        #region METHODS
        /// <summary>
        /// TODO: Documentation GetBytes
        /// </summary>
        /// <returns></returns>
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

            byte[] type;

            switch (attribute.Type)
            {
                // STUN Core required
                case StunAttributeType.MappedAddress:
                    type = BitConverter.GetBytes(StunAttribute.MAPPED_ADDRESS);
                    break;

                case StunAttributeType.Username:
                    type = BitConverter.GetBytes(StunAttribute.USERNAME);
                    break;

                case StunAttributeType.MessageIntegrity:
                    type = BitConverter.GetBytes(StunAttribute.MESSAGE_INTEGRITY);
                    break;

                case StunAttributeType.ErrorCode:
                    type = BitConverter.GetBytes(StunAttribute.ERROR_CODE);
                    break;

                case StunAttributeType.UnknownAttribute:
                default:
                    type = BitConverter.GetBytes(StunAttribute.UNKNOWN_ATTRIBUTE);
                    break;

                case StunAttributeType.Realm:
                    type = BitConverter.GetBytes(StunAttribute.REALM);
                    break;

                case StunAttributeType.Nonce:
                    type = BitConverter.GetBytes(StunAttribute.NONCE);
                    break;

                case StunAttributeType.XorMappedAddress:
                    type = BitConverter.GetBytes(StunAttribute.XOR_MAPPED_ADDRESS);
                    break;

                // STUN Core optional
                case StunAttributeType.Software:
                    type = BitConverter.GetBytes(StunAttribute.SOFTWARE);
                    break;

                case StunAttributeType.AlternateServer:
                    type = BitConverter.GetBytes(StunAttribute.ALTERNATE_SERVER);
                    break;

                case StunAttributeType.FingerPrint:
                    type = BitConverter.GetBytes(StunAttribute.FINGERPRINT);
                    break;

                // TURN Extension
                case StunAttributeType.ChannelNumber:
                    type = BitConverter.GetBytes(StunAttribute.CHANNEL_NUMBER);
                    break;

                case StunAttributeType.LifeTime:
                    type = BitConverter.GetBytes(StunAttribute.LIFETIME);
                    break;

                case StunAttributeType.XorPeerAddress:
                    type = BitConverter.GetBytes(StunAttribute.XOR_PEER_ADDRESS);
                    break;

                case StunAttributeType.Data:
                    type = BitConverter.GetBytes(StunAttribute.DATA);
                    break;

                case StunAttributeType.XorRelayedAddress:
                    type = BitConverter.GetBytes(StunAttribute.XOR_RELAYED_ADDRESS);
                    break;

                case StunAttributeType.EvenPort:
                    type = BitConverter.GetBytes(StunAttribute.EVEN_PORT);
                    break;

                case StunAttributeType.RequestedTransport:
                    type = BitConverter.GetBytes(StunAttribute.REQUESTED_TRANSPORT);
                    break;

                case StunAttributeType.DontFragment:
                    type = BitConverter.GetBytes(StunAttribute.DONT_FRAGMENT);
                    break;

                case StunAttributeType.ReservationToken:
                    type = BitConverter.GetBytes(StunAttribute.RESERVATION_TOKEN);
                    break;
            }
            Array.Reverse(type);

            byte[] length = BitConverter.GetBytes(attribute.Length);
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

            StunAttributeType stunType;

            UInt16 type = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(attribute, 0, 2), 0));

            switch (type)
            {
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

                case StunAttribute.UNKNOWN_ATTRIBUTE:
                default:
                    stunType = StunAttributeType.UnknownAttribute;
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
            }

            UInt16 length = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(attribute, 2, 2), 0));

            byte[] value = StunUtilities.SubArray(attribute, 4, length);

            return new StunAttribute(stunType, value);
        }
        #endregion
    }

    /// <summary>
    /// TODO: Documentation Enum
    /// </summary>
    public enum StunAttributeType
    {
        // STUN Core required
        MappedAddress,
        Username,
        MessageIntegrity,
        ErrorCode,
        UnknownAttribute,
        Realm,
        Nonce,
        XorMappedAddress,
        // STUN Core optional
        Software,
        AlternateServer,
        FingerPrint,
        // TURN Extension
        ChannelNumber,
        LifeTime,
        XorPeerAddress,
        Data,
        XorRelayedAddress,
        EvenPort,
        RequestedTransport,
        DontFragment,
        ReservationToken
    }
}
