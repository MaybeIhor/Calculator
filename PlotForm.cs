using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Calculus
{
    public partial class plotForm : Form
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        public plotForm() => InitializeComponent();

        public void SetDarkMode()
        {
            plotBox.DarkSide();
            DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        public void SetArguments(string functionX, string functionY)
        {
            plotBox.FunctionX = functionX;
            plotBox.FunctionY = functionY;
        }
    }

    public class PlotBox : Control
    {
        private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");

        public string FunctionX { get; set; }
        public string FunctionY { get; set; }

        private Color gridColor = Color.FromArgb(220, 220, 220);
        private Color axisColor = Color.Black;
        private readonly Color functionXColor = Color.FromArgb(58, 144, 219);
        private readonly Color functionYColor = Color.FromArgb(150, 34, 34);

        public PlotBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = SystemColors.Window;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            g.Clear(BackColor);

            int size = Math.Min(Width, Height);
            int offsetX = (Width - size) / 2;
            int offsetY = (Height - size) / 2;

            g.SetClip(new Rectangle(offsetX, offsetY, size, size));

            DrawGrid(g);
            DrawAxes(g);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawFunction(g, FunctionX, functionXColor, "x");
            DrawFunction(g, FunctionY, functionYColor, "y");
        }

        private void DrawGrid(Graphics g)
        {
            using (var pen = new Pen(gridColor, 1))
            {
                for (int i = -9; i <= 9; i++)
                {
                    if (i == 0) continue;
                    float x = MapXToScreen(i);
                    float y = MapYToScreen(i);
                    g.DrawLine(pen, x, 0, x, Height);
                    g.DrawLine(pen, 0, y, Width, y);
                }
            }
        }

        private void DrawAxes(Graphics g)
        {
            using (var pen = new Pen(axisColor, 1))
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(ForeColor))
            {
                float xCenter = MapXToScreen(0);
                float yCenter = MapYToScreen(0);

                g.DrawLine(pen, 0, yCenter, Width, yCenter);
                g.DrawLine(pen, xCenter, 0, xCenter, Height);
                g.DrawString("0", font, brush, xCenter + 4, yCenter + 4);
            }
        }

        private void DrawFunction(Graphics g, string function, Color color, string variable)
        {
            if (string.IsNullOrWhiteSpace(function)) return;

            using (var pen = new Pen(color, 2))
            {
                var points = new List<PointF>();
                double step = 20.0 / Width;
                double? prevX = null, prevY = null;

                for (double x = -10; x <= 10; x += step)
                {
                    try
                    {
                        string expr = function.Replace(variable, x < 0 ? $"(0{x.ToString(EnUs)})" : x.ToString(EnUs));
                        double y = form.Evaluate(expr);

                        if (double.IsNaN(y) || double.IsInfinity(y))
                        {
                            if (points.Count > 1) g.DrawLines(pen, points.ToArray());
                            points.Clear();
                            prevX = prevY = null;
                            continue;
                        }

                        bool currInBounds = y >= -10.0 && y <= 10.0;
                        bool prevInBounds = prevY.HasValue && prevY.Value >= -10.0 && prevY.Value <= 10.0;

                        if (prevX.HasValue && prevInBounds != currInBounds)
                        {
                            double clipY = prevY.Value < -10.0 || y < -10.0 ? -10.0 : 10.0;
                            double t = (clipY - prevY.Value) / (y - prevY.Value);
                            double clipX = prevX.Value + t * (x - prevX.Value);

                            points.Add(new PointF(MapXToScreen(clipX), MapYToScreen(clipY)));

                            if (points.Count > 1) g.DrawLines(pen, points.ToArray());
                            points.Clear();

                            if (currInBounds) points.Add(new PointF(MapXToScreen(clipX), MapYToScreen(clipY)));
                        }

                        if (currInBounds)
                        {
                            points.Add(new PointF(MapXToScreen(x), MapYToScreen(y)));
                        }
                        else if (points.Count > 0)
                        {
                            if (points.Count > 1) g.DrawLines(pen, points.ToArray());
                            points.Clear();
                        }

                        prevX = x;
                        prevY = y;
                    }
                    catch
                    {
                        if (points.Count > 1) g.DrawLines(pen, points.ToArray());
                        points.Clear();
                        prevX = prevY = null;
                    }
                }

                if (points.Count > 1) g.DrawLines(pen, points.ToArray());
            }
        }

        private float MapXToScreen(double x)
        {
            int size = Math.Min(Width, Height);
            int offset = (Width - size) / 2;
            return offset + (float)((x + 10.0) / 20.0 * size);
        }

        private float MapYToScreen(double y)
        {
            int size = Math.Min(Width, Height);
            int offset = (Height - size) / 2;
            return offset + (float)((10.0 - y) / 20.0 * size);
        }

        public void DarkSide()
        {
            BackColor = Color.FromArgb(25, 25, 25);
            ForeColor = Color.White;
            gridColor = Color.FromArgb(60, 60, 60);
            axisColor = Color.FromArgb(155, 155, 155);
            Invalidate();
        }
    }
}