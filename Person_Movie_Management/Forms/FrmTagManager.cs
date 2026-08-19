using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Guna.UI2.WinForms;
using System.Linq;

namespace Person_Movie_Management.Forms
{
    public partial class FrmTagManager : Form
    {
        private TagRepository _tagRepo;
        private int _currentUserId;
        public List<int> SelectedTagIds { get; private set; }
        
        private string[] predefinedColors = new string[] { "#ef4444", "#f97316", "#eab308", "#22c55e", "#0ea5e9", "#6366f1", "#d946ef", "#8b5cf6" };

        private Guna2ContextMenuStrip _contextMenuTag = null!;
        private ToolStripMenuItem _menuDeleteTag = null!;
        private ToolStripMenuItem _menuRenameTag = null!;
        private Person_Movie_Management.Models.Tag? _selectedContextTag;
        private ToolTip _toolTip = null!;

        public FrmTagManager(int userId, List<int> selectedTags)
        {
            InitializeComponent();
            _currentUserId = userId;
            SelectedTagIds = selectedTags ?? new List<int>();
            _tagRepo = new TagRepository();
            _toolTip = new ToolTip();
            
            InitContextMenu();
            ApplyTheme();

            txtNewTag.MaxLength = 30; // Giới hạn tên tag tối đa 30 ký tự

            txtNewTag.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnAddTag_Click(s, e);
                }
            };
        }

        private void InitContextMenu()
        {
            _contextMenuTag = new Guna2ContextMenuStrip
            {
                RenderStyle = 
                {
                    ArrowColor = Color.FromArgb(139, 92, 246),
                    BorderColor = Color.FromArgb(40, 48, 82),
                    ColorTable = null,
                    RoundedEdges = true,
                    SelectionArrowColor = Color.White,
                    SelectionBackColor = Color.FromArgb(139, 92, 246),
                    SelectionForeColor = Color.White,
                    SeparatorColor = Color.FromArgb(40, 48, 82),
                    TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit
                },
                BackColor = Color.FromArgb(22, 28, 56),
                ForeColor = Color.FromArgb(241, 245, 249),
                Size = new Size(160, 60)
            };

            _menuRenameTag = new ToolStripMenuItem
            {
                Text = "Đổi tên tag",
                Image = UIHelper.CreateIcon("\uE70F", 12f),
                Size = new Size(160, 24)
            };
            _menuRenameTag.Click += MenuRenameTag_Click;

            _menuDeleteTag = new ToolStripMenuItem
            {
                Text = "Xóa tag",
                Image = UIHelper.CreateIcon("\uE74D", 12f),
                Size = new Size(160, 24)
            };
            _menuDeleteTag.Click += MenuDeleteTag_Click;

            _contextMenuTag.Items.AddRange(new ToolStripItem[] { _menuRenameTag, _menuDeleteTag });
        }

        private void MenuRenameTag_Click(object? sender, EventArgs e)
        {
            var tagToRename = _selectedContextTag;
            if (tagToRename == null && _contextMenuTag.SourceControl is Guna2Button sourceBtn)
            {
                tagToRename = sourceBtn.Tag as Person_Movie_Management.Models.Tag;
            }

            if (tagToRename == null) return;

            using var frmInput = new FrmInputBox("Đổi Tên Tag", "Nhập tên mới cho tag:", tagToRename.TagName, note: "💡 Giới hạn tối đa 30 ký tự.", maxLength: 30);
            frmInput.TopMost = true;
            if (frmInput.ShowDialog(this) == DialogResult.OK)
            {
                string newName = frmInput.InputValue.Trim();
                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show(this, "Tên thẻ tag không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.Equals(newName, tagToRename.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string formattedName = newName.Length > 30 ? newName.Substring(0, 30) : newName;

                // Kiểm tra trùng thẻ tag khi đổi tên
                if (_tagRepo.Exists(_currentUserId, formattedName, tagToRename.Id))
                {
                    MessageBox.Show(this, $"Thẻ tag \"{formattedName}\" đã tồn tại trong danh sách của bạn.", "Trùng thẻ tag", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                tagToRename.TagName = formattedName;
                if (_tagRepo.Update(tagToRename))
                {
                    LoadTags();
                }
                else
                {
                    MessageBox.Show(this, "Không thể cập nhật thẻ tag. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MenuDeleteTag_Click(object? sender, EventArgs e)
        {
            var tagToDelete = _selectedContextTag;
            if (tagToDelete == null && _contextMenuTag.SourceControl is Guna2Button sourceBtn)
            {
                tagToDelete = sourceBtn.Tag as Person_Movie_Management.Models.Tag;
            }

            if (tagToDelete == null) return;

            var confirm = MessageBox.Show(
                this,
                $"Bạn có chắc chắn muốn xóa tag \"{tagToDelete.TagName}\" không?\n\nLưu ý: Tag này sẽ bị xóa khỏi tất cả video liên quan.",
                "Xác nhận xóa tag",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                bool success = _tagRepo.Delete(tagToDelete.Id);
                if (success)
                {
                    SelectedTagIds.Remove(tagToDelete.Id);
                    LoadTags();
                }
                else
                {
                    MessageBox.Show(this, "Không thể xóa tag này. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = UIHelper.BgPanel;
            pnlTop.BackColor = UIHelper.BgDark;
            flpTags.BackColor = UIHelper.BgPanel;
            txtNewTag.FillColor = UIHelper.BgDark;
            txtNewTag.ForeColor = UIHelper.TextPrimary;
        }

        private void FrmTagManager_Load(object sender, EventArgs e)
        {
            LoadTags();
        }

        private void LoadTags()
        {
            flpTags.Controls.Clear();
            var tags = _tagRepo.GetAllByUser(_currentUserId);
            
            foreach (var tag in tags)
            {
                var isSelected = SelectedTagIds.Contains(tag.Id);
                var btn = new Guna2Button
                {
                    Text = tag.TagName,
                    Tag = tag,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    BorderRadius = 15,
                    BorderThickness = isSelected ? 2 : 1,
                    BorderColor = ColorTranslator.FromHtml(tag.ColorHex ?? "#6366f1"),
                    FillColor = isSelected ? ColorTranslator.FromHtml(tag.ColorHex ?? "#6366f1") : Color.Transparent,
                    ForeColor = isSelected ? Color.White : UIHelper.TextPrimary,
                    Cursor = Cursors.Hand,
                    AutoSize = true,
                    Margin = new Padding(5),
                    Padding = new Padding(10, 5, 10, 5),
                    ContextMenuStrip = _contextMenuTag
                };

                _toolTip.SetToolTip(btn, "• Chuột trái: Chọn / Bỏ chọn\n• Chuột phải: Xóa / Đổi tên tag");

                btn.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        _selectedContextTag = tag;
                    }
                };
                
                btn.Click += (s, e) => 
                {
                    if (SelectedTagIds.Contains(tag.Id))
                    {
                        SelectedTagIds.Remove(tag.Id);
                        btn.FillColor = Color.Transparent;
                        btn.ForeColor = UIHelper.TextPrimary;
                        btn.BorderThickness = 1;
                    }
                    else
                    {
                        SelectedTagIds.Add(tag.Id);
                        btn.FillColor = ColorTranslator.FromHtml(tag.ColorHex ?? "#6366f1");
                        btn.ForeColor = Color.White;
                        btn.BorderThickness = 2;
                    }
                };

                flpTags.Controls.Add(btn);
            }
        }

        private void btnAddTag_Click(object? sender, EventArgs e)
        {
            string tagName = txtNewTag.Text.Trim();
            if (string.IsNullOrWhiteSpace(tagName))
            {
                MessageBox.Show(this, "Vui lòng nhập tên thẻ tag.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewTag.Focus();
                return;
            }

            if (tagName.Length > 30)
            {
                tagName = tagName.Substring(0, 30);
            }

            // Kiểm tra trùng thẻ tag trước khi thêm
            if (_tagRepo.Exists(_currentUserId, tagName))
            {
                MessageBox.Show(this, $"Thẻ tag \"{tagName}\" đã tồn tại trong danh sách của bạn.", "Trùng thẻ tag", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewTag.Focus();
                txtNewTag.SelectAll();
                return;
            }
            
            // Random color
            Random rnd = new Random();
            string color = predefinedColors[rnd.Next(predefinedColors.Length)];
            
            var newTag = new Person_Movie_Management.Models.Tag
            {
                UserId = _currentUserId,
                TagName = tagName,
                ColorHex = color
            };
            
            int id = _tagRepo.Insert(newTag);
            if (id > 0)
            {
                SelectedTagIds.Add(id);
                txtNewTag.Text = "";
                LoadTags();
            }
            else if (id == -1)
            {
                MessageBox.Show(this, $"Thẻ tag \"{tagName}\" đã tồn tại trong danh sách của bạn.", "Trùng thẻ tag", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewTag.Focus();
                txtNewTag.SelectAll();
            }
            else
            {
                MessageBox.Show(this, "Không thể thêm thẻ tag. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
