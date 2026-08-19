namespace Person_Movie_Management.Forms
{
    partial class FrmInputBox
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
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            pnlTop = new Guna.UI2.WinForms.Guna2Panel();
            pnlIconBadge = new Guna.UI2.WinForms.Guna2Panel();
            lblIcon = new Label();
            lblTitle = new Label();
            btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            lblPrompt = new Label();
            txtInput = new Guna.UI2.WinForms.Guna2TextBox();
            lblWarningNote = new Label();
            btnHardDelete = new Guna.UI2.WinForms.Guna2GradientButton();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            btnOk = new Guna.UI2.WinForms.Guna2GradientButton();
            guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(components);
            pnlTop.SuspendLayout();
            pnlIconBadge.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 16;
            guna2Elipse1.TargetControl = this;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(pnlIconBadge);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(btnClose);
            pnlTop.CustomizableEdges = customizableEdges13;
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlTop.Size = new Size(540, 52);
            pnlTop.TabIndex = 0;
            // 
            // pnlIconBadge
            // 
            pnlIconBadge.BorderRadius = 8;
            pnlIconBadge.Controls.Add(lblIcon);
            pnlIconBadge.CustomizableEdges = customizableEdges9;
            pnlIconBadge.FillColor = Color.FromArgb(127, 29, 29);
            pnlIconBadge.Location = new Point(20, 11);
            pnlIconBadge.Name = "pnlIconBadge";
            pnlIconBadge.ShadowDecoration.CustomizableEdges = customizableEdges10;
            pnlIconBadge.Size = new Size(30, 30);
            pnlIconBadge.TabIndex = 2;
            // 
            // lblIcon
            // 
            lblIcon.BackColor = Color.Transparent;
            lblIcon.Dock = DockStyle.Fill;
            lblIcon.Font = new Font("Segoe UI", 11F);
            lblIcon.ForeColor = Color.White;
            lblIcon.Location = new Point(0, 0);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(30, 30);
            lblIcon.TabIndex = 0;
            lblIcon.Text = "⚠️";
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(58, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(139, 28);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Xác nhận xóa";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BorderRadius = 6;
            btnClose.CustomizableEdges = customizableEdges11;
            btnClose.FillColor = Color.Transparent;
            btnClose.HoverState.FillColor = Color.FromArgb(239, 68, 68);
            btnClose.HoverState.IconColor = Color.White;
            btnClose.IconColor = Color.FromArgb(148, 163, 184);
            btnClose.Location = new Point(492, 6);
            btnClose.Name = "btnClose";
            btnClose.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnClose.Size = new Size(40, 40);
            btnClose.TabIndex = 0;
            // 
            // lblPrompt
            // 
            lblPrompt.Font = new Font("Segoe UI", 10F);
            lblPrompt.ForeColor = Color.FromArgb(203, 213, 225);
            lblPrompt.Location = new Point(24, 62);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(492, 28);
            lblPrompt.TabIndex = 1;
            lblPrompt.Text = "Nhập 'delete' để xóa TẤT CẢ mục trên trang này:";
            // 
            // txtInput
            // 
            txtInput.BorderColor = Color.FromArgb(71, 85, 105);
            txtInput.BorderRadius = 10;
            txtInput.Cursor = Cursors.IBeam;
            txtInput.CustomizableEdges = customizableEdges7;
            txtInput.DefaultText = "";
            txtInput.FillColor = Color.FromArgb(15, 23, 42);
            txtInput.FocusedState.BorderColor = Color.FromArgb(239, 68, 68);
            txtInput.Font = new Font("Segoe UI", 10.5F);
            txtInput.ForeColor = Color.White;
            txtInput.Location = new Point(24, 95);
            txtInput.Margin = new Padding(3, 4, 3, 4);
            txtInput.Name = "txtInput";
            txtInput.PlaceholderForeColor = Color.FromArgb(100, 116, 139);
            txtInput.PlaceholderText = "Nhập 'delete' để xác nhận...";
            txtInput.SelectedText = "";
            txtInput.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtInput.Size = new Size(492, 44);
            txtInput.TabIndex = 2;
            txtInput.TextOffset = new Point(8, 0);
            // 
            // lblWarningNote
            // 
            lblWarningNote.Font = new Font("Segoe UI", 9.25F);
            lblWarningNote.ForeColor = Color.FromArgb(251, 146, 160);
            lblWarningNote.Location = new Point(24, 146);
            lblWarningNote.Name = "lblWarningNote";
            lblWarningNote.Size = new Size(492, 26);
            lblWarningNote.TabIndex = 3;
            lblWarningNote.Text = "💡 Lưu ý: 'Vào Thùng Rác' có thể khôi phục, 'Xóa Vĩnh Viễn' không thể phục hồi.";
            // 
            // btnHardDelete
            // 
            btnHardDelete.BorderRadius = 10;
            btnHardDelete.Cursor = Cursors.Hand;
            btnHardDelete.CustomizableEdges = customizableEdges1;
            btnHardDelete.FillColor = Color.FromArgb(239, 68, 68);
            btnHardDelete.FillColor2 = Color.FromArgb(220, 38, 38);
            btnHardDelete.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnHardDelete.ForeColor = Color.White;
            btnHardDelete.HoverState.FillColor = Color.FromArgb(220, 38, 38);
            btnHardDelete.HoverState.FillColor2 = Color.FromArgb(185, 28, 28);
            btnHardDelete.Location = new Point(24, 182);
            btnHardDelete.Name = "btnHardDelete";
            btnHardDelete.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnHardDelete.Size = new Size(145, 42);
            btnHardDelete.TabIndex = 4;
            btnHardDelete.Text = "Xóa Vĩnh Viễn";
            btnHardDelete.Click += btnHardDelete_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderRadius = 10;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.CustomizableEdges = customizableEdges3;
            btnCancel.FillColor = Color.FromArgb(51, 65, 85);
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(203, 213, 225);
            btnCancel.HoverState.FillColor = Color.FromArgb(71, 85, 105);
            btnCancel.HoverState.ForeColor = Color.White;
            btnCancel.Location = new Point(255, 182);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnCancel.Size = new Size(115, 42);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Hủy Bỏ";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.BorderRadius = 10;
            btnOk.Cursor = Cursors.Hand;
            btnOk.CustomizableEdges = customizableEdges5;
            btnOk.FillColor = Color.FromArgb(99, 102, 241);
            btnOk.FillColor2 = Color.FromArgb(129, 140, 248);
            btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.HoverState.FillColor = Color.FromArgb(79, 70, 229);
            btnOk.HoverState.FillColor2 = Color.FromArgb(99, 102, 241);
            btnOk.Location = new Point(380, 182);
            btnOk.Name = "btnOk";
            btnOk.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnOk.Size = new Size(136, 42);
            btnOk.TabIndex = 6;
            btnOk.Text = "Xác Nhận";
            btnOk.Click += btnOk_Click;
            // 
            // guna2DragControl1
            // 
            guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            guna2DragControl1.TargetControl = pnlTop;
            guna2DragControl1.UseTransparentDrag = true;
            // 
            // FrmInputBox
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(30, 41, 59);
            ClientSize = new Size(540, 248);
            Controls.Add(btnHardDelete);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(lblWarningNote);
            Controls.Add(txtInput);
            Controls.Add(lblPrompt);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmInputBox";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlIconBadge.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private Guna.UI2.WinForms.Guna2Panel pnlIconBadge;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private System.Windows.Forms.Label lblPrompt;
        private Guna.UI2.WinForms.Guna2TextBox txtInput;
        private System.Windows.Forms.Label lblWarningNote;
        private Guna.UI2.WinForms.Guna2GradientButton btnHardDelete;
        private Guna.UI2.WinForms.Guna2GradientButton btnOk;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
    }
}
