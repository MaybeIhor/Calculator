using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Calculus
{
    public partial class PlotForm : System.Windows.Forms.Form
    {
        private bool dark;
        private readonly List<FunctionData> functions = new List<FunctionData>();
        private readonly List<ToolStripButton> functionButtons = new List<ToolStripButton>();
        private int selectedIndex = -1;

        public PlotForm(bool dark)
        {
            InitializeComponent();
            toolStrip.Renderer = new FixedRenderer();
            addButton.Click += AddButton_Click;
            UpdateScrollButtons();

            if (dark)
                GetDark();
        }

        public void GetDark()
        {
            Dwm.DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            toolStrip.BackColor = Color.FromArgb(25, 25, 25);
            toolStrip.ForeColor = Color.White;
            plotBox.DarkSide();
            dark = true;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new AddForm(dark))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;

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

        private void AddFunctionButton(FunctionData funcData)
        {
            var btn = new ToolStripButton
            {
                Text = funcData.Expression,
                Tag = funcData,
                AutoSize = false,
                AutoToolTip = false,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ImageScaling = ToolStripItemImageScaling.None,
                BackgroundImageLayout = ImageLayout.None,
                Visible = false,
                Size = new Size(246, 29),
                Padding = new Padding(0)
            };

            btn.Paint += (s, e) =>
            {
                var rect = new Rectangle(6, e.ClipRectangle.Height - 1, Math.Max(btn.Width - 12, 0), 1);
                using (var pen = new Pen(funcData.Color))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            btn.Click += (s, e) =>
            {
                int idx = functionButtons.IndexOf(btn);

                functions.Remove(funcData);
                toolStrip.Items.Remove(btn);
                functionButtons.RemoveAt(idx);

                plotBox.SetFunctions(functions);
                plotBox.Invalidate();

                selectedIndex = -1;

                if (functionButtons.Count > 0)
                    SelectButton(Math.Min(idx, functionButtons.Count - 1));
                else
                    UpdateScrollButtons();
            };

            toolStrip.Items.Add(btn);
            functionButtons.Add(btn);
            SelectButton(functionButtons.Count - 1);
        }

        private void SelectButton(int index)
        {
            if (functionButtons.Count == 0) return;

            index = Math.Max(0, Math.Min(index, functionButtons.Count - 1));

            if (index == selectedIndex) return;

            if (selectedIndex >= 0 && selectedIndex < functionButtons.Count)
                functionButtons[selectedIndex].Visible = false;

            selectedIndex = index;
            functionButtons[selectedIndex].Visible = true;
            UpdateScrollButtons();
        }

        private void RightButton_Click(object sender, EventArgs e) => SelectButton(selectedIndex + 1);
        private void LeftButton_Click(object sender, EventArgs e) => SelectButton(selectedIndex - 1);

        private void UpdateScrollButtons()
        {
            leftButton.Enabled = selectedIndex > 0;
            rightButton.Enabled = selectedIndex >= 0 && selectedIndex < functionButtons.Count - 1;
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
        public List<FunctionData> Functions { get; set; } = new List<FunctionData>();

        private Color gridColor = Color.FromArgb(220, 220, 220);
        private Color axisColor = Color.Black;
        private double gridScale = 10.0;

        public PlotBox()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw,
                true);

            BackColor = SystemColors.Window;
        }

        public void SetFunctions(List<FunctionData> funcs) => Functions = funcs;

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            gridScale = e.Delta < 0 ? Math.Min(gridScale + 10.0, 100.0) : Math.Max(gridScale - 10.0, 10.0);

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

            foreach (var func in Functions)
                DrawFunction(g, func.Expression, func.Color);
        }

        private void DrawGrid(Graphics g)
        {
            using var pen = new Pen(gridColor, 1);
            using var font = new Font("Segoe UI", 9);
            using var brush = new SolidBrush(ForeColor);
            double step = gridScale / 10.0;

            for (int i = -9; i <= 9; i++)
            {
                if (i == 0) continue;

                float x = MapXToScreen(i * step);
                float y = MapYToScreen(i * step);

                g.DrawLine(pen, x, 0, x, Height);
                g.DrawLine(pen, 0, y, Width, y);

                if (i == 1)
                    g.DrawString((gridScale / 10).ToString("0"), font, brush, x, MapYToScreen(0) + 4);
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

            function = function.ToLowerInvariant();

            using (var pen = new Pen(color, 2))
            {
                var points = new List<PointF>();

                double rangeMin = -gridScale;
                double rangeMax = gridScale;
                double step = (rangeMax - rangeMin) / Width;

                double? prevX = null;
                double? prevY = null;

                void Flush()
                {
                    if (points.Count > 1)
                        g.DrawLines(pen, points.ToArray());

                    points.Clear();
                }

                for (double x = rangeMin; x <= rangeMax; x += step)
                {
                    try
                    {
                        string xs = x.ToString(EnUs);
                        string expr = function.Replace("x", x < 0 ? "(0" + xs + ")" : xs);
                        double y = Form.Evaluate(expr);

                        if (double.IsNaN(y) || double.IsInfinity(y))
                        {
                            Flush();
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
                            Flush();

                            if (currInBounds)
                                points.Add(new PointF(MapXToScreen(clipX), MapYToScreen(clipY)));
                        }

                        if (currInBounds)
                            points.Add(new PointF(MapXToScreen(x), MapYToScreen(y)));
                        else if (points.Count > 0)
                            Flush();

                        prevX = x;
                        prevY = y;
                    }
                    catch
                    {
                        Flush();
                        prevX = prevY = null;
                    }
                }
                Flush();
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