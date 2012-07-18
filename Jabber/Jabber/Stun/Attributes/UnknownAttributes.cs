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
using System.Text;
using System.Globalization;

namespace Jabber.Stun.Attributes
{
    /// <summary>
    /// The UNKNOWN-ATTRIBUTES attribute is present only in an error response
    /// when the response code in the ERROR-CODE attribute is 420.
    /// </summary>
    public class UnknownAttributes : StunAttribute
    {
        #region MEMBERS
        /// <summary>
        /// Contains every UNKNOWN-ATTRIBUTE values in network-byte order
        /// </summary>
        private List<UInt16> attributes = new List<UInt16>();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Contains a copy of the hexadecimal representation of every UNKNOWN-ATTRIBUTE values in network-byte order
        /// </summary>
        public String[] AttributesHex
        {
            get
            {
                String[] result = new String[this.attributes.Count];
                Int32 i = 0;

                foreach (UInt16 attributeValue in this.attributes)
                {
                    result[i] = String.Format(CultureInfo.CurrentCulture,
                                              "{0:X4}",
                                              attributeValue);
                    i++;
                }

                return result;
            }
        }
        /// <summary>
        /// Contains a copy of every UNKNOWN-ATTRIBUTE values in network-byte order
        /// </summary>
        public UInt16[] AttributesValues
        {
            get
            {
                UInt16[] result = new UInt16[this.attributes.Count];

                this.attributes.CopyTo(result, 0);

                return result;
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a UnknownAttributes based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">The StunAttribute base for this UnknownAttributes</param>
        public UnknownAttributes(StunAttribute attribute)
            : base(StunAttributeType.UnknownAttributes, attribute.Value)
        {
            if (attribute.Type != StunAttributeType.UnknownAttributes)
                throw new ArgumentException("is not a valid UNKNOWN-ATTRIBUTES attribute", "attribute");

            Int32 offset = 0;

            while (offset < this.ValueLength)
            {
                byte[] unknownAttrBytes = StunUtilities.SubArray(this.Value, offset, 2);
                UInt16 unknownAttrValue = StunUtilities.ReverseBytes(BitConverter.ToUInt16(unknownAttrBytes, 0));

                this.attributes.Add(unknownAttrValue);

                offset += 2;
            }
        }
        #endregion
    }
}