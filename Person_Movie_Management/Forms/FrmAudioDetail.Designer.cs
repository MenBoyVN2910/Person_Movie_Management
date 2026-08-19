namespace Person_Movie_Management.Forms
{
    partial class FrmAudioDetail
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
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picCover = new System.Windows.Forms.PictureBox();
            this.lblCoverHint = new System.Windows.Forms.Label();
            this.txtAudioCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnChooseAudio = new Guna.UI2.WinForms.Guna2Button();
            this.lblSelectedAudio = new System.Windows.Forms.Label();
            this.txtNote = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblCharCount = new System.Windows.Forms.Label();
            this.btnSave = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
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
            // pnlMain
            // 
            this.pnlMain.BorderRadius = 16;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.lblCharCount);
            this.pnlMain.Controls.Add(this.txtNote);
            this.pnlMain.Controls.Add(this.btnChooseAudio);
            this.pnlMain.Controls.Add(this.lblSelectedAudio);
            this.pnlMain.Controls.Add(this.txtAudioCode);
            this.pnlMain.Controls.Add(this.lblCoverHint);
            this.pnlMain.Controls.Add(this.picCover);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(750, 600);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitle.Location = new System.Drawing.Point(25, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(230, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🎵 Thêm Âm Thanh";
            // 
            // picCover
            // 
            this.picCover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(38)))), ((int)(((byte)(72)))));
            this.picCover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCover.Location = new System.Drawing.Point(25, 80);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(220, 220);
            this.picCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCover.TabIndex = 1;
            this.picCover.TabStop = false;
            this.picCover.Click += new System.EventHandler(this.picCover_Click);
            // 
            // lblCoverHint
            // 
            this.lblCoverHint.BackColor = System.Drawing.Color.Transparent;
            this.lblCoverHint.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblCoverHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCoverHint.Location = new System.Drawing.Point(25, 305);
            this.lblCoverHint.Name = "lblCoverHint";
            this.lblCoverHint.Size = new System.Drawing.Size(220, 20);
            this.lblCoverHint.TabIndex = 2;
            this.lblCoverHint.Text = "📷 Click để chọn ảnh bìa";
            this.lblCoverHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCoverHint.Click += new System.EventHandler(this.picCover_Click);
            // 
            // txtAudioCode
            // 
            this.txtAudioCode.BorderRadius = 10;
            this.txtAudioCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAudioCode.DefaultText = "";
            this.txtAudioCode.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtAudioCode.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtAudioCode.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAudioCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtAudioCode.Location = new System.Drawing.Point(280, 80);
            this.txtAudioCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtAudioCode.Name = "txtAudioCode";
            this.txtAudioCode.PasswordChar = '\0';
            this.txtAudioCode.PlaceholderText = "🎵 Tên bài hát / Mã âm thanh";
            this.txtAudioCode.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.txtAudioCode.SelectedText = "";
            this.txtAudioCode.Size = new System.Drawing.Size(430, 42);
            this.txtAudioCode.TabIndex = 3;
            // 
            // btnChooseAudio
            // 
            this.btnChooseAudio.BorderRadius = 10;
            this.btnChooseAudio.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.btnChooseAudio.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnChooseAudio.ForeColor = System.Drawing.Color.White;
            this.btnChooseAudio.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnChooseAudio.Location = new System.Drawing.Point(280, 140);
            this.btnChooseAudio.Name = "btnChooseAudio";
            this.btnChooseAudio.Size = new System.Drawing.Size(180, 42);
            this.btnChooseAudio.TabIndex = 4;
            this.btnChooseAudio.Text = "🎧 Chọn file .mp3";
            this.btnChooseAudio.Click += new System.EventHandler(this.btnChooseAudio_Click);
            // 
            // lblSelectedAudio
            // 
            this.lblSelectedAudio.AutoSize = true;
            this.lblSelectedAudio.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedAudio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSelectedAudio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSelectedAudio.Location = new System.Drawing.Point(475, 151);
            this.lblSelectedAudio.Name = "lblSelectedAudio";
            this.lblSelectedAudio.Size = new System.Drawing.Size(147, 19);
            this.lblSelectedAudio.TabIndex = 5;
            this.lblSelectedAudio.Text = "Chưa chọn file (Max 50MB, 20p)";
            // 
            // txtNote
            // 
            this.txtNote.BorderRadius = 10;
            this.txtNote.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNote.DefaultText = "";
            this.txtNote.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtNote.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtNote.Location = new System.Drawing.Point(280, 200);
            this.txtNote.Margin = new System.Windows.Forms.Padding(4);
            this.txtNote.MaxLength = 500;
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.PasswordChar = '\0';
            this.txtNote.PlaceholderText = "📝  Ghi chú (Tối đa 500 ký tự)...";
            this.txtNote.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.txtNote.SelectedText = "";
            this.txtNote.Size = new System.Drawing.Size(430, 240);
            this.txtNote.TabIndex = 6;
            // 
            // lblCharCount
            // 
            this.lblCharCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCharCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblCharCount.Location = new System.Drawing.Point(280, 445);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new System.Drawing.Size(430, 20);
            this.lblCharCount.TabIndex = 9;
            this.lblCharCount.Text = "0 / 500 ký tự";
            this.lblCharCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 12;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(530, 480);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 45);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "✓  LƯU LẠI";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 12;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnCancel.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(330, 480);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 45);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "✕  HỦY";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmAudioDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(750, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmAudioDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audio Detail";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picCover;
        private System.Windows.Forms.Label lblCoverHint;
        private Guna.UI2.WinForms.Guna2TextBox txtAudioCode;
        private Guna.UI2.WinForms.Guna2Button btnChooseAudio;
        private System.Windows.Forms.Label lblSelectedAudio;
        private Guna.UI2.WinForms.Guna2TextBox txtNote;
        private System.Windows.Forms.Label lblCharCount;
        private Guna.UI2.WinForms.Guna2GradientButton btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}
