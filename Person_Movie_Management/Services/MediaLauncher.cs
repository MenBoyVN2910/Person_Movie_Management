using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Person_Movie_Management.Services
{
    public static class MediaLauncher
    {
        public static void LaunchMedia(string targetPath, int sourceType)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = targetPath,
                    UseShellExecute = true // Bắt buộc để Windows tự điều hướng theo giao thức file/http
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở tệp tin hoặc liên kết.\nLỗi: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
