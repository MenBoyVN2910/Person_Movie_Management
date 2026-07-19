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

        public FrmTagManager(int userId, List<int> selectedTags)
        {
            InitializeComponent();
            _currentUserId = userId;
            SelectedTagIds = selectedTags ?? new List<int>();
            _tagRepo = new TagRepository();
            
            ApplyTheme();
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
                    Padding = new Padding(10, 5, 10, 5)
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

        private void btnAddTag_Click(object sender, EventArgs e)
        {
            string tagName = txtNewTag.Text.Trim();
            if (string.IsNullOrEmpty(tagName)) return;
            
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
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
