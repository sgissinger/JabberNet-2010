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
    public class StunClassicRFC
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
        /// Contains a RESPONSE-ADDRESS attribute if this message contains one
        /// </summary>
        public MappedAddress ResponseAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ResponseAddress);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.ResponseAddress, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.ResponseAddress, value));
            }
        }
        /// <summary>
        /// Contains a CHANGED-ADDRESS attribute if this message contains one
        /// </summary>
        public MappedAddress ChangedAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ChangedAddress);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.ChangedAddress, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.ChangedAddress, value));
            }
        }
        /// <summary>
        /// Contains a CHANGE-REQUEST attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute ChangeRequest
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ChangeRequest);

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
        /// Contains a SOURCE-ADDRESS attribute if this message contains one
        /// </summary>
        public MappedAddress SourceAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.SourceAddress);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.SourceAddress, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.SourceAddress, value));
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
        /// Contains a PASSWORD attribute if this message contains one, otherwise returns null
        /// </summary>
        public UTF8Attribute Password
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Password);

                if (attribute != null)
                    return new UTF8Attribute(StunAttributeType.Password, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new UTF8Attribute(StunAttributeType.Password, value));
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
        /// Contains a REFLECTED-FROM attribute if this message contains one
        /// </summary>
        public MappedAddress ReflectedFrom
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ReflectedFrom);

                if (attribute != null)
                    return new MappedAddress(StunAttributeType.ReflectedFrom, attribute);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new MappedAddress(StunAttributeType.ReflectedFrom, value));
            }
        }
        /// <summary>
        /// Contains a XOR-MAPPED-ADDRESS attribute if this message contains one
        /// </summary>
        public XorMappedAddress XorMappedAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.XorMappedAddressAlt);

                if (attribute != null)
                    return new XorMappedAddress(StunAttributeType.XorMappedAddressAlt, attribute, this.boundMessage.TransactionID);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new XorMappedAddress(StunAttributeType.XorMappedAddressAlt, value, this.boundMessage.TransactionID));
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="message"></param>
        public StunClassicRFC(StunMessage message)
        {
            this.boundMessage = message;
        }

        /// <summary>
        /// String summary of this attribute
        /// </summary>
        /// <returns>A String with informations about this attribute</returns>
        public override String ToString()
        {
            return "RFC3489";
        }
        #endregion
    }
}