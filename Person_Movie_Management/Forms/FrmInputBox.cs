using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public partial class FrmInputBox : Form
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string InputValue { get; private set; } = string.Empty;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsHardDelete { get; private set; } = false;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int MaxLength
        {
            get => txtInput.MaxLength;
            set => txtInput.MaxLength = value;
        }

        public FrmInputBox(string title, string prompt, string defaultValue = "", bool showHardDelete = false, string note = "", string placeholder = "", int maxLength = 0)
        {
            InitializeComponent();
            
            lblTitle.Text = title;
            lblPrompt.Text = prompt;
            if (maxLength > 0)
            {
                txtInput.MaxLength = maxLength;
            }
            txtInput.Text = defaultValue;
            
            ApplyTheme();
            SetupLayout(title, prompt, showHardDelete, note, placeholder);

            this.TopMost = true;
            this.KeyPreview = true;
            this.KeyDown += FrmInputBox_KeyDown;
            txtInput.KeyDown += TxtInput_KeyDown;
            this.Load += (s, e) => { txtInput.Focus(); txtInput.SelectAll(); };
        }

        private void SetupLayout(string title, string prompt, bool showHardDelete, string note, string placeholder)
        {
            bool isDelete = showHardDelete || 
                            title.Contains("xóa", StringComparison.OrdinalIgnoreCase) || 
                            title.Contains("delete", StringComparison.OrdinalIgnoreCase) ||
                            prompt.Contains("delete", StringComparison.OrdinalIgnoreCase);

            if (showHardDelete)
            {
                lblIcon.Text = "⚠️";
                pnlIconBadge.FillColor = Color.FromArgb(127, 29, 29);
                
                btnHardDelete.Visible = true;
                btnHardDelete.Location = new Point(24, 182);
                btnHardDelete.Size = new Size(145, 42);
                btnHardDelete.Text = "Xóa Vĩnh Viễn";

                btnCancel.Location = new Point(180, 182);
                btnCancel.Size = new Size(105, 42);
                btnCancel.Text = "Hủy Bỏ";

                btnOk.Location = new Point(295, 182);
                btnOk.Size = new Size(221, 42);
                btnOk.Text = "Vào Thùng Rác";
                btnOk.FillColor = Color.FromArgb(99, 102, 241);
                btnOk.FillColor2 = Color.FromArgb(129, 140, 248);

                lblWarningNote.Visible = true;
                lblWarningNote.Text = string.IsNullOrEmpty(note) 
                    ? "💡 Lưu ý: 'Vào Thùng Rác' có thể khôi phục, 'Xóa Vĩnh Viễn' không thể phục hồi." 
                    : note;
                lblWarningNote.ForeColor = Color.FromArgb(251, 146, 160);

                txtInput.PlaceholderText = string.IsNullOrEmpty(placeholder) ? "Nhập 'delete' để xác nhận..." : placeholder;
                txtInput.FocusedState.BorderColor = Color.FromArgb(239, 68, 68);
            }
            else if (isDelete)
            {
                lblIcon.Text = "🗑️";
                pnlIconBadge.FillColor = Color.FromArgb(127, 29, 29);

                btnHardDelete.Visible = false;

                btnCancel.Location = new Point(255, 182);
                btnCancel.Size = new Size(115, 42);
                btnCancel.Text = "Hủy Bỏ";

                btnOk.Location = new Point(380, 182);
                btnOk.Size = new Size(136, 42);
                btnOk.Text = "Xác Nhận Xóa";
                btnOk.FillColor = Color.FromArgb(239, 68, 68);
                btnOk.FillColor2 = Color.FromArgb(220, 38, 38);
                btnOk.HoverState.FillColor = Color.FromArgb(220, 38, 38);
                btnOk.HoverState.FillColor2 = Color.FromArgb(185, 28, 28);

                lblWarningNote.Visible = true;
                lblWarningNote.Text = string.IsNullOrEmpty(note) 
                    ? "💡 Lưu ý: Nhập 'delete' để xác nhận xóa toàn bộ." 
                    : note;
                lblWarningNote.ForeColor = Color.FromArgb(251, 146, 160);

                txtInput.PlaceholderText = string.IsNullOrEmpty(placeholder) ? "Nhập 'delete' để xác nhận..." : placeholder;
                txtInput.FocusedState.BorderColor = Color.FromArgb(239, 68, 68);
            }
            else
            {
                lblIcon.Text = "✏️";
                pnlIconBadge.FillColor = Color.FromArgb(67, 56, 202);

                btnHardDelete.Visible = false;

                btnCancel.Location = new Point(255, 182);
                btnCancel.Size = new Size(115, 42);
                btnCancel.Text = "Hủy Bỏ";

                btnOk.Location = new Point(380, 182);
                btnOk.Size = new Size(136, 42);
                btnOk.Text = "Xác Nhận";
                btnOk.FillColor = UIHelper.AccentPrimary;
                btnOk.FillColor2 = UIHelper.AccentSecondary;

                lblWarningNote.Visible = !string.IsNullOrEmpty(note);
                lblWarningNote.Text = note;
                lblWarningNote.ForeColor = Color.FromArgb(148, 163, 184);

                txtInput.PlaceholderText = string.IsNullOrEmpty(placeholder) ? "Nhập nội dung..." : placeholder;
                txtInput.FocusedState.BorderColor = UIHelper.AccentPrimary;
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = UIHelper.BgPanel;
            pnlTop.BackColor = UIHelper.BgDark;
            txtInput.FillColor = UIHelper.BgDark;
            txtInput.ForeColor = UIHelper.TextPrimary;
            txtInput.BorderColor = UIHelper.Border;
            
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblPrompt.ForeColor = UIHelper.TextSecondary;
            
            btnOk.FillColor = UIHelper.AccentPrimary;
            btnOk.FillColor2 = UIHelper.AccentSecondary;
            
            btnCancel.FillColor = UIHelper.BgCard;
            btnCancel.ForeColor = UIHelper.TextSecondary;
            btnCancel.HoverState.FillColor = UIHelper.BgCardHover;
            btnCancel.HoverState.ForeColor = UIHelper.TextPrimary;

            btnHardDelete.FillColor = Color.FromArgb(239, 68, 68);
            btnHardDelete.FillColor2 = Color.FromArgb(220, 38, 38);
            btnHardDelete.HoverState.FillColor = Color.FromArgb(220, 38, 38);
            btnHardDelete.HoverState.FillColor2 = Color.FromArgb(185, 28, 28);
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnOk_Click(sender, EventArgs.Empty);
            }
        }

        private void FrmInputBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                btnCancel_Click(sender, EventArgs.Empty);
            }
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            InputValue = txtInput.Text;
            IsHardDelete = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHardDelete_Click(object? sender, EventArgs e)
        {
            InputValue = txtInput.Text;
            IsHardDelete = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(UIHelper.Border, 1.5f);
            var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using var path = GetRoundedRectanglePath(rect, 16);
            e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
