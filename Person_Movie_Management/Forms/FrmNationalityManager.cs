using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmNationalityManager : Form
    {
        private int _userId;
        private List<(string Name, int ActorCount)> _allNationalities = new List<(string, int)>();

        public FrmNationalityManager(int userId)
        {
            InitializeComponent();
            _userId = userId;
            txtNewName.MaxLength = 40;
        }

        private void FrmNationalityManager_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            _allNationalities = AppServices.ActorRepo.GetNationalitiesWithCount(_userId);
            RenderList();
        }

        private void RenderList()
        {
            flpList.SuspendLayout();
            flpList.Controls.Clear();

            string search = txtSearch.Text.Trim().ToLower();
            var filtered = _allNationalities.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Name.ToLower().Contains(search));
            }

            var list = filtered.ToList();

            if (list.Count == 0)
            {
                lblEmpty.Visible = true;
                lblEmpty.Text = string.IsNullOrEmpty(search) 
                    ? "Chưa có quốc tịch nào. Hãy thêm quốc tịch mới ở trên!" 
                    : "Không tìm thấy quốc tịch phù hợp.";
                flpList.Controls.Add(lblEmpty);
            }
            else
            {
                lblEmpty.Visible = false;
                foreach (var item in list)
                {
                    flpList.Controls.Add(CreateItemCard(item.Name, item.ActorCount));
                }
            }

            flpList.ResumeLayout();
        }

        private Control CreateItemCard(string name, int actorCount)
        {
            var pnlCard = new Guna2Panel
            {
                Size = new Size(455, 52),
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(42, 53, 90),
                FillColor = Color.FromArgb(24, 31, 58),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Default
            };

            // Flag/Globe Icon + Name
            var lblName = new Label
            {
                Text = $"🌍  {name}",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(14, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Actor Count Tag
            var lblCount = new Label
            {
                Text = actorCount > 0 ? $"({actorCount} diễn viên)" : "(0 diễn viên)",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = actorCount > 0 ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139),
                Location = new Point(lblName.Right + 10, 17),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Position adjustments
            lblName.TextChanged += (s, e) =>
            {
                lblCount.Location = new Point(lblName.Right + 10, 17);
            };

            // Edit button
            var btnEdit = new Guna2Button
            {
                Text = "✏️ Sửa",
                Size = new Size(68, 34),
                Location = new Point(305, 9),
                BorderRadius = 8,
                FillColor = Color.FromArgb(42, 53, 90),
                HoverState = { FillColor = Color.FromArgb(139, 92, 246) },
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(224, 231, 255),
                Cursor = Cursors.Hand
            };
            btnEdit.Click += (s, e) => EditNationality(name);

            // Delete button
            var btnDelete = new Guna2Button
            {
                Text = "🗑️ Xóa",
                Size = new Size(68, 34),
                Location = new Point(378, 9),
                BorderRadius = 8,
                FillColor = Color.FromArgb(60, 25, 40),
                HoverState = { FillColor = Color.FromArgb(239, 68, 68) },
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(252, 165, 165),
                Cursor = Cursors.Hand
            };
            btnDelete.Click += (s, e) => DeleteNationality(name, actorCount);

            // Hover effect
            pnlCard.MouseEnter += (s, e) =>
            {
                pnlCard.BorderColor = Color.FromArgb(139, 92, 246);
                pnlCard.FillColor = Color.FromArgb(32, 40, 72);
            };
            pnlCard.MouseLeave += (s, e) =>
            {
                var rect = pnlCard.RectangleToScreen(pnlCard.ClientRectangle);
                if (!rect.Contains(Cursor.Position))
                {
                    pnlCard.BorderColor = Color.FromArgb(42, 53, 90);
                    pnlCard.FillColor = Color.FromArgb(24, 31, 58);
                }
            };

            pnlCard.Controls.Add(lblName);
            pnlCard.Controls.Add(lblCount);
            pnlCard.Controls.Add(btnEdit);
            pnlCard.Controls.Add(btnDelete);

            return pnlCard;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newName = txtNewName.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Vui lòng nhập tên quốc tịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewName.Focus();
                return;
            }

            if (_allNationalities.Any(x => string.Equals(x.Name, newName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Quốc tịch '{newName}' đã tồn tại trong danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNewName.Focus();
                return;
            }

            bool ok = AppServices.ActorRepo.AddNationality(_userId, newName);
            if (ok)
            {
                txtNewName.Text = "";
                LoadData();
            }
            else
            {
                MessageBox.Show("Không thể thêm quốc tịch. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditNationality(string oldName)
        {
            var frmInput = new FrmInputBox("Đổi Tên Quốc Tịch", $"Nhập tên mới cho quốc tịch '{oldName}':", oldName);
            if (frmInput.ShowDialog(this) == DialogResult.OK)
            {
                string newName = frmInput.InputValue.Trim();
                if (string.IsNullOrWhiteSpace(newName) || string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (_allNationalities.Any(x => string.Equals(x.Name, newName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Quốc tịch '{newName}' đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool ok = AppServices.ActorRepo.UpdateNationality(_userId, oldName, newName);
                if (ok)
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật quốc tịch. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteNationality(string name, int actorCount)
        {
            string msg = actorCount > 0
                ? $"Bạn có chắc chắn muốn xóa quốc tịch '{name}'?\n\nCó {actorCount} diễn viên đang mang quốc tịch này. Quốc tịch của họ sẽ được chuyển thành 'Chưa rõ'."
                : $"Bạn có chắc chắn muốn xóa quốc tịch '{name}'?";

            var result = MessageBox.Show(msg, "Xác nhận xóa quốc tịch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool ok = AppServices.ActorRepo.DeleteNationality(_userId, name);
                if (ok)
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không thể xóa quốc tịch. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RenderList();
        }

        private void txtNewName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnAdd_Click(sender, e);
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
