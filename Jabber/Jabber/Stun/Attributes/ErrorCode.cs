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
using System.Reflection;

namespace Jabber.Stun.Attributes
{
    /// <summary>
    /// The ERROR-CODE attribute is used in error response messages. It contains a numeric error code value
    /// in the range of 300 to 699 plus a textual reason phrase encoded in UTF-8 [RFC3629], and is consistent
    /// in its code assignments and semantics with SIP [RFC3261] and HTTP [RFC2616]
    /// </summary>
    public class ErrorCode : StunAttribute
    {
        #region PROPERTIES
        /// <summary>
        /// The Class represents the hundreds digit of the error code. The value MUST be between 3 and 6
        /// </summary>
        public byte Class
        {
            get { return this.Value[2]; }
        }
        /// <summary>
        /// The Number represents the error code modulo 100, and its value MUST be between 0 and 99.
        /// </summary>
        public byte Code
        {
            get { return this.Value[3]; }
        }
        /// <summary>
        /// The reason phrase is meant for user consumption, and can be anything appropriate for the error code.
        /// Recommended reason phrases for the defined error codes are included in the IANA registry
        /// for error codes.  The reason phrase MUST be a UTF-8 [RFC3629] encoded
        /// sequence of less than 128 characters (which can be as long as 763 bytes).
        /// </summary>
        public String Reason
        {
            get { return StunAttribute.Encoder.GetString(StunUtilities.SubArray(this.Value, 4, this.ValueLength - 4)); }
        }
        /// <summary>
        /// Represents the entire error code, the class * 100 + the code
        /// </summary>
        public UInt16 ErrorTypeShort
        {
            get { return (UInt16)(this.Class * 100 + this.Code); }
        }
        /// <summary>
        /// Represents the ErrorCodeType enum of this ERROR-CODE attribute based on the ErrorTypeValue
        /// </summary>
        public ErrorCodeType ErrorType
        {
            get
            {
                foreach (FieldInfo field in typeof(ErrorCodeType).GetFields())
                {
                    Object[] fieldAttributes = field.GetCustomAttributes(typeof(StunValueAttribute), false);

                    if (fieldAttributes.Length == 1)
                    {
                        StunValueAttribute stunValueAttribute = fieldAttributes.GetValue(0) as StunValueAttribute;

                        if (stunValueAttribute != null &&
                            stunValueAttribute.Value == this.ErrorTypeShort)
                        {
                            return (ErrorCodeType)Enum.Parse(typeof(ErrorCodeType), field.Name);
                        }
                    }
                }
                return ErrorCodeType.Unknown;
            }
        }
        #endregion

        #region CONSTRUCTORS & FINALIZERS
        /// <summary>
        /// Constructs a MappedAddress based on an existing StunAttribute
        /// </summary>
        /// <param name="attribute">Base StunAttribute used to construct this specialized attribute</param>
        public ErrorCode(StunAttribute attribute)
            : base(StunAttributeType.ErrorCode, attribute.Value)
        {
            if (attribute.Type != StunAttributeType.ErrorCode)
                throw new ArgumentException("is not a valid ERROR-CODE attribute", "attribute");
        }
        #endregion

        #region METHODS
        /// <summary>
        /// String summary of this ERROR-CODE attribute
        /// </summary>
        /// <returns>A String with informations about this ERROR-CODE attribute</returns>
        public override String ToString()
        {
            return String.Format(CultureInfo.CurrentCulture,
                                 "{0}, {1}",
                                 this.Type,
                                 this.ErrorType);
        }
        #endregion
    }

    /// <summary>
    /// Enumeration of any STUN errors types
    /// Each error type has a matching constant defined in STUN [RFC5389], in TURN [RFC5766] or in SIP [3261]
    /// </summary>
    public enum ErrorCodeType
    {
        /// <summary>
        /// Miscellaneous errors unknown to this library
        /// </summary>
        [StunValue(0xFFFF)]
        Unknown,

        #region STUN Core
        /// <summary>
        /// The client should contact an alternate server for this request. This error response MUST
        /// only be sent if the request included a USERNAME attribute and a valid MESSAGE-INTEGRITY
        /// attribute; otherwise, it MUST NOT be sent and error code 400 (Bad Request) is suggested.
        /// This error response MUST be protected with the MESSAGE-INTEGRITY attribute, and receivers
        /// MUST validate the MESSAGE-INTEGRITY of this response before redirecting themselves to an alternate server
        /// </summary>
        [StunValue(300)]
        TryAlternate,
        /// <summary>
        /// The request was malformed. The client SHOULD NOT retry the request without modification 
        /// from the previous attempt. The server may not be able to generate a valid MESSAGE-INTEGRITY
        /// for this error, so the client MUST NOT expect a valid MESSAGE-INTEGRITY attribute on this response
        /// </summary>
        [StunValue(400)]
        BadRequest,
        /// <summary>
        /// The request did not contain the correct credentials to proceed
        /// The client should retry the request with proper credentials
        /// </summary>
        [StunValue(401)]
        Unauthorized,
        /// <summary>
        /// The server received a STUN packet containing a comprehension-required attribute that it did not understand
        /// The server MUST put this unknown attribute in the UNKNOWN-ATTRIBUTE attribute of its error response
        /// </summary>
        [StunValue(420)]
        UnknownAttribute,
        /// <summary>
        /// The NONCE used by the client was no longer valid
        /// The client should retry, using the NONCE provided in the response
        /// </summary>
        [StunValue(438)]
        StaleNonce,
        /// <summary>
        /// The server has suffered a temporary error. The client should try again.
        /// </summary>
        [StunValue(500)]
        ServerError,
        #endregion

        #region TURN Extension
        /// <summary>
        /// The request was valid but cannot be performed due to administrative or similar restrictions
        /// </summary>
        [StunValue(403)]
        Forbidden,
        /// <summary>
        /// A request was received by the server that requires an allocation to be in place, but no allocation exists,
        /// or a request was received that requires no allocation, but an allocation exists.
        /// </summary>
        [StunValue(437)]
        AllocationMismatch,
        /// <summary>
        /// The credentials in the (non-Allocate) request do not match those used to create the allocation.
        /// </summary>
        [StunValue(441)]
        WrongCredentials,
        /// <summary>
        /// The Allocate request asked the server to use a transport protocol between the server and the peer
        /// that the server does not support. NOTE: This does NOT refer to the transport protocol used in the 5-tuple.
        /// </summary>
        [StunValue(442)]
        UnsupportedTransportProtocol,
        /// <summary>
        /// No more allocations using this username can be created at the present time.
        /// </summary>
        [StunValue(486)]
        AllocationQuotaReached,
        /// <summary>
        /// The server is unable to carry out the request due to some capacity limit being reached. In an Allocate
        /// response, this could be due to the server having no more relayed transport addresses available
        /// at that time, having none with the requested properties, or the one that corresponds to the specified
        /// reservation token is not available.
        /// </summary>
        [StunValue(508)]
        InsufficientCapacity,
        #endregion

        #region SIP Base
        /// <summary>
        /// The method specified in the Request-Line is understood, but not allowed for the address identified by the Request-URI
        /// The response MUST include an Allow header field containing a list of valid methods for the indicated address
        /// </summary>
        [StunValue(405)]
        MethodNotAllowed,
        #endregion

        #region ICE
        /// <summary>
        /// The client asserted an ICE role (controlling or controlled) that is in conflict with the role of the server.
        /// </summary>
        [StunValue(487)]
        RoleConflict,
        #endregion

        #region STUN Classic
        /// <summary>
        /// The Binding Request did contain a MESSAGE-INTEGRITY attribute,
        /// but it used a shared secret that has expired.
        /// The client should obtain a new shared secret and try again.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(430)]
        StaleCredentials,
        /// <summary>
        /// The Binding Request contained a MESSAGE-INTEGRITY attribute,
        /// but the HMAC failed verification.
        /// This could be a sign of a potential attack, or client implementation error.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(431)]
        IntegrityCheckFailure,
        /// <summary>
        /// The Binding Request contained a MESSAGE-INTEGRITY attribute,
        /// but not a USERNAME attribute.
        /// Both must be present for integrity checks.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(432)]
        MissingUsername,
        /// <summary>
        /// The Shared Secret request has to be sent over TLS, but was not received over TLS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(433)]
        UseTls,
        /// <summary>
        /// The server is refusing to fulfill the request. The client should not retry.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        [StunValue(600)]
        GlobalFailure
        #endregion
    }
}