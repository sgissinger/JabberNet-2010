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

#if !__MonoCS__
    #define UI_OK
#endif

namespace Jabber.Connection
{
    using System;


#if UI_OK
    using System.Security.Cryptography.X509Certificates;
    using System.Net.Security;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Diagnostics;
#endif

    /// <summary>
    /// Intentionally-ugly form to deal with bad certificates.  Because you don't like it, you should catch XmppStream.OnInvalidCertificate,
    /// and do something better.
    /// </summary>
    public partial class CertificatePrompt
#if UI_OK
        : Form
#endif
    {
#if UI_OK
        private X509Certificate2 m_cert;

        /// <summary>
        /// Create an ugly form to prompt the user about an invalid certificate.
        /// </summary>
        /// <param name="cert">The invalid certificate</param>
        /// <param name="chain">The CA chain for the cert</param>
        /// <param name="errors">The errors associated with the certificate</param>
        public CertificatePrompt(X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            InitializeComponent();

            m_cert = cert;

            lblSubject.Text = m_cert.SubjectName.Name;

            if ((errors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                lblSubject.ForeColor = Color.Red;

            lblBegin.Text = cert.NotBefore.ToString();
            lblEnd.Text = cert.NotAfter.ToString();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            Debug.Assert(!this.InvokeRequired);

            X509Certificate2UI.DisplayCertificate(m_cert);

            if (m_cert.Verify())
                this.DialogResult = DialogResult.OK;
        }
#endif

    }
}
