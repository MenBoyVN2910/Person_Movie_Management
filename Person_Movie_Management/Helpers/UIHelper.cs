using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Person_Movie_Management.Helpers
{
    public static class UIHelper
    {
        // ═══════════════════════════════════════════════
        // Elegant Dark-Blue Theme — Modern & Bright
        // ═══════════════════════════════════════════════

        public enum ThemeMode { Dark, Light }
        public static ThemeMode CurrentTheme { get; private set; } = ThemeMode.Dark;

        // Core Backgrounds (layered depth — brighter navy tones)
        public static Color BgDark;
        public static Color BgPanel;
        public static Color BgCard;
        public static Color BgCardHover;
        public static Color BgSurface;

        // Accent Colors (vibrant indigo-based palette)
        public static Color AccentPrimary;
        public static Color AccentSecondary;
        public static Color AccentTertiary;
        public static Color AccentGold;

        // Status Colors (brighter, more vivid)
        public static Color Success = Color.FromArgb(74, 222, 128);
        public static Color Warning = Color.FromArgb(251, 191, 36);
        public static Color Danger  = Color.FromArgb(251, 113, 133);
        public static Color Info    = Color.FromArgb(56, 189, 248);

        // Text (higher contrast for readability)
        public static Color TextPrimary;
        public static Color TextSecondary;
        public static Color TextMuted;

        // Border / Divider (visible but subtle)
        public static Color Border;

        static UIHelper()
        {
            ApplyTheme(ThemeMode.Dark);
        }

        public static void ApplyTheme(ThemeMode mode)
        {
            CurrentTheme = mode;
            if (mode == ThemeMode.Dark)
            {
                BgDark = Color.FromArgb(15, 23, 42);
                BgPanel = Color.FromArgb(30, 41, 59);
                BgCard = Color.FromArgb(51, 65, 85);
                BgCardHover = Color.FromArgb(71, 85, 105);
                BgSurface = Color.FromArgb(30, 41, 59);

                AccentPrimary = Color.FromArgb(99, 102, 241);
                AccentSecondary = Color.FromArgb(129, 140, 248);
                AccentTertiary = Color.FromArgb(244, 114, 182);
                AccentGold = Color.FromArgb(253, 224, 71);

                TextPrimary = Color.FromArgb(248, 250, 252);
                TextSecondary = Color.FromArgb(203, 213, 225);
                TextMuted = Color.FromArgb(148, 163, 184);

                Border = Color.FromArgb(71, 85, 105);
            }
            else
            {
                // Light Theme
                BgDark = Color.FromArgb(248, 250, 252);        // slate-50
                BgPanel = Color.FromArgb(241, 245, 249);       // slate-100
                BgCard = Color.White;                          // white
                BgCardHover = Color.FromArgb(226, 232, 240);   // slate-200
                BgSurface = Color.FromArgb(241, 245, 249);     // slate-100

                AccentPrimary = Color.FromArgb(79, 70, 229);   // indigo-600
                AccentSecondary = Color.FromArgb(99, 102, 241); // indigo-500
                AccentTertiary = Color.FromArgb(236, 72, 153);  // pink-500
                AccentGold = Color.FromArgb(234, 179, 8);      // yellow-500

                TextPrimary = Color.FromArgb(15, 23, 42);      // slate-900
                TextSecondary = Color.FromArgb(51, 65, 85);    // slate-700
                TextMuted = Color.FromArgb(100, 116, 139);     // slate-500

                Border = Color.FromArgb(203, 213, 225);        // slate-300
            }
        }

        // ═══════════════════════════════════════════════
        // Gradient Presets (brighter, more vivid)
        // ═══════════════════════════════════════════════

        public static LinearGradientBrush CreateGradient(Rectangle rect, Color c1, Color c2, float angle = 135f)
        {
            if (rect.Width <= 0) rect = new Rectangle(rect.X, rect.Y, 1, rect.Height);
            if (rect.Height <= 0) rect = new Rectangle(rect.X, rect.Y, rect.Width, 1);
            return new LinearGradientBrush(rect, c1, c2, angle);
        }

        // Stat card gradient presets (brighter variants)
        public static readonly Color GradViolet1 = Color.FromArgb(99, 102, 241);   // indigo-500
        public static readonly Color GradViolet2 = Color.FromArgb(167, 139, 250);  // violet-400
        public static readonly Color GradRose1   = Color.FromArgb(244, 114, 182);  // pink-400
        public static readonly Color GradRose2   = Color.FromArgb(251, 146, 160);  // rose-300
        public static readonly Color GradEmerald1 = Color.FromArgb(52, 211, 153);  // emerald-400
        public static readonly Color GradEmerald2 = Color.FromArgb(110, 231, 183); // emerald-300
        public static readonly Color GradAmber1  = Color.FromArgb(251, 191, 36);   // amber-400
        public static readonly Color GradAmber2  = Color.FromArgb(253, 224, 71);   // yellow-300
        public static readonly Color GradSky1    = Color.FromArgb(56, 189, 248);   // sky-400
        public static readonly Color GradSky2    = Color.FromArgb(125, 211, 252);  // sky-300

        public static Image CropToFill(Image original, int targetWidth, int targetHeight)
        {
            if (original == null) return null;
            
            float targetAspect = (float)targetWidth / targetHeight;
            float originalAspect = (float)original.Width / original.Height;

            int cropWidth = original.Width;
            int cropHeight = original.Height;
            int cropX = 0;
            int cropY = 0;

            if (originalAspect > targetAspect)
            {
                cropWidth = (int)(original.Height * targetAspect);
                cropX = (original.Width - cropWidth) / 2;
            }
            else
            {
                cropHeight = (int)(original.Width / targetAspect);
                cropY = (original.Height - cropHeight) / 2;
            }

            var bmp = new Bitmap(targetWidth, targetHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(original, new Rectangle(0, 0, targetWidth, targetHeight), 
                            new Rectangle(cropX, cropY, cropWidth, cropHeight), GraphicsUnit.Pixel);
            }
            return bmp;
        }

        // ═══════════════════════════════════════════════
        // Font Presets
        // ═══════════════════════════════════════════════

        public static readonly Font FontTitle   = new Font("Segoe UI", 22F, FontStyle.Bold);
        public static readonly Font FontH2      = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font FontH3      = new Font("Segoe UI", 13F, FontStyle.Bold);
        public static readonly Font FontBody    = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static readonly Font FontCaption = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FontStatNum = new Font("Segoe UI", 32F, FontStyle.Bold);

        // ═══════════════════════════════════════════════
        // Helper: Apply premium styling to Guna2Button
        // ═══════════════════════════════════════════════
        public static void StyleMenuButton(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = Color.Transparent;
            btn.ForeColor = TextSecondary;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            btn.BorderRadius = 10;
            
            // Left indicator setup
            btn.CustomBorderThickness = new Padding(4, 0, 0, 0);
            btn.CustomBorderColor = Color.Transparent;
            
            btn.HoverState.FillColor = Color.FromArgb(20, AccentPrimary);
            btn.HoverState.ForeColor = TextPrimary;
            
            btn.CheckedState.FillColor = Color.FromArgb(30, AccentPrimary); // lighter fill
            btn.CheckedState.ForeColor = TextPrimary;
            btn.CheckedState.CustomBorderColor = AccentPrimary; // Indicator color
            btn.CheckedState.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            
            btn.Animated = true;
        }

        public static Bitmap CreateIcon(string symbol, float emSize = 14f)
        {
            var bmp = new Bitmap(24, 24);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            
            using var font = new Font("Segoe MDL2 Assets", emSize);
            using var brush = new SolidBrush(Color.White);
            
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            
            g.DrawString(symbol, font, brush, new RectangleF(0, 0, 24, 24), format);
            return bmp;
        }

        public static string ShowInputBox(string title, string promptText, string defaultValue = "")
        {
            var form = new Form
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = BgDark,
                ForeColor = TextPrimary,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            var label = new Label { Left = 20, Top = 20, Width = 340, Text = promptText, Font = FontBody };
            var textBox = new TextBox { Left = 20, Top = 55, Width = 340, Text = defaultValue, Font = FontBody, BackColor = BgPanel, ForeColor = TextPrimary };
            
            var confirmation = new Button 
            { 
                Text = "OK", Left = 160, Width = 90, Top = 100, DialogResult = DialogResult.OK,
                BackColor = AccentPrimary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat 
            };
            confirmation.FlatAppearance.BorderSize = 0;
            
            var cancel = new Button 
            { 
                Text = "Hủy", Left = 270, Width = 90, Top = 100, DialogResult = DialogResult.Cancel,
                BackColor = BgPanel, ForeColor = TextPrimary, FlatStyle = FlatStyle.Flat 
            };
            cancel.FlatAppearance.BorderSize = 1;
            cancel.FlatAppearance.BorderColor = Border;

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(confirmation);
            form.Controls.Add(cancel);
            form.AcceptButton = confirmation;
            form.CancelButton = cancel;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
