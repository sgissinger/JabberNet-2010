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

namespace Jabber.Stun
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class StunMessage
    {
        #region CONSTANTS
        // STUN Core
        private const UInt16 METHOD_BINDING = 0x0001;

        // TURN Extension
        private const UInt16 METHOD_ALLOCATE = 0x0003;
        private const UInt16 METHOD_REFRESH = 0x0004;
        private const UInt16 METHOD_SEND = 0x0006;
        private const UInt16 METHOD_DATA = 0x0007;
        private const UInt16 METHOD_CREATE_PERMISSION = 0x0008;
        private const UInt16 METHOD_CHANNEL_BIND = 0x0009;

        // STUN Core
        private const UInt16 CLASS_REQUEST = 0x0000;
        private const UInt16 CLASS_INDICATION = 0x0010;
        private const UInt16 CLASS_SUCCESS_RESPONSE = 0x0100;
        private const UInt16 CLASS_ERROR = 0x0110;

        private const UInt32 MAGIC_COOKIE = 0x2112A442;
        #endregion

        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private Dictionary<StunAttributeType, StunAttribute> attributes = new Dictionary<StunAttributeType, StunAttribute>();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunMethodType MethodType { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunMethodClass MethodClass { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public UInt16 Length
        {
            get { return (UInt16)StunUtilities.GetBytes(this.attributes).Length; }
        }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public byte[] TransactionID { get; private set; }
        /// <summary>
        /// TODO: Documentation Property
        /// </summary>
        public StunAttribute[] Attributes
        {
            get 
            {
                StunAttribute[] result = new StunAttribute[this.attributes.Count];

                this.attributes.Values.CopyTo(result, 0);

                return result; 
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mclass"></param>
        /// <param name="transactionId"></param>
        public StunMessage(StunMethodType type, StunMethodClass mclass, byte[] transactionId)
        {
            this.MethodType = type;
            this.MethodClass = mclass;
            this.TransactionID = transactionId;
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
        /// TODO: Documentation SetAttribute
        /// </summary>
        /// <param name="attribute"></param>
        public void SetAttribute(StunAttribute attribute)
        {
            this.SetAttribute(attribute, true);
        }

        /// <summary>
        /// TODO: Documentation SetAttribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="replaceExistingAttribute"></param>
        public void SetAttribute(StunAttribute attribute, Boolean replaceExistingAttribute)
        {
            if (!this.attributes.ContainsKey(attribute.Type))
                this.attributes.Add(attribute.Type, attribute);
            else
                if (replaceExistingAttribute)
                    this.attributes[attribute.Type] = attribute;
        }

        /// <summary>
        /// TODO: Documentation GetAttribute
        /// </summary>
        /// <param name="type"></param>
        public StunAttribute GetAttribute(StunAttributeType type)
        {
            return this.attributes.ContainsKey(type) ? this.attributes[type] : null;
        }

        /// <summary>
        /// TODO: Documentation ClearAttributes
        /// </summary>
        public void ClearAttributes()
        {
            this.attributes.Clear();
        }

        /// <summary>
        /// Converts a StunMessage to a byte array implicitly (no cast needed).
        /// </summary>
        /// <param name="message">StunMessage who's byte array representation we want</param>
        /// <returns>Byte array version of the StunMessage</returns>
        public static implicit operator byte[](StunMessage message)
        {
            if (message == null)
                return null;

            UInt16 methodType;

            switch (message.MethodType)
            {
                case StunMethodType.Binding:
                default:
                    methodType = StunMessage.METHOD_BINDING;
                    break;

                case StunMethodType.Allocate:
                    methodType = StunMessage.METHOD_ALLOCATE;
                    break;

                case StunMethodType.Refresh:
                    methodType = StunMessage.METHOD_REFRESH;
                    break;

                case StunMethodType.Send:
                    methodType = StunMessage.METHOD_SEND;
                    break;

                case StunMethodType.Data:
                    methodType = StunMessage.METHOD_DATA;
                    break;

                case StunMethodType.CreatePermission:
                    methodType = StunMessage.METHOD_CREATE_PERMISSION;
                    break;

                case StunMethodType.ChannelBind:
                    methodType = StunMessage.METHOD_CHANNEL_BIND;
                    break;
            }

            UInt16 methodClass;

            switch (message.MethodClass)
            {
                case StunMethodClass.Request:
                default:
                    methodClass = StunMessage.CLASS_REQUEST;
                    break;

                case StunMethodClass.Indication:
                    methodClass = StunMessage.CLASS_INDICATION;
                    break;

                case StunMethodClass.SuccessResponse:
                    methodClass = StunMessage.CLASS_SUCCESS_RESPONSE;
                    break;

                case StunMethodClass.Error:
                    methodClass = StunMessage.CLASS_ERROR;
                    break;
            }

            byte[] computedType = BitConverter.GetBytes(((UInt16)(methodType | methodClass)));
            Array.Reverse(computedType);

            byte[] messageLength = BitConverter.GetBytes((UInt16)StunUtilities.GetBytes(message.attributes).Length);
            Array.Reverse(messageLength); 

            byte[] magicCookie = BitConverter.GetBytes(StunMessage.MAGIC_COOKIE);
            Array.Reverse(magicCookie);

            byte[] result = new byte[computedType.Length + messageLength.Length + magicCookie.Length +
                                     message.TransactionID.Length + StunUtilities.GetBytes(message.attributes).Length];

            computedType.CopyTo(result, 0);
            messageLength.CopyTo(result, computedType.Length);
            magicCookie.CopyTo(result, computedType.Length + messageLength.Length);
            message.TransactionID.CopyTo(result, computedType.Length + messageLength.Length + magicCookie.Length);
            StunUtilities.GetBytes(message.attributes).CopyTo(result, computedType.Length + messageLength.Length + magicCookie.Length + message.TransactionID.Length);

            return result;
        }

        /// <summary>
        /// Converts a byte array to a StunMessage implicitly (no cast needed).
        /// </summary>
        /// <param name="message">byte array who's StunMessage representation we want</param>
        /// <returns>StunMessage version of the Byte array </returns>
        public static implicit operator StunMessage(byte[] message)
        {
            if (message.Length < 20)
                return null;

            StunMethodClass methodClass;

            UInt16 computedType = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(message, 0, 2), 0));
            UInt16 extractedMethodClass = (UInt16)(computedType >> 4 << 4); // Erase method type bits

            switch (extractedMethodClass)
            {
                case StunMessage.CLASS_REQUEST:
                default:
                    methodClass = StunMethodClass.Request;
                    break;

                case StunMessage.CLASS_INDICATION:
                    methodClass = StunMethodClass.Indication;
                    break;

                case StunMessage.CLASS_SUCCESS_RESPONSE:
                    methodClass = StunMethodClass.SuccessResponse;
                    break;

                case StunMessage.CLASS_ERROR:
                    methodClass = StunMethodClass.Error;
                    break;
            }

            StunMethodType methodType;

            UInt16 extractedMethodType = (UInt16)(computedType - extractedMethodClass);

            switch (extractedMethodType)
            {
                case StunMessage.METHOD_BINDING:
                default:
                    methodType = StunMethodType.Binding;
                    break;

                case StunMessage.METHOD_ALLOCATE:
                    methodType = StunMethodType.Allocate;
                    break;

                case StunMessage.METHOD_REFRESH:
                    methodType = StunMethodType.Refresh;
                    break;

                case StunMessage.METHOD_SEND:
                    methodType = StunMethodType.Send;
                    break;

                case StunMessage.METHOD_DATA:
                    methodType = StunMethodType.Data;
                    break;

                case StunMessage.METHOD_CREATE_PERMISSION:
                    methodType = StunMethodType.CreatePermission;
                    break;

                case StunMessage.METHOD_CHANNEL_BIND:
                    methodType = StunMethodType.ChannelBind;
                    break;
            }

            UInt16 length = StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(message,2, 2), 0));

            byte[] transactionId = StunUtilities.SubArray(message, 8, 12);
            
            StunMessage result = new StunMessage(methodType, methodClass, transactionId);

            result.ImportAttributes(StunUtilities.SubArray(message, 20, length));

            return result;
        }

        /// <summary>
        /// TODO: Documentation ImportAttributes
        /// </summary>
        /// <param name="attributes"></param>
        private void ImportAttributes(byte[] attributes)
        {
            Int32 offset = 0;
            Int32 fullLength = attributes.Length;

            while (offset < fullLength)
            {
                UInt16 length = (UInt16)(StunUtilities.ReverseBytes(BitConverter.ToUInt16(StunUtilities.SubArray(attributes, offset + 2, 2), 0)) + 4); // Attribute header is 4 bytes long

                if (length % 4 != 0)
                    length = (UInt16)(length + (4 - length % 4)); // Adjust original length to a padded 32bit length

                StunAttribute attr = StunUtilities.SubArray(attributes, offset, length);

                // When doing auto conversion of a byte array, according to STUN RFC 5389 
                // only the first attribute of a given type must be taken into account
                this.SetAttribute(attr, false);

                offset += length;
            }
        }
        #endregion
    }

    /// <summary>
    /// TODO: Documentation Enum
    /// </summary>
    public enum StunMethodType
    {
        // STUN Core
        Binding,
        // TURN EXtension
        Allocate,
        Refresh,
        Send,
        Data,
        CreatePermission,
        ChannelBind
    }

    /// <summary>
    /// TODO: Documentation Enum
    /// </summary>
    public enum StunMethodClass
    {
        Request,
        Indication,
        SuccessResponse,
        Error
    }
}