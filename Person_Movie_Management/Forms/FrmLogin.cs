using System;
using System.Drawing;
using System.Threading.Tasks;
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
            
            // Enter key support
            txtUsername.KeyDown += TxtInput_KeyDown;
            txtPassword.KeyDown += TxtInput_KeyDown;
            
            LoadRememberedCredentials();
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnLogin_Click(this, EventArgs.Empty);
            }
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

        private void ShowError(string message)
        {
            lblError.Text = "⚠ " + message;
            lblError.Visible = true;
        }

        private void HideError()
        {
            lblError.Visible = false;
            lblError.Text = "";
        }

        private void SetLoadingState(bool isLoading)
        {
            btnLogin.Text = isLoading ? "Đang đăng nhập..." : "ĐĂNG NHẬP";
            btnLogin.Enabled = !isLoading;
            txtUsername.Enabled = !isLoading;
            txtPassword.Enabled = !isLoading;
        }
        
        private void LoadRememberedCredentials()
        {
            string rememberFilePath = System.IO.Path.Combine(
                Person_Movie_Management.Data.DatabaseHelper.AppDataFolder, "remember.dat");
            if (System.IO.File.Exists(rememberFilePath))
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(rememberFilePath);
                    if (lines.Length >= 2)
                    {
                        txtUsername.Text = lines[0];
                        txtPassword.Text = System.Text.Encoding.UTF8.GetString(
                            Convert.FromBase64String(lines[1]));
                        chkRememberMe.Checked = true;
                    }
                }
                catch { }
            }
        }
        
        private void SaveRememberedCredentials(string username, string password)
        {
            string rememberFilePath = System.IO.Path.Combine(
                Person_Movie_Management.Data.DatabaseHelper.AppDataFolder, "remember.dat");
            if (chkRememberMe.Checked)
            {
                string encodedPassword = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(password));
                System.IO.File.WriteAllLines(rememberFilePath, new[] { username, encodedPassword });
            }
            else
            {
                if (System.IO.File.Exists(rememberFilePath))
                    System.IO.File.Delete(rememberFilePath);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            HideError();

            // Validate inputs
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Vui lòng nhập tên đăng nhập.");
                txtUsername.Focus();
                return;
            }

            if (username.Length < 4)
            {
                ShowError("Tên đăng nhập phải có ít nhất 4 ký tự.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Vui lòng nhập mật khẩu.");
                txtPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Mật khẩu phải có ít nhất 6 ký tự.");
                txtPassword.Focus();
                return;
            }

            // Show loading state
            SetLoadingState(true);

            try
            {
                // Run BCrypt verify on background thread to avoid UI freeze
                var (success, user, message) = await Task.Run(() =>
                    _authService.Login(username, password));

                if (success && user != null)
                {
                    SaveRememberedCredentials(username, password);

                    // SessionManager.Login đã được gọi bên trong AuthService.Login()
                    // Không cần gọi lại ở đây

                    FrmMain main = new FrmMain();
                    main.Show();
                    this.Hide();
                }
                else
                {
                    ShowError(message);
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi hệ thống: " + ex.Message);
            }
            finally
            {
                SetLoadingState(false);
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
