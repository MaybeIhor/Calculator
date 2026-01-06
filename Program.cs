using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Calculus
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form());
        }
    }
    public class FixedRenderer : ToolStripSystemRenderer
    {
        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        private static extern void DwmGetColorizationParameters(out DWMCOLORIZATIONPARAMS parameters);

        [StructLayout(LayoutKind.Sequential)]
        private struct DWMCOLORIZATIONPARAMS
        {
            public uint ColorizationColor;
            public uint ColorizationAfterglow;
            public uint ColorizationColorBalance;
            public uint ColorizationAfterglowBalance;
            public uint ColorizationBlurBalance;
            public uint ColorizationGlassReflectionIntensity;
            public uint ColorizationOpaqueBlend;
        }

        private static Color GetWindowsAccentColor()
        {
            try
            {
                DwmGetColorizationParameters(out DWMCOLORIZATIONPARAMS parameters);
                uint color = parameters.ColorizationColor;

                byte a = (byte)((color >> 24) & 0xFF);
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)(color & 0xFF);

                return Color.FromArgb(255, r, g, b);
            }
            catch
            {
                return Color.FromArgb(28, 130, 255);
            }
        }

        private readonly Color accent = GetWindowsAccentColor();

        public FixedRenderer() { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

            if (e.Item.Selected || e.Item.Pressed)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, accent)))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }

                using (Pen pen = new Pen(Color.FromArgb(100, accent), 1))
                {
                    rc.Width -= 1;
                    rc.Height -= 1;
                    e.Graphics.DrawRectangle(pen, rc);
                }
            }
        }
    }
}
