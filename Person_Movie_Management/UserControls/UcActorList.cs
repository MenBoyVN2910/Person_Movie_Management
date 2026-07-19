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

        public UcActorList()
        {
            InitializeComponent();
            
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            this.Load += UcActorList_Load;
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
            _allActors = AppServices.ActorRepo.GetAllByUser(SessionManager.CurrentUser!.Id);
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

            var resultList = filtered.ToList();
            
            flowLayoutPanel.SuspendLayout();
            flowLayoutPanel.Controls.Clear();

            foreach (var actor in resultList)
            {
                var card = new UcActorCard(actor);
                card.ActorClicked += Card_ActorClicked;
                flowLayoutPanel.Controls.Add(card);
            }

            flowLayoutPanel.ResumeLayout();
            
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Open FrmActorDetail in add mode
            // We will create FrmActorDetail next
            var frm = new FrmActorDetail(null);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void Card_ActorClicked(object? sender, Actor actor)
        {
            var frm = new FrmActorDetail(actor);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}
