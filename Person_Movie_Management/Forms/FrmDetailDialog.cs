using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public partial class FrmDetailDialog : Form
    {
        public FrmDetailDialog(string title, string content)
        {
            InitializeComponent();
            this.BackColor = UIHelper.BgDark;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitle.Text = title;
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Font = UIHelper.FontH2;

            txtContent.Text = content;
            txtContent.FillColor = UIHelper.BgCard;
            txtContent.ForeColor = UIHelper.TextSecondary;
            txtContent.Font = UIHelper.FontBody;
            txtContent.BorderColor = UIHelper.Border;
            
            btnOK.FillColor = UIHelper.GradViolet1;
            btnOK.FillColor2 = UIHelper.GradRose1;
            btnOK.ForeColor = Color.White;
            btnOK.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnOK.Animated = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
