
namespace Muzzle.Controls
{
    public partial class RosterTree
    {

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RosterTree));
            this.il = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // il
            // 
            this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
            this.il.TransparentColor = System.Drawing.Color.Empty;
            this.il.Images.SetKeyName(0, "StatusOffline.png");
            this.il.Images.SetKeyName(1, "StatusOnline.png");
            this.il.Images.SetKeyName(2, "StatusAway.png");
            this.il.Images.SetKeyName(3, "StatusXa.png");
            this.il.Images.SetKeyName(4, "StatusDnd.png");
            this.il.Images.SetKeyName(5, "StatusChatty.png");
            this.il.Images.SetKeyName(6, "TreeExpanded.png");
            this.il.Images.SetKeyName(7, "TreeContracted.png");
            this.il.Images.SetKeyName(8, "blank");
            // 
            // RosterTree
            // 
            this.AllowDrop = true;
            this.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.ImageIndex = 1;
            this.ImageList = this.il;
            this.LineColor = System.Drawing.Color.Black;
            this.SelectedImageIndex = 0;
            this.ShowLines = false;
            this.ShowRootLines = false;
            this.Sorted = true;
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ImageList il;
        private System.Windows.Forms.ToolTip tt;
        private System.ComponentModel.IContainer components;
    }
}