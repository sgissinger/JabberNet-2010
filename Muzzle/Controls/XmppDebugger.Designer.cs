
namespace Muzzle.Controls
{
    public partial class XmppDebugger
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtSend = new System.Windows.Forms.RichTextBox();
            this.splitter = new System.Windows.Forms.Splitter();
            this.rtDebug = new Muzzle.Controls.BottomScrollRichText();
            this.SuspendLayout();
            // 
            // rtSend
            // 
            this.rtSend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtSend.Location = new System.Drawing.Point(0, 110);
            this.rtSend.Name = "rtSend";
            this.rtSend.Size = new System.Drawing.Size(150, 40);
            this.rtSend.TabIndex = 0;
            this.rtSend.Text = "";
            this.rtSend.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtSend_KeyUp);
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 107);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(150, 3);
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // rtDebug
            // 
            this.rtDebug.BackColor = System.Drawing.SystemColors.Window;
            this.rtDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtDebug.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.rtDebug.Location = new System.Drawing.Point(0, 0);
            this.rtDebug.MaxLines = 500;
            this.rtDebug.Name = "rtDebug";
            this.rtDebug.ReadOnly = true;
            this.rtDebug.Size = new System.Drawing.Size(150, 107);
            this.rtDebug.TabIndex = 2;
            this.rtDebug.Text = "";
            this.rtDebug.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rtDebug_KeyUp);
            // 
            // XmppDebugger
            // 
            this.Controls.Add(this.rtDebug);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.rtSend);
            this.Name = "XmppDebugger";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.XmppDebugger_KeyUp);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.RichTextBox rtSend;
        private System.Windows.Forms.Splitter splitter;
        private Muzzle.Controls.BottomScrollRichText rtDebug;
    }
}