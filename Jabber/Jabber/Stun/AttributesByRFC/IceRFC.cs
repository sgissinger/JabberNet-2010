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

namespace Jabber.Stun.AttributesByRFC
{
    /// <summary>
    /// TODO: Documentation Class
    /// </summary>
    public class IceRFC
    {
        #region MEMBERS
        /// <summary>
        /// TODO: Documentation Member
        /// </summary>
        private StunMessage boundMessage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a PRIORITY attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute Priority
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.Priority);

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
        /// Contains a USE-CANDIDATE attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute UseCandidate
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.UseCandidate);

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
        /// Contains a ICE-CONTROLLED attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute IceControlled
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.IceControlled);

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
        /// Contains a ICE-CONTROLLING attribute if this message contains one, otherwise returns null
        /// </summary>
        public StunAttribute IceControlling
        {
            get
            {
                StunAttribute attribute = this.boundMessage.GetAttribute(StunAttributeType.IceControlling);

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
        public IceRFC(StunMessage message)
        {
            this.boundMessage = message;
        }

        /// <summary>
        /// String summary of this attribute
        /// </summary>
        /// <returns>A String with informations about this attribute</returns>
        public override String ToString()
        {
            return "RFC5245";
        }
        #endregion
    }
}