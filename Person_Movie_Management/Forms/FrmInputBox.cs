using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public partial class FrmInputBox : Form
    {
        public string InputValue { get; private set; } = string.Empty;

        public FrmInputBox(string title, string prompt, string defaultValue = "")
        {
            InitializeComponent();
            ApplyTheme();
            
            lblTitle.Text = title;
            lblPrompt.Text = prompt;
            txtInput.Text = defaultValue;
            
            this.Load += (s, e) => { txtInput.Focus(); };
        }

        private void ApplyTheme()
        {
            this.BackColor = UIHelper.BgPanel;
            pnlTop.BackColor = UIHelper.BgDark;
            txtInput.FillColor = UIHelper.BgDark;
            txtInput.ForeColor = UIHelper.TextPrimary;
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblPrompt.ForeColor = UIHelper.TextPrimary;
            
            btnOk.FillColor = UIHelper.AccentPrimary;
            btnOk.FillColor2 = UIHelper.AccentSecondary;
            
            btnCancel.FillColor = UIHelper.BgCard;
            btnCancel.ForeColor = UIHelper.TextSecondary;
            btnCancel.HoverState.FillColor = UIHelper.BgCardHover;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            InputValue = txtInput.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
