namespace Person_Movie_Management.UserControls
{
    partial class UcActorList
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlTop = new Panel();
            btnAdd = new Guna.UI2.WinForms.Guna2GradientButton();
            btnDeleteAll = new Guna.UI2.WinForms.Guna2Button();
            btnNationalities = new Guna.UI2.WinForms.Guna2Button();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            lblTitle = new Label();
            flowLayoutPanel = new Person_Movie_Management.Helpers.VirtualWrapPanel();
            lblEmpty = new Label();
            pnlTop.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(btnAdd);
            pnlTop.Controls.Add(btnDeleteAll);
            pnlTop.Controls.Add(btnNationalities);
            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(3, 4, 3, 4);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(29, 27, 29, 13);
            pnlTop.Size = new Size(1086, 107);
            pnlTop.TabIndex = 0;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.BorderRadius = 12;
            btnAdd.CustomizableEdges = customizableEdges1;
            btnAdd.FillColor = Color.FromArgb(139, 92, 246);
            btnAdd.FillColor2 = Color.FromArgb(236, 72, 153);
            btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(910, 24);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAdd.Size = new Size(147, 56);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "+ Thêm mới";
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDeleteAll
            // 
            btnDeleteAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteAll.BorderColor = Color.FromArgb(127, 29, 29);
            btnDeleteAll.BorderRadius = 12;
            btnDeleteAll.BorderThickness = 1;
            btnDeleteAll.Cursor = Cursors.Hand;
            btnDeleteAll.CustomizableEdges = customizableEdges3;
            btnDeleteAll.FillColor = Color.FromArgb(60, 20, 20);
            btnDeleteAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDeleteAll.ForeColor = Color.FromArgb(252, 165, 165);
            btnDeleteAll.HoverState.BorderColor = Color.FromArgb(239, 68, 68);
            btnDeleteAll.HoverState.FillColor = Color.FromArgb(239, 68, 68);
            btnDeleteAll.HoverState.ForeColor = Color.White;
            btnDeleteAll.Location = new Point(760, 24);
            btnDeleteAll.Margin = new Padding(3, 4, 3, 4);
            btnDeleteAll.Name = "btnDeleteAll";
            btnDeleteAll.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnDeleteAll.Size = new Size(143, 56);
            btnDeleteAll.TabIndex = 5;
            btnDeleteAll.Text = "🗑 Xóa tất cả";
            btnDeleteAll.Click += btnDeleteAll_Click;
            // 
            // btnNationalities
            // 
            btnNationalities.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNationalities.BorderColor = Color.FromArgb(62, 75, 122);
            btnNationalities.BorderRadius = 12;
            btnNationalities.BorderThickness = 1;
            btnNationalities.Cursor = Cursors.Hand;
            btnNationalities.CustomizableEdges = customizableEdges5;
            btnNationalities.FillColor = Color.FromArgb(34, 42, 78);
            btnNationalities.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNationalities.ForeColor = Color.FromArgb(224, 231, 255);
            btnNationalities.HoverState.BorderColor = Color.FromArgb(139, 92, 246);
            btnNationalities.HoverState.FillColor = Color.FromArgb(139, 92, 246);
            btnNationalities.HoverState.ForeColor = Color.White;
            btnNationalities.Location = new Point(583, 24);
            btnNationalities.Margin = new Padding(3, 4, 3, 4);
            btnNationalities.Name = "btnNationalities";
            btnNationalities.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnNationalities.Size = new Size(166, 56);
            btnNationalities.TabIndex = 2;
            btnNationalities.Text = "🌐 Sửa quốc tịch";
            btnNationalities.Click += btnNationalities_Click;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSearch.BorderRadius = 12;
            txtSearch.Cursor = Cursors.IBeam;
            txtSearch.CustomizableEdges = customizableEdges7;
            txtSearch.DefaultText = "";
            txtSearch.FillColor = Color.FromArgb(22, 28, 56);
            txtSearch.FocusedState.BorderColor = Color.FromArgb(139, 92, 246);
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.ForeColor = Color.FromArgb(241, 245, 249);
            txtSearch.Location = new Point(309, 24);
            txtSearch.Margin = new Padding(5, 5, 5, 5);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderForeColor = Color.FromArgb(100, 116, 139);
            txtSearch.PlaceholderText = "🔍  Tìm kiếm diễn viên...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtSearch.Size = new Size(263, 56);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(241, 245, 249);
            lblTitle.Location = new Point(29, 29);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(182, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "💃 Diễn viên";
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.BackColor = Color.FromArgb(15, 23, 42);
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.Location = new Point(0, 107);
            flowLayoutPanel.Margin = new Padding(3, 4, 3, 4);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Padding = new Padding(23, 27, 23, 27);
            flowLayoutPanel.Size = new Size(1086, 693);
            flowLayoutPanel.TabIndex = 1;
            // 
            // lblEmpty
            // 
            lblEmpty.Dock = DockStyle.Fill;
            lblEmpty.Font = new Font("Segoe UI", 14F);
            lblEmpty.ForeColor = Color.FromArgb(100, 116, 139);
            lblEmpty.Location = new Point(0, 107);
            lblEmpty.Name = "lblEmpty";
            lblEmpty.Size = new Size(1086, 693);
            lblEmpty.TabIndex = 2;
            lblEmpty.Text = "Không có diễn viên nào. Nhấn + Thêm mới để bắt đầu.";
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UcActorList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(flowLayoutPanel);
            Controls.Add(lblEmpty);
            Controls.Add(pnlTop);
            Margin = new Padding(3, 4, 3, 4);
            Name = "UcActorList";
            Size = new Size(1086, 800);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnNationalities;
        private Guna.UI2.WinForms.Guna2Button btnDeleteAll;
        private Guna.UI2.WinForms.Guna2GradientButton btnAdd;
        private Person_Movie_Management.Helpers.VirtualWrapPanel flowLayoutPanel;
        private System.Windows.Forms.Label lblEmpty;
    }
}
