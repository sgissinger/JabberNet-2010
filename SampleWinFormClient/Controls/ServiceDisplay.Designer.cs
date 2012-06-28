
namespace SampleWinFormClient.Controls
{
    public partial class ServiceDisplay
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
            if (disposing && (components != null))
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
            this.tvServices = new System.Windows.Forms.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pgServices = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            //
            // tvServices
            //
            this.tvServices.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvServices.Location = new System.Drawing.Point(0, 0);
            this.tvServices.Name = "tvServices";
            this.tvServices.ShowLines = false;
            this.tvServices.ShowPlusMinus = false;
            this.tvServices.ShowNodeToolTips = true;
            this.tvServices.ShowRootLines = false;
            this.tvServices.Size = new System.Drawing.Size(175, 281);
            this.tvServices.TabIndex = 1;
            this.tvServices.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tvServices_AfterCollapse);
            this.tvServices.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvServices_AfterExpand);
            this.tvServices.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(tvServices_NodeMouseDoubleClick);
            this.tvServices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvServices_AfterSelect);
            //
            // splitter2
            //
            this.splitter2.Location = new System.Drawing.Point(175, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 281);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            //
            // pgServices
            //
            this.pgServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgServices.Location = new System.Drawing.Point(178, 0);
            this.pgServices.Name = "pgServices";
            this.pgServices.Size = new System.Drawing.Size(366, 281);
            this.pgServices.TabIndex = 3;
            //
            // ServiceDisplay
            //
            this.Controls.Add(this.pgServices);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.tvServices);
            this.Name = "ServiceDisplay";
            this.Size = new System.Drawing.Size(544, 281);
            this.ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.TreeView tvServices;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.PropertyGrid pgServices;
    }
}