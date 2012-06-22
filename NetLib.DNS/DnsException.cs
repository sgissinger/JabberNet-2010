using System;
using System.Runtime.Serialization;

namespace NetLib.DNS
{
    /// <summary>
    /// Represents the exception that occurs when a <see cref="DnsRequest"/>
    /// fails.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The exception that occurs when a DNS request fails at any level.
    /// </para>
    /// <para>
    /// This class is used to represent two broad types of exceptions:
    /// <list type="bullet">
    ///     <item>Win32 API Exceptions that occurred when calling the DnsQuery API</item>
    ///     <item>Exceptions of other types that occurred when working with
    ///     the <see cref="DnsRequest"/> and <see cref="DnsResponse"/>
    ///     classes.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Win32 errors that are DNS specific are specified in the
    /// <see cref="DnsQueryReturnCode"/> enumeration but if the
    /// <see cref="ErrorCode"/> returned is not defined in that
    /// enum then the number returned will be defined in WinError.h.
    /// </para>
    /// <para>
    /// Exceptions of other types are available through the
    /// InnerException property.
    /// </para>
    /// </remarks>
    [Serializable]
    public class DnsException : ApplicationException, ISerializable
    {
        private readonly uint errcode = (uint)DnsQueryReturnCode.SUCCESS;

        /// <summary>
        /// Initializes a new instance of <see cref="DnsException"/>
        /// </summary>
        /// <remarks>
        /// Used to raise a <see cref="DnsException"/> with all the default
        /// properties. The message property will return: Unspecified
        /// DNS exception.
        /// </remarks>
        public DnsException()
            : base("Unspecified DNS exception")
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DnsException"/>
        /// </summary>
        /// <param name="message">the human readable description of the problem</param>
        /// <remarks>
        /// Used to raise a <see cref="DnsException"/> where the only important
        /// information is a description about the error. The <see cref="ErrorCode"/>
        /// property will return 0 or SUCCESS indicating that the DNS API calls
        /// succeeded, regardless of whether they did or did not.
        /// </remarks>
        public DnsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DnsException"/>
        /// </summary>
        /// <param name="message">the human readable description of the problem</param>
        /// <param name="errcode">the error code (<see cref="DnsQueryReturnCode"/>)
        /// if the DnsQuery api failed</param>
        /// <remarks>
        /// Used to raise a <see cref="DnsException"/> where the underlying DNS
        /// API call fails. In this case, the <see cref="ErrorCode"/> property
        /// is the most important information about the exception. In most cases,
        /// the number returned is a value in the <see cref="DnsQueryReturnCode"/>
        /// enum however, if it is not, the error is defined in WinError.h.
        /// </remarks>
        public DnsException(string message, uint errcode)
            : base(message)
        {
            this.errcode = errcode;
        }

        /// <summary>
        /// Gets the error code (<see cref="DnsQueryReturnCode"/>)
        /// if the DnsQuery api failed. Will be set to success (0) if the API
        /// didn't fail but another part of the code did.
        /// </summary>
        /// <remarks>
        /// Win32 errors that are DNS specific are specified in the
        /// <see cref="DnsQueryReturnCode"/> enumeration but if the
        /// <see cref="ErrorCode"/> returned is not defined in that
        /// enum then the number returned will be defined in WinError.h.
        /// </remarks>
        /// <value>Value will be defined in WinError.h if not defined in the
        /// <see cref="DnsQueryReturnCode"/> enum.</value>
        /// <example>
        /// This example shows how to decypher the return of the
        /// ErrorCode property.
        /// <code>
        /// try
        /// {
        ///     ...
        /// }
        /// catch(DnsException dnsEx)
        /// {
        ///     int errcode = dnsEx.ErrorCode;
        ///     if (! Enum.IsDefined(typeof(DnsQueryReturnCode), errcode))
        ///     {
        ///         //defined in winerror.h
        ///         Console.WriteLine("WIN32 Error: {0}", errcode);
        ///         return;
        ///     }
        ///
        ///     DnsQueryReturnCode errretcode = (DnsQueryReturnCode) errcode;
        ///     if (errretcode == DnsQueryReturnCode.SUCCESS)
        ///     {
        ///         //inner exception contains the goodies
        ///         Console.WriteLine(dnsEx.InnerException.ToString());
        ///         return;
        ///     }
        ///
        ///     //dns error
        ///     Console.WriteLine("DNS Error: {0}", errretcode.ToString("g"));
        /// }
        /// </code>
        /// </example>
        public uint ErrorCode
        {
            get
            {
                return errcode;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DnsException"/>
        /// </summary>
        /// <param name="message">the human readable description of the
        /// problem</param>
        /// <param name="innerException">the exception that caused the
        /// underlying error</param>
        /// <remarks>
        /// Used to raise a <see cref="DnsException"/> where the exception is
        /// some other type but a typeof(DnsException) is desired to be raised
        /// instead. In this case, the <see cref="ErrorCode"/> property
        /// always returns 0 or SUCCESS and is a useless property.
        /// </remarks>
        public DnsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("errcode", errcode);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DnsException"/> for <see cref="ISerializable"/>
        /// </summary>
        /// <param name="info">the serialization information</param>
        /// <param name="context">the context</param>
        /// <remarks>
        /// Used by the <see cref="ISerializable"/> interface.
        /// </remarks>
        public DnsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            errcode = info.GetUInt32("errcode");
        }
    }
}
