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
using System;
using System.Windows.Forms;

namespace Muzzle.Controls
{
    /// <summary>
    /// Summary description for JidMulti.
    /// </summary>
    public partial class JidMulti : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Create a JidMulti control
        /// </summary>
        public JidMulti()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a range of JIDs or strings to the list.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange(object[] range)
        {
            lstJID.Items.AddRange(range);
        }

        /// <summary>
        /// Get the list of JIDs in the control currently.
        /// </summary>
        /// <returns></returns>
        public string[] GetValues()
        {
            string[] vals = new string[lstJID.Items.Count];
            for (int i=0; i < vals.Length; i++)
            {
                vals[i] = lstJID.Items[i].ToString();
            }
            return vals;
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                Jabber.JID jid = new Jabber.JID(txtEntry.Text);
                lstJID.Items.Add(jid);
                txtEntry.Clear();
                error.SetError(txtEntry, null);
            }
            catch
            {
                error.SetError(txtEntry, "Invalid JID");
            }
            this.Cursor = Cursors.Default;
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                Jabber.JID jid = new Jabber.JID(txtEntry.Text);
                int i = 0;
                foreach (object o in lstJID.Items)
                {
                    if (jid.Equals(o))
                    {
                        lstJID.Items.RemoveAt(i);
                        txtEntry.Clear();
                        error.SetError(txtEntry, null);
                        break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                error.SetError(txtEntry, "Invalid JID: " + ex.ToString());
            }
            this.Cursor = Cursors.Default;
        }

        private void lstJID_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstJID.SelectedIndex >= 0)
                txtEntry.Text = lstJID.Items[lstJID.SelectedIndex].ToString();
        }
    }
}
