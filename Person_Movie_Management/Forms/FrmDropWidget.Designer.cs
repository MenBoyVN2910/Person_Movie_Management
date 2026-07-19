namespace Person_Movie_Management.Forms
{
    partial class FrmDropWidget
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(80, 80);
            this.Name = "FrmDropWidget";
            this.Text = "DropZone";
            this.ResumeLayout(false);
        }
    }
}
