using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.UserControls
{
    public partial class UcActorCard : UserControl
    {
        private Actor _actor;
        public event EventHandler<Actor>? ActorClicked;

        public UcActorCard(Actor actor)
        {
            InitializeComponent();
            _actor = actor;
            
            lblName.Text = actor.Name;
            lblNationality.Text = string.IsNullOrEmpty(actor.Nationality) ? "Chưa rõ" : actor.Nationality;

            if (!string.IsNullOrEmpty(actor.AvatarPath))
            {
                string fullPath = FileHelper.GetFullPath(actor.AvatarPath);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        var img = FileHelper.LoadImageSafe(fullPath);
                        picAvatar.Image = new Bitmap(img);
                        img.Dispose();
                    }
                    catch { }
                }
            }

            // Style
            var elipse = new Guna.UI2.WinForms.Guna2Elipse { TargetControl = this, BorderRadius = 15 };
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            ActorClicked?.Invoke(this, _actor);
        }
    }
}
