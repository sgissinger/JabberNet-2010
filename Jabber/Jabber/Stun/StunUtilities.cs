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
using System.Net;
using System.Net.Sockets;
using Jabber.Stun.Attributes;

namespace Jabber.Stun
{
    /// <summary>
    /// Collection of generics utility methods that can be used by any STUN classes
    /// </summary>
    public static class StunUtilities
    {
        #region STUN
        /// <summary>
        /// Contains a new randomized TransactionID of 96-bits (12byte) on each call
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

        /// <summary>
        /// Helper method that returns needed informations to begin peer-to-peer Punch Hole operations
        /// </summary>
        /// <param name="address">The IP Address of the STUN server</param>
        /// <param name="type">The connection type used to do the STUN Binding Request</param>
        /// <returns>
        /// A key-value pair where :
        ///  * the key is the local IPEndPoint from where the STUN request occurs
        ///  * the value is the MappedAddress returned by the STUN server
        /// </returns>
        public static KeyValuePair<IPEndPoint, MappedAddress> GetMappedAddressFrom(String address, ProtocolType type)
        {
            StunMessage msg = new StunMessage(StunMethodType.Binding, StunMethodClass.Request, StunUtilities.NewTransactionId);

            StunClient cli = new StunClient();
            cli.Connect(address, type);

            StunMessage resp = cli.SendMessage(msg);

            IPEndPoint stunningEP = cli.StunningEP;
            MappedAddress mappedAddress = resp.MappedAddress;

            cli.Close();

            return new KeyValuePair<IPEndPoint, MappedAddress>(stunningEP, mappedAddress);
        }
        #endregion

        #region BYTES OPERATIONS
        /// <summary>
        /// Retrieves a sub-array of an array
        /// </summary>
        /// <typeparam name="T">Type of element in the array</typeparam>
        /// <param name="data">The array where to search for a sub-array</param>
        /// <param name="index">The index from where the sub-array starts (included in the sub-array and 0 indexed)</param>
        /// <param name="length">The number of array elements to retrieve</param>
        /// <returns>A sub-array of T elements</returns>
        public static T[] SubArray<T>(T[] data, Int32 index, Int32 length)
        {
            T[] result = new T[length];

            Array.Copy(data, index, result, 0, length);

            return result;
        }

        /// <summary>
        /// Convert a StunAttribute's list to an array of bytes
        /// </summary>
        /// <param name="attributes">The StunAttribute's list to convert</param>
        /// <returns>The array of bytes representing the StunAttribute's list</returns>
        public static byte[] GetBytes(StunAttribute[] attributes)
        {
            if (attributes.Length == 0)
                return new byte[0];

            Int32 totalLength = 0;

            foreach (StunAttribute attribute in attributes)
            {
                totalLength += attribute.GetBytes().Length;
            }

            byte[] result = new byte[totalLength];

            Int32 offset = 0;

            foreach (StunAttribute attribute in attributes)
            {
                attribute.GetBytes().CopyTo(result, offset);

                offset += attribute.GetBytes().Length;
            }

            return result;
        }

        /// <summary>
        /// Add 0 padding to an array of bytes making it 32bits bounded
        /// </summary>
        /// <param name="value">The array of bytes to pad</param>
        /// <returns>The array of bytes padded...or not</returns>
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
        /// Reverse byte order in 16 bits unsigned short numbers.
        /// This can be used to change between little-endian and big-endian.
        /// </summary>
        /// <param name="value">The unsigned short to reverse</param>
        /// <returns>The unsigned short reversed</returns>
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        /// <summary>
        /// Reverse byte order in 32 bits unsigned integer numbers.
        /// This can be used to change between little-endian and big-endian.
        /// </summary>
        /// <param name="value">The unsigned integer to reverse</param>
        /// <returns>The unsigned integer reversed</returns>
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        /// <summary>
        /// Reverse byte order in 64 bits unsigned long numbers.
        /// This can be used to change between little-endian and big-endian.
        /// </summary>
        /// <param name="value">The unsigned long to reverse</param>
        /// <returns>The unsigned long reversed</returns>
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }
        #endregion
    }
}