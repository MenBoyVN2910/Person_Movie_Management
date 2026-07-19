using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmLogin : Form
    {
        private readonly AuthService _authService;

        public FrmLogin()
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
            StyleTextBox(txtPassword);
            
            // Login Button
            btnLogin.FillColor = UIHelper.GradViolet1;
            btnLogin.FillColor2 = UIHelper.GradRose1;
            btnLogin.Animated = true;
            
            lnkRegister.LinkColor = UIHelper.AccentPrimary;
            
            LoadRememberedCredentials();
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
        
        private void LoadRememberedCredentials()
        {
            string rememberFilePath = System.IO.Path.Combine(Person_Movie_Management.Data.DatabaseHelper.AppDataFolder, "remember.dat");
            if (System.IO.File.Exists(rememberFilePath))
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(rememberFilePath);
                    if (lines.Length >= 2)
                    {
                        txtUsername.Text = lines[0];
                        txtPassword.Text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(lines[1]));
                        chkRememberMe.Checked = true;
                    }
                }
                catch { }
            }
        }
        
        private void SaveRememberedCredentials(string username, string password)
        {
            string rememberFilePath = System.IO.Path.Combine(Person_Movie_Management.Data.DatabaseHelper.AppDataFolder, "remember.dat");
            if (chkRememberMe.Checked)
            {
                string encodedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
                System.IO.File.WriteAllLines(rememberFilePath, new[] { username, encodedPassword });
            }
            else
            {
                if (System.IO.File.Exists(rememberFilePath))
                {
                    System.IO.File.Delete(rememberFilePath);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (success, user, message) = _authService.Login(txtUsername.Text, txtPassword.Text);
            
            if (success && user != null)
            {
                SaveRememberedCredentials(txtUsername.Text, txtPassword.Text);
                
                // Mở Form Main
                FrmMain main = new FrmMain();
                main.Show();
                SessionManager.Login(user);
                this.Hide();

            }
            else
            {
                MessageBox.Show(message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister();
            frmRegister.Show();
            this.Hide();
        }
    }
}
