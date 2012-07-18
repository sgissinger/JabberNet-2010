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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        /// Contains a new randomized TransactionID of 96bits (12bytes) on each call
        /// It follows STUN [RFC5389] as in STUN Classic [RFC3489] the length of this property is 128bits (16bytes)
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
        /// Helper method using UDP or TCP that returns needed informations to begin peer-to-peer Punch Hole operations
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
            return StunUtilities.GetMappedAddressFrom(null, address, type);
        }

        /// <summary>
        /// Helper method using UDP or TCP that returns needed informations to begin peer-to-peer Punch Hole operations
        /// using an existing IPEndPoint
        /// </summary>
        /// <param name="stunningEP">The IPEndPoint to which the socket will be bound to</param>
        /// <param name="address">The IP Address of the STUN server</param>
        /// <param name="type">The connection type used to do the STUN Binding Request</param>
        /// <returns>
        /// A key-value pair where :
        ///  * the key is the local IPEndPoint from where the STUN request occurs
        ///  * the value is the MappedAddress returned by the STUN server
        /// </returns>
        public static KeyValuePair<IPEndPoint, MappedAddress> GetMappedAddressFrom(IPEndPoint stunningEP, String address, ProtocolType type)
        {
            StunMessage msg = new StunMessage(StunMethodType.Binding, StunMethodClass.Request, StunUtilities.NewTransactionId);

            StunClient cli = new StunClient(stunningEP);
            cli.Connect(address, type);

            StunMessage resp = cli.SendMessage(msg);

            if (stunningEP == null)
                stunningEP = cli.StunningEP;

            MappedAddress mappedAddress = resp.MappedAddress;

            cli.Close();

            return new KeyValuePair<IPEndPoint, MappedAddress>(stunningEP, mappedAddress);
        }

        /// <summary>
        /// Helper method using TLS over TCP that returns needed informations to begin peer-to-peer Punch Hole operations
        /// </summary>
        /// <param name="address">The IP Address of the STUN server</param>
        /// <param name="remoteCertificateValidationHandler">The callback handler which validate STUN Server TLS certificate</param>
        /// <param name="clientCertificate">
        /// Client certificate used for mutual authentication. This certificate must be in PKCS #12 format and must contains its private key
        /// The simpler way to create a certificate of this type is to follow this makecert tutorial http://www.inventec.ch/chdh/notes/14.htm.
        /// Once your certificate is created : launch "mmc", CTRL+M, select "Certificates", add, choose "Local machine".
        /// Find your certificate under "Personal", it must have a little key in its icon, right click on it, choose "All tasks > Export...".
        /// Check the "Export key" checkbox, finish the process and then you have a valid X509Certificate2 with its private key in it
        /// </param>
        /// <returns>
        /// A key-value pair where :
        ///  * the key is the local IPEndPoint from where the STUN request occurs
        ///  * the value is the MappedAddress returned by the STUN server
        /// </returns>
        public static KeyValuePair<IPEndPoint, MappedAddress> GetMappedAddressFrom(String address, RemoteCertificateValidationCallback remoteCertificateValidationHandler, X509Certificate2 clientCertificate)
        {
            return StunUtilities.GetMappedAddressFrom(null, address, remoteCertificateValidationHandler, clientCertificate);
        }

        /// <summary>
        /// Helper method using TLS over TCP that returns needed informations to begin peer-to-peer Punch Hole operations
        /// using an existing IPEndPoint
        /// </summary>
        /// <param name="stunningEP">The IPEndPoint to which the socket will be bound to</param>
        /// <param name="address">The IP Address of the STUN server</param>
        /// <param name="remoteCertificateValidationHandler">The callback handler which validate STUN Server TLS certificate</param>
        /// <param name="clientCertificate">
        /// Client certificate used for mutual authentication. This certificate must be in PKCS #12 format and must contains its private key
        /// The simpler way to create a certificate of this type is to follow this makecert tutorial http://www.inventec.ch/chdh/notes/14.htm.
        /// Once your certificate is created : launch "mmc", CTRL+M, select "Certificates", add, choose "Local machine".
        /// Find your certificate under "Personal", it must have a little key in its icon, right click on it, choose "All tasks > Export...".
        /// Check the "Export key" checkbox, finish the process and then you have a valid X509Certificate2 with its private key in it
        /// </param>
        /// <returns>
        /// A key-value pair where :
        ///  * the key is the local IPEndPoint from where the STUN request occurs
        ///  * the value is the MappedAddress returned by the STUN server
        /// </returns>
        public static KeyValuePair<IPEndPoint, MappedAddress> GetMappedAddressFrom(IPEndPoint stunningEP, String address, RemoteCertificateValidationCallback remoteCertificateValidationHandler, X509Certificate2 clientCertificate)
        {
            StunMessage msg = new StunMessage(StunMethodType.Binding, StunMethodClass.Request, StunUtilities.NewTransactionId);

            StunClient cli = new StunClient(stunningEP);
            cli.Connect(address, remoteCertificateValidationHandler, clientCertificate);

            StunMessage resp = cli.SendMessage(msg);

            if (stunningEP == null)
                stunningEP = cli.StunningEP;

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
        /// Tests that two byte arrays content are equals or not
        /// </summary>
        /// <param name="b1">The first array to compare</param>
        /// <param name="b2">The second array to compare</param>
        /// <returns>True if the two arrays content is the same otherwise false</returns>
        public static Boolean ByteArraysEquals(byte[] b1, byte[] b2)
        {
            if (b1 == b2)
                return true;

            if (b1 == null || b2 == null)
                return false;

            if (b1.Length != b2.Length) 
                return false;

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }

            return true;
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