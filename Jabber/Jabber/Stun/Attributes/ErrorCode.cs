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
    /// The ERROR-CODE attribute is used in error response messages. It contains a numeric error code value
    /// in the range of 300 to 699 plus a textual reason phrase encoded in UTF-8 [RFC3629], and is consistent
    /// in its code assignments and semantics with SIP [RFC3261] and HTTP [RFC2616]
    /// </summary>
    public class ErrorCode : StunAttribute
    {
        #region CONSTANTS
        #region STUN Core
        private const UInt16 TRY_ALTERNATE = 300;
        private const UInt16 BAD_REQUEST = 400;
        private const UInt16 UNAUTHORIZED = 401;
        private const UInt16 UNKNOWN_ATTRIBUTE = 420;
        private const UInt16 STALE_NONCE = 438;
        private const UInt16 SERVER_ERROR = 500;
        #endregion
        #region TURN Extension
        private const UInt16 FORBIDDEN = 403;
        private const UInt16 ALLOCATION_MISMATCH = 437;
        private const UInt16 WRONG_CREDENTIALS = 441;
        private const UInt16 UNSUPPORTED_TRANSPORT_PROTOCOL = 442;
        private const UInt16 ALLOCATION_QUOTA_REACHED = 486;
        private const UInt16 INSUFFICIENT_CAPACITY = 508;
        #endregion
        #region SIP Based
        private const UInt16 METHOD_NOT_ALLOWED = 405;
        #endregion
        #region STUN Classic
        [Obsolete("Defined in RFC3489")]
        private const UInt16 STALE_CREDENTIALS = 430;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 INTEGRITY_CHECK_FAILURE = 431;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 MISSING_USERNAME = 432;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 USE_TLS = 433;
        [Obsolete("Defined in RFC3489")]
        private const UInt16 GLOBAL_FAILURE = 600;
        #endregion
        #endregion

        #region PROPERTIES
        /// <summary>
        /// The Class represents the hundreds digit of the error code. The value MUST be between 3 and 6
        /// </summary>
        public byte Class
        {
            get
            {
                return StunUtilities.SubArray(this.Value, 2, 1)[0];
            }
        }
        /// <summary>
        /// The Number represents the error code modulo 100, and its value MUST be between 0 and 99.
        /// </summary>
        public byte Code
        {
            get
            {
                return StunUtilities.SubArray(this.Value, 3, 1)[0];
            }
        }
        /// <summary>
        /// The reason phrase is meant for user consumption, and can be anything appropriate for the error code.
        /// Recommended reason phrases for the defined error codes are included in the IANA registry
        /// for error codes.  The reason phrase MUST be a UTF-8 [RFC3629] encoded
        /// sequence of less than 128 characters (which can be as long as 763 bytes).
        /// </summary>
        public String Reason
        {
            get
            {
                return StunAttribute.Encoder.GetString(StunUtilities.SubArray(this.Value, 4, this.ValueLength - 4));
            }
        }
        /// <summary>
        /// Represents the entire error code, the class * 100 + the code
        /// </summary>
        public UInt16 ErrorTypeValue
        {
            get
            {
                return (UInt16)(this.Class * 100 + this.Code);
            }
        }
        /// <summary>
        /// Represents the ErrorCodeType enum of this ERROR-CODE attribute based on the ErrorTypeValue
        /// </summary>
        public ErrorCodeType ErrorType
        {
            get
            {
                ErrorCodeType result;

                switch (this.ErrorTypeValue)
                {
                    default:
                        result = ErrorCodeType.Unknown;
                        break;

                    // STUN Core
                    case ErrorCode.TRY_ALTERNATE:
                        result = ErrorCodeType.TryAlternate;
                        break;

                    case ErrorCode.BAD_REQUEST:
                        result = ErrorCodeType.BadRequest;
                        break;

                    case ErrorCode.UNAUTHORIZED:
                        result = ErrorCodeType.Unauthorized;
                        break;

                    case ErrorCode.UNKNOWN_ATTRIBUTE:
                        result = ErrorCodeType.UnknownAttribute;
                        break;

                    case ErrorCode.STALE_NONCE:
                        result = ErrorCodeType.StaleNonce;
                        break;

                    case ErrorCode.SERVER_ERROR:
                        result = ErrorCodeType.ServerError;
                        break;

                    // TURN Extension
                    case ErrorCode.FORBIDDEN:
                        result = ErrorCodeType.Forbidden;
                        break;

                    case ErrorCode.ALLOCATION_MISMATCH:
                        result = ErrorCodeType.AllocationMismatch;
                        break;

                    case ErrorCode.WRONG_CREDENTIALS:
                        result = ErrorCodeType.WrongCredentials;
                        break;

                    case ErrorCode.UNSUPPORTED_TRANSPORT_PROTOCOL:
                        result = ErrorCodeType.UnsupportedTransportProtocol;
                        break;

                    case ErrorCode.ALLOCATION_QUOTA_REACHED:
                        result = ErrorCodeType.AllocationQuotaReached;
                        break;

                    case ErrorCode.INSUFFICIENT_CAPACITY:
                        result = ErrorCodeType.InsufficientCapacity;
                        break;

                    // SIP Based
                    case ErrorCode.METHOD_NOT_ALLOWED:
                        result = ErrorCodeType.MethodNotAllowed;
                        break;
                }

                return result;
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
        Unknown,
        #region STUN Core
        /// <summary>
        /// The client should contact an alternate server for this request. This error response MUST
        /// only be sent if the request included a USERNAME attribute and a valid MESSAGE-INTEGRITY
        /// attribute; otherwise, it MUST NOT be sent and error code 400 (Bad Request) is suggested.
        /// This error response MUST be protected with the MESSAGE-INTEGRITY attribute, and receivers
        /// MUST validate the MESSAGE-INTEGRITY of this response before redirecting themselves to an alternate server
        /// </summary>
        TryAlternate,
        /// <summary>
        /// The request was malformed. The client SHOULD NOT retry the request without modification 
        /// from the previous attempt. The server may not be able to generate a valid MESSAGE-INTEGRITY
        /// for this error, so the client MUST NOT expect a valid MESSAGE-INTEGRITY attribute on this response
        /// </summary>
        BadRequest,
        /// <summary>
        /// The request did not contain the correct credentials to proceed
        /// The client should retry the request with proper credentials
        /// </summary>
        Unauthorized,
        /// <summary>
        /// The server received a STUN packet containing a comprehension-required attribute that it did not understand
        /// The server MUST put this unknown attribute in the UNKNOWN-ATTRIBUTE attribute of its error response
        /// </summary>
        UnknownAttribute,
        /// <summary>
        /// The NONCE used by the client was no longer valid
        /// The client should retry, using the NONCE provided in the response
        /// </summary>
        StaleNonce,
        /// <summary>
        /// The server has suffered a temporary error. The client should try again.
        /// </summary>
        ServerError,
        #endregion

        #region TURN Extension
        /// <summary>
        /// The request was valid but cannot be performed due to administrative or similar restrictions
        /// </summary>
        Forbidden,
        /// <summary>
        /// A request was received by the server that requires an allocation to be in place, but no allocation exists,
        /// or a request was received that requires no allocation, but an allocation exists.
        /// </summary>
        AllocationMismatch,
        /// <summary>
        /// The credentials in the (non-Allocate) request do not match those used to create the allocation.
        /// </summary>
        WrongCredentials,
        /// <summary>
        /// The Allocate request asked the server to use a transport protocol between the server and the peer
        /// that the server does not support. NOTE: This does NOT refer to the transport protocol used in the 5-tuple.
        /// </summary>
        UnsupportedTransportProtocol,
        /// <summary>
        /// No more allocations using this username can be created at the present time.
        /// </summary>
        AllocationQuotaReached,
        /// <summary>
        /// The server is unable to carry out the request due to some capacity limit being reached. In an Allocate
        /// response, this could be due to the server having no more relayed transport addresses available
        /// at that time, having none with the requested properties, or the one that corresponds to the specified
        /// reservation token is not available.
        /// </summary>
        InsufficientCapacity,
        #endregion

        #region SIP Base
        /// <summary>
        /// The method specified in the Request-Line is understood, but not allowed for the address identified by the Request-URI
        /// The response MUST include an Allow header field containing a list of valid methods for the indicated address
        /// </summary>
        MethodNotAllowed,
        #endregion

        #region STUN Classic
        /// <summary>
        /// The Binding Request did contain a MESSAGE-INTEGRITY attribute,
        /// but it used a shared secret that has expired.
        /// The client should obtain a new shared secret and try again.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        StaleCredentials,
        /// <summary>
        /// The Binding Request contained a MESSAGE-INTEGRITY attribute,
        /// but the HMAC failed verification.
        /// This could be a sign of a potential attack, or client implementation error.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        IntegrityCheckFailure,
        /// <summary>
        /// The Binding Request contained a MESSAGE-INTEGRITY attribute,
        /// but not a USERNAME attribute.
        /// Both must be present for integrity checks.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        MissingUsername,
        /// <summary>
        /// The Shared Secret request has to be sent over TLS, but was not received over TLS.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        UseTls,
        /// <summary>
        /// The server is refusing to fulfill the request. The client should not retry.
        /// </summary>
        [Obsolete("Defined in RFC3489")]
        GlobalFailure
        #endregion
    }
}