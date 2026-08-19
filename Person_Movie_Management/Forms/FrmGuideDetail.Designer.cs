namespace Person_Movie_Management.Forms
{
    partial class FrmGuideDetail
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
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.flpNav = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlContent = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlBody = new Guna.UI2.WinForms.Guna2Panel();
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.pnlGuideHeader = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTopicDesc = new System.Windows.Forms.Label();
            this.lblTopicTitle = new System.Windows.Forms.Label();
            this.lblTopicBadge = new System.Windows.Forms.Label();
            this.pnlFooter = new Guna.UI2.WinForms.Guna2Panel();
            this.btnPrev = new Guna.UI2.WinForms.Guna2Button();
            this.btnNext = new Guna.UI2.WinForms.Guna2Button();
            this.btnOK = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pnlMain.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlGuideHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
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
            this.pnlMain.Controls.Add(this.pnlContent);
            this.pnlMain.Controls.Add(this.pnlSidebar);
            this.pnlMain.Controls.Add(this.pnlTop);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(14)))), ((int)(((byte)(26)))));
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(980, 680);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Transparent;
            this.pnlTop.Controls.Add(this.lblHeaderTitle);
            this.pnlTop.Controls.Add(this.btnClose);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(980, 52);
            this.pnlTop.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.Location = new System.Drawing.Point(20, 12);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(340, 30);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "📖 Sổ Tay Hướng Dẫn Sử Dụng";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Animated = true;
            this.btnClose.BorderRadius = 8;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.IconColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(926, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(42, 30);
            this.btnClose.TabIndex = 1;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.Transparent;
            this.pnlSidebar.Controls.Add(this.flpNav);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 52);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(265, 628);
            this.pnlSidebar.TabIndex = 1;
            // 
            // flpNav
            // 
            this.flpNav.AutoScroll = true;
            this.flpNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpNav.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpNav.Location = new System.Drawing.Point(0, 0);
            this.flpNav.Name = "flpNav";
            this.flpNav.Padding = new System.Windows.Forms.Padding(12, 6, 8, 12);
            this.flpNav.Size = new System.Drawing.Size(265, 628);
            this.flpNav.TabIndex = 0;
            this.flpNav.WrapContents = false;
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.Transparent;
            this.pnlContent.Controls.Add(this.pnlBody);
            this.pnlContent.Controls.Add(this.pnlFooter);
            this.pnlContent.Controls.Add(this.pnlGuideHeader);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(265, 52);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(10, 0, 16, 12);
            this.pnlContent.Size = new System.Drawing.Size(715, 628);
            this.pnlContent.TabIndex = 2;
            // 
            // pnlGuideHeader
            // 
            this.pnlGuideHeader.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.pnlGuideHeader.BorderRadius = 14;
            this.pnlGuideHeader.BorderThickness = 1;
            this.pnlGuideHeader.Controls.Add(this.lblTopicDesc);
            this.pnlGuideHeader.Controls.Add(this.lblTopicTitle);
            this.pnlGuideHeader.Controls.Add(this.lblTopicBadge);
            this.pnlGuideHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGuideHeader.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(34)))), ((int)(((byte)(58)))));
            this.pnlGuideHeader.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(22)))), ((int)(((byte)(38)))));
            this.pnlGuideHeader.Location = new System.Drawing.Point(10, 0);
            this.pnlGuideHeader.Name = "pnlGuideHeader";
            this.pnlGuideHeader.Size = new System.Drawing.Size(689, 92);
            this.pnlGuideHeader.TabIndex = 0;
            // 
            // lblTopicBadge
            // 
            this.lblTopicBadge.AutoSize = true;
            this.lblTopicBadge.BackColor = System.Drawing.Color.Transparent;
            this.lblTopicBadge.Font = new System.Drawing.Font("Segoe UI Emoji", 26F);
            this.lblTopicBadge.Location = new System.Drawing.Point(16, 16);
            this.lblTopicBadge.Name = "lblTopicBadge";
            this.lblTopicBadge.Size = new System.Drawing.Size(67, 59);
            this.lblTopicBadge.TabIndex = 0;
            this.lblTopicBadge.Text = "🌐";
            // 
            // lblTopicTitle
            // 
            this.lblTopicTitle.AutoSize = true;
            this.lblTopicTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTopicTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTopicTitle.ForeColor = System.Drawing.Color.White;
            this.lblTopicTitle.Location = new System.Drawing.Point(90, 16);
            this.lblTopicTitle.Name = "lblTopicTitle";
            this.lblTopicTitle.Size = new System.Drawing.Size(265, 32);
            this.lblTopicTitle.TabIndex = 1;
            this.lblTopicTitle.Text = "Trang Phim Online";
            // 
            // lblTopicDesc
            // 
            this.lblTopicDesc.AutoSize = true;
            this.lblTopicDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblTopicDesc.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblTopicDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblTopicDesc.Location = new System.Drawing.Point(92, 52);
            this.lblTopicDesc.Name = "lblTopicDesc";
            this.lblTopicDesc.Size = new System.Drawing.Size(460, 21);
            this.lblTopicDesc.TabIndex = 2;
            this.lblTopicDesc.Text = "Mô tả ngắn về chức năng trang...";
            // 
            // pnlBody
            // 
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(19)))), ((int)(((byte)(34)))));
            this.pnlBody.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(50)))), ((int)(((byte)(75)))));
            this.pnlBody.BorderRadius = 14;
            this.pnlBody.BorderThickness = 1;
            this.pnlBody.Controls.Add(this.txtContent);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(10, 92);
            this.pnlBody.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(16, 14, 16, 14);
            this.pnlBody.Size = new System.Drawing.Size(689, 466);
            this.pnlBody.TabIndex = 1;
            // 
            // txtContent
            // 
            this.txtContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(19)))), ((int)(((byte)(34)))));
            this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContent.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtContent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.txtContent.Location = new System.Drawing.Point(16, 14);
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(657, 438);
            this.txtContent.TabIndex = 0;
            this.txtContent.Text = "";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.Transparent;
            this.pnlFooter.Controls.Add(this.btnPrev);
            this.pnlFooter.Controls.Add(this.btnNext);
            this.pnlFooter.Controls.Add(this.btnOK);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(10, 558);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlFooter.Size = new System.Drawing.Size(689, 58);
            this.pnlFooter.TabIndex = 2;
            // 
            // btnPrev
            // 
            this.btnPrev.Animated = true;
            this.btnPrev.BorderRadius = 10;
            this.btnPrev.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnPrev.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnPrev.ForeColor = System.Drawing.Color.White;
            this.btnPrev.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnPrev.Location = new System.Drawing.Point(0, 10);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(140, 42);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "◀  Trang trước";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Animated = true;
            this.btnNext.BorderRadius = 10;
            this.btnNext.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnNext.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.btnNext.Location = new System.Drawing.Point(150, 10);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(140, 42);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Trang sau  ▶";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Animated = true;
            this.btnOK.BorderRadius = 10;
            this.btnOK.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnOK.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(549, 10);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 42);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Đã hiểu ✓";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmGuideDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(14)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(980, 680);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmGuideDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hướng Dẫn Sử Dụng";
            this.pnlMain.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlSidebar.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlGuideHeader.ResumeLayout(false);
            this.pnlGuideHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlTop;
        private System.Windows.Forms.Label lblHeaderTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar;
        private System.Windows.Forms.FlowLayoutPanel flpNav;
        private Guna.UI2.WinForms.Guna2Panel pnlContent;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlGuideHeader;
        private System.Windows.Forms.Label lblTopicBadge;
        private System.Windows.Forms.Label lblTopicTitle;
        private System.Windows.Forms.Label lblTopicDesc;
        private Guna.UI2.WinForms.Guna2Panel pnlBody;
        private System.Windows.Forms.RichTextBox txtContent;
        private Guna.UI2.WinForms.Guna2Panel pnlFooter;
        private Guna.UI2.WinForms.Guna2Button btnPrev;
        private Guna.UI2.WinForms.Guna2Button btnNext;
        private Guna.UI2.WinForms.Guna2GradientButton btnOK;
    }
}
