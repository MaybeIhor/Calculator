using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calculus
{
    public partial class addForm : Form
    {
        public string FunctionExpression => functionBox.Text;
        public Color FunctionColor { get; private set; } = Color.Blue;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        public addForm(bool dark)
        {
            InitializeComponent();
            toolStrip.Renderer = new FixedRenderer();
            Change_Color();

            if (dark)
                GetDark();
        }

        public void GetDark()
        {
            DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
            toolStrip.BackColor = Color.FromArgb(25, 25, 25);
            functionBox.BackColor = Color.FromArgb(25, 25, 25);
            colorBox.BackColor = Color.FromArgb(25, 25, 25);
            toolStrip.ForeColor = SystemColors.Window;
            functionBox.ForeColor = SystemColors.Window;
            colorBox.ForeColor = SystemColors.Window;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(functionBox.Text))
            {
                DialogResult = DialogResult.None;
            }
            else
                DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ColorBox_TextChanged(object sender, EventArgs e)
        {
            Change_Color();
        }

        private void Change_Color()
        {
            try
            {
                string hex = colorBox.Text.TrimStart('#');
                if (hex.Length == 6)
                {
                    FunctionColor = ColorTranslator.FromHtml("#" + hex);
                    colorLabel.BackColor = FunctionColor;
                }
            }
            catch { }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                OkButton_Click(this, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Escape)
            {
                CancelButton_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
