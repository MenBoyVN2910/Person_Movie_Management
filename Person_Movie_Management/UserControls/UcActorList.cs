using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.Services;
using Person_Movie_Management.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.UserControls
{
    public partial class UcActorList : UserControl
    {
        private List<Actor> _allActors = new List<Actor>();
        private System.Windows.Forms.Timer _searchDebounceTimer;
        private Guna.UI2.WinForms.Guna2ComboBox cmbFilterNationality;

        public UcActorList()
        {
            InitializeComponent();
            
            flowLayoutPanel.ItemWidth = 230;
            flowLayoutPanel.ItemHeight = 340;
            flowLayoutPanel.ItemMargin = 18;
            flowLayoutPanel.ActorClicked += FlowLayoutPanel_ActorClicked;

            // Quốc tịch ComboBox
            cmbFilterNationality = new Guna.UI2.WinForms.Guna2ComboBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Size = new System.Drawing.Size(180, 42),
                BorderRadius = 12,
                FillColor = UIHelper.BgCard,
                ForeColor = UIHelper.TextPrimary,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DrawMode = DrawMode.OwnerDrawFixed,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilterNationality.Items.Add("Tất cả Quốc tịch");
            cmbFilterNationality.SelectedIndex = 0;
            cmbFilterNationality.SelectedIndexChanged += (s, e) => FilterAndRender();
            pnlTop.Controls.Add(cmbFilterNationality);

            // Bỏ anchors xung đột để tự tính layout
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnNationalities.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnDeleteAll.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            pnlTop.Resize += (s, e) => LayoutTopControls();
            LayoutTopControls();

            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            this.Load += UcActorList_Load;
        }

        private void LayoutTopControls()
        {
            int currentX = pnlTop.Width - 25;

            if (btnAdd != null && btnAdd.Visible)
            {
                currentX -= btnAdd.Width;
                btnAdd.Location = new System.Drawing.Point(currentX, 18);
                currentX -= 10;
            }
            if (btnDeleteAll != null && btnDeleteAll.Visible)
            {
                currentX -= btnDeleteAll.Width;
                btnDeleteAll.Location = new System.Drawing.Point(currentX, 18);
                currentX -= 10;
            }
            if (btnNationalities != null && btnNationalities.Visible)
            {
                currentX -= btnNationalities.Width;
                btnNationalities.Location = new System.Drawing.Point(currentX, 18);
                currentX -= 10;
            }
            if (cmbFilterNationality != null && cmbFilterNationality.Visible)
            {
                currentX -= cmbFilterNationality.Width;
                cmbFilterNationality.Location = new System.Drawing.Point(currentX, 18);
                currentX -= 10;
            }

            int titleRight = lblTitle.Right + 20;
            int availableW = currentX - titleRight;
            if (availableW < 180) availableW = 180;
            txtSearch.Width = Math.Min(260, availableW);
            txtSearch.Location = new System.Drawing.Point(currentX - txtSearch.Width, 18);
        }

        private void UcActorList_Load(object? sender, EventArgs e)
        {
            if (!DesignMode && SessionManager.IsLoggedIn)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            if (SessionManager.CurrentUser == null) return;
            _allActors = AppServices.ActorRepo.GetAllByUser(SessionManager.CurrentUser.Id);

            // Populate nationalities dropdown
            string? currentSelected = cmbFilterNationality.SelectedIndex > 0 ? cmbFilterNationality.SelectedItem?.ToString() : null;
            cmbFilterNationality.Items.Clear();
            cmbFilterNationality.Items.Add("Tất cả Quốc tịch");

            var nats = _allActors
                .Where(a => !string.IsNullOrWhiteSpace(a.Nationality))
                .Select(a => a.Nationality!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            foreach (var nat in nats)
            {
                cmbFilterNationality.Items.Add(nat);
            }

            if (currentSelected != null && cmbFilterNationality.Items.Contains(currentSelected))
            {
                cmbFilterNationality.SelectedItem = currentSelected;
            }
            else
            {
                cmbFilterNationality.SelectedIndex = 0;
            }

            FilterAndRender();
        }

        private void FilterAndRender()
        {
            var filtered = _allActors.AsEnumerable();

            string query = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(query))
            {
                filtered = filtered.Where(a => a.Name.ToLower().Contains(query));
            }

            if (cmbFilterNationality != null && cmbFilterNationality.SelectedIndex > 0)
            {
                string selectedNat = cmbFilterNationality.SelectedItem?.ToString() ?? "";
                filtered = filtered.Where(a => string.Equals(a.Nationality?.Trim(), selectedNat, StringComparison.OrdinalIgnoreCase));
            }

            var resultList = filtered.ToList();
            var items = resultList.Cast<object>().ToList();
            
            flowLayoutPanel.SetData(items, null);
            
            lblEmpty.Visible = resultList.Count == 0;
            flowLayoutPanel.Visible = resultList.Count > 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            FilterAndRender();
        }

        private void btnNationalities_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn || SessionManager.CurrentUser == null) return;
            var frm = new FrmNationalityManager(SessionManager.CurrentUser.Id);
            frm.ShowDialog(this.ParentForm);
            LoadData();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn || SessionManager.CurrentUser == null) return;

            if (_allActors.Count == 0)
            {
                MessageBox.Show("Không có diễn viên nào để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var inputDialog = new FrmInputBox(
                "Xác nhận xóa Diễn Viên",
                $"Bạn sắp xóa TẤT CẢ {_allActors.Count} diễn viên.",
                defaultValue: "",
                showHardDelete: false,
                note: "⚠️ Lưu ý: Nhập 'delete' để xác nhận xóa toàn bộ diễn viên.",
                placeholder: "Nhập 'delete' để xác nhận...");

            if (inputDialog.ShowDialog() == DialogResult.OK &&
                inputDialog.InputValue.Trim().ToLower() == "delete")
            {
                int deleted = AppServices.ActorRepo.DeleteAll(SessionManager.CurrentUser.Id);
                MessageBox.Show($"Đã xóa thành công {deleted} diễn viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else if (inputDialog.DialogResult == DialogResult.OK)
            {
                MessageBox.Show("Xác nhận không hợp lệ. Hủy thao tác xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var frm = new FrmActorDetail(null);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void FlowLayoutPanel_ActorClicked(object? sender, Actor actor)
        {
            var frm = new FrmActorDetail(actor);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}
