using System;
using System.Runtime.InteropServices;
using NetLib.DNS.Records;

namespace NetLib.DNS
{
    /// <summary>
    /// Possible arguments for the DnsRecordListFree api
    /// </summary>
    /// <remarks>
    /// This enum is used by the DnsRecordListFree API.
    /// </remarks>
    enum DnsFreeType : uint
    {
        /// <summary>
        /// Reserved.
        /// </summary>
        FreeFlat = 0,

        /// <summary>
        /// Frees the record list returned by the DnsQuery API
        /// </summary>
        FreeRecordList
    }

    /// <summary>
    /// Represents one DNS request. Allows for a complete DNS record lookup
    /// on a given _Domain using the Windows API.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The DnsRequest class represents a complete DNS request for a given
    /// _Domain on a specified DNS server, including all options. The
    /// DnsRequest class uses the Windows API to do the query and the dlls
    /// used are only found on Windows 2000 or higher machines. The class
    /// will throw a <see cref="NotSupportedException"/> exception if run
    /// on an machine not capable of using the APIs that are required.
    /// </para>
    /// <para>
    /// Version Information
    /// </para>
    /// <para>
    ///         3/8/2003 v1.1 (C#) - Released on 5/31/2003
    /// </para>
    /// <para>
    /// Created by: Bill Gearhart. Based on code by Patrik Lundin.
    /// See version 1.0 remarks below. Specific attention was given
    /// to the exposed interface which got a 110% overhaul.
    /// </para>
    /// <para>
    /// Notable changes from the previous version:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             structs filled with constants were changed to enums
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             .net datatypes were changed to c# datatypes
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             every object is now in it's own *.cs file
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             custom collections and exceptions added
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             better object orientation - request and response classes
    ///             created for the dns query request/response session so that
    ///             it follows the .NET model
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             eliminated duplicate recs returned by an ALL query
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             bad api return code enumeration added
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             ToString() overridden to provide meaningful info for many
    ///             of the dns data structs
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             documentation and notes were created for all classes
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             added check to ensure code only runs on w2k or better
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             obsolete DNS record types are now marked as such
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             newer enum values added to DnsQueryType enum
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             compiled html documentation was written which always takes
    ///             20 times longer than writing the code does.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             this list of changes was compiled by your's truly...
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             smoothed out object and member names so they were more
    ///             intuitive - for instance: DNS_MX_DATA became MXRecord
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             added call to DnsRecordListFree API to free resources after
    ///             DnsQuery call
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             altered DnsQuery API call to allow for servers other than the
    ///             local DNS server from being queried
    ///         </description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    ///     4/15/2002 v1.0 (C#)
    /// </para>
    /// <para>
    /// Created by: Patrik Lundin
    /// </para>
    /// <para>
    /// Based on code found at:
    /// <a href="http://www.c-sharpcorner.com/Code/2002/April/DnsResolver.asp">http://www.c-sharpcorner.com/Code/2002/April/DnsResolver.asp</a>
    ///
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Initial implementation.
    ///         </description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Use the <see cref="DnsRequest"/> and <see cref="DnsResponse"/> objects
    /// together to get DNS information for aspemporium.com from the nameserver
    /// where the site is hosted.
    /// <code>
    /// using System;
    /// using netlib.Dns;
    /// using netlib.Dns.Records;
    ///
    /// namespace ClassLibrary1
    /// {
    ///     class __loader
    ///     {
    ///         static void Main()
    ///         {
    ///             try
    ///             {
    ///                 DnsRequest request = new DnsRequest();
    ///                 request.TreatAsFQDN=true;
    ///                 request.BypassCache=true;
    ///                 request.Servers.Add("dns.compresolve.com");
    ///                 request._domain = "aspemporium.com";
    ///                 DnsResponse response = request.GetResponse();
    ///
    ///                 Console.WriteLine("Addresses");
    ///                 Console.WriteLine("--------------------------");
    ///                 foreach(ARecord addr in response.ARecords)
    ///                     Console.WriteLine("\t{0}", addr.ToString());
    ///                 Console.WriteLine();
    ///
    ///                 Console.WriteLine("Name Servers");
    ///                 Console.WriteLine("--------------------------");
    ///                 foreach(PTRRecord ns in response.NSRecords)
    ///                     Console.WriteLine("\t{0}", ns.ToString());
    ///                 Console.WriteLine();
    ///
    ///                 Console.WriteLine("Mail Exchanges");
    ///                 Console.WriteLine("--------------------------");
    ///                 foreach(MXRecord exchange in response.MXRecords)
    ///                     Console.WriteLine("\t{0}", exchange.ToString());
    ///                 Console.WriteLine();
    ///
    ///                 Console.WriteLine("Canonical Names");
    ///                 Console.WriteLine("--------------------------");
    ///                 foreach(PTRRecord cname in response.GetRecords(DnsRecordType.CNAME))
    ///                     Console.WriteLine("\t{0}", cname.ToString());
    ///                 Console.WriteLine();
    ///
    ///                 Console.WriteLine("Start of Authority Records");
    ///                 Console.WriteLine("--------------------------");
    ///                 foreach(SOARecord soa in response.GetRecords(DnsRecordType.SOA))
    ///                     Console.WriteLine("\t{0}", soa.ToString());
    ///                 Console.WriteLine();
    ///
    ///                 //foreach(DnsWrapper wrap in response.RawRecords)
    ///                 //{
    ///                 //  Console.WriteLine(wrap.RecordType);
    ///                 //}
    ///
    ///                 response = null;
    ///                 request = null;
    ///             }
    ///             catch(DnsException ex)
    ///             {
    ///                 Console.WriteLine("EXCEPTION DOING DNS QUERY:");
    ///                 Console.WriteLine("\t{0}", ((DnsQueryReturnCode) ex.ErrorCode).ToString("g"));
    ///
    ///                 if (ex.InnerException != null)
    ///                     Console.WriteLine(ex.InnerException.ToString());
    ///             }
    ///         }
    ///     }
    /// }
    ///
    /// </code>
    /// </example>
    ///
    public class DnsRequest
    {
        /// <summary>
        /// http://msdn.microsoft.com/library/en-us/dns/dns/dnsquery.asp
        /// </summary>
        [DllImport("dnsapi", EntryPoint = "DnsQuery_A")]
        private static extern uint DnsQuery(
            [MarshalAs(UnmanagedType.LPStr)]
            string Name,

            [MarshalAs(UnmanagedType.U2)]
            DnsRecordType Type,

            [MarshalAs(UnmanagedType.U4)]
            DnsQueryType Options,

            IntPtr Servers,

            [In, Out]
            ref IntPtr QueryResultsSet,

            IntPtr Reserved
            );

        /// <summary>
        /// http://msdn.microsoft.com/library/en-us/dns/dns/dnsrecordlistfree.asp
        /// </summary>
        [DllImport("dnsapi", EntryPoint = "DnsRecordListFree")]
        private static extern void DnsRecordListFree(
            IntPtr RecordList,

            DnsFreeType FreeType
            );

        private DnsQueryType QueryType;
        private string _Domain;

        /// <summary>
        /// Gets or sets whether or not to use TCP only for the query.
        /// </summary>
        /// <value>Boolean indicating whether or not to use TCP instead of UDP for the query</value>
        /// <remarks>
        /// If set to true, the DNS query will be done via TCP rather than UDP. This
        /// is useful if the DNS service you are trying to reach is running on
        /// TCP but not on UDP.
        /// </remarks>
        public bool UseTCPOnly
        {
            get
            {
                return GetSetting(DnsQueryType.USE_TCP_ONLY);
            }

            set
            {
                SetSetting(DnsQueryType.USE_TCP_ONLY, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to accept truncated results —
        /// does not retry under TCP.
        /// </summary>
        /// <value>Boolean indicating whether or not to accept truncated results.</value>
        /// <remarks>
        /// Determines wherher or not the server will be re-queried in the event
        /// that a response was truncated.
        /// </remarks>
        public bool AcceptTruncatedResponse
        {
            get
            {
                return GetSetting(DnsQueryType.ACCEPT_TRUNCATED_RESPONSE);
            }

            set
            {
                SetSetting(DnsQueryType.ACCEPT_TRUNCATED_RESPONSE, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to perform an iterative query
        /// </summary>
        /// <value>Boolean indicating whether or not to use recursion
        /// to resolve the query.</value>
        /// <remarks>
        /// Specifically directs the DNS server not to perform
        /// recursive resolution to resolve the query.
        /// </remarks>
        public bool NoRecursion
        {
            get
            {
                return GetSetting(DnsQueryType.NO_RECURSION);
            }

            set
            {
                SetSetting(DnsQueryType.NO_RECURSION, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to bypass the resolver cache
        /// on the lookup.
        /// </summary>
        /// <remarks>
        /// Setting this to true allows you to specify one or more DNS servers
        /// to query instead of querying the local DNS cache and server.
        /// If false is set, the list of servers is ignored and the local DNS
        /// cache and server is used to resolve the query.
        /// </remarks>
        public bool BypassCache
        {
            get
            {
                return GetSetting(DnsQueryType.BYPASS_CACHE);
            }

            set
            {
                SetSetting(DnsQueryType.BYPASS_CACHE, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to direct DNS to perform a
        /// query on the local cache only
        /// </summary>
        /// <value>Boolean indicating whether or not to only use the
        /// DNS cache to resolve a query.</value>
        /// <remarks>
        /// This option allows you to query the local DNS cache only instead
        /// of making a DNS request over either UDP or TCP.
        /// This property represents the logical opposite of the
        /// <see cref="WireOnly"/> property.
        /// </remarks>
        public bool QueryCacheOnly
        {
            get
            {
                return GetSetting(DnsQueryType.NO_WIRE_QUERY);
            }

            set
            {
                SetSetting(DnsQueryType.NO_WIRE_QUERY, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to direct DNS to perform a
        /// query using the network only, bypassing local information.
        /// </summary>
        /// <value>Boolean indicating whether or not to use the
        /// network only instead of local information.</value>
        /// <remarks>
        /// This property represents the logical opposite of the
        /// <see cref="QueryCacheOnly"/> property.
        /// </remarks>
        public bool WireOnly
        {
            get
            {
                return GetSetting(DnsQueryType.WIRE_ONLY);
            }

            set
            {
                SetSetting(DnsQueryType.WIRE_ONLY, value);
            }
        }


        /// <summary>
        /// Gets or sets whether or not to direct DNS to ignore the
        /// local name.
        /// </summary>
        /// <value>Boolean indicating whether or not to ignore the local name.</value>
        /// <remarks>
        /// Determines how the DNS query handles local names.
        /// </remarks>
        public bool NoLocalName
        {
            get
            {
                return GetSetting(DnsQueryType.NO_LOCAL_NAME);
            }

            set
            {
                SetSetting(DnsQueryType.NO_LOCAL_NAME, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to prevent the DNS query from
        /// consulting the HOSTS file.
        /// </summary>
        /// <value>Boolean indicating whether or not to deny access to
        /// the HOSTS file when querying.</value>
        /// <remarks>
        /// Determines how the DNS query handles accessing the HOSTS file when
        /// querying for DNS information.
        /// </remarks>
        public bool NoHostsFile
        {
            get
            {
                return GetSetting(DnsQueryType.NO_HOSTS_FILE);
            }

            set
            {
                SetSetting(DnsQueryType.NO_HOSTS_FILE, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to prevent the DNS query from
        /// using NetBT for resolution.
        /// </summary>
        /// <value>Boolean indicating whether or not to deny access to
        /// NetBT during the query.</value>
        /// <remarks>
        /// Determines how the DNS query handles accessing NetBT when
        /// querying for DNS information.
        /// </remarks>
        public bool NoNetbt
        {
            get
            {
                return GetSetting(DnsQueryType.NO_NETBT);
            }

            set
            {
                SetSetting(DnsQueryType.NO_NETBT, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to direct DNS to return
        /// the entire DNS response message.
        /// </summary>
        /// <value>Boolean indicating whether or not to return the entire
        /// response.</value>
        /// <remarks>
        /// Determines how the DNS query expects the response to be
        /// received from the server.
        /// </remarks>
        public bool QueryReturnMessage
        {
            get
            {
                return GetSetting(DnsQueryType.RETURN_MESSAGE);
            }

            set
            {
                SetSetting(DnsQueryType.RETURN_MESSAGE, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to prevent the DNS
        /// response from attaching suffixes to the submitted
        /// name in a name resolution process.
        /// </summary>
        /// <value>Boolean indicating whether or not to allow
        /// suffix attachment during resolution.</value>
        /// <remarks>
        /// Determines how the DNS server handles suffix appending
        /// to the submitted name during name resolution.
        /// </remarks>
        public bool TreatAsFQDN
        {
            get
            {
                return GetSetting(DnsQueryType.TREAT_AS_FQDN);
            }

            set
            {
                SetSetting(DnsQueryType.TREAT_AS_FQDN, value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not to store records
        /// with the TTL corresponding to the minimum value
        /// TTL from among all records
        /// </summary>
        /// <value>Boolean indicating whether or not to
        /// use TTL values from all records.</value>
        /// <remarks>
        /// Determines how the DNS query handles TTL values.
        /// </remarks>
        public bool DontResetTTLValues
        {
            get
            {
                return GetSetting(DnsQueryType.DONT_RESET_TTL_VALUES);
            }

            set
            {
                SetSetting(DnsQueryType.DONT_RESET_TTL_VALUES, value);
            }
        }

        private bool GetSetting(DnsQueryType type)
        {
            DnsQueryType srchval = type;
            bool isset = (QueryType & srchval) == srchval;
            return isset;
        }

        private void SetSetting(DnsQueryType type, bool newvalue)
        {
            DnsQueryType srchval = type;
            bool isset = (QueryType & srchval) == srchval;
            bool newset = newvalue;

            //compare
            if (isset.CompareTo(newset) == 0)
                return;

            //toggle
            QueryType ^= srchval;
        }

        /// <summary>
        /// Gets or sets the _Domain to query. The _Domain must be a hostname,
        /// not an IP address.
        /// </summary>
        /// <remarks>
        /// This method is expecting a hostname, not an IP address. The
        /// system will fail with a <see cref="DnsException"/> when
        /// <see cref="GetResponse"/> is called if _domain is an IP address.
        /// </remarks>
        /// <value>String representing the _Domain that DNS information
        /// is desired for. This should be set to a hostname and not an
        /// IP Address.</value>
        public string _domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DnsRequest"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_domain"/> property is set to null
        /// and all other properties have their default value
        /// of false, except for <see cref="TreatAsFQDN"/> which has a value
        /// of true. The system is set to use the local DNS
        /// server for all queries.
        /// </remarks>
        public DnsRequest()
        {
            Initialize(null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DnsRequest"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_domain"/> property is set to the domain
        /// argument and all other properties have their default value
        /// of false, except for <see cref="TreatAsFQDN"/> which has a value
        /// of true. The system is set to use the local DNS
        /// server for all queries.
        /// </remarks>
        /// <param name="domain">The hostname that DNS information is desired for.
        /// This should not be an ip address. For example: yahoo.com</param>
        public DnsRequest(string domain)
        {
            Initialize(domain);
        }

        private void Initialize(string domain)
        {
            _domain = domain;
            QueryType = DnsQueryType.STANDARD | DnsQueryType.TREAT_AS_FQDN;
        }

        /// <summary>
        /// Queries the local DNS server for information about
        /// this instance of <see cref="DnsRequest"/> and returns
        /// the response as a <see cref="DnsResponse"/>
        /// </summary>
        /// <returns>A <see cref="DnsResponse"/> object containing the response
        /// from the DNS server.</returns>
        /// <exception cref="NotSupportedException">
        /// The code is running on a machine lesser than Windows 2000
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="_domain"/> property is null
        /// </exception>
        /// <exception cref="DnsException">
        /// The DNS query itself failed or parsing of the returned
        /// response failed
        /// </exception>
        /// <remarks>
        /// Returns a <see cref="DnsResponse"/> representing the response
        /// from the DNS server or one of the exceptions noted in the
        /// exceptions area, the most common of which is the
        /// <see cref="DnsException"/>.
        /// </remarks>
        public DnsResponse GetResponse(DnsRecordType dnstype)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new NotSupportedException("This API is found only on Windows NT or better.");

            if (_domain == null)
                throw new ArgumentNullException();

            string strDomain = _domain;
            DnsQueryType querytype = QueryType;

            object Data = new object();

            IntPtr ppQueryResultsSet = IntPtr.Zero;
            try
            {
                uint ret = DnsQuery(strDomain, dnstype, querytype, IntPtr.Zero, ref ppQueryResultsSet, IntPtr.Zero);
                if (ret != 0)
                    throw new DnsException("DNS query fails", ret);

                DnsResponse resp = new DnsResponse();
                // Parse the records.
                // Call function to loop through linked list and fill an array of records
                do
                {
                    // Get the DNS_RECORD
                    DnsRecord dnsrec = (DnsRecord)Marshal.PtrToStructure(
                        ppQueryResultsSet,
                        typeof(DnsRecord)
                        );

                    // Get the Data part
                    GetData(ppQueryResultsSet, ref dnsrec, ref Data);

                    // Wrap data in a struct with the type and data
                    DnsWrapper wrapper = new DnsWrapper();
                    wrapper.RecordType = dnsrec.RecordType;
                    wrapper.RecordData = Data;

                    // Note: this is *supposed* to return many records of the same type.  Don't check for uniqueness.
                    // Add wrapper to array
                    //if (! resp.RawRecords.Contains(wrapper))
                    resp.RawRecords.Add(wrapper);

                    ppQueryResultsSet = dnsrec.Next;
                } while (ppQueryResultsSet != IntPtr.Zero);

                return resp;
            }
            catch (DnsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnsException("unspecified error", ex);
            }
            finally
            {
                //ensure unmanaged code cleanup occurs

                //free pointer to DNS record block
                DnsRecordListFree(ppQueryResultsSet, DnsFreeType.FreeRecordList);
            }
        }

        private static void GetData(IntPtr ptr, ref DnsRecord dnsrec, ref object Data)
        {
            int size = ptr.ToInt32() + Marshal.SizeOf(dnsrec);
            ptr = new IntPtr(size);// Skip over the header portion of the DNS_RECORD to the data portion.
            switch (dnsrec.RecordType)
            {
                case DnsRecordType.A:
                    Data = (ARecord)Marshal.PtrToStructure(ptr, typeof(ARecord));
                    break;

                case DnsRecordType.CNAME:
                case DnsRecordType.PTR:
                    Data = (PTRRecord)Marshal.PtrToStructure(ptr, typeof(PTRRecord));
                    break;

                case DnsRecordType.MX:
                    Data = (MXRecord)Marshal.PtrToStructure(ptr, typeof(MXRecord));
                    break;

                case DnsRecordType.SOA:
                    Data = (SOARecord)Marshal.PtrToStructure(ptr, typeof(SOARecord));
                    break;

                case DnsRecordType.SRV:
                    Data = (SRVRecord)Marshal.PtrToStructure(ptr, typeof(SRVRecord));
                    break;

                case DnsRecordType.TEXT:
                    Data = (TXTRecord)Marshal.PtrToStructure(ptr, typeof(TXTRecord));
                    break;

                default:
                    Data = null;
                    break;
            }
        }
    }
}