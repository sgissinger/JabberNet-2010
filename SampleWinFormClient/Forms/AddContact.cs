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
using Jabber;

namespace SampleWinFormClient.Forms
{
    public partial class AddContact : Form
    {
        public AddContact()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Jabber ID to subscribe to.
        /// </summary>
        public JID JID
        {
            get
            {
                return new JID(txtJID.Text);
            }
            set
            {
                txtJID.Text = value.ToString();
            }
        }

        public string Nickname
        {
            get
            {
                return txtNickname.Text;
            }
            set
            {
                txtNickname.Text = value;
            }
        }

        /// <summary>
        /// All of the groups, checked or not.
        /// </summary>
        public string[] AllGroups
        {
            get
            {
                string[] items = new string[lbGroups.Items.Count];
                lbGroups.Items.CopyTo(items, 0);
                return items;
            }
            set
            {
                lbGroups.BeginUpdate();
                lbGroups.Items.Clear();
                lbGroups.Items.AddRange(value);
                lbGroups.EndUpdate();
            }
        }

        /// <summary>
        /// The groups that have been selected
        /// </summary>
        public string[] SelectedGroups
        {
            get
            {
                string[] items = new string[lbGroups.CheckedItems.Count];
                lbGroups.CheckedItems.CopyTo(items, 0);
                return items;
            }
            set
            {
                lbGroups.BeginUpdate();
                lbGroups.ClearSelected();
                for (int i = 0; i < lbGroups.Items.Count; i++)
                {
                    for (int j = 0; j < value.Length; j++)
                    {
                        if (((string)lbGroups.Items[i]) == value[j])
                        {
                            lbGroups.SetItemChecked(i, true);
                        }
                    }
                }
                lbGroups.EndUpdate();
            }
        }

        /// <summary>
        /// Use this domain, if one isn't provided in the JID.
        /// </summary>
        public string DefaultDomain { get; set; }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string g = txtGroup.Text.Trim();
            if (g != "")
            {
                int item = lbGroups.Items.Add(g, true);
                lbGroups.TopIndex = item;
                txtGroup.Clear();
            }
        }

        private void txtGroup_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: this doesn't actually work.
            if (e.KeyCode == Keys.Return)
            {
                btnAdd_Click(null, null);
#if NET_20
                e.SuppressKeyPress = true;
#endif
            }
        }

        private void txtJID_Leave(object sender, EventArgs e)
        {
            if (!txtJID.Text.Contains("@") && (this.DefaultDomain != null))
            {
                txtJID.Text = txtJID.Text + "@" + this.DefaultDomain;
            }
            if (txtNickname.Text == "")
            {
                JID jid = new JID(txtJID.Text);
                txtNickname.Text = jid.User;
            }
        }

    }
}
