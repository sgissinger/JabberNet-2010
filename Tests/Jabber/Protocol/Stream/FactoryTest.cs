/* --------------------------------------------------------------------------
 * Copyrights
 *
 * Portions created by or assigned to Cursive Systems, Inc. are
 * Copyright (c) 2002-2008 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 *
 * Jabber-Net is licensed under the LGPL.
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/

using Jabber.Protocol;
using Jabber.Protocol.Stream;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Jabber.Protocol.stream
{
    /// <summary>
    /// Summary description for StreamFactoryTest.
    /// </summary>
    [TestClass]
    public class StreamFactoryTest
    {
        [TestMethod]
        public void Test_Create()
        {
            ElementFactory pf = new ElementFactory();
            pf.AddType(new Factory());
        }
    }
}
