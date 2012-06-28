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

namespace SampleWinFormClient.Forms
{
    /// <summary>
    /// Summary description for SendMessage.
    /// </summary>
    public partial class SendMessage : System.Windows.Forms.Form
    {
        private Jabber.Client.JabberClient m_jc;

        public SendMessage(Jabber.Client.JabberClient jc, string toJid)
            : this(jc)
        {
            txtTo.Text = toJid;
        }

        public SendMessage(Jabber.Client.JabberClient jc)
        {
            InitializeComponent();

            m_jc = jc;
        }

        private void btnSend_Click(object sender, System.EventArgs e)
        {
            Jabber.Protocol.Client.Message msg = new Jabber.Protocol.Client.Message(m_jc.Document);
            msg.To = txtTo.Text;
            if (txtSubject.Text != "")
                msg.Subject = txtSubject.Text;
            msg.Body = txtBody.Text;
            m_jc.Write(msg);
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
