using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public partial class FrmUpdateProgress : Form
    {
        public int SelectedProgress { get; private set; }

        public FrmUpdateProgress(int currentProgress, string itemName = "")
        {
            InitializeComponent();

            SelectedProgress = Math.Clamp(currentProgress, 0, 100);
            
            if (!string.IsNullOrEmpty(itemName))
            {
                lblItemName.Text = $"Mục: {itemName}";
            }
            else
            {
                lblItemName.Text = "Kéo thanh trượt hoặc chọn mốc bên dưới";
            }

            trkProgress.Value = SelectedProgress;
            txtDirectInput.Text = SelectedProgress.ToString();
            UpdateUI();
        }

        private void UpdateUI()
        {
            lblPercentDisplay.Text = $"{SelectedProgress}%";

            if (SelectedProgress == 0)
            {
                lblPercentDisplay.ForeColor = UIHelper.TextMuted;
            }
            else if (SelectedProgress == 100)
            {
                lblPercentDisplay.ForeColor = UIHelper.Success;
            }
            else
            {
                lblPercentDisplay.ForeColor = UIHelper.AccentPrimary;
            }

            HighlightActivePresetButton();
        }

        private void HighlightActivePresetButton()
        {
            btn0.FillColor = SelectedProgress == 0 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn0.ForeColor = SelectedProgress == 0 ? Color.White : UIHelper.TextMuted;

            btn10.FillColor = SelectedProgress == 10 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn10.ForeColor = SelectedProgress == 10 ? Color.White : UIHelper.TextMuted;

            btn30.FillColor = SelectedProgress == 30 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn30.ForeColor = SelectedProgress == 30 ? Color.White : UIHelper.TextMuted;

            btn50.FillColor = SelectedProgress == 50 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn50.ForeColor = SelectedProgress == 50 ? Color.White : UIHelper.TextMuted;

            btn70.FillColor = SelectedProgress == 70 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn70.ForeColor = SelectedProgress == 70 ? Color.White : UIHelper.TextMuted;

            btn90.FillColor = SelectedProgress == 90 ? UIHelper.AccentPrimary : Color.FromArgb(30, 41, 59);
            btn90.ForeColor = SelectedProgress == 90 ? Color.White : UIHelper.TextMuted;

            btn100.FillColor = SelectedProgress == 100 ? UIHelper.Success : Color.FromArgb(30, 41, 59);
            btn100.ForeColor = SelectedProgress == 100 ? Color.White : UIHelper.Success;
        }

        private void trkProgress_Scroll(object sender, ScrollEventArgs e)
        {
            SelectedProgress = trkProgress.Value;
            txtDirectInput.Text = SelectedProgress.ToString();
            UpdateUI();
        }

        private void txtDirectInput_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtDirectInput.Text.Trim(), out int val))
            {
                val = Math.Clamp(val, 0, 100);
                SelectedProgress = val;
                trkProgress.Value = val;
                UpdateUI();
            }
        }

        private void btnPreset_Click(object sender, EventArgs e)
        {
            if (sender is Guna.UI2.WinForms.Guna2Button btn)
            {
                int val = 0;
                if (btn == btn0) val = 0;
                else if (btn == btn10) val = 10;
                else if (btn == btn30) val = 30;
                else if (btn == btn50) val = 50;
                else if (btn == btn70) val = 70;
                else if (btn == btn90) val = 90;
                else if (btn == btn100) val = 100;

                SelectedProgress = val;
                trkProgress.Value = val;
                txtDirectInput.Text = val.ToString();
                UpdateUI();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
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
