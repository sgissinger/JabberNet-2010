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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Xml;

using Jabber.Connection;

namespace Muzzle.Forms
{
    /// <summary>
    /// Base class for forms that configure XmppStream subclasses.
    /// </summary>
    public partial class OptionForm : Form
    {
        private XmppStream m_xmpp;
        private Hashtable m_extra = new Hashtable();

        /// <summary>
        /// Create new form.
        /// </summary>
        /// <param name="xmpp"></param>
        protected OptionForm(XmppStream xmpp)
            : this()
        {
            m_xmpp = xmpp;
        }

        /// <summary>
        /// Create new form
        /// </summary>
        protected OptionForm()
        {
            InitializeComponent();

            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
        }

        /// <summary>
        /// The client connection to manage
        /// </summary>
        public XmppStream Xmpp
        {
            get
            {
                // If we are running in the designer, let's try to auto-hook a JabberClient
                if ((m_xmpp == null) && DesignMode)
                {
                    IDesignerHost host = (IDesignerHost)base.GetService(typeof(IDesignerHost));
                    if (host == null)
                        return null;
                    m_xmpp = StreamComponent.GetStreamFromHost(host);
                }
                return m_xmpp;
            }
            set
            {
                m_xmpp = value;
                if (!DesignMode)
                    ReadXmpp();
            }
        }

        private void WriteValues(Control parent, XmppStream stream)
        {
            if (parent.Tag != null)
            {
                stream[(string)parent.Tag] = GetControlValue(parent);
            }
            if (parent.HasChildren)
            {
                foreach (Control child in parent.Controls)
                {
                    WriteValues(child, stream);
                }
            }
        }

        /// <summary>
        /// Write to the XmppStream the current values.
        /// </summary>
        protected void WriteXmpp()
        {
            if (m_xmpp != null)
                WriteValues(this, m_xmpp);
        }

        /// <summary>
        /// Write the configuration values to the given XmppStream.
        /// </summary>
        /// <param name="stream">The stream to configure</param>
        public void Configure(XmppStream stream)
        {
            WriteValues(this, stream);
        }

        private void WriteElem(XmlElement root, Control c)
        {
            if (c.Tag != null)
            {
                root.AppendChild(root.OwnerDocument.CreateElement((string)c.Tag)).InnerText =
                    GetControlValue(c).ToString();
            }
            if (c.HasChildren)
            {
                foreach (Control child in c.Controls)
                {
                    WriteElem(root, child);
                }
            }
        }

        /// <summary>
        /// Write the current connection properties to an XML config file.
        /// TODO: Replace this with a better ConfigFile implementation that can write.
        /// </summary>
        /// <param name="file"></param>
        public void WriteToFile(string file)
        {
            XmlDocument doc = new XmlDocument();
            string name = "JabberClient";
            if (m_xmpp != null)
                name = m_xmpp.GetType().Name;
            XmlElement root = (XmlElement)doc.CreateElement(name);
            doc.AppendChild(root);

            WriteElem(root, this);

            foreach (DictionaryEntry ent in m_extra)
            {
                root.AppendChild(doc.CreateElement((string)ent.Key)).InnerText = ent.Value.ToString();
            }

            XmlTextWriter xw = new XmlTextWriter(file, System.Text.Encoding.UTF8);
            xw.Formatting = Formatting.Indented;
            doc.WriteContentTo(xw);
            xw.Close();
        }

        private void ReadControls(Control parent)
        {
            if (parent == null)
                return;
            if (m_xmpp == null)
                return;

            if (parent.Tag != null)
                SetControlValue(parent, m_xmpp[(string)parent.Tag]);
            if (parent.HasChildren)
            {
                foreach (Control child in parent.Controls)
                {
                    ReadControls(child);
                }
            }
        }

        /// <summary>
        /// Read current values from the XmppStream
        /// </summary>
        protected void ReadXmpp()
        {
            ReadControls(this);
        }

        /// <summary>
        /// Read connection properties from the given file,
        /// pop up the dialog to see if the user wants to change them,
        /// save the changes, and
        /// connect to the server.
        /// </summary>
        /// <param name="propertyFile">The name of the file to store connection information in.</param>
        /// <returns>True if the user hit OK, otherwise false</returns>
        public bool Login(string propertyFile)
        {
            if (this.Xmpp == null)
                throw new ArgumentNullException("Client must be set", "Xmpp");
            if (propertyFile != null)
                ReadFromFile(propertyFile);
            if (ShowDialog() != DialogResult.OK)
                return false;
            if (propertyFile != null)
                WriteToFile(propertyFile);
            this.Xmpp.Connect();
            return true;
        }

        /// <summary>
        /// Set the connection properties from an XML config file.
        /// TODO: Replace this with a better ConfigFile implementation that can write.
        /// </summary>
        /// <param name="file"></param>
        public void ReadFromFile(string file)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(file);
            }
            catch (XmlException)
            {
                return;
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }

            XmlElement root = doc.DocumentElement;
            if (root == null)
                return;
            foreach (XmlNode node in root.ChildNodes)
            {
                XmlElement elem = node as XmlElement;
                if (elem == null)
                    continue;
                try
                {
                    this[elem.Name] = elem.InnerText;
                }
                catch (ArgumentException)
                {
                    // ignored
                }
            }
            WriteXmpp();
        }

        private Control FindComponentByTag(Control parent, string tag)
        {
            if ((string)parent.Tag == tag)
                return parent;
            if (parent.HasChildren)
            {
                foreach (Control c in parent.Controls)
                {
                    Control possible = FindComponentByTag(c, tag);
                    if (possible != null)
                        return possible;
                }
            }
            return null;
        }

        private object GetControlValue(Control c)
        {
            if (c == null)
                return null;
            CheckBox chk = c as CheckBox;
            if (chk != null)
                return chk.Checked;
            TextBox txt = c as TextBox;
            if (txt != null)
                return txt.Text;
            ComboBox cmb = c as ComboBox;
            if (cmb != null)
                return cmb.SelectedItem;
            NumericUpDown num = c as NumericUpDown;
            if (num != null)
                return (int)num.Value;
            throw new ArgumentException("Control with no tag", c.Name);
        }

        private void SetControlValue(Control c, object val)
        {
            CheckBox chk = c as CheckBox;
            if (chk != null)
            {
                if (val is bool)
                    chk.Checked = (bool)val;
                else if (val is string)
                    chk.Checked = bool.Parse((string)val);
                return;
            }
            TextBox txt = c as TextBox;
            if (txt != null)
            {
                txt.Text = (string)val;
                return;
            }
            ComboBox cmb = c as ComboBox;
            if (cmb != null)
            {
                if (cmb.SelectedItem.GetType().IsAssignableFrom(val.GetType()))
                    cmb.SelectedItem = val;
                else if (val is string)
                {
                    cmb.SelectedItem = Enum.Parse(cmb.SelectedItem.GetType(), (string)val);
                }
                return;
            }
            NumericUpDown num = c as NumericUpDown;
            if (num != null)
            {
                if (val is int)
                    num.Value = (int)val;
                else if (val is string)
                {
                    num.Value = int.Parse((string)val);
                }

                return;
            }
        }

        /// <summary>
        /// Set/Get the value of an option, as it currently exists in a control.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public object this[string option]
        {
            get
            {
                Control c = FindComponentByTag(this, option);
                if (c == null)
                {
                    if (m_extra.Contains(option))
                        return m_extra[option];
                    return null;
                }
                return GetControlValue(c);
            }
            set
            {
                Control c = FindComponentByTag(this, option);
                if (c == null)
                {
                    //throw new ArgumentException("Unknown option", option);
                    m_extra[option] = value;
                }
                else
                    SetControlValue(c, value);
            }
        }

        /// <summary>
        /// This field is required.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Required(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (!box.Enabled)
                return;
            if ((box.Text == null) || (box.Text == ""))
            {
                e.Cancel = true;
                error.SetError(box, "Required");
            }
        }

        /// <summary>
        /// Clear any error blinkies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClearError(object sender, System.EventArgs e)
        {
            error.SetError((Control)sender, "");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                return;

            WriteXmpp();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OptionForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
                ReadXmpp();
        }
    }
}
