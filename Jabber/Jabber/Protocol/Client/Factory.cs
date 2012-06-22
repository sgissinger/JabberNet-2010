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


namespace Jabber.Protocol.Client
{
    /// <summary>
    /// ElementFactory for the jabber:client namespace.
    /// </summary>
    public class Factory : IPacketTypes
    {
        private static QnameType[] s_qnt = new QnameType[]
        {
            new QnameType("presence", URI.CLIENT, typeof(Jabber.Protocol.Client.Presence)),
            new QnameType("message",  URI.CLIENT, typeof(Jabber.Protocol.Client.Message)),
            new QnameType("iq",       URI.CLIENT, typeof(Jabber.Protocol.Client.IQ)),
            new QnameType("error",    URI.CLIENT, typeof(Jabber.Protocol.Client.Error)),
            // meh.  jabber protocol really isn't right WRT to namespaces.
            new QnameType("presence", URI.ACCEPT, typeof(Jabber.Protocol.Client.Presence)),
            new QnameType("message",  URI.ACCEPT, typeof(Jabber.Protocol.Client.Message)),
            new QnameType("iq",       URI.ACCEPT, typeof(Jabber.Protocol.Client.IQ)),
            new QnameType("error",    URI.ACCEPT, typeof(Jabber.Protocol.Client.Error))
        };
        QnameType[] IPacketTypes.Types { get { return s_qnt; } }
    }
}
