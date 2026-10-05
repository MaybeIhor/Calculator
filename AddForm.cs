using System;
using System.Drawing;
using System.Windows.Forms;

namespace Calculus
{
    public partial class AddForm : System.Windows.Forms.Form
    {
        public string FunctionExpression => functionBox.Text;
        public Color FunctionColor { get; private set; } = Color.Blue;

        public AddForm(bool dark)
        {
            InitializeComponent();
            toolStrip.Renderer = new FixedRenderer();
            Change_Color();

            if (dark)
                GetDark();
        }

        public void GetDark()
        {
            Dwm.DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);

            var bg = Color.FromArgb(25, 25, 25);
            toolStrip.BackColor = functionBox.BackColor = colorBox.BackColor = bg;
            toolStrip.ForeColor = functionBox.ForeColor = colorBox.ForeColor = SystemColors.Window;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = string.IsNullOrWhiteSpace(functionBox.Text) ? DialogResult.None : DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ColorBox_TextChanged(object sender, EventArgs e) => Change_Color();

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