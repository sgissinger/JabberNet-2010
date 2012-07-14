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
    public static class StunUtilities
    {
        #region PROPERTIES
        /// <summary>
        /// TODO: Documentation NewTransactionId
        /// </summary>
        public static byte[] NewTransactionId
        {
            get
            {
                byte[] transactionId = new byte[12];

                new Random().NextBytes(transactionId);

                return transactionId;
            }
        }
        #endregion

        #region EXTENSIONS
        /// <summary>
        /// TODO: Documentation SubArray
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] data, Int32 index, Int32 length)
        {
            T[] result = new T[length];

            Array.Copy(data, index, result, 0, length);

            return result;
        }

        /// <summary>
        /// TODO: Documentation GetBytes
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static byte[] GetBytes(Dictionary<StunAttributeType, StunAttribute> attributes)
        {
            if (attributes.Count == 0)
                return new byte[0];

            StunAttribute[] attributeList = new StunAttribute[attributes.Count];

            attributes.Values.CopyTo(attributeList, 0);

            Int32 totalLength = 0;

            foreach (StunAttribute attribute in attributeList)
            {
                totalLength += attribute.GetBytes().Length;
            }

            byte[] result = new byte[totalLength];

            Int32 offset = 0;

            foreach (StunAttribute attribute in attributeList)
            {
                attribute.GetBytes().CopyTo(result, offset);

                offset += attribute.GetBytes().Length;
            }

            return result;
        }

        /// <summary>
        /// TODO: Documentation PadTo32Bits
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] PadTo32Bits(byte[] value)
        {
            if (value.Length == 0)
                return new byte[0];

            byte[] result;

            // 4 bytes are equals to 32 bits
            Int32 neededPadding = value.Length % 4;

            switch (neededPadding)
            {
                case 0:
                default:
                    return value;

                case 1:
                    result = new byte[value.Length + 3];

                    value.CopyTo(result, 0);
                    new byte[] { 0, 0, 0 }.CopyTo(result, value.Length);

                    return result;

                case 2:
                    result = new byte[value.Length + 2];

                    value.CopyTo(result, 0);
                    new byte[] { 0, 0 }.CopyTo(result, value.Length);

                    return result;

                case 3:
                    result = new byte[value.Length + 1];

                    value.CopyTo(result, 0);
                    new byte[] { 0 }.CopyTo(result, value.Length);

                    return result;
            }
        }

        /// <summary>
        /// TODO: Documentation ReverseBytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        /// <summary>
        /// TODO: Documentation ReverseBytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        #endregion
    }
}