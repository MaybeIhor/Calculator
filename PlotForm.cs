using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private bool dark = false;
        private List<FunctionData> functions = new List<FunctionData>();

        public plotForm(bool dark)
        {
            InitializeComponent();
            toolStrip.Renderer = new FixedRenderer();
            addButton.Click += new EventHandler(this.addButton_Click);
            if (dark)
                GetDark();
        }

        public void GetDark()
        {
            DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            toolStrip.BackColor = Color.FromArgb(25, 25, 25);
            toolStrip.ForeColor = Color.White;
            plotBox.DarkSide();
            dark = true;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new addForm(dark))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var funcData = new FunctionData
                    {
                        Expression = dialog.FunctionExpression,
                        Color = dialog.FunctionColor
                    };

                    functions.Add(funcData);
                    AddFunctionButton(funcData);
                    plotBox.SetFunctions(functions);
                    plotBox.Invalidate();
                }
            }
        }

        private void AddFunctionButton(FunctionData funcData)
        {
            var btn = new ToolStripButton
            {
                Text = funcData.Expression.Length > 12 ? funcData.Expression.Substring(0, 12) + "..." : funcData.Expression,
                Tag = funcData,
                AutoSize = false,
                AutoToolTip = false,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ImageScaling = ToolStripItemImageScaling.None,
                BackgroundImageLayout = ImageLayout.None,
                Size = new Size(100, 29),
                Padding = new Padding(0, 0, 0, 0)
            };

            btn.Paint += (s, e) =>
            {
                var rect = new Rectangle(6, e.ClipRectangle.Height - 1, 86, 1);
                using (var brush = new Pen(funcData.Color))
                {
                    e.Graphics.DrawRectangle(brush, rect);
                }
            };

            btn.Click += (s, e) =>
            {
                functions.Remove(funcData);
                toolStrip.Items.Remove(btn);
                plotBox.SetFunctions(functions);
                plotBox.Invalidate();
            };

            toolStrip.Items.Add(btn);
        }
    }

    [Serializable]
    public class FunctionData
    {
        public string Expression { get; set; }
        public Color Color { get; set; }
    }

    public class PlotBox : Control
    {
        private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<FunctionData> functions { get; set; } = new List<FunctionData>();

        private Color gridColor = Color.FromArgb(220, 220, 220);
        private Color axisColor = Color.Black;
        private double gridScale = 10.0;

        public PlotBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = SystemColors.Window;
        }

        public void SetFunctions(List<FunctionData> funcs)
        {
            functions = funcs;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
                gridScale = Math.Min(gridScale + 10.0, 100.0);
            else
                gridScale = Math.Max(gridScale - 10.0, 10.0);
            Invalidate();
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

            foreach (var func in functions)
            {
                DrawFunction(g, func.Expression, func.Color);
            }
        }

        private void DrawGrid(Graphics g)
        {
            using (var pen = new Pen(gridColor, 1))
            {
                for (int i = -9; i <= 9; i++)
                {
                    if (i == 0) continue;
                    double step = gridScale / 10.0;
                    float x = MapXToScreen(i * step);
                    float y = MapYToScreen(i * step);
                    g.DrawLine(pen, x, 0, x, Height);
                    g.DrawLine(pen, 0, y, Width, y);
                    if (i == 1)
                        g.DrawString((gridScale / 10).ToString("0"), new Font("Segoe UI", 9), new SolidBrush(ForeColor), x, MapYToScreen(0) + 4);
                }
            }
        }

        private void DrawAxes(Graphics g)
        {
            using (var pen = new Pen(axisColor, 1))
            {
                float xCenter = MapXToScreen(0);
                float yCenter = MapYToScreen(0);

                g.DrawLine(pen, 0, yCenter, Width, yCenter);
                g.DrawLine(pen, xCenter, 0, xCenter, Height);
            }
        }

        private void DrawFunction(Graphics g, string function, Color color)
        {
            if (string.IsNullOrWhiteSpace(function)) return;

            using (var pen = new Pen(color, 2))
            {
                var points = new List<PointF>();
                double rangeMin = -gridScale;
                double rangeMax = gridScale;
                double step = (rangeMax - rangeMin) / Width;
                double? prevX = null, prevY = null;

                for (double x = rangeMin; x <= rangeMax; x += step)
                {
                    try
                    {
                        string expr = function.Replace("x", x < 0 ? $"(0{x.ToString(EnUs)})" : x.ToString(EnUs));
                        double y = form.Evaluate(expr);

                        if (double.IsNaN(y) || double.IsInfinity(y))
                        {
                            if (points.Count > 1) g.DrawLines(pen, points.ToArray());
                            points.Clear();
                            prevX = prevY = null;
                            continue;
                        }

                        bool currInBounds = y >= rangeMin && y <= rangeMax;
                        bool prevInBounds = prevY.HasValue && prevY.Value >= rangeMin && prevY.Value <= rangeMax;

                        if (prevX.HasValue && prevInBounds != currInBounds)
                        {
                            double clipY = prevY.Value < rangeMin || y < rangeMin ? rangeMin : rangeMax;
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
            return offset + (float)((x + gridScale) / (gridScale * 2.0) * size);
        }

        private float MapYToScreen(double y)
        {
            int size = Math.Min(Width, Height);
            int offset = (Height - size) / 2;
            return offset + (float)((gridScale - y) / (gridScale * 2.0) * size);
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