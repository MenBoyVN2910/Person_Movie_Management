namespace Person_Movie_Management.Forms
{
    partial class FrmBatchImport
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlTextContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.txtUrls = new System.Windows.Forms.RichTextBox();
            this.pnlInstruction = new Guna.UI2.WinForms.Guna2Panel();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.btnStart = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlMain.SuspendLayout();
            this.pnlTextContainer.SuspendLayout();
            this.pnlInstruction.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 16;
            this.guna2Elipse1.TargetControl = this;
            // 
            // pnlMain
            // 
            this.pnlMain.BorderRadius = 16;
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnStart);
            this.pnlMain.Controls.Add(this.progressBar);
            this.pnlMain.Controls.Add(this.pnlInstruction);
            this.pnlMain.Controls.Add(this.pnlTextContainer);
            this.pnlMain.Controls.Add(this.btnClose);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(650, 520);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(225, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔗 Thêm Link Hàng Loạt";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.IconColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(600, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 35);
            this.btnClose.TabIndex = 1;
            // 
            // pnlTextContainer
            // 
            this.pnlTextContainer.BackColor = System.Drawing.Color.Transparent;
            this.pnlTextContainer.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(75)))), ((int)(((byte)(120)))));
            this.pnlTextContainer.BorderRadius = 12;
            this.pnlTextContainer.BorderThickness = 1;
            this.pnlTextContainer.Controls.Add(this.txtUrls);
            this.pnlTextContainer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.pnlTextContainer.Location = new System.Drawing.Point(20, 55);
            this.pnlTextContainer.Name = "pnlTextContainer";
            this.pnlTextContainer.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.pnlTextContainer.Size = new System.Drawing.Size(610, 250);
            this.pnlTextContainer.TabIndex = 2;
            // 
            // txtUrls
            // 
            this.txtUrls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtUrls.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUrls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUrls.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtUrls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtUrls.Location = new System.Drawing.Point(12, 10);
            this.txtUrls.Name = "txtUrls";
            this.txtUrls.Size = new System.Drawing.Size(586, 230);
            this.txtUrls.TabIndex = 0;
            this.txtUrls.Text = "";
            // 
            // pnlInstruction
            // 
            this.pnlInstruction.BackColor = System.Drawing.Color.Transparent;
            this.pnlInstruction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(95)))));
            this.pnlInstruction.BorderRadius = 12;
            this.pnlInstruction.BorderThickness = 1;
            this.pnlInstruction.Controls.Add(this.lblStatus);
            this.pnlInstruction.Controls.Add(this.lblNote);
            this.pnlInstruction.Controls.Add(this.lblInstruction);
            this.pnlInstruction.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(22)))), ((int)(((byte)(46)))));
            this.pnlInstruction.Location = new System.Drawing.Point(20, 315);
            this.pnlInstruction.Name = "pnlInstruction";
            this.pnlInstruction.Padding = new System.Windows.Forms.Padding(14, 10, 14, 10);
            this.pnlInstruction.Size = new System.Drawing.Size(610, 85);
            this.pnlInstruction.TabIndex = 3;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(140)))), ((int)(((byte)(248)))));
            this.lblInstruction.Location = new System.Drawing.Point(12, 10);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(430, 17);
            this.lblInstruction.TabIndex = 0;
            this.lblInstruction.Text = "📌 Hướng Dẫn: Dán các Link URL của video vào khung trên";
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(191)))), ((int)(((byte)(36)))));
            this.lblNote.Location = new System.Drawing.Point(12, 33);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(415, 15);
            this.lblNote.TabIndex = 1;
            this.lblNote.Text = "⚠️ Lưu Ý: Mỗi Hàng 1 link và phải xuống dòng bằng Enter";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblStatus.Location = new System.Drawing.Point(12, 56);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(260, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "🟢 Xanh: Hợp lệ | 🟡 Vàng: Trùng | 🔴 Đỏ: Lỗi";
            // 
            // progressBar
            // 
            this.progressBar.BorderRadius = 6;
            this.progressBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.progressBar.Location = new System.Drawing.Point(20, 412);
            this.progressBar.Name = "progressBar";
            this.progressBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.progressBar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.progressBar.Size = new System.Drawing.Size(610, 12);
            this.progressBar.TabIndex = 4;
            this.progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.progressBar.Visible = false;
            // 
            // btnStart
            // 
            this.btnStart.BorderRadius = 10;
            this.btnStart.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnStart.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(440, 445);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(190, 45);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "🚀  Bắt Đầu Nhập";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 10;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnCancel.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(280, 445);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 45);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "✕  Đóng";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.TargetControl = this.pnlMain;
            // 
            // FrmBatchImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(650, 520);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmBatchImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Import";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlTextContainer.ResumeLayout(false);
            this.pnlInstruction.ResumeLayout(false);
            this.pnlInstruction.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private Guna.UI2.WinForms.Guna2Panel pnlTextContainer;
        private System.Windows.Forms.RichTextBox txtUrls;
        private Guna.UI2.WinForms.Guna2Panel pnlInstruction;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Label lblStatus;
        private Guna.UI2.WinForms.Guna2ProgressBar progressBar;
        private Guna.UI2.WinForms.Guna2GradientButton btnStart;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}
