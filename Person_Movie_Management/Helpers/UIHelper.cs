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

        // Core Backgrounds (layered depth — brighter navy tones)
        public static readonly Color BgDark       = Color.FromArgb(15, 23, 42);        // #0F172A — slate-900
        public static readonly Color BgPanel      = Color.FromArgb(30, 41, 59);        // #1E293B — slate-800
        public static readonly Color BgCard       = Color.FromArgb(51, 65, 85);        // #334155 — slate-700
        public static readonly Color BgCardHover  = Color.FromArgb(71, 85, 105);       // #475569 — slate-600
        public static readonly Color BgSurface    = Color.FromArgb(30, 41, 59);        // #1E293B — slate-800

        // Accent Colors (vibrant indigo-based palette)
        public static readonly Color AccentPrimary   = Color.FromArgb(99, 102, 241);   // #6366F1 — indigo-500
        public static readonly Color AccentSecondary = Color.FromArgb(129, 140, 248);  // #818CF8 — indigo-400
        public static readonly Color AccentTertiary  = Color.FromArgb(244, 114, 182);  // #F472B6 — pink-400
        public static readonly Color AccentGold      = Color.FromArgb(253, 224, 71);   // #FDE047 — yellow-300

        // Status Colors (brighter, more vivid)
        public static readonly Color Success = Color.FromArgb(74, 222, 128);    // #4ADE80 — green-400
        public static readonly Color Warning = Color.FromArgb(251, 191, 36);    // #FBBF24 — amber-400
        public static readonly Color Danger  = Color.FromArgb(251, 113, 133);   // #FB7185 — rose-400
        public static readonly Color Info    = Color.FromArgb(56, 189, 248);    // #38BDF8 — sky-400

        // Text (higher contrast for readability)
        public static readonly Color TextPrimary   = Color.FromArgb(248, 250, 252);   // #F8FAFC — slate-50
        public static readonly Color TextSecondary = Color.FromArgb(203, 213, 225);   // #CBD5E1 — slate-300
        public static readonly Color TextMuted     = Color.FromArgb(148, 163, 184);   // #94A3B8 — slate-400

        // Border / Divider (visible but subtle)
        public static readonly Color Border = Color.FromArgb(71, 85, 105);     // #475569 — slate-600

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
    }
}
