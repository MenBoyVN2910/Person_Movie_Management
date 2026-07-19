namespace Person_Movie_Management.Forms
{
    partial class FrmDetailDialog
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
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtContent = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnOK = new Guna.UI2.WinForms.Guna2GradientButton();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.TargetControl = this;
            this.guna2Elipse1.BorderRadius = 16;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(43, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            // 
            // txtContent
            // 
            this.txtContent.BorderRadius = 10;
            this.txtContent.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContent.DefaultText = "";
            this.txtContent.Location = new System.Drawing.Point(30, 80);
            this.txtContent.Margin = new System.Windows.Forms.Padding(4);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.PasswordChar = '\0';
            this.txtContent.PlaceholderText = "";
            this.txtContent.ReadOnly = true;
            this.txtContent.SelectedText = "";
            this.txtContent.Size = new System.Drawing.Size(540, 300);
            this.txtContent.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.BorderRadius = 12;
            this.btnOK.Location = new System.Drawing.Point(200, 400);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(200, 45);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Đóng";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmDetailDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 470);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.lblTitle);
            this.Name = "FrmDetailDialog";
            this.Text = "Detail";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtContent;
        private Guna.UI2.WinForms.Guna2GradientButton btnOK;
    }
}
