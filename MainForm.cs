using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Calculus
{
    public partial class form : Form
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] val, int size);

        private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
        private static readonly string PiString = Math.PI.ToString(EnUs);
        private static readonly string EString = Math.E.ToString(EnUs);

        private string temp = string.Empty;

        public form()
        {
            InitializeComponent();
            ApplyTheme();
            toolStrip.Renderer = new FixedRenderer();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void ApplyTheme()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
            {
                if (key?.GetValue("AppsUseLightTheme") is object themeValue && int.Parse(themeValue.ToString()) == 0)
                {
                    DwmSetWindowAttribute(this.Handle, 20, new[] { 1 }, 4);
                    Color darkBg = Color.FromArgb(255, 25, 25, 25);
                    Color darkerBg = Color.FromArgb(255, 19, 19, 19);
                    Color lightText = SystemColors.Window;

                    this.BackColor = darkBg;
                    textBox1.BackColor = darkBg;
                    textBox2.BackColor = darkerBg;
                    label1.BackColor = darkBg;
                    toolStrip.BackColor = darkBg;
                    toolStrip.ForeColor = lightText;
                    textBox1.ForeColor = lightText;
                    label1.ForeColor = lightText;
                    textBox2.ForeColor = SystemColors.ControlDark;
                }
                else
                {
                    textBox2.ForeColor = SystemColors.WindowFrame;
                }
            }
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                int selectionStart = textBox1.SelectionStart;
                int selectionLength = textBox1.SelectionLength;

                if (selectionLength > 0)
                {
                    textBox1.Text = textBox1.Text.Remove(selectionStart, selectionLength);
                    textBox1.SelectionStart = selectionStart;
                }
                else if (selectionStart > 0)
                {
                    textBox1.Text = textBox1.Text.Remove(selectionStart - 1, 1);
                    textBox1.SelectionStart = selectionStart - 1;
                }

                textBox1.Focus();
            }
        }
        private void ToolStripButton2_Click(object sender, EventArgs e) => textBox1.Text = string.Empty;

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;
            toolStripButton3.Text = this.TopMost ? "Unpin" : "Pin";
        }

        private void InsertText(string text) => textBox1.SelectedText = text;

        private void ToolStripButton4_Click(object sender, EventArgs e) => InsertText("(");
        private void ToolStripButton5_Click(object sender, EventArgs e) => InsertText(")");
        private void ToolStripButton6_Click(object sender, EventArgs e) => InsertText("7");
        private void ToolStripButton7_Click(object sender, EventArgs e) => InsertText("8");
        private void ToolStripButton8_Click(object sender, EventArgs e) => InsertText("9");
        private void ToolStripButton9_Click(object sender, EventArgs e) => InsertText("rt(2,");
        private void ToolStripButton10_Click(object sender, EventArgs e) => InsertText("^");
        private void ToolStripButton11_Click(object sender, EventArgs e) => InsertText("4");
        private void ToolStripButton12_Click(object sender, EventArgs e) => InsertText("5");
        private void ToolStripButton13_Click(object sender, EventArgs e) => InsertText("6");
        private void ToolStripButton14_Click(object sender, EventArgs e) => InsertText("*");
        private void ToolStripButton15_Click(object sender, EventArgs e) => InsertText("/");
        private void ToolStripButton16_Click(object sender, EventArgs e) => InsertText("1");
        private void ToolStripButton17_Click(object sender, EventArgs e) => InsertText("2");
        private void ToolStripButton18_Click(object sender, EventArgs e) => InsertText("3");
        private void ToolStripButton19_Click(object sender, EventArgs e) => InsertText("-");
        private void ToolStripButton20_Click(object sender, EventArgs e) => InsertText("+");
        private void ToolStripButton21_Click(object sender, EventArgs e) => InsertText("0");
        private void ToolStripButton22_Click(object sender, EventArgs e) => InsertText(".");
        private void ToolStripButton23_Click(object sender, EventArgs e) => InsertText("π");
        private void ToolStripButton24_Click(object sender, EventArgs e) => Solve();
        private void ToolStripButton25_Click(object sender, EventArgs e) => InsertText("sin(");
        private void ToolStripButton26_Click(object sender, EventArgs e) => InsertText("cos(");
        private void ToolStripButton27_Click(object sender, EventArgs e) => InsertText("tan(");
        private void ToolStripButton28_Click(object sender, EventArgs e) => InsertText("log(10,");
        private void ToolStripButton29_Click(object sender, EventArgs e) => InsertText("e");

        public static double Evaluate(string ex)
        {
            Stack<double> operands = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            int i = 0;

            while (i < ex.Length)
            {
                if (char.IsWhiteSpace(ex[i]))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(ex[i]) || ex[i] == '.')
                {
                    int start = i;
                    while (i < ex.Length && (char.IsDigit(ex[i]) || ex[i] == '.'))
                        i++;

                    operands.Push(double.Parse(ex.Substring(start, i - start), EnUs));
                }
                else if (char.IsLetter(ex[i]))
                {
                    int start = i;
                    while (i < ex.Length && char.IsLetter(ex[i]))
                        i++;

                    operators.Push(ex.Substring(start, i - start));
                }
                else if (ex[i] == '(')
                {
                    operators.Push("(");
                    i++;
                }
                else if (ex[i] == ',')
                {
                    while (operators.Peek() != "(")
                        Apply(operands, operators.Pop());
                    i++;
                }
                else if (ex[i] == ')')
                {
                    while (operators.Peek() != "(")
                        Apply(operands, operators.Pop());

                    operators.Pop();

                    if (operators.Count > 0 && IsFunction(operators.Peek()))
                        Apply(operands, operators.Pop());

                    i++;
                }
                else if (IsOperator(ex[i]))
                {
                    string current = ex[i].ToString();
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(current))
                        Apply(operands, operators.Pop());

                    operators.Push(current);
                    i++;
                }
                else
                {
                    throw new Exception();
                }
            }

            while (operators.Count > 0)
                Apply(operands, operators.Pop());

            return operands.Pop();
        }

        private static bool IsOperator(char c) => c == '+' || c == '-' || c == '*' || c == '/' || c == '^';

        private static bool IsFunction(string op) => op == "sin" || op == "cos" || op == "tan" || op == "log" || op == "rt";

        private static void Apply(Stack<double> operands, string op)
        {
            if (op == "sin" || op == "cos" || op == "tan")
            {
                double b = operands.Pop();
                double radians = b * Math.PI / 180;

                switch (op)
                {
                    case "sin": operands.Push(Math.Sin(radians)); break;
                    case "cos": operands.Push(Math.Cos(radians)); break;
                    case "tan": operands.Push(Math.Tan(radians)); break;
                }
            }
            else
            {
                double b = operands.Pop();
                double a = operands.Pop();

                switch (op)
                {
                    case "+": operands.Push(a + b); break;
                    case "-": operands.Push(a - b); break;
                    case "*": operands.Push(a * b); break;
                    case "/": operands.Push(a / b); break;
                    case "^": operands.Push(Math.Pow(a, b)); break;
                    case "log": operands.Push(Math.Log(b, a)); break;
                    case "rt": operands.Push(Math.Pow(b, 1 / a)); break;
                }
            }
        }

        private static int Precedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-": return 1;
                case "*":
                case "/": return 2;
                case "^": return 3;
                case "sin":
                case "cos":
                case "tan":
                case "log": return 4;
                default: return 0;
            }
        }

        private void Solve()
        {
            string func = textBox1.Text.Replace("π", PiString).Replace("e", EString).Replace(" ", string.Empty);

            for (int i = 0; i < func.Length; i++)
            {
                if (func[i] == '-' && (i == 0 || func[i - 1] == '('))
                {
                    func = func.Insert(i, "0");
                }
            }

            try
            {
                double result = Evaluate(func);
                Send(result.ToString(EnUs));
            }
            catch (Exception)
            {
                Send("error");
            }
        }

        private void Send(string s)
        {
            if (label1.Text == "⏶")
            {
                label1.Text = "⏷";
                (temp, textBox2.Text) = (textBox2.Text, temp);
            }

            if (textBox2.Text != string.Empty)
                textBox2.AppendText(Environment.NewLine + s);
            else
                textBox2.AppendText(s);

            temp = textBox1.Text;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Solve();
            }
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            label1.Text = label1.Text == "⏷" ? "⏶" : "⏷";
            (temp, textBox2.Text) = (textBox2.Text, temp);
            textBox2.ScrollToCaret();
        }
    }

    public class FixedRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }
}