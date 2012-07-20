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
using Jabber.Stun.Attributes;

namespace Jabber.Stun.AttributesByRFC
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class StunRFC
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private StunMessage boundMessage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a MAPPED-ADDRESS attribute if this message contains one
        /// </summary>
        public MappedAddress MappedAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.MappedAddress);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.MappedAddress, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.MappedAddress, value));
            }
        }
        /// <summary>
        /// Contains a XOR-MAPPED-ADDRESS attribute if this message contains one
        /// </summary>
        public XorMappedAddress XorMappedAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.XorMappedAddress);

                if (attribute != null)
                    return new XorMappedAddress(StunAttributeType.XorMappedAddress, attribute, this.boundMessage.TransactionID);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new XorMappedAddress(StunAttributeType.XorMappedAddress, value, this.boundMessage.TransactionID));
            }
        }
        /// <summary>
        /// Contains a USERNAME attribute if this message contains one, otherwise returns null
        /// </summary>
        public UTF8Attribute Username
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Username);

                if (attribute != null)
                    return new UTF8Attribute(StunAttributeType.Username, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new UTF8Attribute(StunAttributeType.Username, value));
            }
        }
        /// <summary>
        /// Contains a MESSAGE-INTEGRITY attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute MessageIntegrity
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.MessageIntegrity);

                if (attribute != null)
                    return attribute;

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(value);
            }
        }
        /// <summary>
        /// Contains a FINGERPRINT attribute if this message contains one, otherwise returns null
        /// </summary>
        public HexAttribute FingerPrint
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.FingerPrint);

                if (attribute != null)
                    return new HexAttribute(StunAttributeType.FingerPrint, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new HexAttribute(StunAttributeType.FingerPrint, value));
            }
        }
        /// <summary>
        /// Contains an ERROR-CODE attribute if this message contains one
        /// </summary>
        public ErrorCode ErrorCode
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ErrorCode);

                if (attribute != null)
                    return new ErrorCode(attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(value);
            }
        }
        /// <summary>
        /// Contains a REALM attribute if this message contains one, otherwise returns null
        /// </summary>
        public UTF8Attribute Realm
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Realm);

                if (attribute != null)
                    return new UTF8Attribute(StunAttributeType.Realm, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new UTF8Attribute(StunAttributeType.Realm, value));
            }
        }
        /// <summary>
        /// Contains a NONCE attribute if this message contains one, otherwise returns null
        /// </summary>
        public UTF8Attribute Nonce
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Nonce);

                if (attribute != null)
                    return new UTF8Attribute(StunAttributeType.Nonce, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new UTF8Attribute(StunAttributeType.Nonce, value));
            }
        }
        /// <summary>
        /// Contains an UNKNOWN-ATTRIBUTES attribute if this message contains one, otherwise returns null
        /// </summary>
        public UnknownAttributes UnknownAttributes
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.UnknownAttributes);

                if (attribute != null)
                    return new UnknownAttributes(attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(value);
            }
        }
        /// <summary>
        /// Contains a SOFTWARE attribute if this message contains one, otherwise returns null
        /// </summary>
        public UTF8Attribute Software
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Software);

                if (attribute != null)
                    return new UTF8Attribute(StunAttributeType.Software, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new UTF8Attribute(StunAttributeType.Software, value));
            }
        }
        /// <summary>
        /// Contains an ALTERNATE-SERVER attribute if this message contains one, otherwise returns null
        /// </summary>
        public MappedAddress AlternateServer
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.AlternateServer);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.AlternateServer, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.AlternateServer, value));
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="message"></param>
        public StunRFC(StunMessage message)
        {
            this.boundMessage = message;
        }

        /// <summary>
        /// String summary of this attribute
        /// </summary>
        /// <returns>A String with informations about this attribute</returns>
        public override String ToString()
        {
            return "RFC5389";
        }
        #endregion
    }
}