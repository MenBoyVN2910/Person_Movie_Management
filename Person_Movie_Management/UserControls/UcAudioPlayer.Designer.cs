namespace Person_Movie_Management.UserControls
{
    partial class UcAudioPlayer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            CleanUp();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerPlayback = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerPlayback
            // 
            this.timerPlayback.Interval = 50;
            this.timerPlayback.Tick += new System.EventHandler(this.timerPlayback_Tick);
            // 
            // UcAudioPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UcAudioPlayer";
            this.Size = new System.Drawing.Size(920, 90);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Timer timerPlayback;
    }
}
