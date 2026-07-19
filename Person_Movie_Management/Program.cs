using System;
using System.Windows.Forms;
using Person_Movie_Management.Data;
using Person_Movie_Management.Forms;

namespace Person_Movie_Management
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize Database
            DatabaseHelper.Initialize();

            ApplicationConfiguration.Initialize();
            
            // Show Login Form first
            Application.Run(new FrmLogin());
        }
    }
}