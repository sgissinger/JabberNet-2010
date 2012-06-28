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
using System.Windows.Forms;

namespace Muzzle.Forms
{
    /// <summary>
    /// A generic input getter dialog.
    /// </summary>
    public partial class InputBox : Form
    {
        /// <summary>
        /// Create
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pop up the input box with the given parameters
        /// </summary>
        /// <param name="title">The text of the window title</param>
        /// <param name="prompt">Prompt value.  Include colon if desired.</param>
        /// <param name="defaultValue">Initial value of the input box</param>
        /// <returns></returns>
        public DialogResult ShowDialog(string title, string prompt, string defaultValue)
        {
            this.Text = title;
            label1.Text = prompt;
            textBox1.Text = defaultValue;
            return this.ShowDialog();
        }

        /// <summary>
        /// The value entered by the user
        /// </summary>
        public string Value
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
