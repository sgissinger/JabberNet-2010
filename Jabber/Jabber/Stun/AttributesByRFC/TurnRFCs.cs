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
    public class TurnRFCs
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private StunMessage boundMessage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a CHANNEL-NUMBER attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute ChannelNumber
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ChannelNumber);

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
        /// Contains a LIFETIME attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute LifeTime
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.LifeTime);

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
        /// Contains a XOR-PEER-ADDRESS attribute if this message contains one
        /// </summary>
        public XorMappedAddress XorPeerAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.XorPeerAddress);

                if (attribute != null)
                    return new XorMappedAddress(StunAttributeType.XorPeerAddress, attribute, this.boundMessage.TransactionID);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new XorMappedAddress(StunAttributeType.XorPeerAddress, value, this.boundMessage.TransactionID));
            }
        }
        /// <summary>
        /// Contains a DATA attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute Data
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Data);

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
        /// Contains a XOR-RELAYED-ADDRESS attribute if this message contains one
        /// </summary>
        public XorMappedAddress XorRelayedAddress
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.XorRelayedAddress);

                if (attribute != null)
                    return new XorMappedAddress(StunAttributeType.XorRelayedAddress, attribute, this.boundMessage.TransactionID);

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(new XorMappedAddress(StunAttributeType.XorRelayedAddress, value, this.boundMessage.TransactionID));
            }
        }
        /// <summary>
        /// Contains a EVEN-PORT attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute EvenPort
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.EvenPort);

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
        /// Contains a REQUESTED-TRANSPORT attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute RequestedTransport
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.RequestedTransport);

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
        /// Contains a DONT-FRAGMENT attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute DontFragment
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.DontFragment);

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
        /// Contains a RESERVATION-TOKEN attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute ReservationToken
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ReservationToken);

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
        /// Contains a CONNECTION-ID attribute if this message contains one, otherwise returns null
        /// Used by TURN-TCP [RFC6062]
        /// </summary>
        public StunAttribute ConnectionId
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.ConnectionId);

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
        /// Contains a REQUESTED-ADDRESS-FAMILY attribute if this message contains one, otherwise returns null
        /// Used by TURN-IPV6 [RFC6156]
        /// </summary>
        public StunAttribute RequestedAddressFamily
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.RequestedAddressFamily);

                if (attribute != null)
                    return attribute;

                return null;
            }
            set
            {
                this.boundMessage.SetAttribute(value);
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// TODO: Documentation Constructor
        /// </summary>
        /// <param name="message"></param>
        public TurnRFCs(StunMessage message)
        {
            this.boundMessage = message;
        }

        /// <summary>
        /// String summary of this attribute
        /// </summary>
        /// <returns>A String with informations about this attribute</returns>
        public override String ToString()
        {
            return "RFC5766, RFC6062, RFC6156";
        }
        #endregion
    }
}
