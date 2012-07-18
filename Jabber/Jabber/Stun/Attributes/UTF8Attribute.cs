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

namespace Jabber.Stun.Attributes
{
    /// <summary>
    /// Represents a STUN attribute whose value is an UTF8 string
    /// </summary>
    public abstract class UTF8Attribute: StunAttribute
    {
        #region PROPERTIES
        /// <summary>
        /// Contains the UTF8 decoded string of this attribute's value
        /// </summary>
        public String ValueString
        {
            get { return StunAttribute.Encoder.GetString(this.Value); }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an UTF8Attribute based on an existing StunAttribute
        /// </summary>
        /// <param name="type">The StunAttributeType associated with the attribute parameter</param>
        /// <param name="attribute">The StunAttribute base for this UTF8Attribute</param>
        public UTF8Attribute(StunAttributeType type, StunAttribute attribute)
            : base(type, attribute.Value)
        { }
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
                                 this.ValueString);
        }
        #endregion
    }

    /// <summary>
    /// The USERNAME attribute is used for message integrity. It identifies the username and password
    /// combination used in the message-integrity check
    /// The value of USERNAME is a variable-length value. It MUST contain a UTF-8 [RFC3629] encoded
    /// sequence of less than 513 bytes, and MUST have been processed using SASLprep [RFC4013]
    /// </summary>
    public class Username : UTF8Attribute
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an Username based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this Username</param>
        public Username(StunAttribute attribute)
            : base(StunAttributeType.Username, attribute)
        {
            if (attribute.Type != StunAttributeType.Username)
                throw new ArgumentException("is not a valid USERNAME attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// The REALM attribute may be present in requests and responses. It contains text that
    /// meets the grammar for "realm-value" as described in [RFC3261] but without
    /// the double quotes and their surrounding whitespace
    /// </summary>
    public class Realm : UTF8Attribute
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an Realm based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this Realm</param>
        public Realm(StunAttribute attribute)
            : base(StunAttributeType.Realm, attribute)
        {
            if (attribute.Type != StunAttributeType.Realm)
                throw new ArgumentException("is not a valid REALM attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// The SOFTWARE attribute contains a textual description of the software being used
    /// by the agent sending the message. It is used by clients and servers.
    /// Its value SHOULD include manufacturer and version number.
    /// The attribute has no impact on operation of the protocol,
    /// and serves only as a tool for diagnostic and debugging purposes
    /// </summary>
    public class Software : UTF8Attribute
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an Software based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this Software</param>
        public Software(StunAttribute attribute)
            : base(StunAttributeType.Software, attribute)
        {
            if (attribute.Type != StunAttributeType.Software)
                throw new ArgumentException("is not a valid SOFTWARE attribute", "attribute");
        }
        #endregion
    }

    /// <summary>
    /// The NONCE attribute may be present in requests and responses. It contains a sequence
    /// of qdtext or quoted-pair, which are defined in [RFC3261].
    /// Note that this means that the NONCE attribute will not contain actual quote characters
    /// </summary>
    public class Nonce : UTF8Attribute
    {
        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an Nonce based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this Nonce</param>
        public Nonce(StunAttribute attribute)
            : base(StunAttributeType.Nonce, attribute)
        {
            if (attribute.Type != StunAttributeType.Nonce)
                throw new ArgumentException("is not a valid NONCE attribute", "attribute");
        }
        #endregion
    }
}
