
namespace SampleWinFormClient.Controls
{
    public partial class PubSubDisplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.lbID = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.rtItem = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            //
            // lbID
            //
            this.lbID.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbID.FormattingEnabled = true;
            this.lbID.IntegralHeight = false;
            this.lbID.Location = new System.Drawing.Point(0, 0);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(120, 170);
            this.lbID.TabIndex = 0;
            this.lbID.SelectedIndexChanged += new System.EventHandler(this.lbID_SelectedIndexChanged);
            //
            // splitter1
            //
            this.splitter1.Location = new System.Drawing.Point(120, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 170);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            //
            // rtItem
            //
            this.rtItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtItem.Location = new System.Drawing.Point(123, 0);
            this.rtItem.Name = "rtItem";
            this.rtItem.Size = new System.Drawing.Size(236, 170);
            this.rtItem.TabIndex = 2;
            this.rtItem.Text = "";
            //
            // PubSubDisplay
            //
            this.Controls.Add(this.rtItem);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.lbID);
            this.Name = "PubSubDisplay";
            this.Size = new System.Drawing.Size(359, 170);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.ListBox lbID;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.RichTextBox rtItem;
    }
}