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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Muzzle.Forms;

namespace Muzzle.Controls
{
    /// <summary>
    /// Debug stream for XMPP, so I don't have write it every time.
    /// </summary>
    public partial class XmppDebugger : StreamControl
    {
        private Color m_sendColor = Color.Blue;
        private Color m_recvColor = Color.Orange;
        private Color m_errColor = Color.Red;
        private Color m_otherColor = Color.Green;
        private string m_send = "SEND:";
        private string m_recv = "RECV:";
        private string m_err = "ERROR:";
        private bool m_readOnly = false;
        private string m_last = String.Empty;

        /// <summary>
        /// What color to use for the "SEND:" string.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color SendColor
        {
            get { return this.m_sendColor; }
            set { this.m_sendColor = value; }
        }

        /// <summary>
        /// What color to use for the "RECV:" string.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Orange")]
        public Color ReceiveColor
        {
            get { return this.m_recvColor; }
            set { this.m_recvColor = value; }
        }

        /// <summary>
        /// What color to use for the "ERROR:" string.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Red")]
        public Color ErrorColor
        {
            get { return this.m_errColor; }
            set { this.m_errColor = value; }
        }

        /// <summary>
        /// What color to use for the sent and received text.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "WindowText")]
        public Color TextColor
        {
            get { return this.rtDebug.ForeColor; }
            set { this.rtDebug.ForeColor = value; }
        }

        /// <summary>
        /// What color to use for other text inserted
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Green")]
        public Color OtherColor
        {
            get { return this.m_otherColor; }
            set { this.m_otherColor = value; }
        }

        /// <summary>
        /// Maximum number of lines to keep
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(500)]
        public int MaxLines
        {
            get { return this.rtDebug.MaxLines; }
            set { this.rtDebug.MaxLines = value; }
        }

        /// <summary>
        /// The string to prefix on sent bytes.
        /// </summary>
        [Category("Text")]
        [DefaultValue("SEND:")]
        public string SendPrefix
        {
            get { return this.m_send; }
            set { this.m_send = value; }
        }

        /// <summary>
        /// The string to prefix on sent bytes.
        /// </summary>
        [Category("Text")]
        [DefaultValue("RECV:")]
        public string ReceivePrefix
        {
            get { return this.m_recv; }
            set { this.m_recv = value; }
        }

        /// <summary>
        /// The string to prefix on errors.
        /// </summary>
        [Category("Text")]
        [DefaultValue("ERROR:")]
        public string ErrorPrefix
        {
            get { return this.m_err; }
            set { this.m_err = value; }
        }

        /// <summary>
        /// Number of lines currently printed in RichTextBox
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int NbLines { get; private set; }

        /// <summary>
        /// The string to prefix on errors.
        /// </summary>
        [Category("Text")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.m_readOnly; }
            set 
            {
                this.rtSend.Visible = !value;
                this.splitter.Visible = !value;

                this.m_readOnly = value;
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        public XmppDebugger()
        {
            InitializeComponent();

            this.OnStreamChanged += new Bedrock.ObjectHandler(this.XmppDebugger_OnStreamChanged);
            this.Disposed += new EventHandler(this.XmppDebugger_Disposed);
        }

        private void XmppDebugger_Disposed(object sender, EventArgs e)
        {
            this.rtSend.KeyUp -= this.rtSend_KeyUp;
            this.rtDebug.KeyUp -= this.rtDebug_KeyUp;
            this.KeyUp -= this.XmppDebugger_KeyUp;

            this.OnStreamChanged -= this.XmppDebugger_OnStreamChanged;

            this.m_stream.OnReadText -= this.m_stream_OnReadText;
            this.m_stream.OnWriteText -= this.m_stream_OnWriteText;
            this.m_stream.OnError -= this.m_stream_OnError;
            this.m_stream.OnConnect -= this.m_stream_OnConnect;
        }

        private void XmppDebugger_OnStreamChanged(object sender)
        {
            if (this.m_stream == null)
                return;

            this.m_stream.OnConnect += new Jabber.Connection.StanzaStreamHandler(this.m_stream_OnConnect);
            this.m_stream.OnReadText += new Bedrock.TextHandler(this.m_stream_OnReadText);
            this.m_stream.OnWriteText += new Bedrock.TextHandler(this.m_stream_OnWriteText);
            this.m_stream.OnError += new Bedrock.ExceptionHandler(this.m_stream_OnError);
        }

        private void Write(Color color, string tag, string text)
        {
            this.InvokeOrNot(() =>
            {
                Debug.WriteLine(tag + " " + text);

                this.rtDebug.AppendMaybeScroll(color, tag, text);

                if (this.NbLines > this.MaxLines)
                {
                    Array.Copy(this.rtDebug.Lines, 1,
                               this.rtDebug.Lines, 0, this.rtDebug.Lines.Length - 1);
                }
                else
                    this.NbLines++;
            });
        }

        /// <summary>
        /// Write an error to the log.
        /// </summary>
        /// <param name="error"></param>
        public void WriteError(string error)
        {
            this.Write(this.m_errColor, this.m_err, error);
        }

        private void m_stream_OnError(object sender, Exception ex)
        {
            this.WriteError(ex.ToString());
        }

        private void m_stream_OnConnect(object sender, Jabber.Connection.StanzaStream stream)
        {
            this.InvokeOrNot(() =>
            {
                this.NbLines = 0;
                this.rtDebug.Clear();
            });
        }

        private void m_stream_OnReadText(object sender, string txt)
        {
            // keepalive
            if (txt == " ")
                return;

            this.Write(this.m_recvColor, this.m_recv, txt);
        }

        private void m_stream_OnWriteText(object sender, string txt)
        {
            // keepalive
            if (txt == " ")
                return;

            this.Write(this.m_sendColor, this.m_send, txt);
        }

        /// <summary>
        /// Clear both text boxes
        /// </summary>
        public void Clear()
        {
            this.rtDebug.Clear();
            this.rtSend.Clear();
        }

        /// <summary>
        /// Write other text to the debug log
        /// </summary>
        /// <param name="tag">The tag to prefix with</param>
        /// <param name="text">The text after the tag</param>
        public void Write(string tag, string text)
        {
            this.Write(this.m_otherColor, tag, text);
        }

        private XmlElement ValidateXML()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.rtSend.Text);
                XmlElement elem = doc.DocumentElement;

                if (elem != null)
                    return elem;
            }
            catch (XmlException ex)
            {
                int offset = ex.LinePosition;

                for (int i = 0; (i < ex.LineNumber - 1) && (i < this.rtSend.Lines.Length); i++)
                    offset += this.rtSend.Lines[i].Length + 2;

                this.rtSend.Select(offset, 1);
            }
            return null;
        }

        private void ValidateAndSend()
        {
            XmlElement elem = this.ValidateXML();

            if (elem != null)
            {
                this.Write(elem);
                this.rtSend.Clear();
            }
        }

        private void rtSend_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && e.Control)
                this.ValidateAndSend();
            else if ((e.KeyCode == Keys.Delete) && e.Control)
                this.Clear();
        }

        private void Search(string txt)
        {
            string t = txt == null ? m_last : txt;

            if (String.IsNullOrEmpty(t))
                return;

            m_last = t;
            int start = this.rtDebug.SelectionStart + 1;

            if (start < 0 || start > this.rtDebug.Text.Length)
                start = 0;

            int offset = this.rtDebug.Text.IndexOf(t, start);

            if (offset < 0)
            {
                Console.Beep();
                offset = 0;
            }

            this.rtDebug.Select(offset, t.Length);
            this.rtDebug.ScrollToCaret();
        }

        private void rtDebug_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) && e.Control)
                this.Clear();
            else if ((e.KeyCode == Keys.F) && e.Control)
            {
                InputBox inp = new InputBox();

                if (inp.ShowDialog("Find text", "Find:", String.Empty) != DialogResult.OK)
                    return;

                this.Search(inp.Value);
            }
            else if (e.KeyCode == Keys.F3)
                this.Search(null);
        }

        private void XmppDebugger_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) && e.Control)
                this.Clear();
        }
    }
}
