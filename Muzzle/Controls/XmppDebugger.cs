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
        private string m_last = String.Empty;

        /// <summary>
        /// What color to use for the "SEND:" string.
        /// </summary>
        [Category("Appearance")]
        public Color SendColor
        {
            get { return m_sendColor; }
            set { m_sendColor = value; }
        }

        /// <summary>
        /// What color to use for the "RECV:" string.
        /// </summary>
        [Category("Appearance")]
        public Color ReceiveColor
        {
            get { return m_recvColor; }
            set { m_recvColor = value; }
        }

        /// <summary>
        /// What color to use for the "ERROR:" string.
        /// </summary>
        [Category("Appearance")]
        public Color ErrorColor
        {
            get { return m_errColor; }
            set { m_errColor = value; }
        }

        /// <summary>
        /// What color to use for the sent and received text.
        /// </summary>
        [Category("Appearance")]
        public Color TextColor
        {
            get { return rtDebug.ForeColor; }
            set { rtDebug.ForeColor = value; }
        }

        /// <summary>
        /// What color to use for other text inserted
        /// </summary>
        [Category("Appearance")]
        public Color OtherColor
        {
            get { return m_otherColor; }
            set { m_otherColor = value; }
        }

        /// <summary>
        /// Maximum number of lines to keep
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(500)]
        public int MaxLines
        {
            get { return rtDebug.MaxLines; }
            set { rtDebug.MaxLines = value; }
        }

        /// <summary>
        /// The string to prefix on sent bytes.
        /// </summary>
        [Category("Text")]
        [DefaultValue("SEND:")]
        public string SendPrefix
        {
            get { return m_send; }
            set { m_send = value; }
        }

        /// <summary>
        /// The string to prefix on sent bytes.
        /// </summary>
        [Category("Text")]
        [DefaultValue("RECV:")]
        public string ReceivePrefix
        {
            get { return m_recv; }
            set { m_recv = value; }
        }

        /// <summary>
        /// The string to prefix on errors.
        /// </summary>
        [Category("Text")]
        [DefaultValue("ERROR:")]
        public string ErrorPrefix
        {
            get { return m_err; }
            set { m_err = value; }
        }

        /// <summary>
        /// Create
        /// </summary>
        public XmppDebugger()
        {
            InitializeComponent();

            this.OnStreamChanged += new Bedrock.ObjectHandler(XmppDebugger_OnStreamChanged);

            this.Disposed += new EventHandler(XmppDebugger_Disposed);
        }

        private void XmppDebugger_Disposed(object sender, EventArgs e)
        {
            this.rtSend.KeyUp -= this.rtSend_KeyUp;
            this.rtDebug.KeyUp -= this.rtDebug_KeyUp;
            this.KeyUp -= this.XmppDebugger_KeyUp;

            this.OnStreamChanged -= this.XmppDebugger_OnStreamChanged;

            m_stream.OnReadText -= this.m_stream_OnReadText;
            m_stream.OnWriteText -= this.m_stream_OnWriteText;
            m_stream.OnError -= this.m_stream_OnError;
            m_stream.OnConnect -= this.m_stream_OnConnect;
        }

        private void XmppDebugger_OnStreamChanged(object sender)
        {
            if (m_stream == null)
                return;

            m_stream.OnConnect += new Jabber.Connection.StanzaStreamHandler(m_stream_OnConnect);
            m_stream.OnReadText += new Bedrock.TextHandler(m_stream_OnReadText);
            m_stream.OnWriteText += new Bedrock.TextHandler(m_stream_OnWriteText);
            m_stream.OnError += new Bedrock.ExceptionHandler(m_stream_OnError);
        }

        private void Write(Color color, string tag, string text)
        {
            Debug.WriteLine(tag + " " + text);

            this.InvokeOrNot(() =>
            {
                rtDebug.AppendMaybeScroll(color, tag, text);
            });
        }

        /// <summary>
        /// Write an error to the log.
        /// </summary>
        /// <param name="error"></param>
        public void WriteError(string error)
        {
            Write(m_errColor, m_err, error);
        }

        private void m_stream_OnError(object sender, Exception ex)
        {
            WriteError(ex.ToString());
        }

        private void m_stream_OnConnect(object sender, Jabber.Connection.StanzaStream stream)
        {
            this.InvokeOrNot(() =>
            {
                rtDebug.Clear();
            });
        }

        private void m_stream_OnReadText(object sender, string txt)
        {
            // keepalive
            if (txt == " ")
                return;

            Write(m_recvColor, m_recv, txt);
        }

        private void m_stream_OnWriteText(object sender, string txt)
        {
            // keepalive
            if (txt == " ")
                return;

            Write(m_sendColor, m_send, txt);
        }

        /// <summary>
        /// Clear both text boxes
        /// </summary>
        public void Clear()
        {
            rtDebug.Clear();
            rtSend.Clear();
        }

        /// <summary>
        /// Write other text to the debug log
        /// </summary>
        /// <param name="tag">The tag to prefix with</param>
        /// <param name="text">The text after the tag</param>
        public void Write(string tag, string text)
        {
            Write(m_otherColor, tag, text);
        }

        private XmlElement ValidateXML()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(rtSend.Text);
                XmlElement elem = doc.DocumentElement;
                if (elem != null)
                {
                    return elem;
                }
            }
            catch (XmlException ex)
            {
                int offset = ex.LinePosition;
                for (int i = 0; (i < ex.LineNumber - 1) && (i < rtSend.Lines.Length); i++)
                {
                    offset += rtSend.Lines[i].Length + 2;
                }
                rtSend.Select(offset, 1);
            }
            return null;
        }

        private void ValidateAndSend()
        {
            XmlElement elem = ValidateXML();
            if (elem != null)
            {
                Write(elem);
                rtSend.Clear();
            }
        }

        private void rtSend_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && e.Control)
            {
                ValidateAndSend();
            }
            else if ((e.KeyCode == Keys.Delete) && e.Control)
            {
                Clear();
            }
        }

        private void Search(string txt)
        {
            string t = (txt == null) ? m_last : txt;
            if (t == "")
                return;
            m_last = t;
            int start = rtDebug.SelectionStart + 1;
            if ((start < 0) || (start > rtDebug.Text.Length))
                start = 0;
            int offset = rtDebug.Text.IndexOf(t, start);
            if (offset < 0)
            {
                Console.Beep();
                offset = 0;
            }
            rtDebug.Select(offset, t.Length);
            rtDebug.ScrollToCaret();
        }

        private void rtDebug_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) && e.Control)
            {
                Clear();
            }
            else if ((e.KeyCode == Keys.F) && e.Control)
            {
                InputBox inp = new InputBox();
                if (inp.ShowDialog("Find text", "Find:", "") != DialogResult.OK)
                    return;
                Search(inp.Value);
            }
            else if (e.KeyCode == Keys.F3)
            {
                Search(null);
            }
        }

        private void XmppDebugger_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Delete) && e.Control)
            {
                Clear();
            }
        }
    }
}
