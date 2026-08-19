namespace Person_Movie_Management.Forms
{
    partial class FrmSelectPlaylist
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(components);
            guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(components);
            pnlTop = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Label();
            btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            flpPlaylists = new FlowLayoutPanel();
            btnNewPlaylist = new Guna.UI2.WinForms.Guna2GradientButton();
            pnlTop.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Elipse1
            // 
            guna2Elipse1.BorderRadius = 15;
            guna2Elipse1.TargetControl = this;
            // 
            // guna2DragControl1
            // 
            guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            guna2DragControl1.TargetControl = pnlTop;
            guna2DragControl1.UseTransparentDrag = true;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(btnClose);
            pnlTop.CustomizableEdges = customizableEdges5;
            pnlTop.Dock = DockStyle.Top;
            pnlTop.FillColor = Color.FromArgb(30, 41, 59);
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(3, 4, 3, 4);
            pnlTop.Name = "pnlTop";
            pnlTop.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlTop.Size = new Size(434, 67);
            pnlTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(17, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(232, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "📋 Thêm vào Playlist";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.CustomizableEdges = customizableEdges3;
            btnClose.FillColor = Color.Transparent;
            btnClose.IconColor = Color.White;
            btnClose.Location = new Point(389, 7);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnClose.Size = new Size(40, 47);
            btnClose.TabIndex = 1;
            // 
            // flpPlaylists
            // 
            flpPlaylists.AutoScroll = true;
            flpPlaylists.BackColor = Color.FromArgb(15, 23, 42);
            flpPlaylists.Dock = DockStyle.Fill;
            flpPlaylists.FlowDirection = FlowDirection.TopDown;
            flpPlaylists.Location = new Point(0, 67);
            flpPlaylists.Margin = new Padding(3, 4, 3, 4);
            flpPlaylists.Name = "flpPlaylists";
            flpPlaylists.Padding = new Padding(17, 13, 17, 13);
            flpPlaylists.Size = new Size(434, 426);
            flpPlaylists.TabIndex = 1;
            flpPlaylists.WrapContents = false;
            // 
            // btnNewPlaylist
            // 
            btnNewPlaylist.BorderRadius = 10;
            btnNewPlaylist.CustomizableEdges = customizableEdges1;
            btnNewPlaylist.Dock = DockStyle.Bottom;
            btnNewPlaylist.FillColor = Color.FromArgb(59, 130, 246);
            btnNewPlaylist.FillColor2 = Color.FromArgb(96, 165, 250);
            btnNewPlaylist.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNewPlaylist.ForeColor = Color.White;
            btnNewPlaylist.Location = new Point(0, 493);
            btnNewPlaylist.Margin = new Padding(3, 4, 3, 4);
            btnNewPlaylist.Name = "btnNewPlaylist";
            btnNewPlaylist.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnNewPlaylist.Size = new Size(434, 60);
            btnNewPlaylist.TabIndex = 2;
            btnNewPlaylist.Text = "+ Tạo Playlist mới và thêm";
            btnNewPlaylist.Click += btnNewPlaylist_Click;
            // 
            // FrmSelectPlaylist
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(434, 553);
            Controls.Add(flpPlaylists);
            Controls.Add(btnNewPlaylist);
            Controls.Add(pnlTop);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmSelectPlaylist";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chọn Playlist";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private System.Windows.Forms.FlowLayoutPanel flpPlaylists;
        private Guna.UI2.WinForms.Guna2GradientButton btnNewPlaylist;
    }
}
