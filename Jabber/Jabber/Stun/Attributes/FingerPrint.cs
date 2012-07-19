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
    /// The FINGERPRINT attribute MAY be present in all STUN messages. The value of the
    /// attribute is computed as the CRC-32 of the STUN message up to (but excluding)
    /// the FINGERPRINT attribute itself, XOR'ed with the 32-bit value 0x5354554e
    /// </summary>
    public class FingerPrint : StunAttribute
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
        /// Constructs an FingerPrint based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this FingerPrint</param>
        public FingerPrint(StunAttribute attribute)
            : base(StunAttributeType.FingerPrint, attribute.Value)
        {
            if (attribute.Type != StunAttributeType.FingerPrint)
                throw new ArgumentException("is not a valid FINGERPRINT attribute", "attribute");
        }
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