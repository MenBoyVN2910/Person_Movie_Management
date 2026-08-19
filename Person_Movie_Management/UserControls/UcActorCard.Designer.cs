namespace Person_Movie_Management.UserControls
{
    partial class UcActorCard
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
            this.pnlBase = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlImageContainer = new System.Windows.Forms.Panel();
            this.picAvatar = new Guna.UI2.WinForms.Guna2PictureBox();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblSubInfo = new System.Windows.Forms.Label();
            this.pnlNameRow = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.btnCopyName = new System.Windows.Forms.Label();
            this.pnlBase.SuspendLayout();
            this.pnlImageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.pnlNameRow.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBase
            // 
            this.pnlBase.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(53)))), ((int)(((byte)(90)))));
            this.pnlBase.BorderRadius = 16;
            this.pnlBase.BorderThickness = 1;
            this.pnlBase.Controls.Add(this.pnlImageContainer);
            this.pnlBase.Controls.Add(this.pnlInfo);
            this.pnlBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBase.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(48)))));
            this.pnlBase.Location = new System.Drawing.Point(0, 0);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(230, 340);
            this.pnlBase.TabIndex = 0;
            // 
            // pnlImageContainer
            // 
            this.pnlImageContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(36)))), ((int)(((byte)(68)))));
            this.pnlImageContainer.Controls.Add(this.picAvatar);
            this.pnlImageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImageContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlImageContainer.Name = "pnlImageContainer";
            this.pnlImageContainer.Size = new System.Drawing.Size(230, 245);
            this.pnlImageContainer.TabIndex = 0;
            // 
            // picAvatar
            // 
            this.picAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(36)))), ((int)(((byte)(68)))));
            this.picAvatar.BorderRadius = 16;
            this.picAvatar.CustomizableEdges.BottomLeft = false;
            this.picAvatar.CustomizableEdges.BottomRight = false;
            this.picAvatar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picAvatar.ImageRotate = 0F;
            this.picAvatar.Location = new System.Drawing.Point(0, 0);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(230, 245);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 0;
            this.picAvatar.TabStop = false;
            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(48)))));
            this.pnlInfo.Controls.Add(this.lblSubInfo);
            this.pnlInfo.Controls.Add(this.pnlNameRow);
            this.pnlInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInfo.Location = new System.Drawing.Point(0, 245);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Padding = new System.Windows.Forms.Padding(12, 6, 12, 8);
            this.pnlInfo.Size = new System.Drawing.Size(230, 95);
            this.pnlInfo.TabIndex = 1;
            // 
            // lblSubInfo
            // 
            this.lblSubInfo.AutoEllipsis = true;
            this.lblSubInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubInfo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSubInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSubInfo.Location = new System.Drawing.Point(12, 50);
            this.lblSubInfo.Name = "lblSubInfo";
            this.lblSubInfo.Size = new System.Drawing.Size(206, 32);
            this.lblSubInfo.TabIndex = 1;
            this.lblSubInfo.Text = "Quốc tịch • Năm sinh";
            this.lblSubInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlNameRow
            // 
            this.pnlNameRow.BackColor = System.Drawing.Color.Transparent;
            this.pnlNameRow.Controls.Add(this.lblName);
            this.pnlNameRow.Controls.Add(this.btnCopyName);
            this.pnlNameRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNameRow.Location = new System.Drawing.Point(12, 6);
            this.pnlNameRow.Name = "pnlNameRow";
            this.pnlNameRow.Size = new System.Drawing.Size(206, 44);
            this.pnlNameRow.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(0, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(170, 44);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên diễn viên";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCopyName
            // 
            this.btnCopyName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopyName.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCopyName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCopyName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(180)))), ((int)(((byte)(252)))));
            this.btnCopyName.Location = new System.Drawing.Point(170, 0);
            this.btnCopyName.Name = "btnCopyName";
            this.btnCopyName.Size = new System.Drawing.Size(36, 44);
            this.btnCopyName.TabIndex = 1;
            this.btnCopyName.Text = "📋";
            this.btnCopyName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UcActorCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.Controls.Add(this.pnlBase);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "UcActorCard";
            this.Size = new System.Drawing.Size(230, 340);
            this.pnlBase.ResumeLayout(false);
            this.pnlImageContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlNameRow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Panel pnlBase;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Panel pnlNameRow;
        private System.Windows.Forms.Panel pnlImageContainer;
        private Guna.UI2.WinForms.Guna2PictureBox picAvatar;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label btnCopyName;
        private System.Windows.Forms.Label lblSubInfo;
    }
}
