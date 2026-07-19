using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmRegister : Form
    {
        private readonly AuthService _authService;

        public FrmRegister()
        {
            InitializeComponent();
            _authService = new AuthService();
            
            // Applying UI Theme Colors
            this.BackColor = UIHelper.BgDark;
            
            // Left Panel Gradient
            pnlLeft.FillColor = UIHelper.GradViolet1;
            pnlLeft.FillColor2 = UIHelper.GradRose1;
            pnlLeft.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            lblTitle.ForeColor = Color.White;
            lblSubtitle.ForeColor = Color.FromArgb(220, 255, 255, 255);
            
            // Textboxes
            StyleTextBox(txtUsername);
            StyleTextBox(txtDisplayName);
            StyleTextBox(txtEmail);
            StyleTextBox(txtPassword);
            StyleTextBox(txtConfirmPassword);
            
            // Register Button
            btnRegister.FillColor = UIHelper.GradViolet1;
            btnRegister.FillColor2 = UIHelper.GradRose1;
            btnRegister.Animated = true;
            
            lnkLogin.LinkColor = UIHelper.AccentPrimary;
        }

        private void StyleTextBox(Guna.UI2.WinForms.Guna2TextBox txt)
        {
            txt.FillColor = UIHelper.BgCard;
            txt.BorderColor = UIHelper.Border;
            txt.BorderRadius = 12;
            txt.ForeColor = UIHelper.TextPrimary;
            txt.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txt.HoverState.BorderColor = UIHelper.AccentSecondary;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (success, message) = _authService.Register(
                txtUsername.Text, 
                txtDisplayName.Text, 
                txtPassword.Text, 
                string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text
            );
            
            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FrmLogin login = new FrmLogin();
                login.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(message, "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmLogin login = new FrmLogin();
            login.Show();
            this.Hide();
        }
    }
}
