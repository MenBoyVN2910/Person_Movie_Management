namespace Person_Movie_Management.Forms
{
    partial class FrmUpdateProgress
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
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.pnlTop = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblPercentDisplay = new System.Windows.Forms.Label();
            this.trkProgress = new Guna.UI2.WinForms.Guna2TrackBar();
            this.txtDirectInput = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblInputHint = new System.Windows.Forms.Label();
            this.pnlPresets = new System.Windows.Forms.FlowLayoutPanel();
            this.btn0 = new Guna.UI2.WinForms.Guna2Button();
            this.btn10 = new Guna.UI2.WinForms.Guna2Button();
            this.btn30 = new Guna.UI2.WinForms.Guna2Button();
            this.btn50 = new Guna.UI2.WinForms.Guna2Button();
            this.btn70 = new Guna.UI2.WinForms.Guna2Button();
            this.btn90 = new Guna.UI2.WinForms.Guna2Button();
            this.btn100 = new Guna.UI2.WinForms.Guna2Button();
            this.btnSave = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlPresets.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 16;
            this.guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.pnlTop;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // pnlMain
            // 
            this.pnlMain.BorderRadius = 16;
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.pnlPresets);
            this.pnlMain.Controls.Add(this.lblInputHint);
            this.pnlMain.Controls.Add(this.txtDirectInput);
            this.pnlMain.Controls.Add(this.trkProgress);
            this.pnlMain.Controls.Add(this.lblPercentDisplay);
            this.pnlMain.Controls.Add(this.lblItemName);
            this.pnlMain.Controls.Add(this.pnlTop);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(480, 360);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Transparent;
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(480, 48);
            this.pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitle.Location = new System.Drawing.Point(18, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(185, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "⏱️ Cập Nhật Tiến Độ";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.IconColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(435, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 1;
            // 
            // lblItemName
            // 
            this.lblItemName.BackColor = System.Drawing.Color.Transparent;
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblItemName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblItemName.Location = new System.Drawing.Point(20, 52);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(440, 22);
            this.lblItemName.TabIndex = 1;
            this.lblItemName.Text = "Tiêu đề phim / âm thanh";
            // 
            // lblPercentDisplay
            // 
            this.lblPercentDisplay.BackColor = System.Drawing.Color.Transparent;
            this.lblPercentDisplay.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPercentDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.lblPercentDisplay.Location = new System.Drawing.Point(20, 78);
            this.lblPercentDisplay.Name = "lblPercentDisplay";
            this.lblPercentDisplay.Size = new System.Drawing.Size(440, 50);
            this.lblPercentDisplay.TabIndex = 2;
            this.lblPercentDisplay.Text = "50%";
            this.lblPercentDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trkProgress
            // 
            this.trkProgress.BackColor = System.Drawing.Color.Transparent;
            this.trkProgress.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.trkProgress.HoverState.ThumbColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(139)))), ((int)(((byte)(250)))));
            this.trkProgress.Location = new System.Drawing.Point(25, 135);
            this.trkProgress.Name = "trkProgress";
            this.trkProgress.Size = new System.Drawing.Size(345, 30);
            this.trkProgress.TabIndex = 3;
            this.trkProgress.ThumbColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.trkProgress.Value = 50;
            this.trkProgress.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trkProgress_Scroll);
            // 
            // txtDirectInput
            // 
            this.txtDirectInput.BorderRadius = 8;
            this.txtDirectInput.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDirectInput.DefaultText = "50";
            this.txtDirectInput.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.txtDirectInput.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtDirectInput.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtDirectInput.ForeColor = System.Drawing.Color.White;
            this.txtDirectInput.Location = new System.Drawing.Point(380, 133);
            this.txtDirectInput.Margin = new System.Windows.Forms.Padding(4);
            this.txtDirectInput.MaxLength = 3;
            this.txtDirectInput.Name = "txtDirectInput";
            this.txtDirectInput.PasswordChar = ' ';
            this.txtDirectInput.PlaceholderText = "";
            this.txtDirectInput.SelectedText = "";
            this.txtDirectInput.Size = new System.Drawing.Size(65, 34);
            this.txtDirectInput.TabIndex = 4;
            this.txtDirectInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDirectInput.TextChanged += new System.EventHandler(this.txtDirectInput_TextChanged);
            // 
            // lblInputHint
            // 
            this.lblInputHint.AutoSize = true;
            this.lblInputHint.BackColor = System.Drawing.Color.Transparent;
            this.lblInputHint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblInputHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblInputHint.Location = new System.Drawing.Point(448, 140);
            this.lblInputHint.Name = "lblInputHint";
            this.lblInputHint.Size = new System.Drawing.Size(23, 20);
            this.lblInputHint.TabIndex = 5;
            this.lblInputHint.Text = "%";
            // 
            // pnlPresets
            // 
            this.pnlPresets.BackColor = System.Drawing.Color.Transparent;
            this.pnlPresets.Controls.Add(this.btn0);
            this.pnlPresets.Controls.Add(this.btn10);
            this.pnlPresets.Controls.Add(this.btn30);
            this.pnlPresets.Controls.Add(this.btn50);
            this.pnlPresets.Controls.Add(this.btn70);
            this.pnlPresets.Controls.Add(this.btn90);
            this.pnlPresets.Controls.Add(this.btn100);
            this.pnlPresets.Location = new System.Drawing.Point(20, 180);
            this.pnlPresets.Name = "pnlPresets";
            this.pnlPresets.Size = new System.Drawing.Size(440, 95);
            this.pnlPresets.TabIndex = 6;
            // 
            // btn0
            // 
            this.btn0.BorderRadius = 8;
            this.btn0.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn0.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn0.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn0.Location = new System.Drawing.Point(3, 3);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(138, 38);
            this.btn0.TabIndex = 0;
            this.btn0.Text = "0% (Chưa xem)";
            this.btn0.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn10
            // 
            this.btn10.BorderRadius = 8;
            this.btn10.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn10.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn10.Location = new System.Drawing.Point(147, 3);
            this.btn10.Name = "btn10";
            this.btn10.Size = new System.Drawing.Size(65, 38);
            this.btn10.TabIndex = 1;
            this.btn10.Text = "10%";
            this.btn10.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn30
            // 
            this.btn30.BorderRadius = 8;
            this.btn30.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn30.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn30.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn30.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn30.Location = new System.Drawing.Point(218, 3);
            this.btn30.Name = "btn30";
            this.btn30.Size = new System.Drawing.Size(65, 38);
            this.btn30.TabIndex = 2;
            this.btn30.Text = "30%";
            this.btn30.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn50
            // 
            this.btn50.BorderRadius = 8;
            this.btn50.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn50.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn50.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn50.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn50.Location = new System.Drawing.Point(289, 3);
            this.btn50.Name = "btn50";
            this.btn50.Size = new System.Drawing.Size(65, 38);
            this.btn50.TabIndex = 3;
            this.btn50.Text = "50%";
            this.btn50.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn70
            // 
            this.btn70.BorderRadius = 8;
            this.btn70.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn70.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn70.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn70.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn70.Location = new System.Drawing.Point(360, 3);
            this.btn70.Name = "btn70";
            this.btn70.Size = new System.Drawing.Size(65, 38);
            this.btn70.TabIndex = 4;
            this.btn70.Text = "70%";
            this.btn70.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn90
            // 
            this.btn90.BorderRadius = 8;
            this.btn90.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn90.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn90.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btn90.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn90.Location = new System.Drawing.Point(3, 47);
            this.btn90.Name = "btn90";
            this.btn90.Size = new System.Drawing.Size(65, 38);
            this.btn90.TabIndex = 5;
            this.btn90.Text = "90%";
            this.btn90.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btn100
            // 
            this.btn100.BorderRadius = 8;
            this.btn100.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btn100.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn100.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btn100.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btn100.Location = new System.Drawing.Point(74, 47);
            this.btn100.Name = "btn100";
            this.btn100.Size = new System.Drawing.Size(155, 38);
            this.btn100.TabIndex = 6;
            this.btn100.Text = "100% (Đã xem xong)";
            this.btn100.Click += new System.EventHandler(this.btnPreset_Click);
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 10;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(260, 290);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 45);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "✓  LƯU TIẾN ĐỘ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 10;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnCancel.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(20, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 45);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "✕  HỦY";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmUpdateProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(480, 360);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmUpdateProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cập nhật tiến độ";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlPresets.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblPercentDisplay;
        private Guna.UI2.WinForms.Guna2TrackBar trkProgress;
        private Guna.UI2.WinForms.Guna2TextBox txtDirectInput;
        private System.Windows.Forms.Label lblInputHint;
        private System.Windows.Forms.FlowLayoutPanel pnlPresets;
        private Guna.UI2.WinForms.Guna2Button btn0;
        private Guna.UI2.WinForms.Guna2Button btn10;
        private Guna.UI2.WinForms.Guna2Button btn30;
        private Guna.UI2.WinForms.Guna2Button btn50;
        private Guna.UI2.WinForms.Guna2Button btn70;
        private Guna.UI2.WinForms.Guna2Button btn90;
        private Guna.UI2.WinForms.Guna2Button btn100;
        private Guna.UI2.WinForms.Guna2GradientButton btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}
