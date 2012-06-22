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



namespace Jabber.Protocol.X
{
    /// <summary>
    /// ElementFactory for all currently supported IQ namespaces.
    /// </summary>
    public class Factory : IPacketTypes
    {
        private static QnameType[] s_qnt = new QnameType[]
        {
                    new QnameType("x",     URI.XDELAY,    typeof(Jabber.Protocol.X.Delay)),
                    new QnameType("x",     URI.XEVENT,    typeof(Jabber.Protocol.X.Event)),
                    new QnameType("x",     URI.XOOB,      typeof(Jabber.Protocol.IQ.OOB)),
                    new QnameType("x",     URI.XROSTER,   typeof(Jabber.Protocol.IQ.Roster)),
                    new QnameType("item",  URI.XROSTER,   typeof(Jabber.Protocol.IQ.Item)),
                    new QnameType("group", URI.XROSTER,   typeof(Jabber.Protocol.IQ.Group)),

                    new QnameType("x",     URI.XDATA,     typeof(Jabber.Protocol.X.Data)),
                    new QnameType("field", URI.XDATA,     typeof(Jabber.Protocol.X.Field)),
                    new QnameType("option",URI.XDATA,     typeof(Jabber.Protocol.X.Option)),

                    new QnameType("c",     URI.CAPS,      typeof(Jabber.Protocol.X.Caps)),
        };
        QnameType[] IPacketTypes.Types { get { return s_qnt; } }
    }
}
