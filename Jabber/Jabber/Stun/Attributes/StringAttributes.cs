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
    public class UTF8Attribute : StunAttribute
    {
        #region PROPERTIES
        /// <summary>
        /// Contains the UTF8 decoded string of this attribute's value
        /// </summary>
        public String ValueString
        {
            get { return StunMessage.Encoder.GetString(this.Value); }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a new UTF8Attribute
        /// </summary>
        /// <param name="type">The type of this UTF8Attribute</param>
        /// <param name="value">The value of this UTF8Attribute</param>
        public UTF8Attribute(StunAttributeType type, String value)
            : base(type, StunMessage.Encoder.GetBytes(value))
        { }

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
    /// Represents a STUN attribute whose value is better understood with an hexadeciaml string
    /// </summary>
    public class HexAttribute : StunAttribute
    {
        #region PROPERTIES
        /// <summary>
        /// Contains the hexadecimal representation, in network-byte order, of this attribute's value
        /// </summary>
        public String ValueHex
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture,
                                     "{0:X4}",
                                     StunUtilities.ReverseBytes(BitConverter.ToUInt32(this.Value, 0)));
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs an HexAttribute based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this HexAttribute</param>
        public HexAttribute(StunAttributeType type, StunAttribute attribute)
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
                                 this.ValueHex);
        }
        #endregion
    }
}