using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


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
            txtUsername.MaxLength = 30;
            txtDisplayName.MaxLength = 20;
            txtEmail.MaxLength = 70;
            txtPassword.MaxLength = 30;
            txtConfirmPassword.MaxLength = 30;
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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, pattern);
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

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!IsValidEmail(txtEmail.Text.Trim()))
                {
                    MessageBox.Show(
                        "Email không hợp lệ!\n\n" +
                        "Vui lòng sử dụng email như:\n" +
                        "example@gmail.com\n" +
                        "example@outlook.com\n" +
                        "example@hotmail.com" +
                        "......@.....com",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtEmail.Focus();
                    return;
                }
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
